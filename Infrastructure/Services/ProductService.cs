using Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Services
{
    public class ProductService : IProductService
    {
        public string GetProductName()
        {
            return "Laptop";
        }
    }
}
