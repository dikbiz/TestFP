using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TestFP.Models;

namespace TestFP.AssortmentManagement
{
    public interface IAssortmentLoader
    {
        //   public bool Validate();
        // List<Product> LoadProducts();
        Task<Product[]> LoadProducts();
    }
}