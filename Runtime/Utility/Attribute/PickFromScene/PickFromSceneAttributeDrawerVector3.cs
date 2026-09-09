#if UNITY_EDITOR

using System;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector.Editor.Drawers;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace ACore.Tool
{
    public class PickFromSceneAttributeDrawerVector3 : OdinAttributeDrawer<PickFromSceneAttribute, Vector3>
    {
        private string label;
        private Vector3 current;
        private GUIStyle buttonStyle;
        private GUIContent labelContent;
        private IfAttributeHelper ifAttributeHelper;
        private object valueCondition;
        private bool hideIfCondition;
        private bool showHandles = true;
        private float lastDrawnTime;
        private const float HideTimeout = 2.0f; // longer timeout so it still works in cutscene graph / custom editors

        protected override void Initialize()
        {
            label = Property.NiceName.ToTitleCase();
            current = ValueEntry.SmartValue;
            SceneView.duringSceneGui -= OnSceneGUI;
            SceneView.duringSceneGui += OnSceneGUI;
            SceneView.RepaintAll();
            buttonStyle = new GUIStyle(GUI.skin.button);
            SetupOdinVisibilityAttribute();
            lastDrawnTime = Time.realtimeSinceStartup;
        }

        private void OnSceneGUI(SceneView sceneView)
        {
            try
            {
                if (Property == null) return;
                if (Property.Tree == null) return;
                if (Property.Tree.UnitySerializedObject == null) return;
                if (Property.Tree.UnitySerializedObject.targetObject == null) return;
                if (!Property.IsReachableFromRoot())
                {
                    SceneView.duringSceneGui -= OnSceneGUI;
                    return;
                }
            }
            catch
            {
                SceneView.duringSceneGui -= OnSceneGUI;
                return;
            }

            // Only draw handles if the property was drawn recently (node is open / selected)
            // Longer timeout so it continues working inside cutscene graph windows
            if (Time.realtimeSinceStartup - lastDrawnTime > HideTimeout) return;
            if (!showHandles) return;
            if (!IsVisibleInInspector()) return;

            var _handlePosition = Handles.PositionHandle(ValueEntry.SmartValue, Quaternion.identity);
            var _label = Attribute.UsePathAsAsLabel ? Property.Path.Replace("$", "") : label;
            var _cam = SceneView.lastActiveSceneView?.camera;
            if (_cam != null)
            {
                var _offset = -_cam.transform.up * HandleUtility.GetHandleSize(_handlePosition) * 0.2f;
                Handles.Label(_handlePosition + _offset, _label, buttonStyle);
            }

            if (current == _handlePosition) return;

            ValueEntry.SmartValue = _handlePosition;
            current = _handlePosition;

            try
            {
                ValueEntry?.ApplyChanges();
            }
            catch
            {
                SceneView.duringSceneGui -= OnSceneGUI;
            }
        }

        protected override void DrawPropertyLayout(GUIContent content)
        {
            // Keep the "active" timestamp updated every time the property is drawn
            // (this is what keeps the Scene handles visible while the node is open in the graph)
            lastDrawnTime = Time.realtimeSinceStartup;

            GUILayout.BeginHorizontal();

            label = string.IsNullOrEmpty(Attribute.Label) ? (content?.text ?? Property.NiceName) : Attribute.Label;
            var _value = EditorGUILayout.Vector3Field(label, ValueEntry.SmartValue);

            if (current != _value)
            {
                ValueEntry.SmartValue = _value;
                current = _value;
                SceneView.RepaintAll();
                ValueEntry.ApplyChanges();
            }

            // Pick (set to scene view camera)
            if (SirenixEditorGUI.IconButton(EditorIcons.Flag, buttonStyle))
            {
                SetPositionToCurrentSceneViewFrame();
            }

            // Search / Frame
            if (SirenixEditorGUI.IconButton(EditorIcons.MagnifyingGlass, buttonStyle))
            {
                SetFramePosition(current);
            }

            // Show / Hide handles in Scene View
            if (SirenixEditorGUI.IconButton(showHandles ? EditorIcons.Checkmark : EditorIcons.X, buttonStyle))
            {
                showHandles = !showHandles;
                SceneView.RepaintAll();
            }

            GUILayout.EndHorizontal();
        }

        private void SetupOdinVisibilityAttribute()
        {
            var _condition = "";
            if (TryGetAttribute<ShowIfAttribute>(out var _showIfAttribute))
            {
                _condition = _showIfAttribute.Condition;
                valueCondition = _showIfAttribute.Value;
                hideIfCondition = false;
            }

            if (TryGetAttribute<HideIfAttribute>(out var _hideIfAttribute))
            {
                _condition = _hideIfAttribute.Condition;
                valueCondition = _hideIfAttribute.Value;
                hideIfCondition = true;
            }

            if (string.IsNullOrEmpty(_condition)) return;
            ifAttributeHelper = new IfAttributeHelper(Property, _condition, true);
        }

        private bool TryGetAttribute<T>(out T attribute) where T : Attribute
        {
            attribute = Property.Attributes.GetAttribute<T>();
            return attribute != null;
        }

        private bool IsVisibleInInspector()
        {
            if (ifAttributeHelper == null) return true;
            var _ifValue = ifAttributeHelper.GetValue(valueCondition);
            return hideIfCondition ? !_ifValue : _ifValue;
        }

        private void SetPositionToCurrentSceneViewFrame()
        {
            if (SceneView.lastActiveSceneView?.camera == null) return;
            current = SceneView.lastActiveSceneView.camera.transform.position;
            ValueEntry.SmartValue = current;
            SceneView.RepaintAll();
            ValueEntry.ApplyChanges();
        }

        private void SetFramePosition(Vector3 position)
        {
            SceneView.lastActiveSceneView?.Frame(new Bounds(position, Vector3.one * 10), false);
        }

        ~PickFromSceneAttributeDrawerVector3()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
        }
    }
}

#endif
