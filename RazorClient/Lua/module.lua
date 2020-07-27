local module = {}

function module.requireModules()
    loggers = require("logger.loggers")
    event = require("event.event")
    module.input = require("input.input")
    module.testInvoker = require("test.testInvoker")
    module.time = require("time.time")

end

function module.initModules()
    event.init()
    loggers.init()
    module.input.init()
    module.testInvoker.init()
    module.time.init()
end

return module