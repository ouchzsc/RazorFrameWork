local Timer = require("time.Timer")
local Time = CS.UnityEngine.Time

local time = {}

function time.init()
    time.globalTimer = Timer:new()
    time.now = Time.time
end

function time.onUpdate(dt)
    time.now = time.now + dt
    time.globalTimer:update(dt)
end

return time