local assetMgr = CS.Res.AssetMgr.Instance
local bundleMgr = CS.Res.BundleMgr.Instance
local bundleDepMgr = CS.Res.BundleDepMgr.Instance

local test = {}

function test.f5()
    --assetMgr:loadAsset("capsule", "Capsule", function(asset)
    --    CS.UnityEngine.GameObject.Instantiate(asset)
    --end)
    loader = bundleDepMgr:loadBundleAndDependency("s1", function(ab)
        CS.UnityEngine.SceneManagement.SceneManager.LoadScene("s1")
    end)
    --bundleMgr:loadBundle("s1",function (ab)
    --    --CS.UnityEngine.SceneManager.LoadScene("s1")
    --end)

end

function test.f6()
    loader()
end

function test.f7()

    local asset = require("cfgGen.asset.asset")
    print(asset.DOG.eName)

    --loader = bundleMgr:loadBundle("s1", function(ab)
    --    CS.UnityEngine.SceneManagement.SceneManager.LoadScene("s1")
    --    loader()
    --end)
end

function test.f8()
    bundleMgr:dump()
end

return test