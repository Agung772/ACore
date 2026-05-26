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
    public class PickFromSceneAttributeDrawerVector2 : OdinAttributeDrawer<PickFromSceneAttribute, Vector2>{
        private string label;
        private Vector2 current;
        private GUIStyle buttonStyle;
        private GUIContent labelContent;
        private IfAttributeHelper ifAttributeHelper;
        private object valueCondition;
        private bool hideIfCondition;

        protected override void Initialize() {
            label = Property.NiceName.ToTitleCase();
            current = ValueEntry.SmartValue;
            SceneView.duringSceneGui -= OnSceneGUI;
            SceneView.duringSceneGui += OnSceneGUI;
            SceneView.RepaintAll();
            buttonStyle = new GUIStyle(GUI.skin.button);
            SetupOdinVisibilityAttribute();
        }

        private void OnSceneGUI(SceneView sceneView) {
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
            
            if (Property.LastDrawnValueRect.height <= 0f) return;
            if (!IsVisibleInInspector()) return;
            
            var _handlePosition = Handles.PositionHandle(ValueEntry.SmartValue, Quaternion.identity);
            var _label = Attribute.UsePathAsAsLabel ? Property.Path.Replace("$", "") : label;
            var _cam = SceneView.lastActiveSceneView.camera;
            var _offset = -_cam.transform.up * HandleUtility.GetHandleSize(_handlePosition) * 0.2f;
            Handles.Label(_handlePosition + _offset, _label, buttonStyle);
            
            if (current == (Vector2)_handlePosition) return;
            
            ValueEntry.SmartValue = _handlePosition;
            current = _handlePosition;
            try {
                ValueEntry?.ApplyChanges();
            }
            catch (Exception) {
                SceneView.duringSceneGui -= OnSceneGUI;
            }
        }

        protected override void DrawPropertyLayout(GUIContent content) {
            GUILayout.BeginHorizontal();
            if (Attribute.Label != "") {
                this.label = Attribute.Label;
            }
            else {
                this.label = content != null ? content.text : Property.NiceName;
            }

            var _value = EditorGUILayout.Vector2Field(this.label, ValueEntry.SmartValue);

            if (current != _value) {
                ValueEntry.SmartValue = _value;
                current = _value;
                SceneView.RepaintAll();
                ValueEntry.ApplyChanges();
            }

            if (SirenixEditorGUI.IconButton(EditorIcons.Flag, buttonStyle)) {
                SetPositionToCurrentSceneViewFrame();
            }

            if (SirenixEditorGUI.IconButton(EditorIcons.MagnifyingGlass, buttonStyle)) {
                SetFramePosition(current);
            }

            GUILayout.EndHorizontal();
        }

        private void SetupOdinVisibilityAttribute() {
            var _condition = "";
            if (TryGetAttribute<ShowIfAttribute>(out var _showIfAttribute)) {
                _condition = _showIfAttribute.Condition;
                valueCondition = _showIfAttribute.Value;
                hideIfCondition = false;
            }

            if (TryGetAttribute<HideIfAttribute>(out var _hideIfAttribute)) {
                _condition = _hideIfAttribute.Condition;
                valueCondition = _hideIfAttribute.Value;
                hideIfCondition = true;
            }

            if (_condition == "") return;
            ifAttributeHelper = new IfAttributeHelper(Property, _condition, true);
        }

        private bool TryGetAttribute<T>(out T attribute) where T : Attribute {
            attribute = Property.Attributes.GetAttribute<T>();
            return attribute != null;
        }

        private bool IsVisibleInInspector() {
            if (ifAttributeHelper == null) return true;
            var _ifValue = ifAttributeHelper.GetValue(valueCondition);
            if (hideIfCondition) return !_ifValue;
            return _ifValue;
        }

        private void SetPositionToCurrentSceneViewFrame() {
            if (SceneView.lastActiveSceneView == null) return;
            if (SceneView.lastActiveSceneView.camera == null) return;
            current = SceneView.lastActiveSceneView.camera.transform.position;
            ValueEntry.SmartValue = current;
            SceneView.RepaintAll();
            ValueEntry.ApplyChanges();
        }

        private void SetFramePosition(Vector2 position) {
            if (SceneView.lastActiveSceneView == null) return;
            SceneView.lastActiveSceneView.Frame(new Bounds(position, Vector3.one * 10), false);
        }

        ~PickFromSceneAttributeDrawerVector2() {
            SceneView.duringSceneGui -= OnSceneGUI;
        }
    }
}

#endif