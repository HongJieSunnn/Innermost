using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Innermost.SeedWork
{
    /// <summary>
    /// 工作单元 Unit of Work是用来解决领域模型存储和变更工作，在ORM进行持久化的时候，比如Entity Framework的SaveChanges操作，其实就可以看做是Unit Of Work
    /// </summary>
    /// <remarks>
    /// 详情可以看:https://www.cnblogs.com/xishuai/p/3750154.html
    /// </remarks>
    public interface IUnitOfWork:IDisposable
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
        Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
