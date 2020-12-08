local SceneHome = require("scene.Scene"):new()
local module = require("module")
local PlayerObj = require("player.Player")
local Monster = require("monster.Monster")

function SceneHome:onEnable()
    print("SceneHome:onEnable()")
    self.player = PlayerObj:new()
    self.player:setAssetInfo("Assets/Res/Common/go_player.prefab")
    self.player:show()

    self.monster = Monster:new()
    self.monster:setAssetInfo("Assets/Res/Monster/monster.prefab")
    self.monster:setPos(-3,0)
    self.monster:show()
end

function SceneHome:onDisable()
    self.player:hide()
    self.player = nil

    self.monster:hide()
    self.monster = nil
end

return SceneHome