local SceneHome = require("scene.Scene"):new()
local module = require("module")
local PlayerObj = require("player.PlayerObj")

function SceneHome:onEnable()
    self.player = PlayerObj:new()
    self.player:setAssetInfo("Assets/Res/TestGO.prefab")
    self.player:show()
end

function SceneHome:onDisable()
    self.player:hide()
    self.player = nil
end

return SceneHome