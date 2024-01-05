using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Swerva.Utilities
{
    public delegate Task AsyncEventHandler<TEventArgs>(object sender, TEventArgs e) where TEventArgs : EventArgs;

    public static class AsyncEventHandlerExtensions
    {
        public static IEnumerable<AsyncEventHandler<TEventArgs>> GetHandlers<TEventArgs>(this AsyncEventHandler<TEventArgs> handler) where TEventArgs : EventArgs
        {
            return handler.GetInvocationList().Cast<AsyncEventHandler<TEventArgs>>();
        }

        public static Task InvokeAllAsync<TEventArgs>(this AsyncEventHandler<TEventArgs> handler, object sender, TEventArgs e) where TEventArgs : EventArgs
        {
            return Task.WhenAll(handler.GetHandlers().Select(handleAsync => handleAsync(sender, e)));
        }
    }    
}