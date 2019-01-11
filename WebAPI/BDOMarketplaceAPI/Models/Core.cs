using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BDOMarketplaceAPI.Models
{
    public static class Core
    {
        public static List<ItemTransaction> lastItems = new List<ItemTransaction>();

        public static Dictionary<int, BDOItem> gameItems = new Dictionary<int, BDOItem>();
        public static Dictionary<int, List<ItemTransaction>> items = new Dictionary<int, List<ItemTransaction>>();

        public static void Init()
        {
            APIBackend.Database.Connect("localhost", "bdomp", "root", "test123");
            var hold = APIBackend.Database.Select("SELECT * FROM item_data");
            foreach (var item in hold)
            {
                int itemid = int.Parse(item[0]);
                gameItems.Add(itemid, new BDOItem { name = item[1], url = item[2] });
                var recent = APIBackend.Database.Select(string.Format("SELECT * FROM bdomp.transactions WHERE itemid = '{0}' ORDER BY time LIMIT 10;", itemid));
                items.Add(itemid, new List<ItemTransaction>());
                if (recent.Count != 0)
                {
                    foreach (var it in recent)
                    {
                        ItemTransaction i = new ItemTransaction
                        {
                            itemId = int.Parse(it[0]),
                            price = Int64.Parse(it[1]),
                            registerTime = DateTime.Parse(it[2]).ToString(),
                            amount = int.Parse(it[3]),
                            enchantment = int.Parse(it[4]),
                            name = item[1],
                            url = item[2]
                        };
                        items[itemid].Add(i);
                    }
                }
            }
            hold = null;


            var mostRecentAll = APIBackend.Database.Select(string.Format("SELECT * FROM bdomp.transactions ORDER BY time LIMIT 10;"));
            if (mostRecentAll.Count != 0)
            {
                foreach (var item in mostRecentAll)
                {
                    int id = int.Parse(item[0]);
                    ItemTransaction i = new ItemTransaction
                    {
                        itemId = id,
                        price = int.Parse(item[1]),
                        registerTime = DateTime.Parse(item[2]).ToShortTimeString(),
                        amount = int.Parse(item[3]),
                        name = gameItems[id].name,
                        url = gameItems[id].url
                    };
                    lastItems.Add(i);
                }
            }
        }
    }
}