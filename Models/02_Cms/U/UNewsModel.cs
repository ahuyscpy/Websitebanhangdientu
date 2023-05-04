using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;
using Gemini.Resources;
using SINNOVA.Core;

namespace Gemini.Models._02_Cms.U
{
    public class UNewsModel
    {
        public String Iscopy { get; set; }

        public int IsUpdate { get; set; }

        #region Properties
        [ScaffoldColumn(false)]

        public Guid Guid { get; set; }

        [StringLength(255, ErrorMessageResourceName = "ErrorMaxLength255", ErrorMessageResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "RequiredFill")]
        public String Name { get; set; }

        public bool Active { get; set; }

        [StringLength(2000, ErrorMessageResourceName = "ErrorMaxLength2000", ErrorMessageResourceType = typeof(Resource))]
        public String Note { get; set; }

        public Guid? GuidGroup { get; set; }

        [StringLength(255, ErrorMessageResourceName = "ErrorMaxLength255", ErrorMessageResourceType = typeof(Resource))]
        public String SeoTitle { get; set; }

        [StringLength(255, ErrorMessageResourceName = "ErrorMaxLength255", ErrorMessageResourceType = typeof(Resource))]
        public String SeoDescription { get; set; }

        [StringLength(2000, ErrorMessageResourceName = "ErrorMaxLength2000", ErrorMessageResourceType = typeof(Resource))]
        public String Description { get; set; }

        [AllowHtml]
        [StringLength(255, ErrorMessageResourceName = "ErrorMaxLength255", ErrorMessageResourceType = typeof(Resource))]
        public String SeoImage { get; set; }

        [StringLength(255, ErrorMessageResourceName = "ErrorMaxLength255", ErrorMessageResourceType = typeof(Resource))]
        public String SeoFriendUrl { get; set; }

        [AllowHtml]
        public String ContentNews { get; set; }

        public Guid? GuidImage { get; set; }

        public int? CountView { get; set; }

        public int Status { get; set; }

        [StringLength(255, ErrorMessageResourceName = "ErrorMaxLength255", ErrorMessageResourceType = typeof(Resource))]
        public String TypeNews { get; set; }

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

        public String CreatedByFullName { get; set; }

        public String StatusName { get; set; }

        public String UrlImageFeatured { get; set; }

        public UGalleryModel UGallery { get; set; }

        public List<SReplaceCode> ReplaceCode { get; set; }

        #region Constructor
        public UNewsModel()
        {
        }

        public UNewsModel(UNew uNew)
        {
            Guid = uNew.Guid;
            Name = uNew.Name;
            GuidGroup = uNew.GuidGroup;
            SeoTitle = uNew.SeoTitle;
            ContentNews = HttpUtility.HtmlDecode(uNew.ContentNews);
            SeoDescription = uNew.SeoDescription;
            SeoImage = string.IsNullOrEmpty(uNew.SeoImage) ? "" : "<img  src=\"" + HttpUtility.UrlDecode(uNew.SeoImage) + "\"  />";
            GuidImage = uNew.GuidImage;
            SeoFriendUrl = uNew.SeoFriendUrl;
            CountView = uNew.CountView;
            Active = uNew.Active;
            Note = uNew.Note;
            Description = uNew.Description;
            Status = uNew.Status;
            TypeNews = uNew.TypeNews;
            CreatedAt = uNew.CreatedAt;
            CreatedBy = uNew.CreatedBy;
            UpdatedAt = uNew.UpdatedAt;
            UpdatedBy = uNew.UpdatedBy;
        }
        #endregion

        #region Function
        public void Setvalue(UNew uNew)
        {
            if (IsUpdate == 0)
            {
                uNew.Guid = Guid.NewGuid();
                uNew.CreatedBy = CreatedBy;
                uNew.CreatedAt = DateTime.Now;
            }
            uNew.Name = Name;
            uNew.SeoTitle = SeoTitle;
            uNew.SeoDescription = SeoDescription;
            uNew.SeoImage = vString.FindString(vString.GetValueTostring(SeoImage), "src=\"", "\"");
            uNew.ContentNews = ContentNews;
            uNew.GuidGroup = GuidGroup;
            uNew.GuidImage = GuidImage;
            uNew.SeoFriendUrl = SeoFriendUrl;
            uNew.CountView = CountView;
            uNew.Active = Active;
            uNew.Note = Note;
            uNew.TypeNews = TypeNews;
            uNew.Description = Description;
            uNew.Status = Status;
            uNew.UpdatedAt = DateTime.Now;
            uNew.UpdatedBy = UpdatedBy;
        }
        #endregion
    }
}