using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PromotionEngine
{
    [JsonObject("appSettings")]
    public class AppSettings
    {
        [JsonProperty("promotions")]
        public Promotion[] Promotions { get; set; }

        [JsonProperty("productWithPrices")]
        public ProductWithPrice[] ProductWithPrices { get; set; }
    }

    public partial class ProductWithPrice
    {
        [JsonProperty("productId")]
        public long ProductId { get; set; }

        [JsonProperty("productName")]
        public string ProductName { get; set; }

        [JsonProperty("price")]
        public long Price { get; set; }
    }

    public partial class Promotion
    {
        [JsonProperty("promotionId")]
        public long PromotionId { get; set; }

        [JsonProperty("noOfProducts")]
        public long NoOfProducts { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("productName")]
        public string ProductName { get; set; }

        [JsonProperty("price")]
        public long Price { get; set; }
    }

}
