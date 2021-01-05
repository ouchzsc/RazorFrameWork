local Bullet = require("bullet.Bullet")
local getWorldPosFromScreen = CS.CameraUtils.getWorldPosFromScreen
local module = require("module")

---@class Ab_Shoot:Ability
local Ab_Shoot = require("ability.Ability"):extends()

function Ab_Shoot:setPlayer(player)
    self.player = player
end

function Ab_Shoot:onSpellStart(x, y)
    local bullet = module.poolMgr.objPool:getOrCreate(Bullet) ---@type Bullet
    local mouseX, mouseY = getWorldPosFromScreen(x, y, 0)
    bullet:setAssetInfo("Assets/Res/Common/bullet.prefab")
    bullet:setTargetPos(self.player.x, self.player.y, mouseX, mouseY, self:onGetBulletSpeed())
    bullet:show()
end

function Ab_Shoot:onGetAbilityCastPoint()
    return 0.2
end

function Ab_Shoot:onGetAbilityCoolDown()
    return 0.5
end

function Ab_Shoot:onGetBulletSpeed()
    return 10
end

return Ab_Shoot