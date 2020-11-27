local SceneHome = require("scene.Scene"):new()
local module = require("module")

function SceneHome:onEnable()
    print("scene Home enabled")
end

function SceneHome:onKeyDown(key)
end

return SceneHome