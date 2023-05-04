using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Gemini.Controllers.Bussiness;
using Gemini.Models;
using Gemini.Models._02_Cms.U;
using Gemini.Models._05_Website;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using static Kendo.Mvc.UI.UIPrimitives;

namespace Gemini.Controllers._05_Website
{
    public class WSellController : GeminiController
    {
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        /// <summary>
        /// List view grid
        /// </summary>
        /// <returns></returns>
        public ActionResult List()
        {
            GetSettingUser();
            return View();
        }

        public ActionResult Read([DataSourceRequest] DataSourceRequest request)
        {
            var username = GetUserInSession();

            List<WOrderDetailModel> wOrderDetails = (from wod in DataGemini.WOrderDetails
                                                     join pp in DataGemini.PosProduces on wod.GuidProduce equals pp.Guid
                                                     join wo in DataGemini.WOrders on wod.GuidOrder equals wo.Guid
                                                     join su in DataGemini.SUsers on wo.GuidUser equals su.Guid
                                                     where wod.GuidOrder != null && pp.CreatedBy == username && pp.Active
                                                     select new WOrderDetailModel
                                                     {
                                                         OrderFullAddress = wo.FullAddress,
                                                         OrderFullName = su.FullName,
                                                         OrderMobile = wo.Mobile,
                                                         OrderCreatedAt = wo.CreatedAt,

                                                         ProduceCode = pp.Code,
                                                         ProduceName = pp.Name,
                                                         Guid = wod.Guid,
                                                         GuidOrder = wod.GuidOrder,
                                                         Quantity = wod.Quantity,
                                                         Price = wod.Price,
                                                         Status = wod.Status,
                                                         ListGallery = (from fr in DataGemini.FProduceGalleries
                                                                        join im in DataGemini.UGalleries on fr.GuidGallery equals im.Guid
                                                                        where fr.GuidProduce == pp.Guid
                                                                        select new UGalleryModel
                                                                        {
                                                                            Image = im.Image,
                                                                            CreatedAt = im.CreatedAt
                                                                        }).OrderBy(x => x.CreatedAt).Take(1).ToList(),
                                                     }).OrderByDescending(x => x.OrderCreatedAt).ToList();

            foreach (var item in wOrderDetails)
            {
                item.StatusName = WOrderDetail_Status.dicDesc[item.Status];
                item.OrderCode = item.GuidOrder.ToString();

                var tmpLinkImg = item.ListGallery;
                if (tmpLinkImg.Count == 0)
                {
                    item.ProduceLinkImg0 = "/Content/Custom/empty-album.png";
                }
                else
                {
                    item.ProduceLinkImg0 = tmpLinkImg[0].Image;
                }
            }

            DataSourceResult result = wOrderDetails.ToDataSourceResult(request);
            return Json(result);
        }

        public ActionResult Approve(Guid guid)
        {
            var wOrderDetail = new WOrderDetail();
            try
            {
                wOrderDetail = DataGemini.WOrderDetails.FirstOrDefault(c => c.Guid == guid);
                var lstErrMsg = Validate_Approval(wOrderDetail);

                if (lstErrMsg.Count > 0)
                {
                    DataReturn.StatusCode = Convert.ToInt16(HttpStatusCode.Conflict);
                    DataReturn.MessagError = String.Join("<br/>", lstErrMsg);
                }
                else
                {
                    wOrderDetail.Status = WOrderDetail_Status.Done;

                    if (SaveData("WOrderDetail") && wOrderDetail != null)
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

        private List<string> Validate_Approval(WOrderDetail wOrderDetail)
        {
            List<string> lstErrMsg = new List<string>();

            if (wOrderDetail.Status >= WOrderDetail_Status.Done)
            {
                lstErrMsg.Add("Đơn hàng đã hoàn thành, không thể sửa!");
            }

            return lstErrMsg;
        }
    }
}