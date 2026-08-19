using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ACore
{
    public class BootingPopup : UIBehaviour
    {
        [SerializeField] private Slider loadingSlider;
        [SerializeField] private TextMeshProUGUI loadingTx;

        public void Setup(float progress)
        {
            loadingSlider.value = progress;
            loadingTx.text = $"{progress * 100}%";
        }
    }
}
