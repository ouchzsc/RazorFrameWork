using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Res
{
    public delegate void DelegateVoidObject(Object asset);

    public class AssetMgr : MonoBehaviour
    {
        private int _uuid;
        private readonly Dictionary<int, DelegateVoidObject> _id2Callback = new Dictionary<int, DelegateVoidObject>();
        public static AssetMgr Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        public Action loadAsset(string bundleName, string assetName, DelegateVoidObject callback)
        {
            int id = _uuid++;
            _id2Callback.Add(id, callback);
            var disposeBundleAndDependency = BundleDepMgr.Instance.loadBundleAndDependency(bundleName,
                ab => { StartCoroutine(LoadAssetCoroutine(id, ab, assetName)); });
            return disposeBundleAndDependency;
        }

        private IEnumerator LoadAssetCoroutine(int id, AssetBundle ab, string assetName)
        {
            var req = ab.LoadAssetAsync(assetName);
            yield return req;
            var asset = req.asset;
            var cb = _id2Callback[id];
            _id2Callback.Remove(id);
            cb?.Invoke(asset);
        }
    }
}