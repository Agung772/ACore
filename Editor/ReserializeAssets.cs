using UnityEditor;
using UnityEngine;

namespace ACore
{
    public class ReserializeAssets : MonoBehaviour
    {
        [MenuItem("Tools/ACore/Refresh Assets")]
        public static void ReserializeAll()
        {
            var _allAssetPaths = AssetDatabase.GetAllAssetPaths();

            var _count = 0;

            foreach (var _path in _allAssetPaths)
            {
                if (_path.EndsWith(".prefab") || _path.EndsWith(".asset"))
                {
                    var _asset = AssetDatabase.LoadAssetAtPath<Object>(_path);

                    if (_asset == null)
                        continue;
                    
                    EditorUtility.SetDirty(_asset);

                    _count++;

                    if (_count % 100 == 0)
                    {
                        Debug.Log($"Processed {_count} assets...");
                    }
                }
            }
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"Done! Total processed: {_count}");
        }
    }
}
