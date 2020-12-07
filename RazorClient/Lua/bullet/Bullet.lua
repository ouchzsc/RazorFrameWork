local ASyncGameObject = require("obj.ASyncGameObject")
local module = require("module")
local Vector2 = CS.UnityEngine.Vector2
local VectorUtils = CS.VectorUtils

---@class Bullet:ASyncGameObject
local Bullet = ASyncGameObject:extends()

function Bullet:setTargetPos(x1, y1, x2, y2, speed)
    self.pos = self.pos or {}
    self.pos.x = x1
    self.pos.y = y1
    local dx, dy = VectorUtils.v2Normalized(x2 - x1, y2 - y1)
    self.dirX, self.dirY = dx * speed, dy * speed
end

function Bullet:onEnable(gameObject)
    self.gameObject = gameObject
    self.gameObject.transform.position = self.pos

    self:scheduleTimer("hideMe", "0.5", self.hide)
    self:reg(module.event.onUpdate, self.onUpdate)
    self.startTime = module.timerMgr.now
end

function Bullet:onUpdate(dt)
    self.pos.x = self.pos.x + dt * self.dirX
    self.pos.y = self.pos.y + dt * self.dirY
    self.gameObject.transform.position = self.pos
end

function Bullet:onDisable()
    module.poolMgr.objPool:put(Bullet, self)
end

return Bullet