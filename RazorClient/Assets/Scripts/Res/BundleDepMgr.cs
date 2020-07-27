using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Res
{
    public class BundleDepMgr : MonoBehaviour
    {
        public static BundleDepMgr Instance { get; private set; }
        private AssetBundleManifest _manifest;
        private readonly Dictionary<int, int> _id2UnloadedCnt = new Dictionary<int, int>();
        private readonly Dictionary<int, AssetBundle> _id2AssetBundle = new Dictionary<int, AssetBundle>();

        private readonly Dictionary<int, DelegateVoidAssetBundle> _id2CallBack =
            new Dictionary<int, DelegateVoidAssetBundle>();

        public String packageName;
        private int _uuId;

        private void Awake()
        {
            Instance = this;
            BundleMgr.Instance.loadBundle(packageName,
                ab => { _manifest = ab.LoadAsset<AssetBundleManifest>("AssetBundleManifest"); });
        }

        public Action loadBundleAndDependency(string bundleName, DelegateVoidAssetBundle userCallBack)
        {
            var id = _uuId++;
            List<Action> disposes = new List<Action>();
            var allDeps = _manifest.GetAllDependencies(bundleName);
            _id2UnloadedCnt.Add(id, allDeps.Length + 1);
            _id2CallBack.Add(id, userCallBack);
            disposes.Add(BundleMgr.Instance.loadBundle(bundleName, ab =>
            {
                _id2AssetBundle.Add(id, ab);
                OnAbLoadDone(id);
            }));
            foreach (var dep in allDeps)
            {
                disposes.Add(BundleMgr.Instance.loadBundle(dep, ab => { OnAbLoadDone(id); }));
            }

            return () =>
            {
                foreach (var dispose in disposes)
                {
                    dispose();
                }
            };
        }

        private void OnAbLoadDone(int id)
        {
            _id2UnloadedCnt[id]--;
            if (_id2UnloadedCnt[id] == 0)
            {
                _id2UnloadedCnt.Remove(id);
                var ab = _id2AssetBundle[id];
                _id2AssetBundle.Remove(id);
                var cb = _id2CallBack[id];
                _id2CallBack.Remove(id);
                cb?.Invoke(ab);
            }
        }
    }
}