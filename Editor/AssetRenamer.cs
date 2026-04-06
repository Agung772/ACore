using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;

namespace ACore
{
    public class AssetRenamerOdin : OdinEditorWindow
{
    [MenuItem("Tools/ACore/Asset Rename")]
    private static void Open()
    {
        GetWindow<AssetRenamerOdin>().Show();
    }

    [BoxGroup("Rename Settings")]
    [LabelText("Old Keyword")]
    public string oldKeyword = "";

    [BoxGroup("Rename Settings")]
    [LabelText("New Keyword")]
    public string newKeyword = "";

    [BoxGroup("Scope")]
    [AssetsOnly]
    [FolderPath]
    public string targetFolder = "Assets";

    [BoxGroup("Options")]
    public bool includeSubfolders = true;

    [BoxGroup("Preview"), ReadOnly]
    [TableList]
    public List<RenamePreview> previews = new();

    public class RenamePreview
    {
        [ReadOnly] public string path;
        [ReadOnly] public string oldName;
        [ReadOnly] public string newName;
    }

    [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1f)]
    private void Scan()
    {
        previews.Clear();

        var _guids = AssetDatabase.FindAssets("", new[] { targetFolder });

        foreach (var _guid in _guids)
        {
            var _path = AssetDatabase.GUIDToAssetPath(_guid);

            if (!includeSubfolders)
            {
                var _parent = Path.GetDirectoryName(_path);
                if (_parent != targetFolder)
                    continue;
            }

            var _fileName = Path.GetFileNameWithoutExtension(_path);

            if (!_fileName.Contains(oldKeyword))
                continue;

            var _newName = _fileName.Replace(oldKeyword, newKeyword);

            previews.Add(new RenamePreview
            {
                path = _path,
                oldName = _fileName,
                newName = _newName
            });
        }

        Debug.Log($"🔍 Found {previews.Count} assets to rename");
    }

    [Button(ButtonSizes.Large), GUIColor(0.3f, 1f, 0.3f)]
    private void RenameAll()
    {
        var _success = 0;

        foreach (var _item in previews)
        {
            var _error = AssetDatabase.RenameAsset(_item.path, _item.newName);

            if (string.IsNullOrEmpty(_error))
            {
                _success++;
            }
            else
            {
                Debug.LogError($"❌ Failed: {_item.path} | {_error}");
            }
        }

        AssetDatabase.Refresh();
        Debug.Log($"✅ Renamed {_success} assets");
    }
}
}
