using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Gemini.Controllers.Bussiness;
using Gemini.Models;
using Gemini.Models._02_Cms.U;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Constants = Gemini.Controllers.Bussiness.Constants;

namespace Gemini.Controllers._02_Cms.U
{
    public class UNewsController : GeminiController
    {
        #region Main
        [Authorize]
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            GetSettingUser();
            return View();
        }

        public ActionResult Read([DataSourceRequest] DataSourceRequest request)
        {
            var user = GetSettingUser();

            List<UNewsModel> uNewsModel = (from un in DataGemini.UNews
                                           where un.CreatedBy == user.Username
                                                 || user.IsAdmin
                                           select new UNewsModel
                                           {
                                               Name = un.Name,
                                               Guid = un.Guid,
                                               SeoFriendUrl = un.SeoFriendUrl,
                                               SeoTitle = un.SeoTitle,
                                               SeoDescription = un.SeoDescription,
                                               SeoImage = un.SeoImage,
                                               ContentNews = un.ContentNews,
                                               TypeNews = un.TypeNews,
                                               GuidImage = un.GuidImage,
                                               Description = un.Description,
                                               CountView = un.CountView,
                                               Active = un.Active,
                                               Note = un.Note,
                                               Status = un.Status,
                                               CreatedAt = un.CreatedAt,
                                               CreatedBy = un.CreatedBy,
                                               UpdatedAt = un.UpdatedAt,
                                               UpdatedBy = un.UpdatedBy
                                           }).OrderBy(s => s.Name).ToList();

            uNewsModel.ForEach(x => x.StatusName = UNews_Status.dicDesc[x.Status]);

            DataSourceResult result = uNewsModel.ToDataSourceResult(request);
            return Json(result);
        }

        public ActionResult Create()
        {
            var uNew = new UNew();
            try
            {
                var tmp = DataGemini.SReplaceCodes.ToList();
                var viewModel = new UNewsModel(uNew) { IsUpdate = 0, Active = true, ReplaceCode = tmp };
                return PartialView("Edit", viewModel);
            }
            catch (Exception)
            {
                return Redirect("/Error/ErrorList");
            }
        }

        public ActionResult Edit(Guid guid)
        {
            var uNew = new UNew();
            try
            {
                var tmp = DataGemini.SReplaceCodes.ToList();
                uNew = DataGemini.UNews.FirstOrDefault(c => c.Guid == guid);
                var viewModel = new UNewsModel(uNew) { IsUpdate = 1, ReplaceCode = tmp };
                if (uNew != null) viewModel.ContentNews = HttpUtility.HtmlDecode(uNew.ContentNews);
                return PartialView("Edit", viewModel);
            }
            catch (Exception)
            {
                return Redirect("/Error/ErrorList");
            }
        }

