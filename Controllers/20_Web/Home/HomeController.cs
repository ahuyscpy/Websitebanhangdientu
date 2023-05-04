using System.Web.Mvc;
using Gemini.Controllers.Bussiness;
using Gemini.Models._20_Web;
using System.Linq;
using Gemini.Models._02_Cms.U;
using System.Collections.Generic;
using System.Web;
using Gemini.Models;
using Gemini.Models._03_Pos;

namespace Gemini.Controllers._20_Web.Home
{
    public class HomeController : GeminiController
    {
        #region Language
        public ActionResult ChangeLanguage(string lang)
        {
            new SitesLanguage().SetLanguage(lang);

            return Json(Request.Cookies["language"].Value, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetLanguage()
        {
            if (Request.Cookies["language"] != null)
            {
                return Json(Request.Cookies["language"].Value, JsonRequestBehavior.AllowGet);

            }
            return Json("VI", JsonRequestBehavior.AllowGet);
        }

        public string GetLanguageService()
        {
            if (Request.Cookies["language"] != null)
            {
                return (Request.Cookies["language"].Value);

            }

            return ("VI");
        }
        #endregion

        public ActionResult Index()
        {
            HomeModel model = new HomeModel();
            model.ListPosCategory = new List<PosCategoryModel>();

            model.ListPosCategory = (from cat in DataGemini.PosCategories
                                     where cat.Active
                                     select new PosCategoryModel
                                     {
                                         SeoFriendUrl = cat.SeoFriendUrl,
                                         Name = cat.Name,
                                         OrderBy = cat.OrderBy
                                     }).OrderBy(s => s.OrderBy).ToList();

            return View("Index", model);
        }
        /*
        #region Register
        public ActionResult Register()
        {
            UserRegisterModel user = new UserRegisterModel();
            return View(user);

        }
        #endregion
        */
    }
}