---@class Logger
local Logger = require("obj.Abstract.Object"):extends()
local Time = CS.UnityEngine.Time
local Log = CS.UnityEngine.Debug.Log
local LogWarning = CS.UnityEngine.Debug.LogWarning
local LogError = CS.UnityEngine.Debug.LogError
local PlayerPrefs = CS.UnityEngine.PlayerPrefs

local type = type

local function formatText(format, ...)
    if type(format) == "table" then
        return Util:GetTableDump(format, false) -- Logger.dump(format, 10)
    else
    end

    return string.format(format or "", ...)
end

---@param name string
---@param enable boolean
---@param logTraceBack boolean
---@param logTime boolean
---@return Logger
function Logger:init(name)
    self.name = name or "Lua"
    self.enable = self:getEnable(name, "enable")
    self.logTime = self:getEnable(name, "logTime")
    self.logTraceBack = self:getEnable(name, "logTraceBack")
    return self
end

function Logger:getEnable(loggerName, switchName)
    local prefKey = string.format("logger-%s-%s", loggerName, switchName)
    return PlayerPrefs.GetInt(prefKey, 0) > 0
end

function Logger:setEnable(active)
    local key = string.format("logger-%s-enable", self.name)
    self.enable = active
    if active then
        PlayerPrefs.SetInt(key, 1)
    else
        PlayerPrefs.SetInt(key, 0)
    end
    return self
end

function Logger:setTracebackEnable(active)
    local key = string.format("logger-%s-logTraceBack", self.name)
    self.logTraceBack = active
    if active then
        PlayerPrefs.SetInt(key, 1)
    else
        PlayerPrefs.SetInt(key, 0)
    end
    return self
end

---@param formatString string | table
function Logger:info(formatString, ...)
    if self.enable then
        local text = formatText(formatString, ...)
        Log(self:getLogStr(text, self.logTime, self.logTraceBack, 2))
    end
end

function Logger:error(formatString, ...)
    local text = formatText(formatString, ...)
    LogError(self:getLogStr(text, self.logTime, true, 2))
end

function Logger:warning(formatString, ...)
    local text = formatText(formatString, ...)
    LogWarning(self:getLogStr(text, self.logTime, self.logTraceBack, 2))
end

function Logger:printTable(t, desc)
    local tableStr = Util:GetTableDump(t, false)
    if desc then
        tableStr = string.format("%s: %s", desc, tableStr)
    end
    Log(self:getLogStr(tableStr, self.logTime, self.logTraceBack, 2))
end

---@private
function Logger:getLogStr(str, logTime, logTraceBack, level)
    if logTime then
        str = string.format(" [%0.3f] %s", Time.time, str)
    end
    if logTraceBack then
        str = debug.traceback(str, level + 1)
    end
    str = string.format("[%s] %s", self.name, str)
    return str
end

return Logger