        public ActionResult Copy(Guid guid)
        {
            var uNew = new UNew();
            var clone = new UNew();
            try
            {
                uNew = DataGemini.UNews.FirstOrDefault(c => c.Guid == guid);
                #region Copy

                DataGemini.UNews.Add(clone);
                //Copy values from source to clone
                var sourceValues = DataGemini.Entry(uNew).CurrentValues;
                DataGemini.Entry(clone).CurrentValues.SetValues(sourceValues);
                //Change values of the copied entity
                clone.Name = clone.Name + " - Copy";
                clone.Guid = Guid.NewGuid();
                clone.SeoFriendUrl = clone.Guid.ToString().ToLower();
                clone.CreatedAt = clone.UpdatedAt = DateTime.Now;
                clone.UpdatedBy = clone.CreatedBy = GetUserInSession();
                if (SaveData("UNews"))
                {
                    DataReturn.ActiveCode = clone.Guid.ToString();
                    DataReturn.StatusCode = Convert.ToInt16(HttpStatusCode.OK);
                }
                else
                {
                    DataReturn.StatusCode = Convert.ToInt16(HttpStatusCode.Conflict);
                    DataReturn.MessagError = Constants.CannotCopy + " Date : " + DateTime.Now;
                }
                #endregion
            }
            catch (Exception ex)
            {
                DataGemini.UNews.Remove(clone);
                HandleError(ex);
            }

            return Json(DataReturn, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(Guid guid)
        {
            var uNew = new UNew();
            try
            {
                uNew = DataGemini.UNews.FirstOrDefault(c => c.Guid == guid);
                DataGemini.UNews.Remove(uNew);
                if (SaveData("UNews") && uNew != null)
                {
                    DataReturn.ActiveCode = uNew.Guid.ToString();
                    DataReturn.StatusCode = Convert.ToInt16(HttpStatusCode.OK);
                }
                else
                {
                    DataReturn.StatusCode = Convert.ToInt16(HttpStatusCode.BadRequest);
                    DataReturn.MessagError = Constants.CannotDelete + " Date : " + DateTime.Now;
                }
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }

            return Json(DataReturn, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Update(UNewsModel viewModel)
        {
            var uNew = new UNew();
            try
            {
                var lstErrMsg = Validate_Update(viewModel);

                if (lstErrMsg.Count > 0)
                {
                    DataReturn.StatusCode = Convert.ToInt16(HttpStatusCode.Conflict);
                    DataReturn.MessagError = String.Join("<br/>", lstErrMsg);
                }
                else
                {
                    viewModel.UpdatedBy = viewModel.CreatedBy = GetUserInSession();
                    if (viewModel.IsUpdate == 0)
                    {
                        viewModel.Setvalue(uNew);
                        DataGemini.UNews.Add(uNew);
                    }
                    else if (viewModel.IsUpdate == 1)
                    {
                        uNew = DataGemini.UNews.FirstOrDefault(c => c.Guid == viewModel.Guid);
                        viewModel.Setvalue(uNew);
                    }
                    uNew.SeoFriendUrl = uNew.Guid.ToString();

                    if (SaveData("UNews") && uNew != null)
                    {
                        DataReturn.ActiveCode = uNew.Guid.ToString();
                        DataReturn.StatusCode = Convert.ToInt16(HttpStatusCode.OK);
                    }
                    else
                    {
                        DataReturn.StatusCode = Convert.ToInt16(HttpStatusCode.Conflict);
                        DataReturn.MessagError = Constants.CannotUpdate + " Date : " + DateTime.Now;
                    }
                }
            }
            catch (Exception ex)
            {
                if (viewModel.IsUpdate == 0)
                {
                    DataGemini.UNews.Remove(uNew);
                }
                HandleError(ex);
            }

            return Json(DataReturn, JsonRequestBehavior.AllowGet);
        }

        private List<string> Validate_Update(UNewsModel uNew)
        {
            List<string> lstErrMsg = new List<string>();

            if (uNew.Status >= UNews_Status.Approved)
            {
                lstErrMsg.Add("Tin đã qua bước kiểm định, không thể sửa!");
            }

            return lstErrMsg;
        }

        public ActionResult Approve(Guid guid)
        {
            var uNew = new UNew();
            try
            {
                uNew = DataGemini.UNews.FirstOrDefault(c => c.Guid == guid);
                var lstErrMsg = Validate_Approval(uNew);

                if (lstErrMsg.Count > 0)
                {
                    DataReturn.StatusCode = Convert.ToInt16(HttpStatusCode.Conflict);
                    DataReturn.MessagError = String.Join("<br/>", lstErrMsg);
                }
                else
                {
                    uNew.UpdatedAt = DateTime.Now;
                    uNew.UpdatedBy = GetUserInSession();
                    uNew.Status = UNews_Status.Approved;

                    if (SaveData("UNews") && uNew != null)
                    {
                        DataReturn.StatusCode = Convert.ToInt16(HttpStatusCode.OK);
                    }
                    else
                    {
                        DataReturn.StatusCode = Convert.ToInt16(HttpStatusCode.Conflict);
                        DataReturn.MessagError = Constants.CannotUpdate + " Date : " + DateTime.Now;
                    }
                }
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }

            return Json(DataReturn, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Reject(Guid guid)
        {
            var uNew = new UNew();
            try
            {
                uNew = DataGemini.UNews.FirstOrDefault(c => c.Guid == guid);
                var lstErrMsg = Validate_Approval(uNew);

                if (lstErrMsg.Count > 0)
                {
                    DataReturn.StatusCode = Convert.ToInt16(HttpStatusCode.Conflict);
                    DataReturn.MessagError = String.Join("<br/>", lstErrMsg);
                }
                else
                {
                    uNew.UpdatedAt = DateTime.Now;
                    uNew.UpdatedBy = GetUserInSession();
                    uNew.Status = UNews_Status.Reject;

                    if (SaveData("PosProcess") && uNew != null)
                    {
                        DataReturn.StatusCode = Convert.ToInt16(HttpStatusCode.OK);
                    }
                    else
                    {
                        DataReturn.StatusCode = Convert.ToInt16(HttpStatusCode.Conflict);
                        DataReturn.MessagError = Constants.CannotUpdate + " Date : " + DateTime.Now;
                    }
                }
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }

            return Json(DataReturn, JsonRequestBehavior.AllowGet);
        }

        private List<string> Validate_Approval(UNew uNew)
        {
            List<string> lstErrMsg = new List<string>();

            if (uNew.Status >= UNews_Status.Approved)
            {
                lstErrMsg.Add("Tin đã qua bước kiểm định, không thể sửa!");
            }

            return lstErrMsg;
        }
        #endregion
    }
}