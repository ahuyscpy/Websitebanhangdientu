using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Gemini.Controllers.Bussiness;
using Gemini.Models;
using Gemini.Models._01_Hethong;
using Gemini.Models._02_Cms.U;
using Gemini.Models._03_Pos;
using Gemini.Models._05_Website;
using Gemini.Models._20_Web;
using Newtonsoft.Json;

namespace Gemini.Controllers._20_Web.Home
{
    public class HomeCommonController : GeminiController
    {
        public string GetLanguageService()
        {
            return Request.Cookies["language"] != null ? (Request.Cookies["language"].Value) : ("VI");
        }

        [ChildActionOnly]
        public ActionResult Header()
        {
            ViewBag.CurrentUsername = GetUserInSession();

            var model = new HeaderModel();
            model.ListMenu = new List<SMenuModel>();

            try
            {
                model.ListMenu = (from menu in DataGemini.SMenus
                                  where menu.Active && menu.Type == "WEB"
                                  select new SMenuModel
                                  {
                                      LinkUrl = menu.LinkUrl,
                                      Name = menu.Name,
                                      OrderMenu = menu.OrderMenu,
                                  }).OrderBy(s => s.OrderMenu).ToList();

                var username = GetUserInSession();
                model.CurrentUsername = String.IsNullOrWhiteSpace(username) ? "Đăng nhập" : username;
            }
            catch (Exception ex)
            {
                model.ListMenu = new List<SMenuModel>();
            }

            return PartialView(model);
        }

        [ChildActionOnly]
        public ActionResult Footer()
        {
            var model = new FooterModel();
            model.ListPosCategory = new List<PosCategoryModel>();

            try
            {
                model.ListPosCategory = (from cat in DataGemini.PosCategories
                                         where cat.Active
                                         select new PosCategoryModel
                                         {
                                             SeoFriendUrl = cat.SeoFriendUrl,
                                             Name = cat.Name,
                                             OrderBy = cat.OrderBy
                                         }).OrderBy(s => s.OrderBy).ToList();
            }
            catch (Exception)
            {
                model.ListPosCategory = new List<PosCategoryModel>();
            }

            return PartialView(model);
        }

        #region Home

        public ActionResult PartialLatestProduct()
        {
            ViewBag.CurrentUsername = GetUserInSession();

            var model = new PartialLatestProductModel();

            model.ListPosProduce = (from pp in DataGemini.PosProduces
                                    join pc in DataGemini.PosCategories on pp.GuidCategory equals pc.Guid
                                    where pp.Active && pc.Active && pp.ApprovedStatus == PosProduce_ApprovedStatus.Approved
                                    select new PosProduceModel
                                    {
                                        Guid = pp.Guid,
                                        Name = pp.Name,
                                        NameCategory = pc.Name,
                                        CategorySeoFriendUrl = pc.SeoFriendUrl,
                                        SeoFriendUrl = pp.SeoFriendUrl,
                                        Price = pp.Price,
                                        Unit = pp.Unit,
                                        ListGallery = (from fr in DataGemini.FProduceGalleries
                                                       join im in DataGemini.UGalleries on fr.GuidGallery equals im.Guid
                                                       where fr.GuidProduce == pp.Guid
                                                       select new UGalleryModel
                                                       {
                                                           Image = im.Image,
                                                           CreatedAt = im.CreatedAt
                                                       }).OrderBy(x => x.CreatedAt).Take(1).ToList(),
                                        CreatedAt = pp.CreatedAt,
                                        CreatedBy = pp.CreatedBy,
                                        FullAddress = pp.FullAddress,
                                        Legit = pp.Legit,
                                        LegitCount = pp.LegitCount
                                    }).OrderByDescending(s => s.CreatedAt).Take(20).ToList();

            foreach (var item in model.ListPosProduce)
            {
                var tmpLinkImg = item.ListGallery;
                if (tmpLinkImg.Count == 0)
                {
                    item.LinkImg0 = "/Content/Custom/empty-album.png";
                }
                else
                {
                    item.LinkImg0 = tmpLinkImg[0].Image;
                }
            }

            if (model.ListPosProduce != null && model.ListPosProduce.Any())
            {
                model.ListPosCategory = model.ListPosProduce.GroupBy(x => new { x.NameCategory, x.CategorySeoFriendUrl }).Select(x => new PosCategoryModel()
                {
                    Name = x.Key.NameCategory,
                    SeoFriendUrl = x.Key.CategorySeoFriendUrl
                }).OrderBy(x => x.Name).ToList();
            }

            return PartialView(model);
        }

        public ActionResult PartialFeaturedProduct()
        {
            var model = new PartialFeaturedProductModel();

            model.ListPosProduce = (from pp in DataGemini.PosProduces
                                    join pc in DataGemini.PosCategories on pp.GuidCategory equals pc.Guid
                                    where pp.Active && pp.HotProduce && pc.Active && pp.ApprovedStatus == PosProduce_ApprovedStatus.Approved
                                    select new PosProduceModel
                                    {
                                        Guid = pp.Guid,
                                        Name = pp.Name,
                                        NameCategory = pc.Name,
                                        CategorySeoFriendUrl = pc.SeoFriendUrl,
                                        SeoFriendUrl = pp.SeoFriendUrl,
                                        Price = pp.Price,
                                        Unit = pp.Unit,
                                        ListGallery = (from fr in DataGemini.FProduceGalleries
                                                       join im in DataGemini.UGalleries on fr.GuidGallery equals im.Guid
                                                       where fr.GuidProduce == pp.Guid
                                                       select new UGalleryModel
                                                       {
                                                           Image = im.Image,
                                                           CreatedAt = im.CreatedAt
                                                       }).OrderBy(x => x.CreatedAt).Take(1).ToList(),
                                        FullAddress = pp.FullAddress,
                                        CreatedAt = pp.CreatedAt,
                                        Legit = pp.Legit,
                                        LegitCount = pp.LegitCount
                                    }).OrderBy(s => s.CreatedAt).Take(9).ToList();

            foreach (var item in model.ListPosProduce)
            {
                var tmpLinkImg = item.ListGallery;
                if (tmpLinkImg.Count == 0)
                {
                    item.LinkImg0 = "/Content/Custom/empty-album.png";
                }
                else
                {
                    item.LinkImg0 = tmpLinkImg[0].Image;
                }
            }

            return PartialView(model);
        }

        #endregion

        #region News List

        public ActionResult NewsList(string page)
        {
            int recordMax = 9;
            page = page ?? "page-1";
            string[] arrPage = page.Split('-');
            int numberPage = Convert.ToInt16(arrPage[1]) * recordMax;
            int numberPageActive = Convert.ToInt16(arrPage[1]);

            var models = new NewsListModel();
            models.ListNews = new List<UNewsModel>();

            models.ListPosCategory = (from cat in DataGemini.PosCategories
                                      where cat.Active
                                      select new PosCategoryModel
                                      {
                                          SeoFriendUrl = cat.SeoFriendUrl,
                                          Name = cat.Name,
                                          OrderBy = cat.OrderBy
                                      }).OrderBy(s => s.OrderBy).ToList();

            var listNews = (from un in DataGemini.UNews
                            join su in DataGemini.SUsers on un.CreatedBy equals su.Username
                            where un.Active && un.Status == UNews_Status.Approved
                            select new UNewsModel
                            {
                                Guid = un.Guid,
                                CreatedAt = un.CreatedAt,
                                CreatedBy = un.CreatedBy,
                                CreatedByFullName = su.FullName,
                                ContentNews = un.ContentNews,
                                Name = un.Name,
                                Note = un.Note,
                                SeoFriendUrl = un.SeoFriendUrl,
                                UrlImageFeatured = DefaultImage.ImageEmpty
                            }).OrderByDescending(x => x.CreatedAt);

            //Sent data to view caculate
            ViewData["perpage"] = recordMax;
            ViewData["total"] = listNews.Count();
            ViewData["pageActive"] = numberPageActive;

            //Check page start
            if (Convert.ToInt16(arrPage[1]) == 1)
            {
                numberPage = 0;
            }
            else
            {
                numberPage = numberPage - recordMax;
            }

            models.ListNews = listNews.Skip(numberPage).Take(recordMax).ToList();

            foreach (var item in models.ListNews)
            {
                try
                {
                    var content = HttpUtility.HtmlDecode(item.ContentNews);
                    HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                    doc.LoadHtml(content);
            
                    if (doc.DocumentNode.SelectSingleNode("//img") != null)
                    {
                        item.UrlImageFeatured = doc.DocumentNode.SelectSingleNode("//img").Attributes["src"].Value;
                    }                      
                    
                }
                catch
                {
                    item.UrlImageFeatured = DefaultImage.ImageEmpty;
                }
            }

            return View("~/Views/HomeCommon/NewsList.cshtml", models);
        }

