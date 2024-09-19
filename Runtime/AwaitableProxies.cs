using System.Collections.Generic;
using Chartboost.Logging;

namespace Chartboost
{
    /// <summary>
    /// Utility class utilized to resolve awaitable proxies for C based wrappers.
    /// </summary>
    public static class AwaitableProxies
    {
        private static readonly Dictionary<int, ILater> WaitingProxies = new();
        
        public static (Later<TResult> proxy, int uniqueId) SetupProxy<TResult>()
        {
            var proxy = new Later<TResult>();
            var uniqueId = proxy.GetHashCode();
            WaitingProxies[uniqueId] = proxy;
            LogController.Log($"Added callback proxy for: {uniqueId}", LogLevel.Debug);
            return (proxy, uniqueId);
        }
        
        public static void ResolveCallbackProxy<TResponse>(int uniqueId, TResponse response) {
            if (!WaitingProxies.TryGetValue(uniqueId, out var proxy))
            {
                LogController.Log($"Unable to find callback proxy for: {uniqueId} of type: {typeof(TResponse).Name}", LogLevel.Debug);
                return;
            }

            if (proxy is Later<TResponse> later)
                later.Complete(response);
            WaitingProxies.Remove(uniqueId);
        }
    }
}
