using System.Collections.Generic;

namespace Chartboost
{
    /// <summary>
    /// Utility class utilized to resolve awaitable proxies for C based wrappers.
    /// </summary>
    public static class AwaitableProxies
    {
        private static readonly Dictionary<int, ILater> WaitingProxies = new Dictionary<int, ILater>();
        
        public static (Later<TResult> proxy, int hashCode) SetupProxy<TResult>()
        {
            var proxy = new Later<TResult>();
            var hashCode = proxy.GetHashCode();
            WaitingProxies[hashCode] = proxy;
            return (proxy, hashCode);
        }
        
        public static void ResolveCallbackProxy<TResponse>(int hashCode, TResponse response) {
            if (!WaitingProxies.ContainsKey(hashCode))
                return;
            if (WaitingProxies[hashCode] is Later<TResponse> later) later.Complete(response);
            WaitingProxies.Remove(hashCode);
        }
    }
}
