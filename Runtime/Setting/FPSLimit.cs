using UnityEngine;

namespace ACore
{
    public enum FPSLimit
    {
        Auto,

        [InspectorName("30")]
        FPS30,

        [InspectorName("60")]
        FPS60,

        [InspectorName("90")]
        FPS90,

        [InspectorName("120")]
        FPS120
    }

}