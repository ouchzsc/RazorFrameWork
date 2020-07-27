local SimpleEvent  = require("event.SimpleEvent")
local event = {}

function event.init()
    event.onUpdate = SimpleEvent:new()
    event.onKeyDown = SimpleEvent:new()
end

return event