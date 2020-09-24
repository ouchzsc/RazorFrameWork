local Scene1 = require("scene.Scene"):new()
local module = require("module")

function Scene1:onEnable()
    print("scene1 enable")
    self:reg(module.event.onKeyDown, self.onKeyDown)
end

function Scene1:onKeyDown(key)
    print(key)
end

function Scene1:onDisable()
    print("scene1 disable")
end

return Scene1