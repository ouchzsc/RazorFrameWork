using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Serialization;

namespace Res
{
    public class BundleDepMgr : MonoBehaviour
    {
        public static BundleDepMgr Instance { get; private set; }

        private AssetBundleManifest manifest;
        private Dictionary<String, String> name2nameWithHash = new Dictionary<string, string>();
        public AssetBundleManifest Manifest
        {
            get
            {
                return manifest;
            }
            set
            {
                manifest = value;
                name2nameWithHash.Clear();
                foreach (string name_Hash in value.GetAllAssetBundles())
                {
                    var name = name_Hash.Split('_')[0];
                    name2nameWithHash.Add(name, name_Hash);
                }
            }
        }

        private readonly Dictionary<int, int> _id2UnloadedCnt = new Dictionary<int, int>();
        private readonly Dictionary<int, AssetBundle> _id2AssetBundle = new Dictionary<int, AssetBundle>();

        private readonly Dictionary<int, DelegateVoidAssetBundle> _id2CallBack =
            new Dictionary<int, DelegateVoidAssetBundle>();

        public String packageName;
        private int _uuId;

        private void Start()
        {
            Instance = this;
        }

        public Action loadBundleAndDependency(string bundleName, DelegateVoidAssetBundle userCallBack)
        {
            var id = _uuId++;
            List<Action> disposes = new List<Action>();
            var bundleNameHash = getNameWithHash(bundleName);
            var allDeps = Manifest.GetAllDependencies(bundleNameHash);
            _id2UnloadedCnt.Add(id, allDeps.Length + 1);
            _id2CallBack.Add(id, userCallBack);
            disposes.Add(BundleMgr.Instance.loadBundleByNameHash(bundleNameHash, ab =>
            {
                _id2AssetBundle.Add(id, ab);
                OnAbLoadDone(id);
            }));
            foreach (var dep in allDeps)
            {
                disposes.Add(BundleMgr.Instance.loadBundleByNameHash(dep, ab => { OnAbLoadDone(id); }));
            }

            return () =>
            {
                foreach (var dispose in disposes)
                {
                    dispose();
                }
                _id2UnloadedCnt.Remove(id);
                _id2AssetBundle.Remove(id);
                _id2CallBack.Remove(id);
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

        public String getNameWithHash(String bundleName)
        {
            return name2nameWithHash[bundleName];
        }
        
        public void dump()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"_id2UnloadedCnt:{_id2UnloadedCnt.Count} _id2AssetBundle:{_id2AssetBundle.Count} _id2CallBack:{_id2CallBack.Count}\n");
            foreach (var p in _id2CallBack)
            {
                sb.Append($"{p.Key}\n");
            }
            Debug.Log(sb);
        }
    }
}