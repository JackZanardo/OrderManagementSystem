using OMS.Domain;
using OMS.Repository;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMS.Controllers
{
    /// <summary>
    /// Controller for Orders
    /// Only one instance of OrderController can be instantiated
    /// References OMS.Domain and OMS.Repository
    /// </summary>
    public class OrderController
    {

        private readonly OrderRepository _orderRepository = new OrderRepository();
        private readonly StockRepository _stockRepository = new StockRepository();
        private static OrderController _instance;
        /// <summary>
        /// Default constructor made private to prevent an OrderController being instantiated outside of OrderController
        /// </summary>
        private OrderController() { }
        /// <summary>
        /// Checks if an instance of OrderController already exists and returns it
        /// If not it creates a new instance and returns it
        /// </summary>
        public static OrderController Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new OrderController();
                }
                return _instance;
            }
        }
        /// <summary>
        /// Creates a new order header
        /// </summary>
        /// <returns>OrderHeader</returns>
        public OrderHeader CreateNewOrderHeader()
        {
            var order = _orderRepository.InsertOrderHeader();
            return order;
        }
        /// <summary>
        /// Updates or inserts an order item
        /// </summary>
        /// <param name="orderHeaderId">The order header id</param>
        /// <param name="stockItemId">The stock item id</param>
        /// <param name="quantity">The quantity of stock items</param>
        /// <returns></returns>
        public OrderHeader UpsertOrderItem(int orderHeaderId, int stockItemId, int quantity)
        {
            var stockItem = _stockRepository.GetStockItem(stockItemId);
            var order = _orderRepository.GetOrderHeader(orderHeaderId);
            var item = order.AddOrUpdateOrderItem(stockItem.Id, stockItem.Price, stockItem.Name, quantity);
            _orderRepository.UpsertOrderItem(item);
            return order;
        }
        /// <summary>
        /// Submits an order
        /// </summary>
        /// <param name="orderHeaderId">The order id to be submitted</param>
        /// <returns></returns>
        public OrderHeader SubmitOrder(int orderHeaderId)
        {
            var order = _orderRepository.GetOrderHeader(orderHeaderId);
            order.Submit();
            _orderRepository.UpdateOrderState(order);
            return order;
        }
        /// <summary>
        /// Brings in Order headers from the repository
        /// </summary>
        /// <returns>List of order headers</returns>
        public IEnumerable<OrderHeader> GetOrderHeaders()
        {
            return _orderRepository.GetOrderHeaders();
        }
        /// <summary>
        /// Processes an order to the complete state
        /// If there is not enough stock for any of the items the order will be rejected.
        /// </summary>
        /// <param name="orderHeaderId"></param>
        /// <returns></returns>
        public OrderHeader ProcessOrder(int orderHeaderId)
        {

            
            try
            {
                var order = _orderRepository.GetOrderHeader(orderHeaderId);

                try
                {
                    _stockRepository.DecrementOrderStockItemAmount(order);
                    order.Complete();
                }
                catch(SqlException ex)
                {
                    if(ex.Number == 547)
                    {
                        order.Reject();
                    }
                    else
                    {
                        throw ex;
                    }
                }
                _orderRepository.UpdateOrderState(order);
                return order;
            }           
            catch(InvalidOrderStateException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }


        public OrderHeader DeleteOrderItem(int orderHeaderId, int stockItemId)
        {
            _orderRepository.DeleteOrderItem(orderHeaderId, stockItemId);
            return _orderRepository.GetOrderHeader(orderHeaderId);
        }

        public void ResetDatabase()
        {
            _orderRepository.ResetDatabase();
        }

        public void DeleteOrderHeaderAndOrderItems(int orderHeaderId)
        {
            _orderRepository.DeleteOrderHeaderAndOrderItems(orderHeaderId);
        }
    }
}
