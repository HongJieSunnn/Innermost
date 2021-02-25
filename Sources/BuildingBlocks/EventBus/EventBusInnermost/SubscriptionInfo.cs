using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innermost.EventBusInnermost
{
    public partial class InMemoryEventBusSubscriptionsManager:IEventBusSubscriptionManager
    {
        /// <summary>
        /// 订阅者信息，也就是handler，有多态和模板两种。
        /// </summary>
        public class SubscriptionInfo
        {
            public bool IsDynamic { get;}
            public Type HandlerType { get; }
            /// <summary>
            /// get instance by staic method so that construct is private
            /// </summary>
            /// <param name="isDynamic"></param>
            /// <param name="handlerType"></param>
            private SubscriptionInfo(bool isDynamic,Type handlerType)
            {
                IsDynamic = isDynamic;
                HandlerType = handlerType;
            }

            public static SubscriptionInfo Dynamic(Type handlerType)
            {
                return new SubscriptionInfo(isDynamic: true, handlerType);
            }

            public static SubscriptionInfo Typed(Type handlerType)
            {
                return new SubscriptionInfo(isDynamic: false, handlerType);
            }
        }
    }
}
