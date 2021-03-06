﻿using System.Collections.Generic;

namespace Kudu.Core.Deployment {
    public interface IDeploymentManager {
        IEnumerable<DeployResult> GetResults();
        DeployResult GetResult(string id);
        void Deploy(string id);
        void Deploy();
    }
}
