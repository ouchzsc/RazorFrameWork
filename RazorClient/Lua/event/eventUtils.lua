local eventUtils = {}

function eventUtils.reg(obj, simpleEvt, handler)
    obj.__eventList = obj.__eventList or {}
    obj.__uIdList = obj.__uIdList or {}
    obj.__maxHandlerIndex = obj.__maxHandlerIndex or 0

    local uId = simpleEvt:reg(handler,obj)
    table.insert(obj.__eventList, simpleEvt)
    table.insert(obj.__uIdList, uId)

    obj.__maxHandlerIndex = obj.__maxHandlerIndex + 1


    return obj.__maxHandlerIndex
end

function eventUtils.unReg(obj, handlerIndex)
    local uId = obj.__uIdList[handlerIndex]
    local event = obj.__eventList[handlerIndex]
    obj.__uIdList[handlerIndex] = nil
    obj.__eventList[handlerIndex] = nil
    event:unReg(uId)
end

function eventUtils.unRegAllEvent(obj)
    local maxHandlerId = obj.__maxHandlerIndex
    if maxHandlerId then
        for i = 1, maxHandlerId do
            eventUtils.unReg(obj, i)
        end
    end
    obj.__maxHandlerIndex = nil
end

return eventUtils