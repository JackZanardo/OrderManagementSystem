using OMS.Controllers;
using OMS.Domain;
using OMS.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var orders = OrderController.Instance.GetOrderHeaders();

            OrderController.Instance.ResetDatabase();

            
            //get stock items
            var stock = StockItemController.Instance.GetStockItems();

            var order = OrderController.Instance.CreateNewOrderHeader();

            foreach(var s in stock)
            {
                OrderController.Instance.UpsertOrderItem(order.Id, s.Id, 1);
            }

            //order = OrderController.Instance.SubmitOrder(order.Id);

            //order = OrderController.Instance.ProcessOrder(order.Id);

            Console.ReadKey();

        }
    }
}
