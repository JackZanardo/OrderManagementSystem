using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMS.Domain
{
    public class OrderComplete : OrderState
    {
        public OrderComplete(OrderHeader orderHeader) : base(orderHeader) { }

        public override OrderStates State => OrderStates.Complete;

        public override void Complete(ref OrderState _state)
        {
            throw new InvalidOrderStateException("This order is already complete");
        }

        public override void Reject(ref OrderState _state)
        {
            throw new InvalidOrderStateException("This order is already complete and can not be rejected");
        }

        public override void Submit(ref OrderState _state)
        {
            throw new InvalidOrderStateException("This order is already complete and can not be submitted");
        }
    }
}
