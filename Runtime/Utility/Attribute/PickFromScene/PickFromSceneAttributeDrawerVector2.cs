#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace ACore.Tool
{
    public class PickFromSceneAttributeDrawerVector2 : OdinAttributeDrawer<PickFromSceneAttribute, Vector2>
    {
        private static readonly List<PickFromSceneAttributeDrawerVector2> ActiveDrawers = new List<PickFromSceneAttributeDrawerVector2>();
        private static bool sceneGuiRegistered;

        private string label;
        private Vector2 current;
        private GUIStyle buttonStyle;
        private bool showHandles;
        private double lastDrawnTime;
        private bool isDragging;
        private bool registered;

        protected override void Initialize()
        {
            label = Property.NiceName.ToTitleCase();
            current = ValueEntry.SmartValue;
            showHandles = false;
            isDragging = false;
            lastDrawnTime = EditorApplication.timeSinceStartup;
            buttonStyle = new GUIStyle(GUI.skin.button);
            RegisterDrawer();
        }

        private void RegisterDrawer()
        {
            if (registered) return;

            for (int i = ActiveDrawers.Count - 1; i >= 0; i--)
            {
                var other = ActiveDrawers[i];
                if (other == null)
                {
                    ActiveDrawers.RemoveAt(i);
                    continue;
                }
                if (other == this) continue;
                try
                {
                    if (other.Property != null && Property != null && other.Property.Path == Property.Path)
                        other.UnregisterDrawer();
                }
                catch
                {
                    other.UnregisterDrawer();
                }
            }

            registered = true;
            ActiveDrawers.Add(this);
            if (!sceneGuiRegistered)
            {
                SceneView.duringSceneGui += StaticOnSceneGUI;
                sceneGuiRegistered = true;
            }
        }

        private void UnregisterDrawer()
        {
            if (!registered) return;
            registered = false;
            ActiveDrawers.Remove(this);
            showHandles = false;
            isDragging = false;
            if (ActiveDrawers.Count == 0 && sceneGuiRegistered)
            {
                SceneView.duringSceneGui -= StaticOnSceneGUI;
                sceneGuiRegistered = false;
            }
        }

        private static void StaticOnSceneGUI(SceneView sceneView)
        {
            for (int i = ActiveDrawers.Count - 1; i >= 0; i--)
            {
                var drawer = ActiveDrawers[i];
                if (drawer == null || !drawer.IsPropertyValid())
                {
                    if (drawer != null) drawer.UnregisterDrawer();
                    else ActiveDrawers.RemoveAt(i);
                    continue;
                }
                drawer.DrawSceneHandles();
            }
        }

        private bool IsPropertyValid()
        {
            try
            {
                if (Property == null) return false;
                if (Property.Tree == null) return false;
                var so = Property.Tree.UnitySerializedObject;
                if (so != null && so.targetObject == null) return false;
                if (!Property.IsReachableFromRoot()) return false;
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void DrawSceneHandles()
        {
            if (!showHandles)
            {
                isDragging = false;
                return;
            }

            if (!isDragging && EditorApplication.timeSinceStartup - lastDrawnTime > 1.0)
            {
                showHandles = false;
                isDragging = false;
                return;
            }

            EditorGUI.BeginChangeCheck();
            var handlePosition = Handles.PositionHandle(ValueEntry.SmartValue, Quaternion.identity);

            var drawLabel = Attribute.UsePathAsAsLabel ? Property.Path.Replace("$", "") : label;
            var cam = SceneView.lastActiveSceneView != null ? SceneView.lastActiveSceneView.camera : null;
            if (cam != null)
            {
                var offset = -cam.transform.up * HandleUtility.GetHandleSize(handlePosition) * 0.2f;
                Handles.Label(handlePosition + offset, drawLabel, buttonStyle);
            }

            if (EditorGUI.EndChangeCheck())
            {
                isDragging = true;
                lastDrawnTime = EditorApplication.timeSinceStartup;
                ValueEntry.SmartValue = handlePosition;
                current = handlePosition;
                try { ValueEntry.ApplyChanges(); }
                catch { UnregisterDrawer(); }
            }
            else if (GUIUtility.hotControl == 0)
            {
                isDragging = false;
            }
        }

        protected override void DrawPropertyLayout(GUIContent content)
        {
            if (!registered) RegisterDrawer();
            lastDrawnTime = EditorApplication.timeSinceStartup;

            GUILayout.BeginHorizontal();

            if (!string.IsNullOrEmpty(Attribute.Label))
                label = Attribute.Label;
            else
                label = content != null ? content.text : Property.NiceName;

            var value = EditorGUILayout.Vector2Field(label, ValueEntry.SmartValue);
            if (current != value)
            {
                ValueEntry.SmartValue = value;
                current = value;
                SceneView.RepaintAll();
                ValueEntry.ApplyChanges();
            }

            if (SirenixEditorGUI.IconButton(EditorIcons.Flag, buttonStyle))
                SetPositionToCurrentSceneViewFrame();

            if (SirenixEditorGUI.IconButton(EditorIcons.MagnifyingGlass, buttonStyle))
                SetFramePosition(current);

            if (SirenixEditorGUI.IconButton(showHandles ? EditorIcons.Checkmark : EditorIcons.X, buttonStyle))
            {
                showHandles = !showHandles;
                isDragging = false;
                lastDrawnTime = EditorApplication.timeSinceStartup;
                SceneView.RepaintAll();
            }

            GUILayout.EndHorizontal();
        }

        private void SetPositionToCurrentSceneViewFrame()
        {
            if (SceneView.lastActiveSceneView == null || SceneView.lastActiveSceneView.camera == null) return;
            current = SceneView.lastActiveSceneView.camera.transform.position;
            ValueEntry.SmartValue = current;
            lastDrawnTime = EditorApplication.timeSinceStartup;
            SceneView.RepaintAll();
            ValueEntry.ApplyChanges();
        }

        private void SetFramePosition(Vector2 position)
        {
            if (SceneView.lastActiveSceneView == null) return;
            SceneView.lastActiveSceneView.Frame(new Bounds(position, Vector3.one * 10), false);
        }

        ~PickFromSceneAttributeDrawerVector2()
        {
            UnregisterDrawer();
        }
    }
}

#endif
