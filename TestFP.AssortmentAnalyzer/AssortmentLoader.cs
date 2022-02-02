using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TestFP.Models;

namespace TestFP.AssortmentManagement
{
  
    public class AssortmentLoader: IAssortmentLoader
    {
        public bool Valid { get; set; }
        public string Msg { get; set; }

        /// <summary>
        /// URL to load JSON from
        /// </summary>
        private string _url;
        public AssortmentLoader(string url)
        {
            _url = url;
        }

        //public bool Validate()
        //{
        //    if(string.IsNullOrEmpty(_url))
        //    {
        //        Valid = false;
        //        Msg = "URL is empty!";
        //        return false;
        //    }
        //}


        public async Task<Product[]> LoadProducts()
        {
            if (string.IsNullOrWhiteSpace(_url))
                throw new ArgumentNullException("URL is empty!");
            var client = new RestClient(_url);
            var request = new RestRequest();
            request.Method = Method.Get;
            return  await client.GetAsync<Product[]>(request);
        }


    }
}
