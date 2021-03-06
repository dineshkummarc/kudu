﻿using System;
using System.Linq;
using System.Web.Mvc;
using Kudu.Core.SourceControl;

namespace Kudu.Web.Controllers {
    public class HomeController : Controller {
        private readonly IRepositoryManager _repositoryManager;
        public HomeController(IRepositoryManager repositoryManager) {
            _repositoryManager = repositoryManager;
        }

        public ActionResult Index() {
            PopulateRepositoyTypes();

            return View(_repositoryManager.GetRepositoryType());
        }

        [HttpPost]
        public ActionResult Create(RepositoryType type) {
            try {
                _repositoryManager.CreateRepository(type);

                return Redirect("~/Hubs/SourceControl/index.htm");
            }
            catch(Exception e) {
                ModelState.AddModelError("_FORM", e.Message);
            }

            PopulateRepositoyTypes();
            return View("Index", RepositoryType.None);
        }

        private void PopulateRepositoyTypes() {
            ViewBag.Type = Enum.GetNames(typeof(RepositoryType))
                               .Select((name, value) => new SelectListItem {
                                   Text = name,
                                   Value = value.ToString()
                               })
                               .Skip(1);

        }
    }
}