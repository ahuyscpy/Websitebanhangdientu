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
    
    public partial class MapCampaignSetting
    {
        public System.Guid Guid { get; set; }
        public System.Guid GuidEmailSetting { get; set; }
        public System.Guid GuidCampaign { get; set; }
        public int Take { get; set; }
        public int Skip { get; set; }
        public int Sent { get; set; }
        public System.DateTime CreatedAt { get; set; }
    
        public virtual CrmEmailCampaign CrmEmailCampaign { get; set; }
    }
}
