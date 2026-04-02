#if GOOGLE_MOBILE

using System;
using GoogleMobileAds.Api;
using UnityEngine;

namespace ACore.Google
{
    public class AdRewarded : AdBase
    {
        private RewardedAd data;
        public event Action OnReady;

        public override void Initialize()
        {
            Request();
        }
        
        public override bool CanShow()
        {
            return data != null && data.CanShowAd();
        }

        private void Request()
        {
            var _request = new AdRequest();

            var _setting = Game.GetSO<ASettingData>().googlePlay;
            RewardedAd.Load(_setting.rewardedID, _request, (RewardedAd ad, LoadAdError error) =>
            {
                if (error != null || ad == null)
                {
                    Debug.LogError("RewardedAd failed to load");
                    return;
                }
                
                data = ad;
                data.OnAdFullScreenContentClosed += Request;
                OnReady?.Invoke();
            });
        }
        
        public void Show(Action onComplete, Action onFailed)
        {
            if (CanShow())
            {
                var _isRewarded = false;

                data.Show((Reward reward) =>
                {
                    _isRewarded = true;
                });

                data.OnAdFullScreenContentClosed += () =>
                {
                    if (_isRewarded)
                    {
                        onComplete?.Invoke();
                    }
                    else
                    {
                        Popup.Show<LoadingScreenPopup>();
                        LeanTween.delayedCall(0.5f, () =>
                        {
                            if (_isRewarded) onComplete?.Invoke();
                            else onFailed?.Invoke();
                            Popup.Remove<LoadingScreenPopup>();
                        }).setIgnoreTimeScale(true);
                    }
                };
            }
            else
            {
                onFailed?.Invoke();
                Request();
            }
        }
    }
}

#endif