local eventUtils = {}
local Stream = require("common.Stream")

local function unregisterEvtHandlerEach(unreg)
    unreg()
end

function eventUtils.reg(obj, simpleevt, handler)
    if obj.__evthandlers == nil then
        obj.__evthandlers = Stream:New()
    end

    local unreg = simpleevt:reg(handler, obj)
    local id = obj.__evthandlers:Add(unreg)
    return function()
        local old = obj.__evthandlers:Delete(id)
        if old then
            old()
        end
    end
end

function eventUtils.unRegAllEvent(obj)
    if obj.__evthandlers ~= nil then
        obj.__evthandlers:ForEach(unregisterEvtHandlerEach)
        obj.__evthandlers:Clear()
    end
end

return eventUtils