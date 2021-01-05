local SimpleEvent = require("event.SimpleEvent")
local module = require("module")
---@class Ability:LObject
local Ability = require("obj.LObject"):extends()

function Ability:onNew()
    self.event_cast = SimpleEvent:new()
end

function Ability:onEnable()
    self:reg(self.event_cast, self.onCast)
end

function Ability:onCast(x, y)
    if module.timerMgr.now - self:getLastCastTime() < self:onGetAbilityCoolDown() then
        module.event.onSendTip:trigger("技能CD中")
        return
    end
    self:scheduleTimer("castPoint", self:onGetAbilityCastPoint(), function(self)
        self.lastCastTime = module.timerMgr.now
        if self.onSpellStart then
            self:onSpellStart(x, y)
        end
    end)
end

--应该小于CD时间
function Ability:onGetAbilityCastPoint()
    return 0.3
end

function Ability:onGetAbilityCoolDown()
    return 1
end

function Ability:getLastCastTime()
    return self.lastCastTime or 0
end

return Ability