---@class Loggers
local loggers = {}
local Logger = require("logger.Logger")

function loggers.init()
    loggers.default = Logger:new():init("default"):setEnable(true):setTracebackEnable(true)
    loggers.scene = Logger:new():init("scene"):setEnable(true):setTracebackEnable(true)
    loggers.temp = Logger:new():init("temp"):setEnable(false)
    return loggers
end

return loggers
