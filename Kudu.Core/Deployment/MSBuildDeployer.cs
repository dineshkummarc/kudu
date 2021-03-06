﻿using System;
using System.IO;
using System.Linq;
using Kudu.Core.Infrastructure;
using SystemEnvironment = System.Environment;

namespace Kudu.Core.Deployment {
    public class MSBuildDeployer : IDeployer {
        private readonly IEnvironment _environment;
        private readonly Executable _msbuildExe;

        public MSBuildDeployer(IEnvironment environment) {
            _environment = environment;
            _msbuildExe = new Executable(ResolveMSBuildPath(), environment.RepositoryPath);
        }

        public void Deploy(string id) {            
            // TODO: Have some convention (or setting to determine the deployment project if more than one)
            string projectFile = _environment.GetWebApplicationProjects().FirstOrDefault();

            // Locate the solution directory
            string solutionDir = GetSolutionDir(projectFile);

            // REVIEW: Should we use the msbuild API?
            _msbuildExe.Execute(@"""{0}"" /t:pipelinePreDeployCopyAllFilesToOneFolder /p:_PackageTempDir={1};AutoParameterizationWebConfigConnectionStrings=false;SolutionDir={2}", projectFile, _environment.DeploymentPath, solutionDir);
        }

        private string GetSolutionDir(string projectPath) {
            string path = projectPath;

            while (!_environment.RepositoryPath.Equals(path, StringComparison.OrdinalIgnoreCase)) {
                path = Path.GetDirectoryName(path);
                if (Directory.EnumerateFiles(path, "*.sln").Any()) {
                    // TODO: Ensure that this project is in this solution

                    // Add the trailing slash
                    return path + "//";
                }
            }

            // This is the default value for SolutionDir when it's not defined in a build
            return "*Undefined*";
        }

        private string ResolveMSBuildPath() {
            string windir = SystemEnvironment.GetFolderPath(SystemEnvironment.SpecialFolder.Windows);
            return Path.Combine(windir, @"Microsoft.NET", "Framework", "v4.0.30319", "MSBuild.exe");
        }
    }
}
