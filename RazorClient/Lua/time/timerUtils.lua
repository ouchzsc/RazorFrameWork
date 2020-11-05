local timerUtils = {}
local module = require("module")

function timerUtils.scheduleTimer(obj, fixid, delay, task, ...)
    local id
    if obj.__timer_fixedids == nil then
        obj.__timer_fixedids = {}
    else
        id = obj.__timer_fixedids[fixid]
    end

    id = module.timerMgr.globalTimer:schedule(id, delay, task, obj, ...) --- 这里不用在timer结束的时候清除，是因为这个id是自增的，不考虑回绕
    obj.__timer_fixedids[fixid] = id
end

function timerUtils.scheduleTimerAtFixedRate(obj, fixid, delay, period, task, ...)
    local id
    if obj.__timer_fixedids == nil then
        obj.__timer_fixedids = {}
    else
        id = obj.__timer_fixedids[fixid]
    end

    id = module.timerMgr.globalTimer:scheduleAtFixedRate(id, delay, period, task, obj, ...)
    obj.__timer_fixedids[fixid] = id
end

function timerUtils.unScheduleTimer(obj, fixid)
    if obj.__timer_fixedids then
        local id = obj.__timer_fixedids[fixid]
        if id then
            obj.__timer_fixedids[fixid] = nil
            module.timerMgr.globalTimer:unschedule(id)
        end
    end
end

function timerUtils.unScheduleAllTimer(obj)
    if obj.__timer_fixedids then
        for _, id in pairs(obj.__timer_fixedids) do
            module.timerMgr.globalTimer:unschedule(id)
        end
        obj.__timer_fixedids = nil
    end
end

return timerUtils