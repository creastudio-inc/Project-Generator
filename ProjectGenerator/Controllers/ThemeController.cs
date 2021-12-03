using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Linq;

namespace ProjectGenerator.Controllers
{
    public class FormModal
    {
        public String Class { get; set; } 
        public List<InputModal> Inputs { get; set; } 
     }
    public class InputModal
    {
        public String DivClass { get; set; } 
        public String LabelClass { get; set; } 
        public String InputClass { get; set; } 
        public String InputType { get; set; } 
    }
 
    public class ThemeController : Controller
    {
        // GET: Theme
        public ActionResult Index()
        {
            var form = new FormModal();
            form.Inputs = new List<InputModal>();
            doCreate("http://skote-v-light.codeigniter.themesbrand.com/form-elements", form);
            doCreate("http://skote-v-light.codeigniter.themesbrand.com/form-layouts", form);
            doCreate("http://skote-v-light.codeigniter.themesbrand.com/form-validation", form);
            doCreate("http://skote-v-light.codeigniter.themesbrand.com/form-advanced", form);
            var doclink = new HtmlWeb().Load("http://skote-v-light.codeigniter.themesbrand.com/form-elements");
        

            return View(form);
        }
        public void doCreate(string lien, FormModal form)
        {
            var doc = new HtmlWeb().Load(lien);
            var allElementsWithClassFloat = doc.DocumentNode.SelectNodes("//div[contains(@class, 'form-check') or contains(@class, 'mb-3')]");
            foreach (HtmlNode link in allElementsWithClassFloat)
            {
                if (link.ChildNodes.Count > 4)
                {
                     var InputModal = new InputModal();
                    InputM-odal.DivClass = String.Join(" ", link.Attributes.Where(x => x.Name == "class").Select(x => x.Value).OrderBy(x => x));

                    foreach (var node in link.ChildNodes)
                    {
                        if (node.Name == "label")
                        {
                            InputModal.LabelClass = String.Join(" ", node.Attributes.Where(x => x.Name == "class").Select(x => x.Value).OrderBy(x => x));
                        }
                        if (node.Name == "input")
                        {
                            InputModal.InputClass = String.Join(" ", node.Attributes.Where(x => x.Name == "class").Select(x => x.Value).OrderBy(x => x));
                            InputModal.InputType = node.Attributes.Where(x => x.Name == "type").FirstOrDefault().Value;
                        }
                        if (node.Name == "select")
                        {
                             InputModal.InputClass = String.Join(" ", node.Attributes.Where(x => x.Name == "class").Select(x => x.Value).OrderBy(x => x));
                            InputModal.InputType = "select";
                        }

                    }
                    if(!string.IsNullOrEmpty(InputModal.DivClass) &&!string.IsNullOrEmpty(InputModal.InputClass) && !string.IsNullOrEmpty(InputModal.InputType) && !string.IsNullOrEmpty(InputModal.LabelClass))
                    {

                        bool containsItem = form.Inputs.Any(item => item.DivClass == InputModal.DivClass && item.InputClass == InputModal.InputClass && item.InputType == InputModal.InputType && item.LabelClass == InputModal.LabelClass);
                        if (!containsItem)
                            form.Inputs.Add(InputModal);
                    }
                }
            }
        }
   
    }
}