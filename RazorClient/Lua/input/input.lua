local Input = CS.UnityEngine.Input
local KeyCode = CS.UnityEngine.KeyCode
local module = require("module")
local event = module.event

local input = {}

function input.update()
    if Input.GetKeyDown(KeyCode.F5) then
        event.onKeyDown:trigger(KeyCode.F5)
    end
    if Input.GetKeyDown(KeyCode.F6) then
        event.onKeyDown:trigger(KeyCode.F6)
    end
    if Input.GetKeyDown(KeyCode.F7) then
        event.onKeyDown:trigger(KeyCode.F7)
    end
    if Input.GetKeyDown(KeyCode.F8) then
        event.onKeyDown:trigger(KeyCode.F8)
    end
end

function input.init()
    event.onUpdate:reg(input.update)
end

return input