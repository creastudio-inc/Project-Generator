using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjectGenerator.Controllers
{
    public class Enum
    {
        public enum FlashMessageType
        {
            Success,
            warning,
            Danger
        };
    }
    public class BaseController : Controller
    {

        public void SetFlash(Enum.FlashMessageType type, string text)
        {
            TempData["FlashMessage.Type"] = type;
            TempData["FlashMessage.Text"] = text;
        }
    }
}