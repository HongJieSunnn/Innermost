﻿using Innermost.EventBusInnermost.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innermost.EventBusInnermost.Abstractions
{
    /// <summary>
    /// 多态事件处理者，也就是订阅者。
    /// </summary>
    public interface IDynamicIntegrationEventHandler
    {
        Task Handle(dynamic @event);
    }
}
