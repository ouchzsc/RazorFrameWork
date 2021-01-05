---@class SimpleEvent
local SimpleEvent = {}

function SimpleEvent:new()
    local instance = {}
    setmetatable(instance, self)
    self.__index = self

    instance.handlers = {}
    instance.argSelfs = {}
    instance.emptySlots = {}
    instance.emptySlotsCnt = 0
    instance.lastSlotIndex = 0

    return instance
end

function SimpleEvent:reg(handler, argSelf)
    local uId
    if self.emptySlotsCnt == 0 then
        uId = self.lastSlotIndex + 1
        self.lastSlotIndex = uId
    else
        uId = self.emptySlots[self.emptySlotsCnt]
        self.emptySlots[self.emptySlotsCnt] = nil
        self.emptySlotsCnt = self.emptySlotsCnt - 1
    end
    self.handlers[uId] = handler
    self.argSelfs[uId] = argSelf
    return uId
end

function SimpleEvent:unReg(uId)
    local handler = self.handlers[uId]
    if not handler then
        return
    end
    self.handlers[uId] = nil
    self.argSelfs[uId] = nil
    table.insert(self.emptySlots, uId)
    self.emptySlotsCnt = self.emptySlotsCnt + 1
    if self.tempHandlerList then
        self.tempDelHandlerMap = self.tempDelHandlerMap or {}
        self.tempDelHandlerMap[handler] = true
    end
end

local module = require("module")
local function err(msg, lvl)
    module.loggers.default:error(msg)
end

function SimpleEvent:trigger(...)
    if self.isTriggering then
        return
    end
    self.isTriggering = true

    self.tempHandlerList = self.tempHandlerList or {}
    self.tempArgSelfList = self.tempArgSelfList or {}

    local cnt = 0
    for uId, handler in pairs(self.handlers) do
        cnt = cnt + 1
        self.tempHandlerList[cnt] = handler
        self.tempArgSelfList[cnt] = self.argSelfs[uId]
    end

    for i = 1, cnt do
        local handler = self.tempHandlerList[i]
        local argSelf = self.tempArgSelfList[i]
        self.tempHandlerList[i] = nil
        self.tempArgSelfList[i] = nil
        if handler then
            local tempDelHandlerMap = self.tempDelHandlerMap
            if tempDelHandlerMap and tempDelHandlerMap[handler] then

            else
                if not argSelf then
                    xpcall(handler, err, ...)
                else
                    xpcall(handler, err, argSelf, ...)
                end
            end
        end
    end
    local tempDelHandlerMap = self.tempDelHandlerMap
    if tempDelHandlerMap then
        for k,v in pairs(tempDelHandlerMap) do
            tempDelHandlerMap[k]=nil
        end
    end
    self.isTriggering = nil
end

function SimpleEvent:dump()
    for uId, handler in pairs(self.handlers) do
        local info = debug.getinfo(handler, "nS")
        print(string.format("%s@%s", info.short_src, info.linedefined))
    end
end

return SimpleEvent