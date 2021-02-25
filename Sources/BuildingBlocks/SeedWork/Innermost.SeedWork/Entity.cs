using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innermost.SeedWork
{
    /// <summary>
    /// Innermost中采用DDD的服务的 实体基类
    /// </summary>
    public abstract class Entity
    {
        int? _requestedHashCode;
        int _Id;

        public int Id
        {
            get { return _Id; }
            set { _Id = value; }
        }
        /// <summary>
        /// MediatR.Inotification 通知，也就是发布订阅中的事件
        /// </summary>
        private List<INotification> _domainEvents;
        public IReadOnlyCollection<INotification> DomainEvents => _domainEvents.AsReadOnly();
        public void AddDomainEvent(INotification eventItem)
        {
            _domainEvents = _domainEvents ?? new List<INotification>();
            _domainEvents.Add(eventItem);
        }

        public void RemoveDomainEvent(INotification eventItem)
        {
            _domainEvents?.Remove(eventItem);
        }

        public void ClearDomainEvents()
        {
            _domainEvents?.Clear();
        }

        public bool IsTransient()
        {
            return this.Id == default(Int32);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Entity))
                return false;

            if (Object.ReferenceEquals(this, obj))//如果指向同一实例，相等。
                return true;

            if (this.GetType() != obj.GetType())
                return false;

            Entity entity = (Entity)obj;
            //实体如果是临时的，那么不可能存在和它一样的另一个实体
            if (entity.IsTransient() || this.IsTransient())
                return false;
            else
                return entity.Id == this.Id;//如果不是一个实例，但是Id相等，由于实体具有唯一的Id，所以也相等。
        }

        public override int GetHashCode()
        {
            if(!IsTransient())
            {
                if(!_requestedHashCode.HasValue)
                {
                    _requestedHashCode = this.Id.GetHashCode() ^ 31;
                }
                return _requestedHashCode.Value;
            }
            return base.GetHashCode();
        }

        public static bool operator==(Entity left,Entity right)
        {
            if (Object.Equals(left, null))
                return (Object.Equals(right, null)) ? true : false;
            else
                return left.Equals(right);
        }

        public static bool operator!=(Entity left,Entity right)
        {
            return !(left == right);
        }
    }
}
