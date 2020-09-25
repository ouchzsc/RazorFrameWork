local Lv3 = require("scene.Scene"):new()
local module = require("module")

function Lv3:onEnable()
    self:reg(module.event.onKeyDown, self.onKeyDown)
end

function Lv3:onKeyDown(key)
    print(key)
end

return Lv3