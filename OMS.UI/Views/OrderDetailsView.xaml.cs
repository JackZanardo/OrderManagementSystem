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
    /// Interaction logic for OrderDetailsView.xaml
    /// </summary>
    public partial class OrderDetailsView : Page
    {
        public OrderDetailsView(OrderHeader order)
        {

            DataContext = order;
            InitializeComponent();
        }

        private void btnOrders_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new OrdersView());
        }

        private void btnProcessOrder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var order = (OrderHeader)DataContext;
                switch(order.State)
                {
                    case OrderStates.Pending:
                        DataContext = OrderController.Instance.ProcessOrder(order.Id);
                        break;
                    case OrderStates.New:
                        DataContext = OrderController.Instance.SubmitOrder(order.Id);
                        break;
                    default:
                        throw new InvalidOrderStateException("Order must be new or pending");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        
    }
}
