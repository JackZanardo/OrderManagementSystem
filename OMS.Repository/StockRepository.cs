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
    public class StockRepository
    {

        string _cs = ConfigurationManager.ConnectionStrings["OrderManagementDb"].ConnectionString;
        public IEnumerable<StockItem> GetStockItems()
        {
            var items = new List<StockItem>();
            using (var connection = new SqlConnection(_cs))
            using (var command = new SqlCommand("sp_SelectStockItems",connection))
            {
                connection.Open();
                var reader = command.ExecuteReader();
                while(reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string name = reader.GetString(1);
                    decimal price = reader.GetDecimal(2);
                    int inStock = reader.GetInt32(3);
                    var item = new StockItem(id, name, price, inStock);
                    items.Add(item);
                }
                
            }
            return items;
        }


        public StockItem GetStockItem(int id)
        {
            StockItem item = null;

            using(var connection = new SqlConnection(_cs))
            using(var command = new SqlCommand("sp_SelectStockItemById @id", connection))
            {
                command.Parameters.AddWithValue("@id", id);
                connection.Open();
                var reader = command.ExecuteReader(System.Data.CommandBehavior.SingleRow);
                reader.Read();

                string name = reader.GetString(1);
                decimal price = reader.GetDecimal(2);
                int inStock = reader.GetInt32(3);
                item = new StockItem(id, name, price, inStock);

            }
            return item;

        }

        public void DecrementOrderStockItemAmount(OrderHeader order)
        {
            using(var connection = new SqlConnection(_cs))
            using(var command = connection.CreateCommand())
            {
                connection.Open();
                var transaction = connection.BeginTransaction("UpdateStockLevelTransaction");
                command.Transaction = transaction;
                try
                {
                    command.CommandText = "sp_UpdateStockItemAmount @id, @amount";
                    foreach(var oi in order.OrderItems)
                    {
                        command.Parameters.AddWithValue("@id", oi.StockItemId);
                        command.Parameters.AddWithValue("@amount", -oi.Quantity);
                        command.ExecuteNonQuery();
                        command.Parameters.Clear();
                    }
                    transaction.Commit();
                }
                catch(SqlException ex)
                {
                    transaction.Rollback();
                    throw ex;
                }

            }
        }
    }
}
