---@class Loggers
local loggers = {}
local Logger = require("logger.Logger")

function loggers.init()
    loggers.default = Logger:new():init("default")
    return loggers
end

return loggers
