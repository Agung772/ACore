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
    public class PickFromSceneAttributeDrawerPose : OdinValueDrawer<Pose>
    {
        private string label;
        private Pose current;
        private GUIStyle buttonStyle;
        private IfAttributeHelper ifAttributeHelper;
        private object valueCondition;
        private bool hideIfCondition;

        protected override void Initialize()
        {
            label = Property.NiceName.ToTitleCase();
            current = ValueEntry.SmartValue;

            SceneView.duringSceneGui -= OnSceneGUI;
            SceneView.duringSceneGui += OnSceneGUI;

            SceneView.RepaintAll();

            buttonStyle = new GUIStyle(GUI.skin.button);

            SetupOdinVisibilityAttribute();
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

            if (Property.LastDrawnValueRect.height <= 0f) return;
            if (!IsVisibleInInspector()) return;

            var _pose = ValueEntry.SmartValue;

            EditorGUI.BeginChangeCheck();

            var _position = Handles.PositionHandle(_pose.position, _pose.rotation);
            var _rotation = Handles.RotationHandle(_pose.rotation, _position);

            if (EditorGUI.EndChangeCheck())
            {
                _pose.position = _position;
                _pose.rotation = _rotation;

                ValueEntry.SmartValue = _pose;
                current = _pose;

                try
                {
                    ValueEntry.ApplyChanges();
                }
                catch
                {
                    SceneView.duringSceneGui -= OnSceneGUI;
                }
            }

            DrawVisual(_pose);
        }

        protected override void DrawPropertyLayout(GUIContent content)
        {
            CallNextDrawer(content);

            GUILayout.BeginHorizontal();

            GUILayout.Space(EditorGUI.indentLevel * 15f);

            if (SirenixEditorGUI.IconButton(EditorIcons.Flag, buttonStyle))
            {
                SetPositionToCurrentSceneViewFrame();
            }

            if (SirenixEditorGUI.IconButton(EditorIcons.MagnifyingGlass, buttonStyle))
            {
                SetFramePosition(ValueEntry.SmartValue.position);
            }

            GUILayout.EndHorizontal();
        }

        private void DrawVisual(Pose pose)
        {
            var _size = HandleUtility.GetHandleSize(pose.position) * 0.8f;

            Handles.color = Color.green;
            Handles.ArrowHandleCap(
                0,
                pose.position,
                pose.rotation,
                _size,
                EventType.Repaint
            );

            var _cam = SceneView.lastActiveSceneView.camera;
            if (_cam == null) return;

            var _offset = -_cam.transform.up * HandleUtility.GetHandleSize(pose.position) * 0.2f;

            Handles.Label(
                pose.position + _offset,
                label,
                buttonStyle
            );
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

            var _pose = ValueEntry.SmartValue;

            _pose.position = SceneView.lastActiveSceneView.camera.transform.position;
            _pose.rotation = SceneView.lastActiveSceneView.camera.transform.rotation;

            ValueEntry.SmartValue = _pose;

            current = _pose;

            SceneView.RepaintAll();

            ValueEntry.ApplyChanges();
        }

        private void SetFramePosition(Vector3 position)
        {
            SceneView.lastActiveSceneView?.Frame(
                new Bounds(position, Vector3.one * 10),
                false
            );
        }

        ~PickFromSceneAttributeDrawerPose()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
        }
    }
}

#endif