using System;
using System.Collections;
using System.IO;
using Res;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace ResUpdate
{
    public class PanelResUpdate : MonoBehaviour
    {
        public string ipAddress = "127.0.0.1";
        public string port = "8080";

        public InputField inputField_ip;
        public InputField inputField_port;

        public GameObject go_uri;
        public GameObject go_progress;
        public Image image_progress;

        public Button button;

        void Start()
        {
            button.onClick.AddListener(OnClick);
            inputField_ip.text = ipAddress;
            inputField_port.text = port;
        }

        private void OnClick()
        {
            ipAddress = inputField_ip.text;
            port = inputField_port.text;
            StartCoroutine(DownLoadCoroutine("StandaloneWindows", "StandaloneWindows",
                () =>
                {
                    BundleDepMgr.Instance.Init(() =>
                    {
                        var allBundles = BundleDepMgr.Instance.Manifest.GetAllAssetBundles();
                        foreach (var bundle in allBundles)
                        {
                            Debug.Log(bundle);
                        }
                    });
                }));
        }

        private IEnumerator DownLoadCoroutine(string folder, string filename, Action done)
        {
            var uri = $"http://{ipAddress}:{port}/{Path.Combine(folder, filename)}";
            UnityWebRequest webRequest = UnityWebRequest.Get(uri);
            Debug.Log(uri);
            yield return webRequest.SendWebRequest();
            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                Debug.LogError(webRequest.error);
            }
            else
            {
                var savePath = Path.Combine(Application.temporaryCachePath, folder);
                Directory.CreateDirectory(savePath);
                File.WriteAllBytes(Path.Combine(savePath, filename), webRequest.downloadHandler.data);


                //AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(webRequest);
                done();
            }
        }
    }
}