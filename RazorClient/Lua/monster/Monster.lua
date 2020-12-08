local ASyncGameObject = require("obj.ASyncGameObject")

---@class Monster:ASyncGameObject
local Monster = ASyncGameObject:extends()

function Monster:setPos(x, y)
    self.pos = self.pos or {}
    self.pos.x = x
    self.pos.y = y
end

function Monster:onEnable(gameObject)
    self.gameObject = gameObject
    self.transform = gameObject.transform
end

function Monster:onShow()
    self.transform.position = self.pos
end

return Monster