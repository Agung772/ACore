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
    public class PickFromSceneAttributeDrawerPose : OdinValueDrawer<Pose>
    {
        private string label;
        private Pose current;
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

            var pose = ValueEntry.SmartValue;

            EditorGUI.BeginChangeCheck();

            var position = Handles.PositionHandle(pose.position, pose.rotation);
            var rotation = Handles.RotationHandle(pose.rotation, position);

            if (EditorGUI.EndChangeCheck())
            {
                isDragging = true;
                lastDrawnTime = EditorApplication.timeSinceStartup;

                pose.position = position;
                pose.rotation = rotation;
                ValueEntry.SmartValue = pose;
                current = pose;

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

            DrawVisual(pose);
        }

        protected override void DrawPropertyLayout(GUIContent content)
        {
            lastDrawnTime = EditorApplication.timeSinceStartup;

            var pose = ValueEntry.SmartValue;

            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();

            pose.position = EditorGUILayout.Vector3Field("Position", pose.position);
            pose.eulerAngles = EditorGUILayout.Vector3Field("Rotation", pose.eulerAngles);

            GUILayout.EndVertical();
            GUILayout.BeginVertical(GUILayout.Width(24));
            GUILayout.Space(2);

            if (SirenixEditorGUI.IconButton(EditorIcons.Flag, buttonStyle))
            {
                SetPositionToCurrentSceneViewFrame();
            }

            if (SirenixEditorGUI.IconButton(EditorIcons.MagnifyingGlass, buttonStyle))
            {
                SetFramePosition(pose.position);
            }

            if (SirenixEditorGUI.IconButton(showHandles ? EditorIcons.Checkmark : EditorIcons.X, buttonStyle))
            {
                showHandles = !showHandles;
                isDragging = false;
                lastDrawnTime = EditorApplication.timeSinceStartup;
                SceneView.RepaintAll();
            }

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            if (current.position != pose.position || current.rotation != pose.rotation)
            {
                ValueEntry.SmartValue = pose;
                current = pose;
                SceneView.RepaintAll();
                ValueEntry.ApplyChanges();
            }
        }

        private void DrawVisual(Pose pose)
        {
            var size = HandleUtility.GetHandleSize(pose.position) * 0.8f;
            Handles.color = Color.green;
            Handles.ArrowHandleCap(0, pose.position, pose.rotation, size, EventType.Repaint);

            var cam = SceneView.lastActiveSceneView != null ? SceneView.lastActiveSceneView.camera : null;
            if (cam == null) return;

            var offset = -cam.transform.up * HandleUtility.GetHandleSize(pose.position) * 0.2f;
            Handles.Label(pose.position + offset, label, buttonStyle);
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

            var pose = ValueEntry.SmartValue;
            pose.position = SceneView.lastActiveSceneView.camera.transform.position;
            pose.rotation = SceneView.lastActiveSceneView.camera.transform.rotation;
            ValueEntry.SmartValue = pose;
            current = pose;
            lastDrawnTime = EditorApplication.timeSinceStartup;
            SceneView.RepaintAll();
            ValueEntry.ApplyChanges();
        }

        private void SetFramePosition(Vector3 position)
        {
            if (SceneView.lastActiveSceneView == null) return;
            SceneView.lastActiveSceneView.Frame(new Bounds(position, Vector3.one * 10), false);
        }

        ~PickFromSceneAttributeDrawerPose()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
        }
    }
}

#endif
