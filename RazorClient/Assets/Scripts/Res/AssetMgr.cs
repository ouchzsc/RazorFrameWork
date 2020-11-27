using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Res
{
    public delegate void Delegate_Void_ObjectAction(Object asset, Action action);

    public class AssetMgr : MonoBehaviour
    {
        private int _uuid;

        private readonly Dictionary<int, Delegate_Void_ObjectAction> _id2Callback =
            new Dictionary<int, Delegate_Void_ObjectAction>();

        private readonly Dictionary<int, Action> _id2ReleaseAction = new Dictionary<int, Action>();
        public static AssetMgr Instance { get; private set; }

        public void Awake()
        {
            Instance = this;
        }

        public Action loadAsset(string bundleName, string assetName, Delegate_Void_ObjectAction callback)
        {
            int id = _uuid++;
            var disposeBundleAndDependency = BundleDepMgr.Instance.loadBundleAndDependency(bundleName,
                ab => { StartCoroutine(LoadAssetCoroutine(id, ab, assetName)); });
            _id2Callback.Add(id, callback);
            _id2ReleaseAction.Add(id, disposeBundleAndDependency);
            return () =>
            {
                disposeBundleAndDependency.Invoke();
                _id2Callback.Remove(id);
                _id2ReleaseAction.Remove(id);
            };
        }

        private IEnumerator LoadAssetCoroutine(int id, AssetBundle ab, string assetName)
        {
            var req = ab.LoadAssetAsync(assetName);
            yield return req;
            var asset = req.asset;
            var cb = _id2Callback[id];
            var releaseAction = _id2ReleaseAction[id];
            //_id2Callback.Remove(id);
            //_id2ReleaseAction.Remove(id);
            cb?.Invoke(asset, releaseAction);
        }

        public void dump()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"total Cnt:{_id2Callback.Count}\n");
            foreach (var p in _id2Callback)
            {
                sb.Append($"{p.Key}\n");
            }

            Debug.Log(sb);
        }
    }
}