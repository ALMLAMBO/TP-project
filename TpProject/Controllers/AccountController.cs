﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TpProject.Models.Data;
using TpProject.Models.ViewModels.Account;

namespace TpProject.Controllers {
    public class AccountController : Controller {
        // GET: Account
        public ActionResult Index() {
            return Redirect("~/account/login");
        }

		// GET: account/create-account
		[ActionName("create-account")]
		[HttpGet]
		public ActionResult CreateAccount() {
			return View("CreateAccount");
		}

		// POST: account/create-account
		[ActionName("create-account")]
		[HttpPost]
		public ActionResult CreateAccount(UserVM model) {
			if (!ModelState.IsValid) {
				return View("CreateAccount", model);
			}

			if (!model.Password.Equals(model.ConfirmPassword)) {
				ModelState.AddModelError("", "Passwords do not match.");
				return View("CreateAccount", model);
			}

			using (Db db = new Db()) {
				if (db.Users.Any(x => x.Username.Equals(model.Username))) {
					ModelState.AddModelError("", "Username " + model.Username + " is taken.");
					model.Username = "";
					return View("CreateAccount", model);
				}

				UserDTO userDTO = new UserDTO() {
					FirstName = model.FirstName,
					LastName = model.LastName,
					EmailAddress = model.EmailAddress,
					Username = model.Username,
					Password = model.Password
				};

				db.Users.Add(userDTO);
				db.SaveChanges();

				int id = userDTO.Id;

				UserRoleDTO userRolesDTO = new UserRoleDTO() {
					UserId = id,
					RoleId = 2
				};

				db.UserRoles.Add(userRolesDTO);
				db.SaveChanges();
			}

			TempData["SM"] = "You are now registered and can login.";

			return Redirect("~/account/login");
		}

		// GET: /account/login
		[HttpGet]
		public ActionResult Login() {
			string username = User.Identity.Name;

			if (!string.IsNullOrEmpty(username)) {
				return RedirectToAction("user-profile");
			}

			return View();
		}

		// POST: /account/login
		[HttpPost]
		public ActionResult Login(LoginUserVM model) {
			if (!ModelState.IsValid) {
				return View(model);
			}

			bool isValid = false;

			using (Db db = new Db()) {
				if (db.Users.Any(x => x.Username.Equals(model.Username) 
					&& x.Password.Equals(model.Password))) {
					isValid = true;
				}
			}

			if (!isValid) {
				ModelState.AddModelError("", "Invalid username or password.");
				return View(model);
			} 
			else {
				FormsAuthentication.SetAuthCookie(model.Username, model.RememberMe);
				return Redirect(FormsAuthentication.GetRedirectUrl(model.Username, model.RememberMe));
			}
		}

		// GET: /account/Logout
		public ActionResult Logout() {
			FormsAuthentication.SignOut();
			return Redirect("~/account/login");
		}

		public ActionResult UserNavPartial() {
			string username = User.Identity.Name;
			UserNavPartialVM model;

			using (Db db = new Db()) {
				UserDTO dto = db.Users
					.FirstOrDefault(x => x.Username == username);

				model = new UserNavPartialVM() {
					FirstName = dto.FirstName,
					LastName = dto.LastName
				};
			}

			return PartialView(model);
		}

		// GET: /account/user-profile
		[HttpGet]
		[ActionName("user-profile")]
		public ActionResult UserProfile() {
			string username = User.Identity.Name;
			UserProfileVM model;

			using (Db db = new Db()) {
				UserDTO dto = db.Users
					.FirstOrDefault(x => x.Username == username);

				model = new UserProfileVM(dto);
			}

			return View("UserProfile", model);
		}

		// POST: /account/user-profile
		[HttpPost]
		[ActionName("user-profile")]
		public ActionResult UserProfile(UserProfileVM model) {
			if (!ModelState.IsValid) {
				return View("UserProfile", model);
			}

			if (!string.IsNullOrWhiteSpace(model.Password)) {
				if (!model.Password.Equals(model.ConfirmPassword)) {
					ModelState
						.AddModelError("", "Passwords do not match.");

					return View("UserProfile", model);
				}
			}

			using (Db db = new Db()) {
				string username = User.Identity.Name;

				if (db.Users.Where(x => x.Id != model.Id)
					.Any(x => x.Username == username)) {

					ModelState
						.AddModelError("", "Username " 
						+ model.Username + " already exists.");

					model.Username = "";

					return View("UserProfile", model);
				}

				UserDTO dto = db.Users.Find(model.Id);

				dto.FirstName = model.FirstName;
				dto.LastName = model.LastName;
				dto.EmailAddress = model.EmailAddress;
				dto.Username = model.Username;

				if (!string.IsNullOrWhiteSpace(model.Password)) {
					dto.Password = model.Password;
				}

				db.SaveChanges();
			}

			TempData["SM"] = "You have edited your profile!";

			return Redirect("~/account/user-profile");
		}
	}
}