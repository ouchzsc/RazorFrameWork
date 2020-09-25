local module = {}

function module.requireModules()
    module.loggers = require("logger.loggers")
    module.event = require("event.event")
    module.input = require("input.input")
    module.testInvoker = require("test.testInvoker")
    module.time = require("time.time")
    module.sceneMgr = require("scene.sceneMgr")
end

function module.initModules()
    module.loggers.init()
    module.event.init()
    module.input.init()
    module.testInvoker.init()
    module.time.init()
    module.sceneMgr.init()
end

return module