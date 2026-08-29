#if GOOGLE_MOBILE

using System;
using System.Collections;
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
            var request = new AdRequest();

            var setting = GAME.GetSO<ASetting>().googlePlay;
            RewardedAd.Load(setting.rewardedID, request, (ad, error) =>
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
            if (!CanShow())
            {
                onFailed?.Invoke();
                Request();
                return;
            }

            var isRewarded = false;

            data.Show(reward =>
            {
                isRewarded = true;
            });

            data.OnAdFullScreenContentClosed += OnClosed;
            return;

            void OnClosed()
            {
                data.OnAdFullScreenContentClosed -= OnClosed;
                GAME.Manager.StartCoroutine(WaitUntilReady());
            }

            IEnumerator WaitUntilReady()
            {
                OBJECT.Show<WaitingScreen>();

                while (!Application.isFocused)
                    yield return null;

                yield return null;
                yield return null;
                yield return new WaitForEndOfFrame();

                if (isRewarded)
                    onComplete?.Invoke();
                else
                    onFailed?.Invoke();

                OBJECT.Remove<WaitingScreen>();
            }
        }
    }
}

#endif