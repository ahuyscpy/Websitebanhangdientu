using System;
using System.Web;
using Gemini.Resources;
using System.ComponentModel.DataAnnotations;
using Gemini.Models._02_Cms.U;
using System.Collections.Generic;

namespace Gemini.Models._05_Website
{
    public class WOrderDetailModel
    {
        public int IsUpdate { get; set; }

        #region Properties
        [ScaffoldColumn(false)]
        public Guid Guid { get; set; }

        public Guid? GuidOrder { get; set; }

        public Guid? GuidProduce { get; set; }

        public int? Quantity { get; set; }

        public int Status { get; set; }

        public decimal? Price { get; set; }

        #endregion

        public string StatusName { get; set; }

        public string ProduceLinkImg0 { get; set; }

        public string OrderCode { get; set; }

        public string OrderFullName { get; set; }

        public string OrderFullAddress { get; set; }

        public string OrderMobile { get; set; }

        public string ProduceCode { get; set; }

        public string ProduceName { get; set; }

        public DateTime OrderCreatedAt { get; set; }

        public string OrderCreatedAtString { get; set; }

        public List<UGalleryModel> ListGallery { get; set; }

        #region Constructor
        public WOrderDetailModel()
        {
        }

        public WOrderDetailModel(WOrderDetail wOrderDetail)
        {
            Guid = wOrderDetail.Guid;
            GuidOrder = wOrderDetail.GuidOrder;
            GuidProduce = wOrderDetail.GuidProduce;
            Quantity = wOrderDetail.Quantity;
            Price = wOrderDetail.Price;
            Status = wOrderDetail.Status;
        }
        #endregion

        #region Function
        public void Setvalue(WOrderDetail wOrderDetail)
        {
            if (IsUpdate == 0)
            {
                wOrderDetail.Guid = Guid.NewGuid();
            }
            wOrderDetail.GuidOrder = GuidOrder;
            wOrderDetail.GuidProduce = GuidProduce;
            wOrderDetail.Quantity = Quantity;
            wOrderDetail.Price = Price;
            wOrderDetail.Status = Status;
        }
        #endregion
    }
}