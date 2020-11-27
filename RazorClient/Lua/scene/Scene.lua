local utils = require("event.eventUtils")

---@class scene.Scene:Object
local Scene = require("obj.Abstract.Object"):extends()

---@public
function Scene:init()
    self.isEnabled = false
end

---@public
function Scene:reg(simpleevt, handler)
    utils.reg(self, simpleevt, handler)
end

---@public
function Scene:unRegAllEvent()
    utils.unRegAllEvent(self)
end

function Scene:scheduleTimer(fixid, delay, task, ...)
    utils.scheduleTimer(self, fixid, delay, task, ...)
end

---@public
function Scene:scheduleTimerAtFixedRate(fixid, delay, period, task, ...)
    utils.scheduleTimerAtFixedRate(self, fixid, delay, period, task, ...)
end

---@public
function Scene:unScheduleAllTimer()
    utils.unScheduleAllTimer(self)
end

---@public
function Scene:show()
    if not self.isEnabled then
        self.isEnabled = true
        self:onEnable()
    end
    self:onShow()
end

---@public
function Scene:hide()
    if self.isEnabled then
        self.isEnabled = false
        self:onDisable()
        self:unRegAllEvent()
        self:unScheduleAllTimer()
    end
end

---@protected
function Scene:onEnable()

end

---@protected
function Scene:onDisable()

end

---@protected
function Scene:onShow()

end

return Scene