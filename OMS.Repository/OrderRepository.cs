using OMS.Domain;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMS.Repository
{
    public class OrderRepository
    {
        string _cs = ConfigurationManager.ConnectionStrings["OrderManagementDb"].ConnectionString;

        public OrderHeader InsertOrderHeader()
        {
            OrderHeader order = null;
            using(var connection = new SqlConnection(_cs))
            using(var command = new SqlCommand("sp_InsertOrderHeader",connection))
            {
                connection.Open();

                int id = Convert.ToInt32(command.ExecuteScalar());
                command.CommandText = "SELECT * FROM OrderHeaders WHERE Id = @id";
                command.Parameters.AddWithValue("@id", id);
                using(var reader = command.ExecuteReader(System.Data.CommandBehavior.SingleRow))
                {
                    reader.Read();
                    var datetime = reader.GetDateTime(2);
                    order = new OrderHeader(id, datetime, 1);
                    reader.Close();
                }

            }

            return order;
        }


        public OrderHeader GetOrderHeader(int id)
        {
            OrderHeader order = null;
            using(var connection = new SqlConnection(_cs))
            using(var command = new SqlCommand("sp_SelectOrderHeaderById @id", connection))
            {
                command.Parameters.AddWithValue("@id", id);
                connection.Open();
                using(var reader = command.ExecuteReader())
                {

                    while(reader.Read())
                    {

                        if(order == null)
                        {
                            int stateId = reader.GetInt32(1);
                            var datetime = reader.GetDateTime(2);
                            order = new OrderHeader(id, datetime, stateId);
                        }
                        if(!reader.IsDBNull(3))
                        {
                            int stockItemId = reader.GetInt32(3);
                            string description = reader.GetString(4);
                            decimal price = reader.GetDecimal(5);
                            int quantity = reader.GetInt32(6);
                            order.AddOrUpdateOrderItem(stockItemId, price, description, quantity);

                        }
                    }
                }
                
            }
            return order;
        }

        public IEnumerable<OrderHeader> GetOrderHeaders()
        {
            var orders = new List<OrderHeader>();
            OrderHeader order = null;
            using(var connection = new SqlConnection(_cs))
            using(var command = new SqlCommand("sp_SelectOrderHeaders", connection))
            {
                connection.Open();
                using(var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        if (order == null || order.Id != id)
                        {
                            int stateId = reader.GetInt32(1);
                            var datetime = reader.GetDateTime(2);
                            order = new OrderHeader(id, datetime, stateId);
                            orders.Add(order);
                        }
                        if (!reader.IsDBNull(3))
                        {
                            int stockItemId = reader.GetInt32(3);
                            string description = reader.GetString(4);
                            decimal price = reader.GetDecimal(5);
                            int quantity = reader.GetInt32(6);
                            order.AddOrUpdateOrderItem(stockItemId, price, description, quantity);
                        }
                    }
                }
            }

            return orders;
        }

        public void UpsertOrderItem(OrderItem orderItem)
        {

            using(var connection = new SqlConnection(_cs))
            using(var command = new SqlCommand("sp_UpsertOrderItem @orderHeaderId, @stockItemId, @description, @price, @quantity", connection))
            {
                connection.Open();
                command.Parameters.AddWithValue("@orderHeaderId", orderItem.OrderHeader.Id);
                command.Parameters.AddWithValue("@stockItemId", orderItem.StockItemId);
                command.Parameters.AddWithValue("@description", orderItem.Description);
                command.Parameters.AddWithValue("@price", orderItem.Price);
                command.Parameters.AddWithValue("@quantity", orderItem.Quantity);
                int rows = command.ExecuteNonQuery();
            }

        }

        public void DeleteOrderItem(int orderHeaderId, int stockItemId)
        {
            using(var connection = new SqlConnection(_cs))
            using(var command = new SqlCommand("sp_DeleteOrderItem @orderHeaderId, @stockItemId", connection))
            {
                command.Parameters.AddWithValue("@orderHeaderId", orderHeaderId);
                command.Parameters.AddWithValue("@stockItemId", stockItemId);
                connection.Open();
                int rows = command.ExecuteNonQuery();
            }
        }

        public void UpdateOrderState(OrderHeader  order)
        {
            using(var connection = new SqlConnection(_cs))
            using(var command = new SqlCommand("sp_UpdateOrderState @orderHeaderId, @stateId", connection))
            {
                command.Parameters.AddWithValue("@orderHeaderId", order.Id);
                command.Parameters.AddWithValue("@stateId", (int)order.State);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public void DeleteOrderHeaderAndOrderItems(int orderHeaderId)
        {
            using(var connection = new SqlConnection(_cs))
            using(var command = new SqlCommand("sp_DeleteOrderHeaderAndOrderItems @orderHeaderId", connection))
            {
                
                command.Parameters.AddWithValue("@orderHeaderId", orderHeaderId);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        

        public void ResetDatabase()
        {
            using (var connection = new SqlConnection(_cs))
            using (var command = new SqlCommand("DELETE FROM OrderItems", connection))
            {
                connection.Open();
                command.ExecuteNonQuery();
                command.CommandText = "DELETE FROM OrderHeaders";
                command.ExecuteNonQuery();
                command.CommandText = "UPDATE [StockItems] SET [InStock] = 10";
                command.ExecuteNonQuery();
            }

        }

    }
}
