namespace AppBlockerSecurity.Services
{
    public static class SecurityManager
    {
        public static string CurrentBlockedPackage { get; set; } = string.Empty;

        private static string _unlockedPackage = string.Empty;
        private static DateTime _unlockedUntil = DateTime.MinValue;

        public static void AuthorizeAccess(int durationInSeconds = 60)
        {
            _unlockedPackage = CurrentBlockedPackage;
            _unlockedUntil = DateTime.Now.AddSeconds(durationInSeconds);
        }

        public static bool IsAuthorized(string packageName)
        {
            // só libera se for o pacote correto E se o tempo ainda não expirou
            return packageName == _unlockedPackage && DateTime.Now < _unlockedUntil;
        }
    }
}