        #endregion

        #region News Detail

        public ActionResult NewsDetail(string seoFriendUrl)
        {
            if (!string.IsNullOrEmpty(seoFriendUrl))
            {
                var models = new NewsDetailModel();

                models.ListPosCategory = (from cat in DataGemini.PosCategories
                                          where cat.Active
                                          select new PosCategoryModel
                                          {
                                              SeoFriendUrl = cat.SeoFriendUrl,
                                              Name = cat.Name,
                                              OrderBy = cat.OrderBy
                                          }).OrderBy(s => s.OrderBy).ToList();

                models.UNews = (from un in DataGemini.UNews
                                join su in DataGemini.SUsers on un.CreatedBy equals su.Username
                                where un.Active && un.Status == UNews_Status.Approved
                                select new UNewsModel
                                {
                                    Guid = un.Guid,
                                    CreatedAt = un.CreatedAt,
                                    CreatedBy = un.CreatedBy,
                                    CreatedByFullName = su.FullName,
                                    ContentNews = un.ContentNews,
                                    Name = un.Name,
                                    Note = un.Note,
                                    SeoFriendUrl = un.SeoFriendUrl,
                                    UrlImageFeatured = DefaultImage.ImageEmpty
                                }).FirstOrDefault();

                if (models.UNews != null)
                {
                    models.ListNewsSameCreatedBy = (from un in DataGemini.UNews
                                                    join su in DataGemini.SUsers on un.CreatedBy equals su.Username
                                                    where un.Active && un.Status == UNews_Status.Approved && un.CreatedBy == models.UNews.CreatedBy && un.Guid != models.UNews.Guid
                                                    select new UNewsModel
                                                    {
                                                        Guid = un.Guid,
                                                        CreatedAt = un.CreatedAt,
                                                        CreatedBy = un.CreatedBy,
                                                        CreatedByFullName = su.FullName,
                                                        ContentNews = un.ContentNews,
                                                        Name = un.Name,
                                                        Note = un.Note,
                                                        SeoFriendUrl = un.SeoFriendUrl,
                                                        UrlImageFeatured = DefaultImage.ImageEmpty
                                                    }).OrderByDescending(s => s.CreatedAt).Take(3).ToList();

                    foreach (var item in models.ListNewsSameCreatedBy)
                    {
                        try
                        {
                            var content = HttpUtility.HtmlDecode(item.ContentNews);
                            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                            doc.LoadHtml(content);
                            var link = doc.DocumentNode.SelectSingleNode("//img").Attributes["src"].Value;

                            item.UrlImageFeatured = link;
                        }
                        catch
                        {
                            item.UrlImageFeatured = DefaultImage.ImageEmpty;
                        }
                    }

                    return View("~/Views/HomeCommon/NewsDetail.cshtml", models);
                }
                else
                {
                    return View("~/Views/HomeCommon/Error_404.cshtml");
                }
            }

            return View("~/Views/HomeCommon/Error_404.cshtml");
        }

        #endregion

        #region Contact Us

        public ActionResult ContactUs()
        {
            var models = new ContactUsModel();

            models.ListPosCategory = (from cat in DataGemini.PosCategories
                                      where cat.Active
                                      select new PosCategoryModel
                                      {
                                          SeoFriendUrl = cat.SeoFriendUrl,
                                          Name = cat.Name,
                                          OrderBy = cat.OrderBy
                                      }).OrderBy(s => s.OrderBy).ToList();

            return View(models);
        }

        #endregion

        #region Produce List By Loved

        public ActionResult ProduceListByLoved(string page, string sortBy)
        {
            ViewBag.CurrentUsername = GetUserInSession();

            int recordMax = 12;
            page = page ?? "page-1";
            string[] arrPage = page.Split('-');
            int numberPage = Convert.ToInt16(arrPage[1]) * recordMax;
            int numberPageActive = Convert.ToInt16(arrPage[1]);

            var models = new ProduceListByLovedModel();
            models.ListPosProduceByLoved = new List<PosProduceModel>();

            models.ListPosCategory = (from cat in DataGemini.PosCategories
                                      where cat.Active
                                      select new PosCategoryModel
                                      {
                                          SeoFriendUrl = cat.SeoFriendUrl,
                                          Name = cat.Name,
                                          OrderBy = cat.OrderBy
                                      }).OrderBy(s => s.OrderBy).ToList();

            models.ListPosProduceLatest = (from pp in DataGemini.PosProduces
                                           join pc in DataGemini.PosCategories on pp.GuidCategory equals pc.Guid
                                           where pp.Active && pc.Active && pp.ApprovedStatus == PosProduce_ApprovedStatus.Approved
                                           select new PosProduceModel
                                           {
                                               Guid = pp.Guid,
                                               Name = pp.Name,
                                               NameCategory = pc.Name,
                                               CategorySeoFriendUrl = pc.SeoFriendUrl,
                                               SeoFriendUrl = pp.SeoFriendUrl,
                                               Price = pp.Price,
                                               Unit = pp.Unit,
                                               ListGallery = (from fr in DataGemini.FProduceGalleries
                                                              join im in DataGemini.UGalleries on fr.GuidGallery equals im.Guid
                                                              where fr.GuidProduce == pp.Guid
                                                              select new UGalleryModel
                                                              {
                                                                  Image = im.Image,
                                                                  CreatedAt = im.CreatedAt
                                                              }).OrderBy(x => x.CreatedAt).Take(1).ToList(),
                                               FullAddress = pp.FullAddress,
                                               CreatedAt = pp.CreatedAt,
                                               Legit = pp.Legit,
                                               LegitCount = pp.LegitCount
                                           }).OrderBy(s => s.CreatedAt).Take(9).ToList();

            foreach (var item in models.ListPosProduceLatest)
            {
                var tmpLinkImg = item.ListGallery;
                if (tmpLinkImg.Count == 0)
                {
                    item.LinkImg0 = "/Content/Custom/empty-album.png";
                }
                else
                {
                    item.LinkImg0 = tmpLinkImg[0].Image;
                }
            }

            if (Request.Cookies["loveProduce"] != null)
            {
                var lstGuidProduceString = string.IsNullOrEmpty(Request.Cookies["loveProduce"].Value) ? String.Empty : Request.Cookies["loveProduce"].Value;
                var lstGuidProduce = lstGuidProduceString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.ToLower().Trim()).ToList();
                if (lstGuidProduce != null && lstGuidProduce.Any())
                {
                    var listPosProduceByLoved = from pp in DataGemini.PosProduces
                                                join pc in DataGemini.PosCategories on pp.GuidCategory equals pc.Guid
                                                where pp.Active && pc.Active && pp.ApprovedStatus == PosProduce_ApprovedStatus.Approved && lstGuidProduce.Contains(pp.Guid.ToString().ToLower().Trim())
                                                select new PosProduceModel
                                                {
                                                    Guid = pp.Guid,
                                                    Name = pp.Name,
                                                    NameCategory = pc.Name,
                                                    CategorySeoFriendUrl = pc.SeoFriendUrl,
                                                    SeoFriendUrl = pp.SeoFriendUrl,
                                                    Price = pp.Price,
                                                    Unit = pp.Unit,
                                                    ListGallery = (from fr in DataGemini.FProduceGalleries
                                                                   join im in DataGemini.UGalleries on fr.GuidGallery equals im.Guid
                                                                   where fr.GuidProduce == pp.Guid
                                                                   select new UGalleryModel
                                                                   {
                                                                       Image = im.Image,
                                                                       CreatedAt = im.CreatedAt
                                                                   }).OrderBy(x => x.CreatedAt).Take(1).ToList(),
                                                    FullAddress = pp.FullAddress,
                                                    CreatedAt = pp.CreatedAt,
                                                    Legit = pp.Legit,
                                                    LegitCount = pp.LegitCount
                                                };

                    if (!string.IsNullOrEmpty(sortBy))
                    {
                        switch (sortBy)
                        {
                            case "newest":
                                listPosProduceByLoved = listPosProduceByLoved.OrderByDescending(x => x.CreatedAt);
                                break;
                            case "oldest":
                                listPosProduceByLoved = listPosProduceByLoved.OrderBy(x => x.CreatedAt);
                                break;
                            case "a-z":
                                listPosProduceByLoved = listPosProduceByLoved.OrderBy(x => x.Name);
                                break;
                            case "z-a":
                                listPosProduceByLoved = listPosProduceByLoved.OrderByDescending(x => x.Name);
                                break;
                            case "priceH-L":
                                listPosProduceByLoved = listPosProduceByLoved.OrderByDescending(x => x.Price);
                                break;
                            case "priceL-H":
                                listPosProduceByLoved = listPosProduceByLoved.OrderBy(x => x.Price);
                                break;
                            default:
                                listPosProduceByLoved = listPosProduceByLoved.OrderByDescending(x => x.CreatedAt);
                                break;
                        }
                    }
                    else
                    {
                        listPosProduceByLoved = listPosProduceByLoved.OrderByDescending(x => x.CreatedAt);
                    }

                    //Sent data to view caculate
                    ViewData["perpage"] = recordMax;
                    ViewData["total"] = listPosProduceByLoved.Count();
                    ViewData["pageActive"] = numberPageActive;

                    //Check page start
                    if (Convert.ToInt16(arrPage[1]) == 1)
                    {
                        numberPage = 0;
                    }
                    else
                    {
                        numberPage = numberPage - recordMax;
                    }

                    models.ListPosProduceByLoved = listPosProduceByLoved.Skip(numberPage).Take(recordMax).ToList();

                    foreach (var item in models.ListPosProduceByLoved)
                    {
                        var tmpLinkImg = item.ListGallery;
                        if (tmpLinkImg.Count == 0)
                        {
                            item.LinkImg0 = "/Content/Custom/empty-album.png";
                        }
                        else
                        {
                            item.LinkImg0 = tmpLinkImg[0].Image;
                        }
                    }

                    return View("~/Views/HomeCommon/ProduceListByLoved.cshtml", models);
                }
                else
                {
                    //Sent data to view caculate
                    ViewData["perpage"] = recordMax;
                    ViewData["total"] = 0;
                    ViewData["pageActive"] = numberPageActive;

                    return View("~/Views/HomeCommon/ProduceListByLoved.cshtml", models);
                }
            }
            else
            {
                return View("~/Views/HomeCommon/Error_404.cshtml");
            }
        }

