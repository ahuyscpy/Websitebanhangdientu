using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Gemini.Models._02_Cms.U;
using Gemini.Resources;

namespace Gemini.Models._03_Pos
{

    public class ImageSize
    {
        public String SmallImage { get; set; }
        public String BigImage { get; set; }
    }

    public class PosProduceModel
    {
        public int IsUpdate { get; set; }

        #region Properties
        [ScaffoldColumn(false)]
        public Guid Guid { get; set; }

        [StringLength(255, ErrorMessageResourceName = "ErrorMaxLength255", ErrorMessageResourceType = typeof(Resource))]
        public String Name { get; set; }

        public bool Active { get; set; }

        [StringLength(2000, ErrorMessageResourceName = "ErrorMaxLength2000", ErrorMessageResourceType = typeof(Resource))]
        public String Note { get; set; }

        public String Description { get; set; }

        public Decimal? Price { get; set; }

        [StringLength(255, ErrorMessageResourceName = "ErrorMaxLength255", ErrorMessageResourceType = typeof(Resource))]
        public String Unit { get; set; }

        public Guid? GuidPartner { get; set; }

        public Guid GuidCategory { get; set; }

        public int Amount { get; set; }

        public int? Legit { get; set; }

        public int? LegitCount { get; set; }

        public String GuidTags { get; set; }

        public Decimal? SaleOffPrice { get; set; }

        [StringLength(255, ErrorMessageResourceName = "ErrorMaxLength255", ErrorMessageResourceType = typeof(Resource))]
        public String SeoTitle { get; set; }

        [StringLength(255, ErrorMessageResourceName = "ErrorMaxLength255", ErrorMessageResourceType = typeof(Resource))]
        public String SeoDescription { get; set; }

        [StringLength(255, ErrorMessageResourceName = "ErrorMaxLength255", ErrorMessageResourceType = typeof(Resource))]
        public String SeoFriendUrl { get; set; }

        public int Sort { get; set; }

        public bool TopProduce { get; set; }

        public bool HotProduce { get; set; }

        [ScaffoldColumn(false)]
        public String MGuidTags { get; set; }

        [StringLength(255, ErrorMessageResourceName = "ErrorMaxLength255", ErrorMessageResourceType = typeof(Resource))]
        public String Code { get; set; }

        [StringLength(255, ErrorMessageResourceName = "ErrorMaxLength255", ErrorMessageResourceType = typeof(Resource))]
        public String Color { get; set; }

        [StringLength(255, ErrorMessageResourceName = "ErrorMaxLength255", ErrorMessageResourceType = typeof(Resource))]
        public String Size { get; set; }

        [StringLength(255, ErrorMessageResourceName = "ErrorMaxLength255", ErrorMessageResourceType = typeof(Resource))]
        public String Status { get; set; }

        [StringLength(2000, ErrorMessageResourceName = "ErrorMaxLength2000", ErrorMessageResourceType = typeof(Resource))]
        public String FullAddress { get; set; }

        [StringLength(255, ErrorMessageResourceName = "ErrorMaxLength255", ErrorMessageResourceType = typeof(Resource))]
        public String Latitude { get; set; }

        [StringLength(255, ErrorMessageResourceName = "ErrorMaxLength255", ErrorMessageResourceType = typeof(Resource))]
        public String Longitude { get; set; }

        public int? ApprovedStatus { get; set; }

        [StringLength(255, ErrorMessageResourceName = "ErrorMaxLength255", ErrorMessageResourceType = typeof(Resource))]
        public String ApprovedBy { get; set; }

        public DateTime? ApprovedAt { get; set; }

        [Editable(false)]
        public DateTime? CreatedAt { get; set; }

        [Editable(false)]
        [StringLength(25, ErrorMessageResourceName = "ErrorMaxLength25", ErrorMessageResourceType = typeof(Resource))]
        public String CreatedBy { get; set; }

        [Editable(false)]
        public DateTime? UpdatedAt { get; set; }

        [Editable(false)]
        [StringLength(25, ErrorMessageResourceName = "ErrorMaxLength25", ErrorMessageResourceType = typeof(Resource))]
        public String UpdatedBy { get; set; }

        #endregion

        public List<UGalleryModel> ListGallery { get; set; }

        public List<ImageSize> ListImage { get; set; }

        public String LinkImg0 { get; set; }

