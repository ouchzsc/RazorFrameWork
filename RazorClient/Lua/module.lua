local module = {}

function module.init()
    module.loggers = require("logger.loggers")
    module.event = require("event.event")
    module.input = require("input.input")
    module.testInvoker = require("test.testInvoker")
    module.timerMgr = require("time.timerMgr")
    module.sceneMgr = require("scene.sceneMgr")
    module.poolMgr = require("pool.poolMgr")
    module.scenes = require("scene.scenes")

    module.event.init0()
    module.poolMgr.init0()

    for _, m in pairs(module) do
        if type(m) == "table" then
            if m.init then
                m.init()
            end
        end
    end
end

return module