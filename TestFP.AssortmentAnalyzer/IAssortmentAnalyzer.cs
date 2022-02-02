using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TestFP.Models;

namespace TestFP.AssortmentManagement
{
    public interface IAssortmentAnalyzer
    {
        //Task<bool> Validate();
        Task<List<Article>> ArticlesWithMostBottles();
        Task<List<Article>> ArticlesWithPriceExactly(double price);
        Task<List<Article>> ArticlesPriceRange();
    }
}
