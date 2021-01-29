using System;

namespace TestWebApiApp
{
    internal class JWTSettings
    {
        public string ValidIssuer { get; set; }
        public string ValidAudience { get; set; }
        public int ExpiresInMins { get; set; }

    }
}