using System;
using Gemini.Models._02_Cms.U;
using System.Collections.Generic;
using Gemini.Models._01_Hethong;
using Gemini.Models._03_Pos;
using Gemini.Models._05_Website;

namespace Gemini.Models._20_Web
{
    public class HomeModel
    {
        public List<PosCategoryModel> ListPosCategory { get; set; }
    }

    public class HeaderModel
    {
        public string CurrentUsername { get; set; }

        public List<SMenuModel> ListMenu { get; set; }
    }

    public class PartialLatestProductModel
    {
        public List<PosProduceModel> ListPosProduce { get; set; }

        public List<PosCategoryModel> ListPosCategory { get; set; }
    }

    public class PartialFeaturedProductModel
    {
        public List<PosProduceModel> ListPosProduce { get; set; }
    }

    public class UserListByFollowModel
    {
        public List<PosCategoryModel> ListPosCategory { get; set; }

        public List<SUserModel> ListUserFollow { get; set; }
    }

    public class NewsListModel
    {
        public List<PosCategoryModel> ListPosCategory { get; set; }

        public List<UNewsModel> ListNews { get; set; }
    }

    public class NewsDetailModel
    {
        public List<PosCategoryModel> ListPosCategory { get; set; }

        public UNewsModel UNews { get; set; }

        public List<UNewsModel> ListNewsSameCreatedBy { get; set; }
    }

    public class ProduceListByLovedModel
    {
        public List<PosCategoryModel> ListPosCategory { get; set; }

        public List<PosProduceModel> ListPosProduceLatest { get; set; }

        public List<PosProduceModel> ListPosProduceByLoved { get; set; }
    }

    public class ProduceListByCreatedByModel
    {
        public SUserModel PosProduceCreatedBy { get; set; }

        public List<PosCategoryModel> ListPosCategory { get; set; }

        public List<PosProduceModel> ListPosProduceLatest { get; set; }

        public List<PosProduceModel> ListPosProduceByCreatedBy { get; set; }
    }
    
    public class ProduceListBySearchModel
    {
        public string KeyWord{ get; set; }

        public List<PosCategoryModel> ListPosCategory { get; set; }

        public List<PosProduceModel> ListPosProduceLatest { get; set; }

        public List<PosProduceModel> ListPosProduceBySearch { get; set; }
    }

    public class ProduceListByCategoryModel
    {
        public PosCategoryModel PosCategory { get; set; }

        public List<PosCategoryModel> ListPosCategory { get; set; }

        public List<PosProduceModel> ListPosProduceLatest { get; set; }

        public List<PosProduceModel> ListPosProduceByCategory { get; set; }
    }

    public class ProduceDetailModel
    {
        public SUserModel SUser { get; set; }

        public List<PosCategoryModel> ListPosCategory { get; set; }

        public PosProduceModel PosProduce { get; set; }

        public SUser PosProduceCreatedBy { get; set; }

        public WRatingProduceModel NewRatingProduce { get; set; }

        public List<PosProduceModel> ListProduceSameCreatedBy { get; set; }

        public List<PosProduceModel> ListProduceSameCategory { get; set; }
    }

    public class FollowUserModel
    {
        public string guidUser { get; set; }

        public SUserModel PosProduce { get; set; }
    }

    public class PartialProduceDetailRatingModel
    {
        public List<WRatingProduceModel> ListRatingProduce { get; set; }
    }

    public class ContactUsModel
    {
        public List<PosCategoryModel> ListPosCategory { get; set; }
    }

    public class FooterModel
    {
        public List<PosCategoryModel> ListPosCategory { get; set; }
    }

    public class ProduceCartModel
    {
        public List<PosCategoryModel> ListPosCategory { get; set; }

        public List<PosProduceModel> ListPosProduceCart { get; set; }

        public Dictionary<string, decimal> ListTotalByUnit { get; set; }
    }

    public class ProduceCartCookieModel
    {
        public string GuidProduce { get; set; }
        public int Quantity { get; set; }
    }
}