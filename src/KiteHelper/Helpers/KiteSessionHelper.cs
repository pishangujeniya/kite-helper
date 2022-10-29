using System.Collections.Concurrent;
using KiteConnectSdk;

namespace KiteHelper.Helpers
{
    public static class KiteSessionHelper
    {
        private static readonly ConcurrentDictionary<string, KiteSdk> KiteSdks = new ConcurrentDictionary<string, KiteSdk>(StringComparer.InvariantCultureIgnoreCase);
        private static readonly ConcurrentDictionary<string, string> KiteSessions = new ConcurrentDictionary<string, string>();

        public static bool IsSessionValid(string sessionId)
        {
            if (string.IsNullOrWhiteSpace(sessionId))
            {
                return false;
            }
            return KiteSessions.ContainsKey(sessionId);
        }

        public static string? AddKiteSession(string userName, KiteSdk kiteSdk)
        {
            if (AddKiteSdk(userName, kiteSdk))
            {
                string sessionId = Guid.NewGuid().ToString();
                if (KiteSessions.TryAdd(sessionId, userName))
                {
                    return sessionId;
                };
            }

            return null;
        }

        public static KiteSdk? GetKiteSdkFromSession(string sessionId)
        {
            if (KiteSessions.TryGetValue(sessionId, out string? userName))
            {
                if (!string.IsNullOrWhiteSpace(userName))
                {
                    return GetKiteSdk(userName);
                }
            }

            return null;
        }


        private static KiteSdk? GetKiteSdk(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName)) throw new ArgumentNullException(userName);

            KiteSdks.TryGetValue(userName, out KiteSdk? kiteSdk);
            return kiteSdk;
        }

        private static bool AddKiteSdk(string userName, KiteSdk kiteSdk)
        {
            if (string.IsNullOrWhiteSpace(userName)) throw new ArgumentNullException(userName);

            KiteSdks.TryRemove(userName, out KiteSdk? _);
            return KiteSdks.TryAdd(userName, kiteSdk);
        }
    }
}
