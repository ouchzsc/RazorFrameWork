---@class Loggers
local loggers = {}
local Logger = require("logger.Logger")

function loggers.init()
    loggers.default = Logger:new():init("default"):setEnable(true)
    loggers.scene = Logger:new():init("scene"):setEnable(true)
    return loggers
end

return loggers
