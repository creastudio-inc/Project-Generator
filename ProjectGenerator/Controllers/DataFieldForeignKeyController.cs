using ProjectGenerator.Models;
using ProjectGenerator.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjectGenerator.Controllers
{
    public class DataFieldForeignKeyController : BaseController
    {
        // GET: DataFieldForeignKey
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Add(Guid TableId,Guid ProjectID)
        {
            ViewBag.ProjectID = ProjectID;
            ViewBag.DataFieldViewID = new SelectList(TableService.doGetListEntity(x=>x.ProjectID== ProjectID).Item1, "Id", "Name");
            return View("Form", new DataFieldForeignKey() { TableViewID = TableId });
        }


        public ActionResult Update(Guid? Id, Guid ProjectID)
        {
            ViewBag.ProjectID = ProjectID;

            var BlogCategory = DataFieldForeignKeyService.doGetEntity(x => x.Id == Id);
            ViewBag.DataFieldViewID = new SelectList(TableService.doGetListEntity(x => x.ProjectID == ProjectID).Item1, "Id", "Name");

            return View("Form", BlogCategory);
        }

        [HttpPost]
        public ActionResult Store(DataFieldForeignKey model, Guid ProjectID)
        {
            if (ModelState.IsValid)
            {
                ViewBag.Message = "All inputs are Valid!";

                DataFieldForeignKeyService.doCreateOrUpdate(model);
                SetFlash(Enum.FlashMessageType.Success, (model.Id == Guid.Empty) ? "successfully created" : "successfully updated");
                return RedirectToAction("Detail", "Database", new { id = model.TableViewID });
            }
            ViewBag.DataFieldViewID = new SelectList(TableService.doGetListEntity(x => x.ProjectID == ProjectID).Item1, "Id", "Name");

            return View("Form",model);
        }

        public ActionResult delete(Guid id,Guid TableViewID)
        {
            DataFieldForeignKeyService.doDeleteEntityByID(id);
            SetFlash(Enum.FlashMessageType.Danger, "successfully deleted");
            return RedirectToAction("Detail", "Database", new { id = TableViewID });
        }
    }
}