using System.Collections.Generic;
using Microsoft.Identity.Client;

namespace BotAuth
{
    // ReSharper disable once InconsistentNaming
    public class InMemoryTokenCacheMSAL
    {
        private readonly string _cacheId;
        private readonly Dictionary<string, object> _cacheData = new Dictionary<string, object>();
        private readonly TokenCache _cache = new TokenCache();

        public InMemoryTokenCacheMSAL()
        {
            _cacheId = "MSAL_TokenCache";
            _cache.SetBeforeAccess(BeforeAccessNotification);
            _cache.SetAfterAccess(AfterAccessNotification);
            Load();
        }

        public InMemoryTokenCacheMSAL(byte[] tokenCache)
        {
            _cacheId = "MSAL_TokenCache";
            _cache.SetBeforeAccess(BeforeAccessNotification);
            _cache.SetAfterAccess(AfterAccessNotification);
            _cache.Deserialize(tokenCache);
        }

        public TokenCache GetMsalCacheInstance()
        {
            _cache.SetBeforeAccess(BeforeAccessNotification);
            _cache.SetAfterAccess(AfterAccessNotification);
            Load();
            return _cache;
        }

        public void SaveUserStateValue(string state) => _cacheData[_cacheId + "_state"] = state;

        public string ReadUserStateValue() => (string)_cacheData[_cacheId + "_state"];

        private void Load()
        {
            if (_cacheData.ContainsKey(_cacheId))
                _cache.Deserialize((byte[])_cacheData[_cacheId]);
        }

        private void Persist()
        {
            // Optimistically set HasStateChanged to false. We need to do it early to avoid losing changes made by a concurrent thread.
            _cache.HasStateChanged = false;

            // Reflect changes in the persistent store
            _cacheData[_cacheId] = _cache.Serialize();
        }

        /*
        // Empties the persistent store.
        public override void Clear(string cliendId)
        {
            base.Clear(cliendId);
            cache.Remove(CacheId);
        }
        */

        // Triggered right before ADAL needs to access the cache.
        // Reload the cache from the persistent store in case it changed since the last access.
        private void BeforeAccessNotification(TokenCacheNotificationArgs args) => Load();

        // Triggered right after ADAL accessed the cache.
        private void AfterAccessNotification(TokenCacheNotificationArgs args)
        {
            // if the access operation resulted in a cache update
            if (_cache.HasStateChanged)
            {
                Persist();
            }
        }
    }
}