        #endregion

        #region Produce List By CreatedBy

        public ActionResult ProduceListByCreatedBy(string createdBy, string page, string sortBy)
        {
            ViewBag.CurrentUsername = GetUserInSession();

            if (!string.IsNullOrEmpty(createdBy))
            {
                int recordMax = 12;
                page = page ?? "page-1";
                string[] arrPage = page.Split('-');
                int numberPage = Convert.ToInt16(arrPage[1]) * recordMax;
                int numberPageActive = Convert.ToInt16(arrPage[1]);

                var models = new ProduceListByCreatedByModel();

                models.ListPosCategory = new List<PosCategoryModel>();
                models.ListPosProduceLatest = new List<PosProduceModel>();
                models.ListPosProduceByCreatedBy = new List<PosProduceModel>();

                models.ListPosCategory = (from cat in DataGemini.PosCategories
                                          where cat.Active
                                          select new PosCategoryModel
                                          {
                                              SeoFriendUrl = cat.SeoFriendUrl,
                                              Name = cat.Name,
                                              OrderBy = cat.OrderBy
                                          }).OrderBy(s => s.OrderBy).ToList();

                models.ListPosProduceLatest = (from pp in DataGemini.PosProduces
                                               join pc in DataGemini.PosCategories on pp.GuidCategory equals pc.Guid
                                               where pp.Active && pc.Active && pp.ApprovedStatus == PosProduce_ApprovedStatus.Approved
                                               select new PosProduceModel
                                               {
                                                   Guid = pp.Guid,
                                                   Name = pp.Name,
                                                   NameCategory = pc.Name,
                                                   CategorySeoFriendUrl = pc.SeoFriendUrl,
                                                   SeoFriendUrl = pp.SeoFriendUrl,
                                                   Price = pp.Price,
                                                   Unit = pp.Unit,
                                                   ListGallery = (from fr in DataGemini.FProduceGalleries
                                                                  join im in DataGemini.UGalleries on fr.GuidGallery equals im.Guid
                                                                  where fr.GuidProduce == pp.Guid
                                                                  select new UGalleryModel
                                                                  {
                                                                      Image = im.Image,
                                                                      CreatedAt = im.CreatedAt
                                                                  }).OrderBy(x => x.CreatedAt).Take(1).ToList(),
                                                   FullAddress = pp.FullAddress,
                                                   CreatedAt = pp.CreatedAt,
                                                   Legit = pp.Legit,
                                                   LegitCount = pp.LegitCount
                                               }).OrderBy(s => s.CreatedAt).Take(9).ToList();

                foreach (var item in models.ListPosProduceLatest)
                {
                    var tmpLinkImg = item.ListGallery;
                    if (tmpLinkImg.Count == 0)
                    {
                        item.LinkImg0 = "/Content/Custom/empty-album.png";
                    }
                    else
                    {
                        item.LinkImg0 = tmpLinkImg[0].Image;
                    }
                }

                models.PosProduceCreatedBy = new SUserModel(DataGemini.SUsers.FirstOrDefault(x => x.Username == createdBy));
                if (string.IsNullOrEmpty(models.PosProduceCreatedBy.Avartar))
                {
                    models.PosProduceCreatedBy.Avartar = DefaultImage.ImageEmpty;
                    models.PosProduceCreatedBy.CountFollow = DataGemini.SUsers.Count(x => x.GuidFollow.Contains(models.PosProduceCreatedBy.Guid.ToString().ToLower().Trim()));
                }

                if (models.PosProduceCreatedBy != null)
                {
                    var listPosProduceByCreatedBy = from pp in DataGemini.PosProduces
                                                    join pc in DataGemini.PosCategories on pp.GuidCategory equals pc.Guid
                                                    where pp.Active && pc.Active && pp.ApprovedStatus == PosProduce_ApprovedStatus.Approved && pp.CreatedBy == models.PosProduceCreatedBy.Username
                                                    select new PosProduceModel
                                                    {
                                                        Guid = pp.Guid,
                                                        Name = pp.Name,
                                                        NameCategory = pc.Name,
                                                        CategorySeoFriendUrl = pc.SeoFriendUrl,
                                                        SeoFriendUrl = pp.SeoFriendUrl,
                                                        Price = pp.Price,
                                                        Unit = pp.Unit,
                                                        ListGallery = (from fr in DataGemini.FProduceGalleries
                                                                       join im in DataGemini.UGalleries on fr.GuidGallery equals im.Guid
                                                                       where fr.GuidProduce == pp.Guid
                                                                       select new UGalleryModel
                                                                       {
                                                                           Image = im.Image,
                                                                           CreatedAt = im.CreatedAt
                                                                       }).OrderBy(x => x.CreatedAt).Take(1).ToList(),
                                                        FullAddress = pp.FullAddress,
                                                        CreatedAt = pp.CreatedAt,
                                                        Legit = pp.Legit,
                                                        LegitCount = pp.LegitCount
                                                    };

                    if (!string.IsNullOrEmpty(sortBy))
                    {
                        switch (sortBy)
                        {
                            case "newest":
                                listPosProduceByCreatedBy = listPosProduceByCreatedBy.OrderByDescending(x => x.CreatedAt);
                                break;
                            case "oldest":
                                listPosProduceByCreatedBy = listPosProduceByCreatedBy.OrderBy(x => x.CreatedAt);
                                break;
                            case "a-z":
                                listPosProduceByCreatedBy = listPosProduceByCreatedBy.OrderBy(x => x.Name);
                                break;
                            case "z-a":
                                listPosProduceByCreatedBy = listPosProduceByCreatedBy.OrderByDescending(x => x.Name);
                                break;
                            case "priceH-L":
                                listPosProduceByCreatedBy = listPosProduceByCreatedBy.OrderByDescending(x => x.Price);
                                break;
                            case "priceL-H":
                                listPosProduceByCreatedBy = listPosProduceByCreatedBy.OrderBy(x => x.Price);
                                break;
                            default:
                                listPosProduceByCreatedBy = listPosProduceByCreatedBy.OrderByDescending(x => x.CreatedAt);
                                break;
                        }
                    }
                    else
                    {
                        listPosProduceByCreatedBy = listPosProduceByCreatedBy.OrderByDescending(x => x.CreatedAt);
                    }

                    //Sent data to view caculate
                    ViewData["perpage"] = recordMax;
                    ViewData["total"] = listPosProduceByCreatedBy.Count();
                    ViewData["pageActive"] = numberPageActive;

                    //Check page start
                    if (Convert.ToInt16(arrPage[1]) == 1)
                    {
                        numberPage = 0;
                    }
                    else
                    {
                        numberPage = numberPage - recordMax;
                    }

                    models.ListPosProduceByCreatedBy = listPosProduceByCreatedBy.Skip(numberPage).Take(recordMax).ToList();

                    foreach (var item in models.ListPosProduceByCreatedBy)
                    {
                        var tmpLinkImg = item.ListGallery;
                        if (tmpLinkImg.Count == 0)
                        {
                            item.LinkImg0 = "/Content/Custom/empty-album.png";
                        }
                        else
                        {
                            item.LinkImg0 = tmpLinkImg[0].Image;
                        }
                    }

                    return View("~/Views/HomeCommon/ProduceListByCreatedBy.cshtml", models);
                }
                else
                {
                    return View("~/Views/HomeCommon/Error_404.cshtml");
                }
            }

            return View("~/Views/HomeCommon/Error_404.cshtml");
        }

