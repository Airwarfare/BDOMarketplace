using BDOMarketplaceAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;

namespace BDOMarketplaceAPI.Controllers
{

    [EnableCors(origins: "http://127.0.0.1:5500", headers: "*", methods: "*")]
    public class ItemController : ApiController
    {
        // GET: api/Item
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Item/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Item
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Item/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Item/5
        public void Delete(int id)
        {
        }

        [EnableCors(origins: "http://127.0.0.1:5500", headers: "*", methods: "*")]
        [Route("api/GetItemsByName/{itemname}")]
        public List<BDOItem> GetItemsByName(string itemname)
        {
            return Core.gameItems.Where(x => x.Value.name.Contains(itemname)).Select(y => new BDOItem { name = y.Value.name, ItemID = y.Key, url = y.Value.url }).ToList();
        }

        [Route("api/GetItem/{itemid}")]
        public List<ItemTransaction> GetItem(int itemId)
        {
            return Core.items[itemId];
        }

        [Route("api/GetItems/{amount}")]
        public List<ItemTransaction> GetItems(int amount)
        {
            amount = amount >= 100 ? 100 : amount;
            return Core.lastItems.Take(amount).ToList();
        }

        [Route("api/AddItem/{api}")]
        public HttpResponseMessage AddItem([FromBody]ItemTransaction item,  string api)
        {
            try
            {

                item.name = Core.gameItems[item.itemId].name;
                item.url = Core.gameItems[item.itemId].url;

                Core.items[item.itemId].Insert(0, item);
                if (Core.items[item.itemId].Count > 10)
                    Core.items[item.itemId].RemoveAt(Core.items[item.itemId].Count - 1);

                Core.lastItems.Insert(0, item);
                if (Core.lastItems.Count >= 100)
                    Core.lastItems.RemoveAt(Core.lastItems.Count - 1);

                Task.Run(() => APIBackend.Database.Insert(string.Format("INSERT INTO transactions (itemid, price, time, amount, enchantment) VALUES('{0}', '{1}', '{2}', '{3}', '{4}')", item.itemId, item.price, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), item.amount, item.enchantment)));


                return new HttpResponseMessage(HttpStatusCode.OK);
            } catch(Exception ex)
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }
    }
}
