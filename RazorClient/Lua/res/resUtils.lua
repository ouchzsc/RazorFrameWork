local assetMgr = CS.Res.AssetMgr.Instance
local bundleMgr = CS.Res.BundleMgr.Instance
local bundleDepMgr = CS.Res.BundleDepMgr.Instance
local asset = require("cfgGen.asset.asset")
local resUtils = {}

--callback(asset); return loader
function resUtils.loadAsset(bundleName, assetName, callback)
    return assetMgr:loadAsset(bundleName, assetName, callback)
end

--callback(asset); return loader
function resUtils.loadAssetByPath(path, callback)
    local assetInfo = asset.get(path)
    return assetMgr:loadAsset(assetInfo.bundleName, assetInfo.assetName, callback)
end

--callback(assetBundle); return loader
function resUtils.loadBundleAndDependency(bundleName, callback)
    return bundleDepMgr:loadBundleAndDependency(bundleName, callback)
end

function resUtils.dump()
    bundleMgr:dump()
end

return resUtils