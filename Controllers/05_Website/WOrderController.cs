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

namespace Gemini.Controllers._05_Website
{
    public class WOrderController : GeminiController
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

            List<WOrderModel> wOrders = (from wo in DataGemini.WOrders
                                         join su in DataGemini.SUsers on wo.GuidUser equals su.Guid
                                         where su.Username == username
                                         select new WOrderModel
                                         {
                                             Guid = wo.Guid,
                                             OrderCode = wo.Guid.ToString(),
                                             Username = su.Username,
                                             Mobile = wo.Mobile,
                                             FullAddress = wo.FullAddress,
                                             CreatedAt = wo.CreatedAt
                                         }).ToList();

            DataSourceResult result = wOrders.OrderByDescending(x => x.CreatedAt).ToDataSourceResult(request);
            return Json(result);
        }

        public ActionResult ReadTabc1([DataSourceRequest] DataSourceRequest request, string guid)
        {
            List<WOrderDetailModel> wOrderDetails = (from wod in DataGemini.WOrderDetails
                                                     join pp in DataGemini.PosProduces on wod.GuidProduce equals pp.Guid
                                                     where wod.GuidOrder != null && wod.GuidOrder.ToString().ToLower() == guid && pp.Active
                                                     select new WOrderDetailModel
                                                     {
                                                         ProduceCode = pp.Code,
                                                         ProduceName = pp.Name,
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
                                                     }).ToList();

            foreach (var item in wOrderDetails)
            {
                item.StatusName = WOrderDetail_Status.dicDesc[item.Status];

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
    }
}