        #endregion

        #region Produce List By Search

        public ActionResult ProduceListBySearch(string keyWord, string page, string sortBy)
        {
            ViewBag.CurrentUsername = GetUserInSession();

            int recordMax = 12;
            page = page ?? "page-1";
            string[] arrPage = page.Split('-');
            int numberPage = Convert.ToInt16(arrPage[1]) * recordMax;
            int numberPageActive = Convert.ToInt16(arrPage[1]);

            var models = new ProduceListBySearchModel();
            models.ListPosProduceBySearch = new List<PosProduceModel>();
            models.KeyWord = keyWord;

            models.ListPosCategory = (from cat in DataGemini.PosCategories
                                      where cat.Active
                                      select new PosCategoryModel
                                      {
                                          SeoFriendUrl = cat.SeoFriendUrl,
                                          Name = cat.Name,
                                          OrderBy = cat.OrderBy
                                      }).OrderBy(s => s.OrderBy).ToList();

            models.ListPosProduceLatest = (from pp in DataGemini.PosProduces
                                           join pc in DataGemini.PosCategories on pp.GuidCategory equals pc.Guid
                                           where pp.Active && pc.Active && pp.ApprovedStatus == PosProduce_ApprovedStatus.Approved
                                           select new PosProduceModel
                                           {
                                               Guid = pp.Guid,
                                               Name = pp.Name,
                                               NameCategory = pc.Name,
                                               CategorySeoFriendUrl = pc.SeoFriendUrl,
                                               SeoFriendUrl = pp.SeoFriendUrl,
                                               Price = pp.Price,
                                               Unit = pp.Unit,
                                               ListGallery = (from fr in DataGemini.FProduceGalleries
                                                              join im in DataGemini.UGalleries on fr.GuidGallery equals im.Guid
                                                              where fr.GuidProduce == pp.Guid
                                                              select new UGalleryModel
                                                              {
                                                                  Image = im.Image,
                                                                  CreatedAt = im.CreatedAt
                                                              }).OrderBy(x => x.CreatedAt).Take(1).ToList(),
                                               FullAddress = pp.FullAddress,
                                               CreatedAt = pp.CreatedAt,
                                               Legit = pp.Legit,
                                               LegitCount = pp.LegitCount
                                           }).OrderBy(s => s.CreatedAt).Take(9).ToList();

            foreach (var item in models.ListPosProduceLatest)
            {
                var tmpLinkImg = item.ListGallery;
                if (tmpLinkImg.Count == 0)
                {
                    item.LinkImg0 = "/Content/Custom/empty-album.png";
                }
                else
                {
                    item.LinkImg0 = tmpLinkImg[0].Image;
                }
            }

            if (!string.IsNullOrEmpty(keyWord))
            {
                var listPosProduceBySearch = from pp in DataGemini.PosProduces
                                             join pc in DataGemini.PosCategories on pp.GuidCategory equals pc.Guid
                                             where pp.Active && pc.Active && pp.ApprovedStatus == PosProduce_ApprovedStatus.Approved
                                             && (pp.Name.Contains(keyWord)
                                                || pp.Note.Contains(keyWord)
                                                || pp.Description.Contains(keyWord)
                                                || pp.CreatedBy.Contains(keyWord)
                                                || pp.FullAddress.Contains(keyWord)
                                                || pc.Name.Contains(keyWord))
                                             select new PosProduceModel
                                             {
                                                 Guid = pp.Guid,
                                                 Name = pp.Name,
                                                 NameCategory = pc.Name,
                                                 CategorySeoFriendUrl = pc.SeoFriendUrl,
                                                 SeoFriendUrl = pp.SeoFriendUrl,
                                                 Price = pp.Price,
                                                 Unit = pp.Unit,
                                                 ListGallery = (from fr in DataGemini.FProduceGalleries
                                                                join im in DataGemini.UGalleries on fr.GuidGallery equals im.Guid
                                                                where fr.GuidProduce == pp.Guid
                                                                select new UGalleryModel
                                                                {
                                                                    Image = im.Image,
                                                                    CreatedAt = im.CreatedAt
                                                                }).OrderBy(x => x.CreatedAt).Take(1).ToList(),
                                                 FullAddress = pp.FullAddress,
                                                 CreatedAt = pp.CreatedAt,
                                                 Legit = pp.Legit,
                                                 LegitCount = pp.LegitCount
                                             };

                if (!string.IsNullOrEmpty(sortBy))
                {
                    switch (sortBy)
                    {
                        case "newest":
                            listPosProduceBySearch = listPosProduceBySearch.OrderByDescending(x => x.CreatedAt);
                            break;
                        case "oldest":
                            listPosProduceBySearch = listPosProduceBySearch.OrderBy(x => x.CreatedAt);
                            break;
                        case "a-z":
                            listPosProduceBySearch = listPosProduceBySearch.OrderBy(x => x.Name);
                            break;
                        case "z-a":
                            listPosProduceBySearch = listPosProduceBySearch.OrderByDescending(x => x.Name);
                            break;
                        case "priceH-L":
                            listPosProduceBySearch = listPosProduceBySearch.OrderByDescending(x => x.Price);
                            break;
                        case "priceL-H":
                            listPosProduceBySearch = listPosProduceBySearch.OrderBy(x => x.Price);
                            break;
                        default:
                            listPosProduceBySearch = listPosProduceBySearch.OrderByDescending(x => x.CreatedAt);
                            break;
                    }
                }
                else
                {
                    listPosProduceBySearch = listPosProduceBySearch.OrderByDescending(x => x.CreatedAt);
                }

                //Sent data to view caculate
                ViewData["perpage"] = recordMax;
                ViewData["total"] = listPosProduceBySearch.Count();
                ViewData["pageActive"] = numberPageActive;

                //Check page start
                if (Convert.ToInt16(arrPage[1]) == 1)
                {
                    numberPage = 0;
                }
                else
                {
                    numberPage = numberPage - recordMax;
                }

                models.ListPosProduceBySearch = listPosProduceBySearch.Skip(numberPage).Take(recordMax).ToList();

                foreach (var item in models.ListPosProduceBySearch)
                {
                    var tmpLinkImg = item.ListGallery;
                    if (tmpLinkImg.Count == 0)
                    {
                        item.LinkImg0 = "/Content/Custom/empty-album.png";
                    }
                    else
                    {
                        item.LinkImg0 = tmpLinkImg[0].Image;
                    }
                }

                return View("~/Views/HomeCommon/ProduceListBySearch.cshtml", models);
            }
            else
            {
                //Sent data to view caculate
                ViewData["perpage"] = recordMax;
                ViewData["total"] = 0;
                ViewData["pageActive"] = numberPageActive;

                return View("~/Views/HomeCommon/ProduceListBySearch.cshtml", models);
            }
        }

        #endregion

        #region Produce List By Category

