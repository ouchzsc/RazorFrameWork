local ASyncGameObject = require("obj.ASyncGameObject")
local module = require("module")

---@class Bullet:ASyncGameObject
local Bullet = ASyncGameObject:extends()

function Bullet:setPos(x, y, z)
    self.pos = self.pos or {}
    local pos = self.pos
    pos.x = x
    pos.y = y
    pos.z = z
end

function Bullet:onEnable(gameObject)
    gameObject.transform.position = self.pos
    self:scheduleTimer("hideMe", "2", self.hide)
end

function Bullet:onDisable()
    module.poolMgr.objPool:put(Bullet, self)
end

return Bullet