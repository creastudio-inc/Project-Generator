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
    public class DatabaseController : BaseController
    {
        // GET: Database
        // GET: Project
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List(PagingParameterModel PagingParameterModel,Guid IDProject)
        {
            ViewBag.IDProject = IDProject;
            ViewBag.SortBy = PagingParameterModel.sortBy;
            ViewBag.IsAsc = PagingParameterModel.isAsc;
            ViewBag.FilterValue = PagingParameterModel.SearchData;

            //Creating the ViewModel's Object
            TableViewModel obj = new TableViewModel();
            //List of the Blog
            var predicate = TableService.GetExpressionPredicateForList(PagingParameterModel.SearchData, IDProject);

            List<SortDescriptor> sortedLists = new List<SortDescriptor>();
            sortedLists.Add(new SortDescriptor()
            {
                Direction = SortDescriptor.SortingDirection.Descending,
                Field = "CreatedOn"
            });
            sortedLists.Add(new SortDescriptor()
            {
                Direction = PagingParameterModel.isAsc ? SortDescriptor.SortingDirection.Ascending : SortDescriptor.SortingDirection.Descending,
                Field = TableService.GetGetFieldName(PagingParameterModel.sortBy)
            });
            var result = TableService.doGetListEntity(predicate, sortedLists, PagingParameterModel.pageNumber, PagingParameterModel.pageSize);

            //Passing the TotalRecordsCount, Current Page and Page Size in the constructore of the Pager Class
            var pager = new Pager(result.Item2, PagingParameterModel.pageNumber, PagingParameterModel.pageSize);
            obj.List = result.Item1.ToList();
            obj.pager = pager;
            return View("List", obj);
        }

        public ActionResult Detail(Guid? id)
        {
            var item = TableService.doGetEntity(x => x.Id == id, x=>x.DataFieldViewModels, x => x.dataFieldForeignKeyViewModels.Select(y => y.TableView)) ;

            return View("Detail", item);
        }

        public ActionResult Add(Guid IdProject)
        {
            return View("Form", new Table() { ProjectID = IdProject });
        }


        public ActionResult Update(Guid? Id)
        {
            var BlogCategory = TableService.doGetEntity(x => x.Id == Id);
            return View("Form", BlogCategory);
        }

        [HttpPost]
        public ActionResult Store(Table model)
        {
            if (ModelState.IsValid)
            {
                ViewBag.Message = "All inputs are Valid!";

                TableService.doCreateOrUpdate(model);
                SetFlash(Enum.FlashMessageType.Success, (model.Id == Guid.Empty) ? "successfully created" : "successfully updated");
                return RedirectToAction("List", new { IDProject = model.ProjectID });
            }
            return View(model);
        }

        public ActionResult delete(Guid id,Guid ProjectID)
        {
            TableService.doDeleteEntityByID(id);

            SetFlash(Enum.FlashMessageType.Danger, "successfully deleted");
            return RedirectToAction("List", new { id = ProjectID });


        }



    }
}