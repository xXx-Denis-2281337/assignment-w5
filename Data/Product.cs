using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace KmaOoad18.Assignments.Week5.Data
{
    public class Product
    {
        public int Id { get; set; }
        public decimal Price { get; set; }
        public string Sku { get; set; }
        public string Name { get; set; }
    }
}
