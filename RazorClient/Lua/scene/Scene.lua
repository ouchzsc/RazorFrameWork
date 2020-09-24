local ASyncObject = require("common.ASyncObject")
local bundleDepMgr = CS.Res.BundleDepMgr.Instance

---@class scene.Scene:common.ASyncObject
local Scene = ASyncObject:new()

function Scene:init(bundleName, sceneName)
    self.bundleName = bundleName
    self.sceneName = sceneName
    self.loader = nil
end

function Scene:loadRes(callBack)
    self.loader = bundleDepMgr:loadBundleAndDependency(self.bundleName, function(ab)
        CS.UnityEngine.SceneManagement.SceneManager.LoadScene(self.sceneName)
        callBack(ab)
    end)
end

function Scene:unloadRes(res)
    self.loader()
    self.loader = nil
end

function Scene:onASyncObjectEnable(res)
    if self.onEnable then
        self:onEnable()
    end
end

function Scene:onASyncObjectDisable()
    if self.onDisable then
        self:onDisable()
    end
end

return Scene