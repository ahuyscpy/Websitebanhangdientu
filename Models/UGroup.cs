//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Gemini.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class UGroup
    {
        public UGroup()
        {
            this.UFaqs = new HashSet<UFaq>();
            this.UHtmls = new HashSet<UHtml>();
            this.UNews = new HashSet<UNew>();
        }
    
        public System.Guid Guid { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
        public string Note { get; set; }
        public System.DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public string Type { get; set; }
        public string SeoTitle { get; set; }
        public string SeoDescription { get; set; }
        public string SeoImage { get; set; }
        public string SeoFriendUrl { get; set; }
        public Nullable<int> CountView { get; set; }
        public Nullable<System.Guid> ParentGuid { get; set; }
        public Nullable<System.Guid> LanguageGuid { get; set; }
    
        public virtual ICollection<UFaq> UFaqs { get; set; }
        public virtual ICollection<UHtml> UHtmls { get; set; }
        public virtual ICollection<UNew> UNews { get; set; }
    }
}
