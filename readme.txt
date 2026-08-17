Request Package
- Odin
- LeanTween
- Newtonsoft Json, link: com.unity.nuget.newtonsoft-json

Request AdMob
- Add package AdMob
- Project > Player > Other Settings > Scripting Define Symbols > Add GOOGLE_MOBILE_ADS

Request Touch Effect
- Bikin prefab dan kasih script TouchEffect.cs
- Letakan prefab di folder Resources

Request Supabase x Google Sign In
Import Google Sign In
- Import semua selain folder "Parse"
- Pada settingsTemplate.gradle, cari "/Assets/GoogleSignIn/Editor/m2repository" dan ganti menjadi "/Assets/GeneratedLocalRepo/GoogleSignIn/Editor/m2repository"
Setup Google Cloud
- Pada Google Cloud masuk ke tab APIs & Services > Credentials > Client
- Create 3 Client ID
- Buka untuk melihat code fingerprint "SHA-1 certificate fingerprint" pada Google Play Console > Protected with Google Play > Buka dropdown pada Google Play Store Protection > Manage Play app signing
- [Apk Name] Android - Play Store, dengan code fingerprint "Application signing key certificate/Sertifikat kunci penandatanganan aplikasi"
- [Apk Name] Android - Upload Key, dengan code fingerprint "Upload key certificate/Sertifikat kunci upload"
- [Apk Name] - Web, dengan code fingerprint "Upload key certificate/Sertifikat kunci upload"
- Simpan Client ID dan Client Secret pada [Apk Name] - Web untuk keperluan Supabase dan ASetting nanti
Setup Supabase
- Pada Supabase Dashboard masuk ke tab Authentication > Sign In / Providers > Auth Providers > cari Google
- Enable Google
- Field Client IDs = seluruh Client ID pada Google Cloud, example: "[Client ID 1],[Client ID 2],[Client ID 3]"
- Field Client Secret (for OAuth) = Client Secret dari [Apk Name] - Web pada Google Cloud
- Field yang lain default
- Save
Setup ASetting (Scriptble Object)
- Centang Supabase
- Pada Supabase masuk ke tab Supabase Dashboard > Project Overview > cari tombol Copy dengan Dropdown
- Field URL = Project URL
- Field Key = Publishable key
- Field Web Client ID = Google Cloud > APIs & Services > Credentials > Client > Client ID pada [Apk Name] - Web