        public ActionResult ProduceListByCategory(string seoFriendUrl, string page, string sortBy)
        {
            ViewBag.CurrentUsername = GetUserInSession();

            if (!string.IsNullOrEmpty(seoFriendUrl))
            {
                int recordMax = 12;
                page = page ?? "page-1";
                string[] arrPage = page.Split('-');
                int numberPage = Convert.ToInt16(arrPage[1]) * recordMax;
                int numberPageActive = Convert.ToInt16(arrPage[1]);

                var models = new ProduceListByCategoryModel();

                models.PosCategory = new PosCategoryModel();
                models.ListPosCategory = new List<PosCategoryModel>();
                models.ListPosProduceLatest = new List<PosProduceModel>();
                models.ListPosProduceByCategory = new List<PosProduceModel>();

                models.ListPosCategory = (from cat in DataGemini.PosCategories
                                          where cat.Active
                                          select new PosCategoryModel
                                          {
                                              SeoFriendUrl = cat.SeoFriendUrl,
                                              Name = cat.Name,
                                              OrderBy = cat.OrderBy
                                          }).OrderBy(s => s.OrderBy).ToList();

                models.ListPosProduceLatest = (from pp in DataGemini.PosProduces
                                               join pc in DataGemini.PosCategories on pp.GuidCategory equals pc.Guid
                                               where pp.Active && pc.Active && pp.ApprovedStatus == PosProduce_ApprovedStatus.Approved
                                               select new PosProduceModel
                                               {
                                                   Guid = pp.Guid,
                                                   Name = pp.Name,
                                                   NameCategory = pc.Name,
                                                   CategorySeoFriendUrl = pc.SeoFriendUrl,
                                                   SeoFriendUrl = pp.SeoFriendUrl,
                                                   Price = pp.Price,
                                                   Unit = pp.Unit,
                                                   ListGallery = (from fr in DataGemini.FProduceGalleries
                                                                  join im in DataGemini.UGalleries on fr.GuidGallery equals im.Guid
                                                                  where fr.GuidProduce == pp.Guid
                                                                  select new UGalleryModel
                                                                  {
                                                                      Image = im.Image,
                                                                      CreatedAt = im.CreatedAt
                                                                  }).OrderBy(x => x.CreatedAt).Take(1).ToList(),
                                                   FullAddress = pp.FullAddress,
                                                   CreatedAt = pp.CreatedAt,
                                                   Legit = pp.Legit,
                                                   LegitCount = pp.LegitCount
                                               }).OrderBy(s => s.CreatedAt).Take(9).ToList();

                foreach (var item in models.ListPosProduceLatest)
                {
                    var tmpLinkImg = item.ListGallery;
                    if (tmpLinkImg.Count == 0)
                    {
                        item.LinkImg0 = "/Content/Custom/empty-album.png";
                    }
                    else
                    {
                        item.LinkImg0 = tmpLinkImg[0].Image;
                    }
                }

                models.PosCategory = (from cat in DataGemini.PosCategories
                                      where cat.Active && cat.SeoFriendUrl.ToLower().Trim() == seoFriendUrl
                                      select new PosCategoryModel
                                      {
                                          Guid = cat.Guid,
                                          SeoFriendUrl = cat.SeoFriendUrl,
                                          Name = cat.Name
                                      }).FirstOrDefault();

                if (models.PosCategory != null)
                {
                    var listPosProduceByCategory = from pp in DataGemini.PosProduces
                                                   join pc in DataGemini.PosCategories on pp.GuidCategory equals pc.Guid
                                                   where pp.Active && pc.Active && pp.ApprovedStatus == PosProduce_ApprovedStatus.Approved && pp.GuidCategory == models.PosCategory.Guid
                                                   select new PosProduceModel
                                                   {
                                                       Guid = pp.Guid,
                                                       Name = pp.Name,
                                                       NameCategory = pc.Name,
                                                       CategorySeoFriendUrl = pc.SeoFriendUrl,
                                                       SeoFriendUrl = pp.SeoFriendUrl,
                                                       Price = pp.Price,
                                                       Unit = pp.Unit,
                                                       ListGallery = (from fr in DataGemini.FProduceGalleries
                                                                      join im in DataGemini.UGalleries on fr.GuidGallery equals im.Guid
                                                                      where fr.GuidProduce == pp.Guid
                                                                      select new UGalleryModel
                                                                      {
                                                                          Image = im.Image,
                                                                          CreatedAt = im.CreatedAt
                                                                      }).OrderBy(x => x.CreatedAt).Take(1).ToList(),
                                                       FullAddress = pp.FullAddress,
                                                       CreatedAt = pp.CreatedAt,
                                                       Legit = pp.Legit,
                                                       LegitCount = pp.LegitCount
                                                   };

                    if (!string.IsNullOrEmpty(sortBy))
                    {
                        switch (sortBy)
                        {
                            case "newest":
                                listPosProduceByCategory = listPosProduceByCategory.OrderByDescending(x => x.CreatedAt);
                                break;
                            case "oldest":
                                listPosProduceByCategory = listPosProduceByCategory.OrderBy(x => x.CreatedAt);
                                break;
                            case "a-z":
                                listPosProduceByCategory = listPosProduceByCategory.OrderBy(x => x.Name);
                                break;
                            case "z-a":
                                listPosProduceByCategory = listPosProduceByCategory.OrderByDescending(x => x.Name);
                                break;
                            case "priceH-L":
                                listPosProduceByCategory = listPosProduceByCategory.OrderByDescending(x => x.Price);
                                break;
                            case "priceL-H":
                                listPosProduceByCategory = listPosProduceByCategory.OrderBy(x => x.Price);
                                break;
                            default:
                                listPosProduceByCategory = listPosProduceByCategory.OrderByDescending(x => x.CreatedAt);
                                break;
                        }
                    }
                    else
                    {
                        listPosProduceByCategory = listPosProduceByCategory.OrderByDescending(x => x.CreatedAt);
                    }

                    //Sent data to view caculate
                    ViewData["perpage"] = recordMax;
                    ViewData["total"] = listPosProduceByCategory.Count();
                    ViewData["pageActive"] = numberPageActive;

                    //Check page start
                    if (Convert.ToInt16(arrPage[1]) == 1)
                    {
                        numberPage = 0;
                    }
                    else
                    {
                        numberPage = numberPage - recordMax;
                    }

                    models.ListPosProduceByCategory = listPosProduceByCategory.Skip(numberPage).Take(recordMax).ToList();

                    foreach (var item in models.ListPosProduceByCategory)
                    {
                        var tmpLinkImg = item.ListGallery;
                        if (tmpLinkImg.Count == 0)
                        {
                            item.LinkImg0 = "/Content/Custom/empty-album.png";
                        }
                        else
                        {
                            item.LinkImg0 = tmpLinkImg[0].Image;
                        }
                    }

                    return View("~/Views/HomeCommon/ProduceListByCategory.cshtml", models);
                }
                else
                {
                    return View("~/Views/HomeCommon/Error_404.cshtml");
                }
            }

            return View("~/Views/HomeCommon/Error_404.cshtml");
        }

        #endregion

        #region Produce Detail

