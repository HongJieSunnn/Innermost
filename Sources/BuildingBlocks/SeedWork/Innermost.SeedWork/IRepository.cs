using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innermost.SeedWork
{
    /// <summary>
    /// 仓储是用来管理实体的集合。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRepository<T>:IAggregateRoot
    {
        IUnitOfWork UnitOfWork { get; }
    }
}
