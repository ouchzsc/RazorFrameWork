local SimpleEvent  = require("event.SimpleEvent")
local event = {}

function event.init0()
    event.onUpdate = SimpleEvent:new()
    event.onKeyDown = SimpleEvent:new()
    event.onMouseButtonDown = SimpleEvent:new()
    event.onSceneLoaded = SimpleEvent:new()
    event.onSceneUnloaded = SimpleEvent:new()
    event.onSendTip = SimpleEvent:new()
end

return event