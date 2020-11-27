local scenes = {}
local module = require("module")
local name2SceneObj = {}

local function onSceneLoaded(sceneName)
    local sceneObj = name2SceneObj[sceneName]
    if sceneObj then
        sceneObj:show()
    end
end

local function onSceneUnloaded(sceneName)
    local sceneObj = name2SceneObj[sceneName]
    if sceneObj then
        sceneObj:hide()
    end
end

local function createScene(cls, sceneName)
    local sceneObj = cls:new()
    name2SceneObj[sceneName] = sceneObj
end

function scenes.init()
    module.event.onSceneLoaded:reg(onSceneLoaded)
    module.event.onSceneUnloaded:reg(onSceneUnloaded)

    scenes.lv1 = createScene(require("scene.SceneHome"), "s1")
end

return scenes