        public ActionResult ProduceDetail(string seoFriendUrl)
        {
            ViewBag.CurrentUsername = GetUserInSession();

            if (!string.IsNullOrEmpty(seoFriendUrl))
            {
                var models = new ProduceDetailModel();

                models.SUser = GetSettingUser();

                models.ListPosCategory = (from cat in DataGemini.PosCategories
                                          where cat.Active
                                          select new PosCategoryModel
                                          {
                                              SeoFriendUrl = cat.SeoFriendUrl,
                                              Name = cat.Name,
                                              OrderBy = cat.OrderBy
                                          }).OrderBy(s => s.OrderBy).ToList();

                models.PosProduce = (from pp in DataGemini.PosProduces
                                     join pc in DataGemini.PosCategories on pp.GuidCategory equals pc.Guid
                                     where pp.Active && pp.ApprovedStatus == PosProduce_ApprovedStatus.Approved && pp.SeoFriendUrl.ToLower().Trim() == seoFriendUrl
                                     select new PosProduceModel
                                     {
                                         Guid = pp.Guid,
                                         Name = pp.Name,
                                         ListGallery = (from fr in DataGemini.FProduceGalleries
                                                        join im in DataGemini.UGalleries on fr.GuidGallery equals im.Guid
                                                        where fr.GuidProduce == pp.Guid
                                                        select new UGalleryModel
                                                        {
                                                            Image = im.Image
                                                        }).ToList(),
                                         Legit = pp.Legit,
                                         LegitCount = pp.LegitCount,
                                         Price = pp.Price,
                                         Unit = pp.Unit,
                                         Note = pp.Note,
                                         Description = pp.Description,
                                         GuidCategory = pp.GuidCategory,
                                         CreatedBy = pp.CreatedBy,
                                         NameCategory = pc.Name,
                                         CategorySeoFriendUrl = pc.SeoFriendUrl,
                                         Latitude = pp.Latitude,
                                         Longitude = pp.Longitude,
                                     }).FirstOrDefault();

                if (models.PosProduce != null)
                {
                    models.PosProduceCreatedBy = DataGemini.SUsers.FirstOrDefault(x => x.Username == models.PosProduce.CreatedBy);
                    if (string.IsNullOrEmpty(models.PosProduceCreatedBy.Avartar))
                    {
                        models.PosProduceCreatedBy.Avartar = DefaultImage.ImageEmpty;
                    }

                    models.ListProduceSameCreatedBy = (from pp in DataGemini.PosProduces
                                                       join pc in DataGemini.PosCategories on pp.GuidCategory equals pc.Guid
                                                       where pp.Active && pc.Active && pp.ApprovedStatus == PosProduce_ApprovedStatus.Approved && pp.CreatedBy == models.PosProduce.CreatedBy && pp.Guid != models.PosProduce.Guid
                                                       select new PosProduceModel
                                                       {
                                                           Guid = pp.Guid,
                                                           Name = pp.Name,
                                                           NameCategory = pc.Name,
                                                           CategorySeoFriendUrl = pc.SeoFriendUrl,
                                                           SeoFriendUrl = pp.SeoFriendUrl,
                                                           Price = pp.Price,
                                                           Unit = pp.Unit,
                                                           ListGallery = (from fr in DataGemini.FProduceGalleries
                                                                          join im in DataGemini.UGalleries on fr.GuidGallery equals im.Guid
                                                                          where fr.GuidProduce == pp.Guid
                                                                          select new UGalleryModel
                                                                          {
                                                                              Image = im.Image,
                                                                              CreatedAt = im.CreatedAt
                                                                          }).OrderBy(x => x.CreatedAt).Take(1).ToList(),
                                                           CreatedAt = pp.CreatedAt,
                                                           CreatedBy = pp.CreatedBy,
                                                           FullAddress = pp.FullAddress,
                                                           Legit = pp.Legit,
                                                           LegitCount = pp.LegitCount
                                                       }).OrderByDescending(s => s.CreatedAt).Take(4).ToList();

                    foreach (var item in models.ListProduceSameCreatedBy)
                    {
                        var tmpLinkImg = item.ListGallery;
                        if (tmpLinkImg.Count == 0)
                        {
                            item.LinkImg0 = "/Content/Custom/empty-album.png";
                        }
                        else
                        {
                            item.LinkImg0 = tmpLinkImg[0].Image;
                        }
                    }

                    models.ListProduceSameCategory = (from pp in DataGemini.PosProduces
                                                      join pc in DataGemini.PosCategories on pp.GuidCategory equals pc.Guid
                                                      where pp.Active && pc.Active && pp.ApprovedStatus == PosProduce_ApprovedStatus.Approved && pp.GuidCategory == models.PosProduce.GuidCategory && pp.Guid != models.PosProduce.Guid
                                                      select new PosProduceModel
                                                      {
                                                          Guid = pp.Guid,
                                                          Name = pp.Name,
                                                          NameCategory = pc.Name,
                                                          CategorySeoFriendUrl = pc.SeoFriendUrl,
                                                          SeoFriendUrl = pp.SeoFriendUrl,
                                                          Price = pp.Price,
                                                          Unit = pp.Unit,
                                                          ListGallery = (from fr in DataGemini.FProduceGalleries
                                                                         join im in DataGemini.UGalleries on fr.GuidGallery equals im.Guid
                                                                         where fr.GuidProduce == pp.Guid
                                                                         select new UGalleryModel
                                                                         {
                                                                             Image = im.Image,
                                                                             CreatedAt = im.CreatedAt
                                                                         }).OrderBy(x => x.CreatedAt).Take(1).ToList(),
                                                          CreatedAt = pp.CreatedAt,
                                                          CreatedBy = pp.CreatedBy,
                                                          FullAddress = pp.FullAddress,
                                                          Legit = pp.Legit,
                                                          LegitCount = pp.LegitCount
                                                      }).OrderByDescending(s => s.CreatedAt).Take(4).ToList();

                    foreach (var item in models.ListProduceSameCategory)
                    {
                        var tmpLinkImg = item.ListGallery;
                        if (tmpLinkImg.Count == 0)
                        {
                            item.LinkImg0 = "/Content/Custom/empty-album.png";
                        }
                        else
                        {
                            item.LinkImg0 = tmpLinkImg[0].Image;
                        }
                    }

                    models.NewRatingProduce = new WRatingProduceModel()
                    {
                        GuidProduce = models.PosProduce.Guid
                    };

                    return View("~/Views/HomeCommon/ProduceDetail.cshtml", models);
                }
                else
                {
                    return View("~/Views/HomeCommon/Error_404.cshtml");
                }
            }

            return View("~/Views/HomeCommon/Error_404.cshtml");
        }

