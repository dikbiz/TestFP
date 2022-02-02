using System;
using System.Collections.Generic;
using System.Text;

namespace TestFP.Models
{
    public class ProductBase
    {
        public int Id { get; set; }
        public string BrandName { get; set; }
        public string Name { get; set; }
        public string DescriptionText { get; set; }
    }
    public class Product: ProductBase
    {
        public List<Article> Articles { get; set; }
    }
}
