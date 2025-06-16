using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wim.Abstractions
{
    public interface IMessageManager
    {
		public abstract void Subscribe(string messageType, Action<object?> callback);
		public abstract void Unsubscribe(string messageType, Action<object?> callback);
		public abstract void ClearSubscriptions();
		public abstract void NotifyAll(string messageType, object? message);
		public abstract void NotifyOne(string messageType, object? message);
    }
}
