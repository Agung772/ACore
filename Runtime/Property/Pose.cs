using System;
using UnityEngine;

namespace ACore
{
    [Serializable]
    public struct Pose
    {
        [PickFromScene] public Vector3 position;
        public Quaternion rotation;

        public Pose(Vector3 position, Quaternion rotation)
        {
            this.position = position;
            this.rotation = rotation;
        }
        
        public Pose(Vector3 position, Vector3 eulerAngles)
        {
            this.position = position;
            this.rotation = Quaternion.Euler(eulerAngles);
        }
        
        public Pose(Vector3 position)
        {
            this.position = position;
            rotation = default;
        }
        
        public Pose(Transform transform)
        {
            this.position = transform.position;
            this.rotation = transform.rotation;
        }

        public Vector3 eulerAngles
        {
            get => rotation.eulerAngles;
            set => rotation = Quaternion.Euler(value);
        }

        public void ApplyTo(Transform target)
        {
            target.SetPositionAndRotation(position, rotation);
        }
        
        public static implicit operator Pose(Transform t) 
            => new(t.position, t.rotation);
    }
}
