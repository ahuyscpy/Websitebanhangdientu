using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Gemini.Controllers.Bussiness;
using Gemini.Models._01_Hethong;

namespace Gemini.Controllers._01_Hethong
{
    public class NavController : GeminiController
    {
        /// <summary>
        /// Polycy Memu
        /// </summary>
        /// <returns></returns>
        public ActionResult Amenu()
        {
            try
            {
                var route = RouteTable.Routes.GetRouteData(HttpContext);
                var currentPortal = "admin";
                if (route != null)
                {
                    currentPortal = route.GetRequiredString("Portal");
                    ViewData["Portal"] = route.GetRequiredString("Portal");
                    var user = GetSettingUser();
                    if (user != null)
                    {
                        ViewData["Username"] = user.Username;
                        ViewData["GuidUser"] = user.Guid.ToString().ToLower().Trim();
                    }
                }

                var models = new List<SMenuModel>();
                var id = (FormsIdentity)User.Identity;
                FormsAuthenticationTicket authTicket = id.Ticket;
                var roleControls = DataGemini.SUsers.Where(s => s.Username.ToUpper().Equals(authTicket.Name.ToUpper())).Select(x => new { x.GuidRole }).FirstOrDefault();
                if (roleControls != null && (roleControls.GuidRole) != null)
                {
                    models = (from frc in DataGemini.FRoleControlMenus
                              join er in DataGemini.SRoles on frc.GuidRole equals er.Guid
                              join em in DataGemini.SMenus on frc.GuidMenu equals em.Guid
                              where frc.GuidRole == roleControls.GuidRole && em.Type == "ADMIN"
                              select new SMenuModel()
                              {
                                  Guid = em.Guid,
                                  Name = em.Name,
                                  Active = em.Active,
                                  Note = em.Note,
                                  FriendUrl = em.FriendUrl,
                                  GuidLanguage = em.GuidLanguage,
                                  GuidParent = em.GuidParent,
                                  Icon = string.IsNullOrEmpty(em.Icon) ? "001_Help" : em.Icon,
                                  LinkUrl = em.LinkUrl,
                                  OrderMenu = em.OrderMenu,
                                  RouterUrl = em.RouterUrl,
                                  Type = em.Type,
                              }).OrderBy(s => s.OrderMenu).ToList();
                }
                return View(models);
            }
            catch
            {
                return Redirect("/Error/ErrorList");
            }

        }

        /// <summary>
        /// Policy Toolbar
        /// </summary>
        /// <returns></returns>
        public ActionResult AToolbar()
        {
            try
            {
                var models = GetAllMainControlInMenu();

                Session["MainControls"] = models.Select(x => x.EventClick).ToList();

                return View(models);
            }
            catch
            {
                return Redirect("/Error/ErrorList");
            }
        }

        /// <summary>
        /// Policy Item 
        /// </summary>
        /// <returns></returns>
        public ActionResult AToolbarItem()
        {
            try
            {
                var models = GetAllEditControlInMenu();

                Session["EditControls"] = models.Select(x => x.EventClick).ToList();

                return View(models);
            }
            catch
            {
                return Redirect("/Error/ErrorList");
            }
        }

        /// <summary>
        /// Policy Tool Bar Under
        /// </summary>
        /// <returns></returns>
        public ActionResult AToolbarc()
        {
            try
            {
                var models = GetAllTabControlInMenu();

                Session["TabControls"] = models.Select(x => x.EventClick).ToList();

                return View(models);
            }
            catch
            {
                return Redirect("/Error/ErrorList");
            }
        }
    }
}