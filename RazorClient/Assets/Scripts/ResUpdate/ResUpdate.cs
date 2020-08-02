using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Res;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;

namespace ResUpdate
{
    public class ResUpdate : MonoBehaviour
    {
        public enum BundlePos
        {
            InStreamingAsset,
            InCache,
        }

        private int _totalBundleCnt;
        private int _bundleConfirmedCnt;

        public string ipAddress = "127.0.0.1";
        public string port = "8080";
        public float Progess { get; private set; }

        public Action<string, float> onBundleUpdate;

        public Dictionary<string, BundlePos> bundleName2Pos = new Dictionary<string, BundlePos>();

        public static ResUpdate Instance { get; private set; }

        public void Awake()
        {
            Instance = this;
            Progess = 0;
        }

        public void StartUpdate()
        {
            StartCoroutine(DownLoadManifest(OnManifestLoadDone));
        }

        private IEnumerator DownLoadManifest(Action<AssetBundleManifest> done)
        {
            var uri = $"http://{ipAddress}:{port}/{Path.Combine("StandaloneWindows", "StandaloneWindows")}";
            var request = UnityWebRequestAssetBundle.GetAssetBundle(uri);
            Debug.Log($"down load manifest {uri}");
            yield return request.SendWebRequest();
            if (request.isNetworkError || request.isHttpError)
                Debug.LogError(request.error);
            else
            {
                var abManifest = DownloadHandlerAssetBundle.GetContent(request);
                var manifest = abManifest.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                done(manifest);
            }
        }

        private void OnManifestLoadDone(AssetBundleManifest ab)
        {
            Debug.Log($"manifest load done: {ab}");
            var allBundles = ab.GetAllAssetBundles();
            _totalBundleCnt = allBundles.Length;
            
            foreach (var bundlePath in allBundles)
            {
                var pathInStreamAs = Path.Combine(Application.streamingAssetsPath, bundlePath);

                if (File.Exists(pathInStreamAs))
                {
                    Debug.Log($"{bundlePath} in streaming Assets");
                    ConfirmBundle(bundlePath, BundlePos.InStreamingAsset);
                    continue;
                }

                var pathInCache = Path.Combine(Application.temporaryCachePath, "StandaloneWindows", bundlePath);
                if (File.Exists(pathInCache))
                {
                    Debug.Log($"{bundlePath} in cache");
                    ConfirmBundle(bundlePath, BundlePos.InCache);
                    continue;
                }

                StartCoroutine(DownloadAndSave(bundlePath));
            }
        }

        private void ConfirmBundle(string bundleName, BundlePos pos)
        {
            _bundleConfirmedCnt++;
            Progess = (float) _bundleConfirmedCnt / _totalBundleCnt;
            bundleName2Pos.Add(bundleName, pos);
            if (_bundleConfirmedCnt == _totalBundleCnt)
            {
                Debug.Log("bundle确认完成");
            }

            onBundleUpdate?.Invoke(bundleName, Progess);
        }

        private IEnumerator DownloadAndSave(string bundlePath)
        {
            var uri = $"http://{ipAddress}:{port}/StandaloneWindows/{bundlePath}";
            Debug.Log($"download:{uri}");
            UnityWebRequest request = UnityWebRequest.Get(uri);
            yield return request.SendWebRequest();
            if (request.isNetworkError || request.isHttpError)
            {
                Debug.LogError(request.error);
            }
            else
            {
                var folder = Path.GetDirectoryName(bundlePath);
                var fileName = Path.GetFileName(bundlePath);
                var fullFolder = Path.Combine(Application.temporaryCachePath, "StandaloneWindows", folder);
                if (!Directory.Exists(fullFolder))
                    Directory.CreateDirectory(fullFolder);
                var fullName = Path.Combine(fullFolder, fileName);
                File.WriteAllBytes(fullName, request.downloadHandler.data);
                Debug.Log($"save file:{fullName}");
                ConfirmBundle(bundlePath, BundlePos.InCache);
            }
        }
    }
}