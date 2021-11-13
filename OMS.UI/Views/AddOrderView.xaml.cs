using OMS.Controllers;
using OMS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OMS.UI.Views
{
    /// <summary>
    /// Interaction logic for AddOrderView.xaml
    /// </summary>
    public partial class AddOrderView : Page
    {
        public AddOrderView(OrderHeader order = null)
        {
            try
            {
                if (order == null)
                {
                    DataContext = OrderController.Instance.CreateNewOrderHeader();
                }
                else
                {
                    DataContext = order;
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            InitializeComponent();
        }

        private void btnAddItem_Click(object sender, RoutedEventArgs e)
        {
            var order = (DataContext as OrderHeader);
            NavigationService.Navigate(new AddOrderItemView(order));
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OrderController.Instance.DeleteOrderHeaderAndOrderItems((DataContext as OrderHeader).Id);
                NavigationService.Navigate(new OrdersView());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OrderController.Instance.SubmitOrder((DataContext as OrderHeader).Id);
                NavigationService.Navigate(new OrdersView());

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnDeleteOrderItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var item = (OrderItem)((Button)e.Source).DataContext;
                DataContext = OrderController.Instance.DeleteOrderItem(item.OrderHeaderId, item.StockItemId);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
