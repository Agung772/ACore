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
    public class PickFromSceneAttributeDrawerPose : OdinValueDrawer<Pose>
    {
        private static readonly List<PickFromSceneAttributeDrawerPose> ActiveDrawers = new List<PickFromSceneAttributeDrawerPose>();
        private static bool sceneGuiRegistered;

        private string label;
        private Pose current;
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
                try { ValueEntry.ApplyChanges(); }
                catch { UnregisterDrawer(); }
            }
            else if (GUIUtility.hotControl == 0)
            {
                isDragging = false;
            }

            DrawVisual(pose);
        }

        protected override void DrawPropertyLayout(GUIContent content)
        {
            if (!registered) RegisterDrawer();
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
                SetPositionToCurrentSceneViewFrame();

            if (SirenixEditorGUI.IconButton(EditorIcons.MagnifyingGlass, buttonStyle))
                SetFramePosition(pose.position);

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
            UnregisterDrawer();
        }
    }
}

#endif
