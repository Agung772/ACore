#if GOOGLE_MOBILE

using System.Collections;
using GoogleMobileAds.Api;
using UnityEngine;

namespace ACore.Google
{
    public class AdBanner : AdBase
    {
        private BannerView data;
        public bool IsShow { get; private set; }

        public override void Initialize()
        {
            var _setting = Game.GetSO<ASettingData>().googlePlay;
            if (_setting.noBanner) return;
            
            Request(checkConnection: false);
            Game.Manager.StartCoroutine(Refresh());

            var _sceneLoader = Game.Get<SceneLoader>();
            _sceneLoader.OnLoaded += Show;
            _sceneLoader.OnUnloaded += Hide;
        }

        public override bool CanShow()
        {
            return data != null;
        }

        private IEnumerator Refresh()
        {
            while (true)
            {
                yield return new WaitForSeconds(30f);
                Request();
            }
        }
        
        public void Show()
        {
            if (IsShow) return;
            
            data.Show();
            IsShow = true;
        }
        public void Hide()
        {
            if (!IsShow) return;
            
            data.Hide();
            IsShow = false;
        }
        private async void Request(bool checkConnection = true)
        {
            if (data != null)
            {
                Hide();
                data.Destroy();
            }
            
            if (!checkConnection || await GameNetwork.IsInternetConnection())
            {
                var _setting = Game.GetSO<ASettingData>().googlePlay;
                data = new BannerView(_setting.bannerID, AdSize.Banner, AdPosition.Top);
                data.OnBannerAdLoaded += BannerAdLoaded;
            
                var _request = new AdRequest();
                IsShow = true;
                data.LoadAd(_request);
            }
        }
        
        private void BannerAdLoaded()
        {
            Debug.Log("Banner Ad Loaded");
        }
    }
}

#endif