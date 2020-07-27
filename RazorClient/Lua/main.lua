local Time = CS.UnityEngine.Time
local main = {}

function main.onStart()
    module = require("module")
    module.requireModules()
    module.initModules()
end

function main.onUpdate()
    local dt = Time.deltaTime
    event.onUpdate:trigger(dt)
    module.time.onUpdate(dt)
end

return main