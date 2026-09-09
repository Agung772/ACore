#if UNITY_EDITOR

using System;
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
        private string label;
        private Vector2 current;
        private GUIStyle buttonStyle;
        private IfAttributeHelper ifAttributeHelper;
        private object valueCondition;
        private bool hideIfCondition;
        private bool showHandles;
        private double lastDrawnTime;
        private bool isDragging;

        protected override void Initialize()
        {
            label = Property.NiceName.ToTitleCase();
            current = ValueEntry.SmartValue;
            showHandles = false;
            isDragging = false;
            lastDrawnTime = EditorApplication.timeSinceStartup;

            SceneView.duringSceneGui -= OnSceneGUI;
            SceneView.duringSceneGui += OnSceneGUI;
            SceneView.RepaintAll();

            buttonStyle = new GUIStyle(GUI.skin.button);
            SetupOdinVisibilityAttribute();
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

        private bool ShouldDrawHandles()
        {
            if (!showHandles) return false;
            if (!IsPropertyValid()) return false;
            if (!IsVisibleInInspector()) return false;
            if (isDragging) return true;
            if (GUIUtility.hotControl != 0) return true;
            return EditorApplication.timeSinceStartup - lastDrawnTime < 1.0;
        }

        private void OnSceneGUI(SceneView sceneView)
        {
            if (!IsPropertyValid())
            {
                SceneView.duringSceneGui -= OnSceneGUI;
                showHandles = false;
                isDragging = false;
                return;
            }

            if (!ShouldDrawHandles())
            {
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
                try
                {
                    ValueEntry.ApplyChanges();
                }
                catch
                {
                    SceneView.duringSceneGui -= OnSceneGUI;
                }
            }
            else if (GUIUtility.hotControl == 0)
            {
                isDragging = false;
            }
        }

        protected override void DrawPropertyLayout(GUIContent content)
        {
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
            {
                SetPositionToCurrentSceneViewFrame();
            }

            if (SirenixEditorGUI.IconButton(EditorIcons.MagnifyingGlass, buttonStyle))
            {
                SetFramePosition(current);
            }

            if (SirenixEditorGUI.IconButton(showHandles ? EditorIcons.Checkmark : EditorIcons.X, buttonStyle))
            {
                showHandles = !showHandles;
                isDragging = false;
                lastDrawnTime = EditorApplication.timeSinceStartup;
                SceneView.RepaintAll();
            }

            GUILayout.EndHorizontal();
        }

        private void SetupOdinVisibilityAttribute()
        {
            var condition = "";
            if (TryGetAttribute<ShowIfAttribute>(out var showIfAttribute))
            {
                condition = showIfAttribute.Condition;
                valueCondition = showIfAttribute.Value;
                hideIfCondition = false;
            }

            if (TryGetAttribute<HideIfAttribute>(out var hideIfAttribute))
            {
                condition = hideIfAttribute.Condition;
                valueCondition = hideIfAttribute.Value;
                hideIfCondition = true;
            }

            if (string.IsNullOrEmpty(condition)) return;
            ifAttributeHelper = new IfAttributeHelper(Property, condition, true);
        }

        private bool TryGetAttribute<T>(out T attribute) where T : Attribute
        {
            attribute = Property.Attributes.GetAttribute<T>();
            return attribute != null;
        }

        private bool IsVisibleInInspector()
        {
            if (ifAttributeHelper == null) return true;
            var ifValue = ifAttributeHelper.GetValue(valueCondition);
            return hideIfCondition ? !ifValue : ifValue;
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
            SceneView.duringSceneGui -= OnSceneGUI;
        }
    }
}

#endif
