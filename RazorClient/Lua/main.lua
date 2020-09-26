local Time = CS.UnityEngine.Time
local module = require("module")

local main = {}

function main.onStart()
    module.requireModules()
    module.initModules()
    module.sceneMgr.switch("s1", "s1")
end

function main.onUpdate()
    local dt = Time.deltaTime
    module.event.onUpdate:trigger(dt)
    module.time.onUpdate(dt)
end

return main