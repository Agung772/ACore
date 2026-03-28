using System.IO;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace ACore.Editor
{
    public class FormatAssetTool : OdinEditorWindow
    {
        [Title("Scope")]
        [InfoBox("Centang untuk memproses semua texture di project. Jika tidak, pilih folder target.")]
        [ToggleLeft] public bool processAllAssets = true;

        [ShowIf("ShowFolderField")]
        [FolderPath(AbsolutePath = false)]
        public string folderPath = "Assets/";

        [Title("Android Settings")]
        [ToggleLeft] public bool overrideAndroid;
        [EnableIf("overrideAndroid")] public TextureImporterFormat androidFormat = TextureImporterFormat.RGBA32;
        [EnableIf("overrideAndroid"), Range(32, 8192)] public int androidMaxSize = 2048;

        [Title("iOS Settings")]
        [ToggleLeft] public bool overrideiOS;
        [EnableIf("overrideiOS")] public TextureImporterFormat iosFormat = TextureImporterFormat.RGBA32;
        [EnableIf("overrideiOS"), Range(32, 8192)] public int iosMaxSize = 2048;

        [Title("Desktop Settings")]
        [ToggleLeft] public bool overrideDesktop;
        [EnableIf("overrideDesktop")] public TextureImporterFormat desktopFormat = TextureImporterFormat.RGBA32;
        [EnableIf("overrideDesktop"), Range(32, 8192)] public int desktopMaxSize = 2048;

        [Button(ButtonSizes.Large)]
        [GUIColor(0f, 1f, 0f)]
        private void ApplyFormatToAll()
        {
            string[] _guids;

            if (processAllAssets)
            {
                _guids = AssetDatabase.FindAssets("t:Texture");
            }
            else
            {
                if (string.IsNullOrEmpty(folderPath) || !Directory.Exists(folderPath))
                {
                    EditorUtility.DisplayDialog("Folder tidak valid", "Pastikan folder path valid dan ada di dalam Assets/", "OK");
                    return;
                }

                _guids = AssetDatabase.FindAssets("t:Texture", new[] { folderPath });
            }

            var _count = 0;

            foreach (var _guid in _guids)
            {
                var _path = AssetDatabase.GUIDToAssetPath(_guid);
                var _importer = AssetImporter.GetAtPath(_path) as TextureImporter;

                if (_importer != null)
                {
                    if (overrideAndroid)
                        ApplyPlatformSettings(_importer, "Android", androidFormat, androidMaxSize);

                    if (overrideiOS)
                        ApplyPlatformSettings(_importer, "iOS", iosFormat, iosMaxSize);

                    if (overrideDesktop)
                        ApplyPlatformSettings(_importer, "Standalone", desktopFormat, desktopMaxSize); // internal name tetap Standalone

                    AssetDatabase.ImportAsset(_path, ImportAssetOptions.ForceUpdate);
                    _count++;
                }
            }

            EditorUtility.DisplayDialog("Selesai", $"✅ {_count} texture berhasil diproses sesuai setting!", "OK");
        }

        private void ApplyPlatformSettings(TextureImporter importer, string platform, TextureImporterFormat format, int maxTextureSize)
        {
            // Skip Single Channel textures (e.g., Alpha 8, R8)
            if (importer.textureType == TextureImporterType.SingleChannel)
            {
                Debug.LogWarning($"⏩ Lewati texture '{importer.assetPath}' karena bertipe SingleChannel.");
                return;
            }

            var _settings = importer.GetPlatformTextureSettings(platform);
            _settings.overridden = true;
            _settings.format = format;
            _settings.maxTextureSize = maxTextureSize;
            _settings.compressionQuality = 100;

            importer.SetPlatformTextureSettings(_settings);
        }


        private bool ShowFolderField() => !processAllAssets;

        [MenuItem("Tools/ACore/Texture 2D Converted")]
        private static void OpenWindow()
        {
            GetWindow<FormatAssetTool>().Show();
        }
    }
}
