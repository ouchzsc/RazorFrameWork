local module = {}

function module.requireModules()
    module.loggers = require("logger.loggers")
    module.event = require("event.event")
    module.input = require("input.input")
    module.testInvoker = require("test.testInvoker")
    module.timerMgr = require("time.timerMgr")
    module.sceneMgr = require("scene.sceneMgr")
    module.poolMgr = require("pool.poolMgr")
end

function module.initModules()
    module.loggers.init()
    module.event.init()
    module.input.init()
    module.testInvoker.init()
    module.timerMgr.init()
    module.sceneMgr.init()
    module.poolMgr.init()
end

return module