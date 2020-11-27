local ASyncGameObject = require("obj.ASyncGameObject")
---@class PlayerObj:ASyncGameObject
local PlayerObj = ASyncGameObject:extends()

function PlayerObj:onEnable()
    print("PlayerObj")
end

function PlayerObj:onDisable()

end

return PlayerObj