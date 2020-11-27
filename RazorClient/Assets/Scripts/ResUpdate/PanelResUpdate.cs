using System;
using System.Collections;
using System.IO;
using Res;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;
using UnityEngine.UI;
using XLua.Cast;

namespace ResUpdate
{
    public class PanelResUpdate : MonoBehaviour
    {
        public InputField inputField_ip;
        public InputField inputField_port;

        public GameObject go_uri;
        public GameObject go_progress;
        public Image image_progress;
        public Button button_EnterGame;

        public Button button;
        public Text text_loading;

        public float barSpeed = 4f;

        private enum LoadState
        {
            Loading,
            Unload,
            Loaded,
        }

        private LoadState _loadState;
        private float bundleProgress;
        private string bundleLoaing;
        private float barProgress;

        private void Start()
        {
            button.onClick.AddListener(OnClick);
            button_EnterGame.onClick.AddListener(OnBtnEnterGame);
            inputField_ip.text = ResUpdate.Instance.ipAddress;
            inputField_port.text = ResUpdate.Instance.port;
        }

        private void OnBtnEnterGame()
        {
            Destroy(gameObject);
            AssetMgr.Instance.loadAsset("boot", "LuaBoot", (asset,free) =>
            {
                GameObject.Instantiate(asset as GameObject);
//                free.Invoke();
            });
        }

        private void OnEnable()
        {
            _loadState = LoadState.Unload;
            ResUpdate.Instance.onBundleUpdate += OnBundleUpdateDone;
            ResUpdate.Instance.onDownloadError += onDownloadError;
            OnShow();
        }

        private void onDownloadError()
        {
            _loadState = LoadState.Unload;
            OnShow();
        }

        private void OnBundleUpdateDone(string bundle, float progress)
        {
            bundleLoaing = bundle;
            bundleProgress = progress;
            OnShow();
        }


        private void OnClick()
        {
            ResUpdate.Instance.ipAddress = inputField_ip.text;
            ResUpdate.Instance.port = inputField_port.text;
            ResUpdate.Instance.StartUpdate();
            _loadState = LoadState.Loading;
            OnShow();
        }

        private void Update()
        {
            var resProgress = bundleProgress;
            if (barProgress < resProgress)
                barProgress += barSpeed * Time.deltaTime;
            OnShowProgress();
            if (barProgress >= 1)
            {
                _loadState = LoadState.Loaded;
                OnShow();
                //StartCoroutine(DelayHideLoading());
            }
        }

        private void OnShow()
        {
            go_progress.SetActive(_loadState == LoadState.Loading);
            go_uri.SetActive(_loadState == LoadState.Unload);
            button_EnterGame.gameObject.SetActive(_loadState == LoadState.Loaded);
            text_loading.text = bundleLoaing;
            OnShowProgress();
        }

        private void OnShowProgress()
        {
            image_progress.fillAmount = barProgress;
        }

        private IEnumerator DelayHideLoading()
        {
            yield return new WaitForSeconds(0.5f);
            _loadState = LoadState.Loaded;
            OnShow();
        }
    }
}