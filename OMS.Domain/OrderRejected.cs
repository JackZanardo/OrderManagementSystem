using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMS.Domain
{
    public class OrderRejected : OrderState
    {
        public OrderRejected(OrderHeader orderHeader) : base(orderHeader) { }

        public override OrderStates State => OrderStates.Rejected;

        public override void Complete(ref OrderState _state)
        {
            throw new InvalidOrderStateException("This order is already rejected and can not be completed");
        }

        public override void Reject(ref OrderState _state)
        {
            throw new InvalidOrderStateException("This order is already been rejected");
        }

        public override void Submit(ref OrderState _state)
        {
            throw new InvalidOrderStateException("This order is rejected and can not be submitted");
        }
    }
}
