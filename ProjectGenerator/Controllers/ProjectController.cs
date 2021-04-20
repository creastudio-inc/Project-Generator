using Infrastructure;
using ProjectGenerator.Models;
using ProjectGenerator.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjectGenerator.Controllers
{
    public class ProjectController : BaseController
    {
        // GET: Project
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List(PagingParameterModel PagingParameterModel)
        {
            ViewBag.SortBy = PagingParameterModel.sortBy;
            ViewBag.IsAsc = PagingParameterModel.isAsc;
            ViewBag.FilterValue = PagingParameterModel.SearchData;

            //Creating the ViewModel's Object
            ProjectViewModel obj = new ProjectViewModel();
            //List of the Blog
            var predicate = ProjectService.GetExpressionPredicateForList(PagingParameterModel.SearchData);

            List<SortDescriptor> sortedLists = new List<SortDescriptor>();
            sortedLists.Add(new SortDescriptor()
            {
                Direction = SortDescriptor.SortingDirection.Descending,
                Field = "CreatedOn"
            });
            sortedLists.Add(new SortDescriptor()
            {
                Direction = PagingParameterModel.isAsc ? SortDescriptor.SortingDirection.Ascending : SortDescriptor.SortingDirection.Descending,
                Field = ProjectService.GetGetFieldName(PagingParameterModel.sortBy)
            });
            var result = ProjectService.doGetListEntity(predicate, sortedLists, PagingParameterModel.pageNumber, PagingParameterModel.pageSize);

            //Passing the TotalRecordsCount, Current Page and Page Size in the constructore of the Pager Class
            var pager = new Pager(result.Item2, PagingParameterModel.pageNumber, PagingParameterModel.pageSize);
            obj.List = result.Item1.ToList();
            obj.pager = pager;
            return View("List", obj);
        }

        public ActionResult Detail(Guid? id)
        {
          var item=  ProjectService.doGetEntity(x=>x.Id== id);
            if (item == null)
            {
                SetFlash(Enum.FlashMessageType.Danger, "not found");
                return RedirectToAction("List");
            }
            return View("Detail", item);
        }
        
        public ActionResult Add()
        {
            return View("Form", new Project());
        }


        public ActionResult Update(Guid? Id)
        {
            var BlogCategory = ProjectService.doGetEntity(x => x.Id == Id);
            return View("Form", BlogCategory);
        }

        [HttpPost]
        public ActionResult Store(Project model)
        {
            if (ModelState.IsValid)
            {
                ViewBag.Message = "All inputs are Valid!";

                ProjectService.doCreateOrUpdate(model);
                SetFlash(Enum.FlashMessageType.Success, (model.Id == Guid.Empty) ? "successfully created" : "successfully updated");
                return RedirectToAction("Detail", new { id = model.Id });
            }
            return View(model);
        }
 
        public ActionResult delete(Guid id)
        {
            ProjectService.doDeleteEntityByID(id);

            SetFlash(Enum.FlashMessageType.Danger,  "successfully deleted");
            return RedirectToAction("List");


        }
    }
}