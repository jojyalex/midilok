using System;
using Newtonsoft.Json;

namespace Nop.Plugin.Widgets.GoogleAnalytics.Api.Models
{
    [JsonObject]
    [Serializable]
    public class Item
    {
        /// <summary>
        /// The ID of the item
        /// </summary>
        [JsonProperty("item_id")]
        public string ItemId { get; set; }

        /// <summary>
        /// The name of the item
        /// </summary>
        [JsonProperty("item_name")]
        public string ItemName { get; set; }

        /// <summary>
        /// A product affiliation to designate a supplying company or brick and mortar store location
        /// </summary>
        [JsonProperty("affiliation")]
        public string Affiliation { get; set; }

        /// <summary>
        /// The coupon name/code associated with the item
        /// </summary>
        [JsonProperty("coupon")]
        public string Coupon { get; set; }

        /// <summary>
        /// The monetary discount value associated with the item
        /// </summary>
        [JsonProperty("discount")]
        public decimal Discount { get; set; }

        /// <summary>
        /// The index/position of the item in a list
        /// </summary>
        [JsonProperty("index")]
        public int Index { get; set; }

        /// <summary>
        /// The brand of the item
        /// </summary>
        [JsonProperty("item_brand")]
        public string ItemBrand { get; set; }

        /// <summary>
        /// The category of the item. If used as part of a category hierarchy or taxonomy then this will be the first category
        /// </summary>
        [JsonProperty("item_category")]
        public string ItemCategory { get; set; }

        /// <summary>
        /// The second category hierarchy or additional taxonomy for the item
        /// </summary>
        [JsonProperty("item_category2")]
        public string ItemCategory2 { get; set; }

        /// <summary>
        /// The third category hierarchy or additional taxonomy for the item
        /// </summary>
        [JsonProperty("item_category3")]
        public string ItemCategory3 { get; set; }

        /// <summary>
        /// The fourth category hierarchy or additional taxonomy for the item
        /// </summary>
        [JsonProperty("item_category4")]
        public string ItemCategory4 { get; set; }

        /// <summary>
        /// The fifth category hierarchy or additional taxonomy for the item
        /// </summary>
        [JsonProperty("item_category5")]
        public string ItemCategory5 { get; set; }

        /// <summary>
        /// The ID of the list in which the item was presented to the user
        /// </summary>
        [JsonProperty("item_list_id")]
        public string ItemListId { get; set; }

        /// <summary>
        /// The name of the list in which the item was presented to the user
        /// </summary>
        [JsonProperty("item_list_name")]
        public string ItemListName { get; set; }

        /// <summary>
        /// The item variant or unique code or description for additional item details/options
        /// </summary>
        [JsonProperty("item_variant")]
        public string ItemVariant { get; set; }

        /// <summary>
        /// The physical location associated with the item (e.g. the physical store location). It's recommended to use the Google Place ID that corresponds to the associated item. A custom location ID can also be used
        /// </summary>
        [JsonProperty("location_id")]
        public string LocationId { get; set; }

        /// <summary>
        /// The monetary price of the item, in units of the specified currency parameter
        /// </summary>
        [JsonProperty("price")]
        public decimal Price { get; set; }

        /// <summary>
        /// Item quantity
        /// </summary>
        [JsonProperty("quantity")]
        public int Quantity { get; set; }

    }
}
