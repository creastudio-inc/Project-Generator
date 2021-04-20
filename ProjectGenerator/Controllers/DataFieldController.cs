using ProjectGenerator.Models;
using ProjectGenerator.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjectGenerator.Controllers
{
    public class DataFieldController : BaseController
    {
        // GET: DataField
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Add(Guid TableId)
        {
            return View("Form", new DataField() { TableId = TableId });
        }


        public ActionResult Update(Guid? Id)
        {
            var BlogCategory = DataFieldService.doGetEntity(x => x.Id == Id);
            return View("Form", BlogCategory);
        }

        [HttpPost]
        public ActionResult Store(DataField model)
        {
            if (ModelState.IsValid)
            {
                ViewBag.Message = "All inputs are Valid!";

                DataFieldService.doCreateOrUpdate(model);
                SetFlash(Enum.FlashMessageType.Success, (model.Id == Guid.Empty) ? "successfully created" : "successfully updated");
                return RedirectToAction("Detail", "Database", new { id = model.TableId });
            }
            return View(model);
        }

        public ActionResult delete(Guid id,Guid TableId)
        {
            DataFieldService.doDeleteEntityByID(id);
            SetFlash(Enum.FlashMessageType.Danger, "successfully deleted");
            return RedirectToAction("Detail", "Database", new { id = TableId });
        }
    }
}