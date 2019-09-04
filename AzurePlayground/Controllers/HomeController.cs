﻿using AzurePlayground.Commands.Security;
using AzurePlayground.Models.Security;
using System.Web.Mvc;

namespace AzurePlayground.Controllers {
    [RoutePrefix("Home")]
    public class HomeController : Controller {
        [Route("~/")]
        [Route]
        [Route("Index")]
        [HttpGet]
        public ActionResult Index() {
            return View();
        }

        [Route("Register")]
        [HttpGet]
        public ActionResult Register() {
            return View(new UserRegistration());
        }

        [Route("Register")]
        [HttpPost]
        public ActionResult Register(UserRegistration model) {
            if (ModelState.IsValid) {
                var command = new RegisterUserCommand();
                var result = command.Execute(model);

                // todo validate result

            }

            if (ModelState.IsValid) {
                return View("Registered");
            }
            else {
                return View(model);
            }
        }
    }
}