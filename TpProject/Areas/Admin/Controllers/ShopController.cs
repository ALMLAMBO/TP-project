﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TpProject.Models.Data;
using TpProject.Models.ViewModels.Shop;

namespace TpProject.Areas.Admin.Controllers {
    public class ShopController : Controller {
        // GET: Admin/Shop/Index
        public ActionResult Index() {
			List<CategoryVM> categoriesVMList;
			using (Db db = new Db()) {
				categoriesVMList = db.Categories
					.ToArray()
					.OrderBy(x => x.Sorting)
					.Select(x => new CategoryVM(x))
					.ToList();
			}
            return View(categoriesVMList);
        }

		[HttpGet]
		public ActionResult AddNewCategory() {
			return View();
		}

		// GET: Admin/Shop/AddNewCategory
		[HttpPost]
		public ActionResult AddNewCategory(CategoryVM model) {
			if(!ModelState.IsValid) {
				return View(model);
			}

			using (Db db = new Db()) {
				CategoryDTO dto = new CategoryDTO();
				dto.Name = model.Name;

				if (db.Categories
					.Any(x => x.Name == model.Name)) {
					ModelState
						.AddModelError("", "That category already exists.");

					return View(model);
				}

				dto.Slug = model.Name.Replace(" ", "-").ToLower();
				dto.Sorting = 100;

				db.Categories.Add(dto);
				db.SaveChanges();
			}
			TempData["SM"] = "You have added a new category!";

			return RedirectToAction("AddNewCategory");
		}
    }
}