using Innermost.EventBusInnermost.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innermost.EventBusInnermost.Abstractions
{
    /// <summary>
    /// 事件处理者的接口，也就是订阅者。
    /// </summary>
    /// <typeparam name="TIntegrationEvent">事件类型</typeparam>
    public interface IIntegrationEventHandler<in TIntegrationEvent>: IIntegrationEventHandler // in修饰符 docs:https://docs.microsoft.com/zh-cn/dotnet/csharp/language-reference/keywords/in-generic-modifier
        where TIntegrationEvent:IntegrationEvent
    {
        Task Handle(TIntegrationEvent @event);
    }

    public interface IIntegrationEventHandler
    {
    }
}
