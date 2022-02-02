using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TestFP.Models;

namespace TestFP.AssortmentManagement
{
    public class AssortmentAnalyzer : IAssortmentAnalyzer
    {
        public string Msg { get; set; }
        public bool Valid { get; set; }


        //private string _url;
        //private List<Product> _products;
        private List<Article> _articles;
        private IAssortmentLoader _loader;

        public AssortmentAnalyzer(IAssortmentLoader loader)
        {
            _loader = loader;
        }
        public async Task<List<Article>> ArticlesPriceRange()
        {
            if(!await Init())
                return null;
            if (_articles == null || _articles.Count == 0)
                return new List<Article>();
            ResolvePrices();
            var articlesWithPrices = _articles.Where(a => a.PricePerLitre.HasValue).OrderBy(a => a.PricePerLitre).ToList();
            if(articlesWithPrices.Count == 0)
                return articlesWithPrices;
            //// sort by prices
            //articlesWithPrices.Sort((a,b) => {
            //    return (int)(a.PricePerLitre - b.PricePerLitre);
            //        });

            List<Article> result = new List<Article>();
            // add the cheapest articles
            double minPrice = articlesWithPrices[0].PricePerLitre.Value;
            result.AddRange(articlesWithPrices.Where(a => a.PricePerLitre == minPrice));
            // add the most expensive
            double maxPrice = articlesWithPrices[articlesWithPrices.Count - 1].PricePerLitre.Value;
            if (maxPrice != minPrice)
                result.AddRange(articlesWithPrices.Where(item => item.PricePerLitre == maxPrice));

            result.ForEach((item) =>
            {
                if (item.Tags == null)
                    item.Tags = new List<string>();
                if (item.PricePerLitre == minPrice)
                    item.Tags.Add("cheapest");
                else
                    item.Tags.Add("most expensive");
            });
            return result;
        }
      

        public async Task<List<Article>> ArticlesWithMostBottles()
        {
            if (!await Init())
                return null;
            if (_articles == null || _articles.Count == 0)
                return new List<Article>();
            var articlesWithBottles = _articles.Where(a => a.Bottles.HasValue).OrderByDescending(a => a.Bottles).ToList();
            if (articlesWithBottles.Count == 0)
                return articlesWithBottles;           

            List<Article> result = new List<Article>();
            result.AddRange(articlesWithBottles.Where(a => a.Bottles == articlesWithBottles[0].Bottles));
            result.ForEach((item) =>
            {
                if (item.Tags == null)
                    item.Tags = new List<string>();
                item.Tags.Add("most bottles");
            });
            return result;
        }

        public async Task<List<Article>> ArticlesWithPriceExactly(double price)
        {
            if (!await Init())
                return null;
            if (_articles == null || _articles.Count == 0)
                return new List<Article>();
            var articlesWithPrices = _articles.Where(a => a.Price == price).OrderBy(a => a.PricePerLitre).ToList();
            return articlesWithPrices;
        }

        private async Task<bool> Init()
        {
            if(_loader == null)
                throw new ArgumentNullException(nameof(_loader));
            
            if(_articles != null && Valid) // already validated and initialized
                return true;    

            try
            {
                var products = await _loader.LoadProducts();
                if (products == null || products.Length == 0)
                {
                    Msg = "Data is empty or can not be deserialized!";
                    return false;
                }
                // fill the ´list of articles
                FillArticles(products);
                Valid = true;   
                return true;

            }
            catch(System.Text.Json.JsonException ex)
            {
                Valid = false;
                Msg = $"The specified JSON has unknown format. Error: {ex.Message}";
                return false;
            }
            catch (System.Net.Http.HttpRequestException httpEx)
            {
                Valid = false;
                Msg = $"Error loading JSON: {httpEx.Message}";
                return false;
            }
            catch (Exception exc)
            {
                Valid = false;
                Msg = exc.Message;
                return false;
            }
        }

        private void FillArticles(Product[] products)
        {
            if(products == null || products.Length == 0) return;
            _articles = new List<Article>();
            foreach (var product in products)
            {
                product.Articles.ForEach(article => article.ParentProduct = new ProductBase
                {
                    Id = product.Id,
                    Name = product.Name,
                    BrandName = product.BrandName,
                    DescriptionText = product.DescriptionText
                });
                _articles.AddRange(product.Articles);
            }
            ParseTextsInArticles();
        }

        private void ParseTextsInArticles()
        {
            if (_articles == null || _articles.Count == 0)
                return;
            //RexEx:  "\((\d +,*\d *)\s"
            _articles.ForEach((article) =>
            {
                if (!string.IsNullOrEmpty(article.PricePerUnitText) &&
                  TryParsePricePerLitre(article.PricePerUnitText, out double pricePerLitre))
                    article.PricePerLitre = pricePerLitre;

                // Bottles
                if(!string.IsNullOrEmpty(article.ShortDescription) && 
                   TryParseBottlesInArticle(article.ShortDescription, out int bottles))
                    article.Bottles = bottles;


               
            });
        }

        private bool TryParseBottlesInArticle(string shortDescription, out int bottles)
        {
            bottles = -1;
            var match = Regex.Match(shortDescription, @"(\d+)\s*x\s*\d");
            if (!match.Success)
                return false;
            // there should be at least 2 groups, the whole string i.e. "(1,8 " and the captureds group "1,8"
            if (match.Groups != null && match.Groups.Count > 1)
                return int.TryParse(match.Groups[1].Value, out bottles);

            return int.TryParse(match.Value.Trim(), out bottles);
        }

        private bool TryParsePricePerLitre(string pricePerUnitText, out double pricePerLitre)
        {
            pricePerLitre = -1;
            var match = Regex.Match(pricePerUnitText, @"\((\d+,*\d*)\s");
            if (!match.Success)
                return false;
            // there should be at least 2 groups, the whole string i.e. "(1,8 " and the captureds group "1,8"
            if (match.Groups != null && match.Groups.Count > 1)
                return double.TryParse(match.Groups[1].Value, System.Globalization.NumberStyles.Any, new CultureInfo("de-DE"), out pricePerLitre);
                
            string priceString = match.Value.Trim();
            return double.TryParse(match.Value.Trim(), System.Globalization.NumberStyles.Any, new CultureInfo("de-DE"), out pricePerLitre);
        }

        /// <summary>
        /// fill the pricePerLitre from Text
        /// format: "(1,80 €/Liter)"
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void ResolvePrices()
        {
            if (_articles == null || _articles.Count == 0)
                return;
            //RexEx:  "\((\d +,*\d *)\s"
            _articles.ForEach((article) =>
            {
                if (string.IsNullOrEmpty(article.PricePerUnitText))
                    return;
                var match = Regex.Match(article.PricePerUnitText, @"\((\d +,*\d *)\s");
                if (!match.Success)
                    return;
                string priceString = match.Value.Trim();
                if (double.TryParse(priceString, System.Globalization.NumberStyles.Any, new CultureInfo("de-DE"), out double price))
                    article.PricePerLitre = price;
            });
        }
    }
}