        public ActionResult PartialProduceDetailRating(string page, string seoFriendUrl)
        {
            int recordMax = 3;
            page = page ?? "page-1";
            string[] arrPage = page.Split('-');
            int numberPage = Convert.ToInt16(arrPage[1]) * recordMax;
            int numberPageActive = Convert.ToInt16(arrPage[1]);

            var model = new PartialProduceDetailRatingModel();

            var arrUrl = Request.Path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            if (arrUrl != null && arrUrl.Length > 1)
            {
                seoFriendUrl = string.IsNullOrEmpty(seoFriendUrl) ? arrUrl[1] : seoFriendUrl;

                var listRatingProduce = (from rp in DataGemini.WRatingProduces
                                         join pp in DataGemini.PosProduces on rp.GuidProduce equals pp.Guid
                                         where pp.SeoFriendUrl == seoFriendUrl && pp.Active && pp.ApprovedStatus == PosProduce_ApprovedStatus.Approved
                                         select new WRatingProduceModel
                                         {
                                             Guid = rp.Guid,
                                             FullName = rp.FullName,
                                             Comment = rp.Comment,
                                             CreatedAt = rp.CreatedAt,
                                             Avatar = rp.Avatar,
                                             Legit = rp.Legit
                                         }).OrderByDescending(s => s.CreatedAt);

                //Sent data to view caculate
                ViewData["perpage"] = recordMax;
                ViewData["total"] = listRatingProduce.Count();
                ViewData["pageActive"] = numberPageActive;

                //Check page start
                if (Convert.ToInt16(arrPage[1]) == 1)
                {
                    numberPage = 0;
                }
                
                else
                {
                    numberPage = numberPage - recordMax;
                }

                model.ListRatingProduce = listRatingProduce.Skip(numberPage).Take(recordMax).ToList();
            }

            return PartialView(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RatingProduce(ProduceDetailModel model)
        {
            var rating = new WRatingProduce();
            try
            {
                if (model.NewRatingProduce.GuidProduce != null && model.NewRatingProduce.GuidProduce != Guid.Empty)
                {
                    model.NewRatingProduce.Setvalue(rating);
                    var user = GetSettingUser();
                    rating.FullName = user != null && !string.IsNullOrEmpty(user.Username) ? user.Username : rating.FullName;
                    rating.Mobile = user != null && !string.IsNullOrEmpty(user.Mobile) ? user.Mobile : rating.Mobile;
                    rating.Email = user != null && !string.IsNullOrEmpty(user.Email) ? user.Email : rating.Email;
                    rating.Avatar = user != null && !string.IsNullOrEmpty(user.Avartar) ? user.Avartar : DefaultImage.ImageEmpty;
                    rating.UpdatedAt = rating.CreatedAt = DateTime.Now;
                    rating.UpdatedBy = rating.CreatedBy = rating.FullName;

                    if (IsValidEmail(rating.Email) && model.NewRatingProduce.Legit > 0)
                    {
                        DataGemini.WRatingProduces.Add(rating);

                        if (SaveData("WRatingProduce") && rating != null)
                        {
                            var posProduce = DataGemini.PosProduces.FirstOrDefault(x => x.Guid == rating.GuidProduce.Value);
                            var lstRating = DataGemini.WRatingProduces.Where(x => x.GuidProduce == posProduce.Guid).ToList();
                            posProduce.LegitCount = lstRating.Count;
                            posProduce.Legit = (int)Math.Round((decimal)lstRating.Sum(x => x.Legit) / (decimal)posProduce.LegitCount, 0);

                            if (SaveData("PosProduce") && posProduce != null)
                            {
                                var sUser = DataGemini.SUsers.FirstOrDefault(x => x.Username == posProduce.CreatedBy);
                                var lstProduce = DataGemini.PosProduces.Where(x => x.LegitCount > 0 && x.CreatedBy == posProduce.CreatedBy);
                                sUser.LegitCount = lstProduce.Sum(x => x.LegitCount);
                                sUser.Legit = (int)Math.Round((decimal)lstProduce.Sum(x => x.Legit * x.LegitCount) / (decimal)sUser.LegitCount, 0);

                                if (SaveData("SUser") && sUser != null)
                                {
                                    DataReturn.StatusCode = Convert.ToInt16(HttpStatusCode.OK);
                                }
                                else
                                {
                                    DataReturn.StatusCode = Convert.ToInt16(HttpStatusCode.BadRequest);
                                    DataReturn.MessagError = Constants.CannotUpdate + " Date : " + DateTime.Now;
                                }
                            }
                            else
                            {
                                DataReturn.StatusCode = Convert.ToInt16(HttpStatusCode.BadRequest);
                                DataReturn.MessagError = Constants.CannotUpdate + " Date : " + DateTime.Now;
                            }
                        }
                        else
                        {
                            DataReturn.StatusCode = Convert.ToInt16(HttpStatusCode.BadRequest);
                            DataReturn.MessagError = Constants.CannotUpdate + " Date : " + DateTime.Now;
                        }
                    }
                    else
                    {
                        DataReturn.StatusCode = Convert.ToInt16(HttpStatusCode.BadRequest);
                        DataReturn.MessagError = Constants.CannotUpdate + " Date : " + DateTime.Now;
                    }
                }
                else
                {
                    DataReturn.StatusCode = Convert.ToInt16(HttpStatusCode.BadRequest);
                    DataReturn.MessagError = Constants.CannotUpdate + " Date : " + DateTime.Now;
                }
            }
            catch (Exception ex)
            {
                DataGemini.WRatingProduces.Remove(rating);

                DataReturn.StatusCode = Convert.ToInt16(HttpStatusCode.BadRequest);
                DataReturn.MessagError = Constants.CannotUpdate + " Date : " + DateTime.Now;
            }
            return Json(DataReturn, JsonRequestBehavior.AllowGet);
        }

        private bool IsValidEmail(string emailaddress)
        {
            try
            {
                if (string.IsNullOrEmpty(emailaddress))
                    return false;

                MailAddress m = new MailAddress(emailaddress);

                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        #endregion

        #region Produce Cart

        [HttpGet]
        public ActionResult ProduceCart()
        {
            var username = GetUserInSession();
            ViewBag.CurrentUsername = username;

            if (string.IsNullOrEmpty(username))
            {
                return View("~/Views/HomeCommon/Error_404.cshtml");
            }
            else
            {
                var models = new ProduceCartModel();
                models.ListPosProduceCart = new List<PosProduceModel>();

                models.ListPosCategory = (from cat in DataGemini.PosCategories
                                          where cat.Active
                                          select new PosCategoryModel
                                          {
                                              SeoFriendUrl = cat.SeoFriendUrl,
                                              Name = cat.Name,
                                              OrderBy = cat.OrderBy
                                          }).OrderBy(s => s.OrderBy).ToList();

                var cookieName = "cartProduce_" + username;
                if (Request.Cookies[cookieName] != null)
                {
                    var cookieData = string.IsNullOrEmpty(Request.Cookies[cookieName].Value) ? String.Empty : Request.Cookies[cookieName].Value;
                    var lstProduce = JsonConvert.DeserializeObject<List<ProduceCartCookieModel>>(cookieData);
                    if (lstProduce != null && lstProduce.Any())
                    {
                        var lstGuidProduce = string.Join(",", lstProduce.Select(x => x.GuidProduce));
                        var listPosProduceCart = (from pp in DataGemini.PosProduces
                                                  join pc in DataGemini.PosCategories on pp.GuidCategory equals pc.Guid
                                                  where pp.Active && pc.Active && lstGuidProduce.Contains(pp.Guid.ToString().ToLower().Trim())
                                                  select new PosProduceModel
                                                  {
                                                      Guid = pp.Guid,
                                                      Code = pp.Code,
                                                      Name = pp.Name,
                                                      NameCategory = pc.Name,
                                                      CategorySeoFriendUrl = pc.SeoFriendUrl,
                                                      SeoFriendUrl = pp.SeoFriendUrl,
                                                      Price = pp.Price,
                                                      Unit = pp.Unit,
                                                      ListGallery = (from fr in DataGemini.FProduceGalleries
                                                                     join im in DataGemini.UGalleries on fr.GuidGallery equals im.Guid
                                                                     where fr.GuidProduce == pp.Guid
                                                                     select new UGalleryModel
                                                                     {
                                                                         Image = im.Image,
                                                                         CreatedAt = im.CreatedAt
                                                                     }).OrderBy(x => x.CreatedAt).Take(1).ToList(),
                                                      CreatedAt = pp.CreatedAt,
                                                      Legit = pp.Legit,
                                                      LegitCount = pp.LegitCount,
                                                  }).ToList();

                        foreach (var item in listPosProduceCart)
                        {
                            item.Quantity = lstProduce.FirstOrDefault(x => x.GuidProduce == item.Guid.ToString().ToLower().Trim())?.Quantity;

                            var tmpLinkImg = item.ListGallery;
                            if (tmpLinkImg.Count == 0)
                            {
                                item.LinkImg0 = "/Content/Custom/empty-album.png";
                            }
                            else
                            {
                                item.LinkImg0 = tmpLinkImg[0].Image;
                            }
                        }

                        models.ListPosProduceCart = listPosProduceCart.Where(x => x.Quantity > 0).ToList();

                        models.ListTotalByUnit = new Dictionary<string, decimal>();
                        foreach (var itemGroup in models.ListPosProduceCart.GroupBy(x => x.Unit))
                        {
                            models.ListTotalByUnit.Add(itemGroup.Key, itemGroup.Sum(x => x.Price.GetValueOrDefault(0) * x.Quantity.GetValueOrDefault(0)));
                        }

                        return View("~/Views/HomeCommon/ProduceCart.cshtml", models);
                    }
                    else
                    {
                        return View("~/Views/HomeCommon/ProduceCart.cshtml", models);
                    }
                }
                else
                {
                    return View("~/Views/HomeCommon/Error_404.cshtml");
                }
            }
        }

        [HttpPost]
        public ActionResult PayCart(string fullAddress, string mobile)
        {
            string msg = "";
            int statusCode = (int)Convert.ToInt16(HttpStatusCode.Conflict);
            try
            {
                var username = GetUserInSession();
                var user = DataGemini.SUsers.FirstOrDefault(x => x.Username == username);
                if (user != null)
                {
                    var cookieName = "cartProduce_" + user.Username;
                    if (Request.Cookies[cookieName] != null)
                    {
                        var cookieData = string.IsNullOrEmpty(Request.Cookies[cookieName].Value) ? String.Empty : Request.Cookies[cookieName].Value;
                        var lstProduce = JsonConvert.DeserializeObject<List<ProduceCartCookieModel>>(cookieData);

                        if (lstProduce != null && lstProduce.Any())
                        {
                            var lstGuidProduce = string.Join(",", lstProduce.Select(x => x.GuidProduce));
                            var listPosProduceCart = (from pp in DataGemini.PosProduces
                                                      join pc in DataGemini.PosCategories on pp.GuidCategory equals pc.Guid
                                                      where pp.Active && pc.Active && lstGuidProduce.Contains(pp.Guid.ToString().ToLower().Trim())
                                                      select new PosProduceModel
                                                      {
                                                          Guid = pp.Guid,
                                                          Code = pp.Code,
                                                          Price = pp.Price,
                                                      }).ToList();

                            //Get payment input
                            WOrder order = new WOrder();
                            //Save order to db
                            order.Guid = Guid.NewGuid();
                            order.GuidUser = user.Guid;
                            order.FullAddress = fullAddress;
                            order.Mobile = mobile;
                            order.CreatedAt = DateTime.Now;
                            order.CreatedBy = username;
                            DataGemini.WOrders.Add(order);

                            List<WOrderDetail> orderDetails = new List<WOrderDetail>();
                            foreach (var item in listPosProduceCart)
                            {
                                orderDetails.Add(new WOrderDetail()
                                {
                                    Guid = Guid.NewGuid(),
                                    GuidOrder = order.Guid,
                                    GuidProduce = item.Guid,
                                    Quantity = (lstProduce.FirstOrDefault(x => x.GuidProduce == item.Guid.ToString().ToLower().Trim())?.Quantity).GetValueOrDefault(0),
                                    Price = item.Price,
                                    Status = WOrderDetail_Status.Inprogress
                                });
                            }
                            DataGemini.WOrderDetails.AddRange(orderDetails);

                            DataGemini.SaveChanges();

                            statusCode = (int)Convert.ToInt16(HttpStatusCode.OK);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }

            return Json(new { StatusCode = statusCode, Message = msg }, "text/plain");
        }

        #endregion

        #region User List By Follow

        public ActionResult UserListByFollow(string page)
        {
            int recordMax = 9;
            page = page ?? "page-1";
            string[] arrPage = page.Split('-');
            int numberPage = Convert.ToInt16(arrPage[1]) * recordMax;
            int numberPageActive = Convert.ToInt16(arrPage[1]);

            var models = new UserListByFollowModel();
            models.ListUserFollow = new List<SUserModel>();

            models.ListPosCategory = (from cat in DataGemini.PosCategories
                                      where cat.Active
                                      select new PosCategoryModel
                                      {
                                          SeoFriendUrl = cat.SeoFriendUrl,
                                          Name = cat.Name,
                                          OrderBy = cat.OrderBy
                                      }).OrderBy(s => s.OrderBy).ToList();

            var username = GetUserInSession();
            if (!string.IsNullOrWhiteSpace(username))
            {
                var currentUser = DataGemini.SUsers.FirstOrDefault(x => x.Username == username);
                if (currentUser != null)
                {
                    var lstGuidFollowString = currentUser.GuidFollow ?? string.Empty;
                    var listUserListByFollow = (from su in DataGemini.SUsers
                                                where su.Active && lstGuidFollowString.Contains(su.Guid.ToString().ToLower().Trim())
                                                select new SUserModel
                                                {
                                                    Guid = su.Guid,
                                                    CreatedAt = su.CreatedAt,
                                                    Avartar = su.Avartar,
                                                    FullName = su.FullName,
                                                    Username = su.Username,
                                                    Note = su.Note,
                                                    Legit = su.Legit,
                                                    LegitCount = su.LegitCount,
                                                }).OrderByDescending(x => x.CreatedAt);

                    //Sent data to view caculate
                    ViewData["perpage"] = recordMax;
                    ViewData["total"] = listUserListByFollow.Count();
                    ViewData["pageActive"] = numberPageActive;

                    //Check page start
                    if (Convert.ToInt16(arrPage[1]) == 1)
                    {
                        numberPage = 0;
                    }
                    else
                    {
                        numberPage = numberPage - recordMax;
                    }

                    models.ListUserFollow = listUserListByFollow.Skip(numberPage).Take(recordMax).ToList();

                    foreach (var item in models.ListUserFollow)
                    {
                        item.Avartar = !string.IsNullOrEmpty(item.Avartar) ? item.Avartar : DefaultImage.ImageEmpty;
                    }

                    return View("~/Views/HomeCommon/UserListByFollow.cshtml", models);
                }
                else
                {
                    return View("~/Views/HomeCommon/Error_404.cshtml");
                }
            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetFollowUser()
        {
            var username = GetUserInSession();
            var user = DataGemini.SUsers.FirstOrDefault(x => x.Username == username);

            if (user != null)
            {
                return Json(new { guidFollow = user.GuidFollow });
            }
            else
            {
                return Json(new { guidFollow = string.Empty });
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FollowUser(FollowUserModel model)
        {
            var username = GetUserInSession();
            var user = DataGemini.SUsers.FirstOrDefault(x => x.Username == username);
            try
            {
                if (user != null && user.Guid != null && user.Guid != Guid.Empty)
                {
                    if (!string.IsNullOrEmpty(user.GuidFollow) && user.GuidFollow.Contains(model.guidUser))
                    {
                        user.GuidFollow = user.GuidFollow.Replace(model.guidUser + ",", "");
                    }
                    else
                    {
                        user.GuidFollow += model.guidUser + ",";
                    }

                    if (SaveData("SUser"))
                    {
                        DataReturn.StatusCode = Convert.ToInt16(HttpStatusCode.OK);
                    }
                    else
                    {
                        DataReturn.StatusCode = Convert.ToInt16(HttpStatusCode.BadRequest);
                        DataReturn.MessagError = Constants.CannotUpdate + " Date : " + DateTime.Now;
                    }
                }
                else
                {
                    return Json(new { url = "/admin" });
                }
            }
            catch (Exception ex)
            {
                DataReturn.StatusCode = Convert.ToInt16(HttpStatusCode.BadRequest);
                DataReturn.MessagError = Constants.CannotUpdate + " Date : " + DateTime.Now;
            }
            return Json(DataReturn, JsonRequestBehavior.AllowGet);
        }

        #endregion

     

        #region Live Chat

        [ChildActionOnly]
        public ActionResult PartialLiveChat()
        {
            var models = new WLiveChatModel();

            var username = GetUserInSession();
            models.CurrentUser = DataGemini.SUsers.FirstOrDefault(x => x.Username == username);

            return PartialView(models);
        }

        #endregion

        public ActionResult Error_404()
        {
            ViewBag.Title = "";
            ViewBag.Description = "";

            return View();
        }

        //public ActionResult Search(string category, string keyword)
        //{
        //    SearchModel models = new SearchModel();
        //    var input = RemoveSign4VietnameseString(keyword);
        //    var listCategory = (from pc in DataGemini.PosCategories
        //                        where pc.SeoFriendUrl == category
        //                        select new PosCategoryModel()
        //                        {
        //                            Guid = pc.Guid,
        //                            SeoFriendUrl = "/danh-muc/" + pc.SeoFriendUrl
        //                        }).FirstOrDefault();
        //    if (listCategory != null)
        //    {
        //        models.Category = listCategory;
        //        var produce = (from pr in DataGemini.PosProduces
        //                       join pc in DataGemini.PosCategories on pr.GuidCategory equals pc.Guid
        //                       where pr.SeoFriendUrl.Contains(input) && pc.Guid == listCategory.Guid
        //                       select new PosProduceModel
        //                       {
        //                           Name = pr.Name,
        //                           Price = pr.Price,
        //                           Unit = pr.Unit,
        //                           Note = pr.Note,
        //                           GuidCategory = pc.Guid,
        //                           NameCategory = pc.Name,
        //                           ParentSeoFriendUrl = pc.SeoFriendUrl,
        //                           SeoFriendUrl = "/danh-muc/" + (from ca in DataGemini.PosCategories
        //                                                          where ca.Guid == pr.GuidCategory
        //                                                          select ca.SeoFriendUrl).FirstOrDefault() + "/" + pr.SeoFriendUrl,
        //                           FeaturedImage = "",
        //                           UGallery = (from ug in DataGemini.UGalleries
        //                                       join fpu in DataGemini.FProduceGalleries on ug.Guid equals fpu.GuidGallery
        //                                       join pro in DataGemini.PosProduces on fpu.GuidProduce equals pro.Guid
        //                                       where pro.Guid == pr.Guid
        //                                       select new UGalleryModel
        //                                       {
        //                                           Image = ug.Image,
        //                                           Guid = ug.Guid,
        //                                       }).FirstOrDefault()
        //                       }).OrderBy(x => x.Name).FirstOrDefault();
        //        if (produce != null)
        //        {
        //            var list = new List<PosProduceModel>();
        //            list.Add(produce);
        //            try
        //            {
        //                var model = new SearchModel();
        //                try
        //                {
        //                    var listNameProduce = string.Empty;
        //                    var listProduce = (from pr in DataGemini.PosProduces
        //                                       join pc in DataGemini.PosCategories on pr.GuidCategory equals pc.Guid
        //                                       where pr.SeoFriendUrl.Contains(input)
        //                                       select new PosProduceModel
        //                                       {
        //                                           Name = pr.Name,
        //                                           Price = pr.Price,
        //                                           Unit = pr.Unit,
        //                                           Note = pr.Note,
        //                                           GuidCategory = pc.Guid,
        //                                           NameCategory = pc.Name,
        //                                           ParentSeoFriendUrl = pc.SeoFriendUrl,
        //                                           SeoFriendUrl = "/danh-muc/" + (from ca in DataGemini.PosCategories
        //                                                                          where ca.Guid == pr.GuidCategory
        //                                                                          select ca.SeoFriendUrl).FirstOrDefault() + "/" + pr.SeoFriendUrl,
        //                                           FeaturedImage = "",
        //                                           UGallery = (from ug in DataGemini.UGalleries
        //                                                       join fpu in DataGemini.FProduceGalleries on ug.Guid equals fpu.GuidGallery
        //                                                       join pro in DataGemini.PosProduces on fpu.GuidProduce equals pro.Guid
        //                                                       where pro.Guid == pr.Guid
        //                                                       select new UGalleryModel
        //                                                       {
        //                                                           Image = ug.Image,
        //                                                           Guid = ug.Guid,
        //                                                       }).FirstOrDefault()
        //                                       }).OrderBy(x => x.Name).ToList();

        //                    list.AddRange(listProduce);

        //                    foreach (var item in list)
        //                    {
        //                        item.FeaturedImage = item.UGallery == null ? DefaultImage.ImageEmpty370x237 : GetImage("370x237", item.UGallery);
        //                        listNameProduce = listNameProduce + item.Name + ";";
        //                    }

        //                    model.listProduce = list;
        //                    model.KeyWord = keyword;
        //                    if (model.listProduce.Any())
        //                    {
        //                        string baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
        //                        ViewBag.Image = baseUrl + DefaultImage.ImageSeo;
        //                        ViewBag.Title = listNameProduce;
        //                        ViewBag.Description = listNameProduce;
        //                    }
        //                }
        //                catch (Exception ex)
        //                {
        //                    model.listProduce = new List<PosProduceModel>();
        //                    model.KeyWord = "";
        //                }

        //                return PartialView("ListSearch", model);
        //            }
        //            catch
        //            {
        //                return Redirect("/HomeCommon/Error_404");
        //            }
        //        }
        //    }
        //    else
        //    {
        //        models.Category = new PosCategoryModel();
        //    }

        //    models.KeyWord = keyword;
        //    models.listProduce = new List<PosProduceModel>();
        //    return PartialView("ListSearch", models);
        //}
    }
}