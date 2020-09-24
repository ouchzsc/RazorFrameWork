local injectUtils = {}
local Stream = require("common.Stream")
local module = require("module")

local function unregisterEvtHandlerEach(unreg)
    unreg()
end

function injectUtils.reg(obj, simpleevt, handler)
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

function injectUtils.unRegAllEvent(obj)
    if obj.__evthandlers ~= nil then
        obj.__evthandlers:ForEach(unregisterEvtHandlerEach)
        obj.__evthandlers:Clear()
    end
end

function injectUtils.unScheduleAllTimer(obj)
    if obj.__timer_fixedids then
        for _, id in pairs(obj.__timer_fixedids) do
            module.time.globalTimer:unschedule(id)
        end
        obj.__timer_fixedids = nil
    end
end

function injectUtils.scheduleTimer(obj, fixid, delay, task, ...)
    local id
    if obj.__timer_fixedids == nil then
        obj.__timer_fixedids = {}
    else
        id = obj.__timer_fixedids[fixid]
    end

    id = module.time.globalTimer:schedule(id, delay, task, obj, ...) --- 这里不用在timer结束的时候清除，是因为这个id是自增的，不考虑回绕
    obj.__timer_fixedids[fixid] = id
end

function injectUtils.scheduleTimerAtFixedRate(obj, fixid, delay, period, task, ...)
    local id
    if obj.__timer_fixedids == nil then
        obj.__timer_fixedids = {}
    else
        id = obj.__timer_fixedids[fixid]
    end

    id = module.time.globalTimer:scheduleAtFixedRate(id, delay, period, task, obj, ...)
    obj.__timer_fixedids[fixid] = id
end

function injectUtils.unScheduleTimer(obj, fixid)
    if obj.__timer_fixedids then
        local id = obj.__timer_fixedids[fixid]
        if id then
            obj.__timer_fixedids[fixid] = nil
            module.time.globalTimer:unschedule(id)
        end
    end
end

return injectUtils