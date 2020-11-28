local ASyncGameObject = require("obj.ASyncGameObject")
local module = require("module")
local pos = { x = 0, y = 0, z = 0 }

---@class Bullet:ASyncGameObject
local Bullet = ASyncGameObject:extends()

function Bullet:setPos(x, y, z)
    pos.x = x
    pos.y = y
    pos.z = z
end

function Bullet:onEnable(gameObject)
    ASyncGameObject.onEnable(self, gameObject)
    gameObject.transform.position = pos
    self:scheduleTimer("hideMe", "2", self.hide)
end

function Bullet:onDisable()
    ASyncGameObject.onDisable(self)
    module.poolMgr.objPool:put(Bullet, self)
end

return Bullet