local Stream = require("common.Stream")

---@class Core.Event.SimpleEvent
local SimpleEvent = {}

function SimpleEvent:new()
    local instance = {}
    setmetatable(instance, SimpleEvent)
    instance.listeners = Stream:New()
    instance.handlerId2Arg1 = nil
    SimpleEvent.__index = SimpleEvent
    return instance
end

function SimpleEvent:reg(handler, argSelf)
    local id = self.listeners:Add(handler)
    if argSelf then
        if not self.handlerId2ArgSelf then
            self.handlerId2ArgSelf = {}
        end
        self.handlerId2ArgSelf[id] = argSelf
    end

    return function()
        self.listeners:Delete(id)
        if self.handlerId2Arg1 then
            self.handlerId2Arg1[id] = nil
        end
    end
end

function SimpleEvent:unreg(id)
    self.listeners:Delete(id)
    if self.handlerId2ArgSelf then
        self.handlerId2ArgSelf[id] = nil
    end
end

local function err(msg, lvl)
    loggers.default:error(msg)
end

local function SafeTriggerEach(handler, id, eventSelf, ...)
    local argSelf
    if eventSelf.handlerId2ArgSelf then
        argSelf = eventSelf.handlerId2ArgSelf[id]
    end

    if argSelf then
        xpcall(handler, err, argSelf, ...)
    else
        xpcall(handler, err, ...)
    end
end

function SimpleEvent:trigger(...)
    self.listeners:ForEach(SafeTriggerEach, self, ...)
end
return SimpleEvent