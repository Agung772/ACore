using System;

namespace ACore
{
    [Serializable]
    public class SupabaseSetting
    {
        public string url = "https://xxx.supabase.co";
        public string key = "sb_xxx";
        
        // Google Cloud → APIs & Services → Credentials → OAuth 2.0 Client IDs
        public string webClientID = "xxx.apps.googleusercontent.com";
    }
}
