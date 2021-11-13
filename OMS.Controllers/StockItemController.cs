using OMS.Domain;
using OMS.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMS.Controllers
{
    public class StockItemController
    {

        private readonly StockRepository _stockRespository = new StockRepository();

        private static StockItemController _instance;

        private StockItemController() { }
        public static StockItemController Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new StockItemController();
                }
                return _instance;
            }
        }


        public IEnumerable<StockItem> GetStockItems()
        {
            return _stockRespository.GetStockItems();
        }

        public StockItem GetStockItem(int id)
        {
            return _stockRespository.GetStockItem(id);
        }

    }
}
