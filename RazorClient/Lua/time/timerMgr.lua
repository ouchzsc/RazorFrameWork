local Timer = require("time.Timer")
local Time = CS.UnityEngine.Time

local timerMgr = {}

function timerMgr.init()
    timerMgr.globalTimer = Timer:new()
    timerMgr.now = Time.time
end

function timerMgr.onUpdate(dt)
    timerMgr.now = timerMgr.now + dt
    timerMgr.globalTimer:update(dt)
end

return timerMgr