        public String LinkImg1 { get; set; }

        public String LinkImg2 { get; set; }

        public List<SReplaceCode> ReplaceCode { get; set; }

        public String NameCategory { get; set; }

        public String NamePartner { get; set; }

        public UGalleryModel UGallery { get; set; }

        public String FeaturedImage { get; set; }

        public String ParentSeoFriendUrl { get; set; }

        public bool IsRightRequestApprove { get; set; }

        public bool IsRightApprove { get; set; }

        public String ApprovedStatusName { get; set; }

        public String CategorySeoFriendUrl { get; set; }

        public int? Quantity { get; set; }

        #region Constructor
        public PosProduceModel()
        {
        }

        public PosProduceModel(PosProduce posProduce)
        {
            Guid = posProduce.Guid;
            Name = posProduce.Name;
            Active = posProduce.Active;
            Note = HttpUtility.HtmlDecode(posProduce.Note);
            CreatedAt = posProduce.CreatedAt;
            CreatedBy = posProduce.CreatedBy;
            UpdatedAt = posProduce.UpdatedAt;
            UpdatedBy = posProduce.UpdatedBy;
            Description = HttpUtility.HtmlDecode(posProduce.Description);
            Price = posProduce.Price;
            Unit = posProduce.Unit;
            GuidPartner = posProduce.GuidPartner;
            GuidCategory = posProduce.GuidCategory;
            Amount = posProduce.Amount;
            GuidTags = posProduce.GuidTags;
            SaleOffPrice = posProduce.SaleOffPrice;
            SeoTitle = posProduce.SeoTitle;
            SeoDescription = posProduce.SeoDescription;
            SeoFriendUrl = posProduce.SeoFriendUrl;
            Sort = posProduce.Sort;
            MGuidTags = posProduce.GuidTags;
            HotProduce = posProduce.HotProduce;
            TopProduce = posProduce.TopProduce;
            MGuidTags = posProduce.GuidTags;
            Code = posProduce.Code;
            Color = posProduce.Color;
            Size = posProduce.Size;
            Status = posProduce.Status;
            FullAddress = posProduce.FullAddress;
            Latitude = posProduce.Latitude;
            Longitude = posProduce.Longitude;
            ApprovedStatus = posProduce.ApprovedStatus;
            ApprovedBy = posProduce.ApprovedBy;
            ApprovedAt = posProduce.ApprovedAt;
            Legit = posProduce.Legit;
            LegitCount = posProduce.LegitCount;
        }
        #endregion

        #region Function
        public void Setvalue(PosProduce posProduce, Guid? guid = null)
        {
            if (IsUpdate == 0)
            {
                if (guid == null || guid == Guid.Empty)
                {
                    posProduce.Guid = Guid.NewGuid();
                }
                else
                {
                    posProduce.Guid = guid.Value;
                }
                posProduce.CreatedBy = CreatedBy;
                posProduce.CreatedAt = DateTime.Now;
            }
            posProduce.Name = Name;
            posProduce.Active = Active;
            posProduce.Note = Note;
            posProduce.UpdatedAt = DateTime.Now;
            posProduce.UpdatedBy = UpdatedBy;
            posProduce.GuidTags = MGuidTags;
            posProduce.Description = Description;
            posProduce.Price = Price;
            posProduce.Unit = Unit;
            posProduce.GuidPartner = GuidPartner;
            posProduce.Amount = Amount;
            posProduce.GuidCategory = GuidCategory;
            posProduce.SaleOffPrice = SaleOffPrice;
            posProduce.SeoTitle = SeoTitle;
            posProduce.SeoDescription = SeoDescription;
            posProduce.SeoFriendUrl = SeoFriendUrl;
            posProduce.Sort = Sort;
            posProduce.HotProduce = HotProduce;
            posProduce.TopProduce = TopProduce;
            posProduce.Code = Code;
            posProduce.Color = Color;
            posProduce.Size = Size;
            posProduce.Status = Status;
            posProduce.FullAddress = FullAddress;
            posProduce.Latitude = Latitude;
            posProduce.Longitude = Longitude;
            posProduce.ApprovedStatus = ApprovedStatus;
            posProduce.ApprovedBy = ApprovedBy;
            posProduce.ApprovedAt = ApprovedAt;
            posProduce.Legit = Legit;
            posProduce.LegitCount = LegitCount;
        }
        #endregion
    }
}