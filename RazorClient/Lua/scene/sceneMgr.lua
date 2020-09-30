local sceneMgr = {}
local SceneUtils = CS.SceneUtils
local module = require("module")
local resUtils = require("res.resUtils")

local scenesLoaded = {}
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
    if sceneName == "s1" then
        local lv3 = require("scene.Lv3"):new()
        lv3:show()
        scenesLoaded[sceneName] = lv3
    end
end

function sceneMgr.onSceneUnloaded(sceneName)
    module.loggers.scene:info("unloaded %s", sceneName)
    module.event.onSceneUnloaded:trigger(sceneName)
    if sceneName == "s1" then
        scenesLoaded[sceneName]:hide()
        scenesLoaded[sceneName] = nil
    end
end

function sceneMgr.getScenesLoaded()
    return scenesLoaded
end

return sceneMgr