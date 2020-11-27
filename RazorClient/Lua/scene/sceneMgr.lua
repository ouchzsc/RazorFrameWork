local sceneMgr = {}
local SceneUtils = CS.SceneUtils
local module = require("module")
local resUtils = require("res.resUtils")
local lastSceneName
local lastSceneLoader

function sceneMgr.switch(bundleName, sceneName)
    if lastSceneLoader and lastSceneName ~= sceneName then
        lastSceneLoader()
    elseif lastSceneName == sceneName then
        return
    end
    lastSceneName = sceneName
    lastSceneLoader = resUtils.loadBundleAndDependency(bundleName, function(ab)
        SceneUtils.loadScene(sceneName)
    end)
end

function sceneMgr.init()
    SceneUtils.listenSceneLoaded(sceneMgr.onSceneLoaded)
    SceneUtils.listenSceneUnload(sceneMgr.onSceneUnload)
end

function sceneMgr.onSceneLoaded(sceneName)
    module.loggers.scene:info("loaded %s", sceneName)
    module.event.onSceneLoaded:trigger(sceneName)
end

function sceneMgr.onSceneUnloaded(sceneName)
    module.loggers.scene:info("unloaded %s", sceneName)
    module.event.onSceneUnloaded:trigger(sceneName)
end

return sceneMgr