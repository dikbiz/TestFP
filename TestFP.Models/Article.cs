using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestFP.Models
{
    public class Article
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("shortDescription")]
        public string ShortDescription { get; set; }
        [JsonProperty("price", Required=Required.Always)]
        public double Price { get; set; }
        [JsonProperty("unit")]
        public string Unit { get; set; }
        [JsonProperty("pricePerUnitText")]
        public string PricePerUnitText { get; set; }
        [JsonProperty("image")]
        public string Image { get; set; }

        //calculated
        public ProductBase ParentProduct { get; set; }
        public double? PricePerLitre { get; set; }
        public int? Bottles { get; set; }
        public List<string> Tags { get; set; }
    }
}
