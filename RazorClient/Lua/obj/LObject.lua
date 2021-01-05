local eventUtils = require("event.eventUtils")
local timerUtils = require("time.timerUtils")

---@class LObject:Object
local LObject = require("obj.Object"):extends()

---@public
function LObject:init()
    self.isEnabled = false
end

---@public
function LObject:reg(simpleevt, handler)
    eventUtils.reg(self, simpleevt, handler)
end

---@public
function LObject:unRegAllEvent()
    eventUtils.unRegAllEvent(self)
end

function LObject:scheduleTimer(fixid, delay, task, ...)
    timerUtils.scheduleTimer(self, fixid, delay, task, ...)
end

---@public
function LObject:scheduleTimerAtFixedRate(fixid, delay, period, task, ...)
    timerUtils.scheduleTimerAtFixedRate(self, fixid, delay, period, task, ...)
end

---@public
function LObject:unScheduleAllTimer()
    timerUtils.unScheduleAllTimer(self)
end

---@public
function LObject:show()
    if not self.isEnabled then
        self.isEnabled = true
        self:onEnable()
    end
    self:onShow()
end

---@public
function LObject:hide()
    if self.isEnabled then
        self.isEnabled = false
        self:onDisable()
        self:unRegAllEvent()
        self:unScheduleAllTimer()
    end
end

---@protected
function LObject:onEnable()

end

---@protected
function LObject:onDisable()

end

---@protected
function LObject:onShow()

end

return LObject