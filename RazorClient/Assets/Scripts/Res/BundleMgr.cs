using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using XLua;

namespace Res
{
    public delegate void DelegateVoidAssetBundle(AssetBundle ab);

    public class BundleMgr : MonoBehaviour
    {
        private int _uuId;

        private readonly HashSet<int> _uuIdSet = new HashSet<int>();
        readonly Dictionary<string, AssetBundle> _name2AssetBundle = new Dictionary<string, AssetBundle>();

        readonly Dictionary<string, DelegateVoidAssetBundle> _name2OnLoadDone =
            new Dictionary<string, DelegateVoidAssetBundle>();

        readonly HashSet<string> _loadingBundles = new HashSet<string>();
        private readonly Dictionary<string, int> _name2RefCnt = new Dictionary<string, int>();

        public static BundleMgr Instance { get; private set; }

        public bool printLog = false;

        public void Awake()
        {
            Instance = this;
        }

        IEnumerator LoadBundleCoroutine(string bundleName)
        {
            string bundlePath = Path.Combine(Application.temporaryCachePath, "StandaloneWindows", bundleName);
            var request = AssetBundle.LoadFromFileAsync(bundlePath);
            yield return request;
            if (_name2OnLoadDone.TryGetValue(bundleName, out var onLoadDone))
            {
                _name2AssetBundle.Add(bundleName, request.assetBundle);
                log($"bundle load done: {bundleName} , refCnt: {_name2RefCnt[bundleName]}, invoke callback");
                onLoadDone.Invoke(request.assetBundle);
            }
            else
            {
                log($"bundle load done: {bundleName} , refCnt: 0, unload bundle");
                request.assetBundle.Unload(true);
            }

            _loadingBundles.Remove(bundleName);
        }

        public Action loadBundleByPureName(string pureName, DelegateVoidAssetBundle userCallBack)
        {
            var name_hash = BundleDepMgr.Instance.getNameWithHash(pureName);
            return loadBundleByNameHash(name_hash, userCallBack);
        }

        public Action loadBundleByNameHash(string bundleName, DelegateVoidAssetBundle userCallBack)
        {
            int id = ++_uuId;
            _uuIdSet.Add(id);

            //引用计数+1
            if (_name2RefCnt.ContainsKey(bundleName))
            {
                _name2RefCnt[bundleName]++;
            }
            else
            {
                _name2RefCnt.Add(bundleName, 1);
            }

            if (_name2AssetBundle.TryGetValue(bundleName, out var ab))
            {
                log(
                    $"loadBundle: {bundleName}, refCnt:{_name2RefCnt[bundleName]}, bundle is loaded, return to user");
                userCallBack?.Invoke(ab);
            }
            else
            {
                if (_loadingBundles.Contains(bundleName))
                {
                    log(
                        $"loadBundle: {bundleName}, refCnt:{_name2RefCnt[bundleName]}, bundle is loading already");
                }
                else
                {
                    log($"loadBundle: {bundleName}, refCnt:{_name2RefCnt[bundleName]}, start coroutine");
                    _name2OnLoadDone.Add(bundleName, null);
                    _loadingBundles.Add(bundleName);
                    StartCoroutine(LoadBundleCoroutine(bundleName));
                }

                //设置监听
                if (!_name2OnLoadDone.ContainsKey(bundleName))
                    _name2OnLoadDone.Add(bundleName, null);
                _name2OnLoadDone[bundleName] += userCallBack;
            }

            //返回dispose方法. 如果需要在userCallBack里释放，新加一个loadBundle方法，userCallBack的时候把dispose作为参数
            return () =>
            {
                if (!_uuIdSet.Contains(id))
                {
                    Debug.LogWarning($"{bundleName} has been disposed already");
                    return;
                }

                _uuIdSet.Remove(id);
                _name2RefCnt[bundleName]--;
                _name2OnLoadDone[bundleName] -= userCallBack;
                if (_name2RefCnt[bundleName] == 0)
                {
                    if (_name2AssetBundle.ContainsKey(bundleName))
                    {
                        log($"release bundle: {bundleName}, refCnt:0, Unload bundle");
                        _name2AssetBundle[bundleName].Unload(true);
                        _name2AssetBundle.Remove(bundleName);

                        _name2RefCnt.Remove(bundleName);

                        _name2OnLoadDone[bundleName] = null;
                        _name2OnLoadDone.Remove(bundleName);
                    }
                    else
                    {
                        log($"release bundle: {bundleName}, refCnt:0，but it is still loading");

                        _name2OnLoadDone[bundleName] = null;
                        _name2OnLoadDone.Remove(bundleName);
                        _name2RefCnt.Remove(bundleName);
                    }
                }
                else
                {
                    log($"release bundle: {bundleName}, refCnt:{_name2RefCnt[bundleName]}");
                }
            };
        }

        public void dump()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"total Cnt:{_name2RefCnt.Count}\n");
            foreach (var p in _name2RefCnt)
            {
                sb.Append($"{p.Key}-{p.Value}\n");
            }

            Debug.Log(sb);
        }

        public void log(object message, UnityEngine.Object context = null)
        {
            if (printLog)
            {
                if (context == null)
                    Debug.Log(message);
                else
                    Debug.Log(message, context);
            }
        }
    }
}