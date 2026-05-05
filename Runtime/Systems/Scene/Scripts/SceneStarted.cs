using UnityEngine;

namespace ACore
{
    public class SceneStarted : LocalBehaviour
    {
        [SerializeField] private AudioClip sceneBGM;
        public void Start()
        {
            sceneBGM.PlayLoop();
        }
    }
}

