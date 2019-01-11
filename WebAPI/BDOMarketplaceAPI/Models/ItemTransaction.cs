using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BDOMarketplaceAPI.Models
{
    public class ItemTransaction
    {
        public string name { get; set; } //64 bytes
        public string url { get; set; } //64 bytes
        public Int64 price { get; set; } //4 bytes
        public int enchantment { get; set; } //4 bytes
        public int amount { get; set; } //4 bytes
        public string registerTime { get; set; } //32bytes
        public string color { get; set; } //14bytes
        public int itemId { get; set; }
    }

    public class BDOItem
    {
        public string name { get; set; }
        public string url { get; set; }
        public int ItemID { get; set; }
    }
}