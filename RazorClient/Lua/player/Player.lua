local ASyncGameObject = require("obj.ASyncGameObject")
local module = require("module")
local Input = CS.UnityEngine.Input
local KeyCode = CS.UnityEngine.KeyCode
local playerUtils = require("player.playerUtils")
local movePosition = CS.TransformUtils.movePosition
local Bullet = require("bullet.Bullet")
local Camera = CS.UnityEngine.Camera
local getWorldPosFromScreen = CS.CameraUtils.getWorldPosFromScreen

---@class Player:ASyncGameObject
local Player = ASyncGameObject:extends()

function Player:onNew()
    ASyncGameObject.onNew(self)
    self.x = 0
    self.y = 0
    self.vx = 0
    self.vy = 0
end

function Player:onEnable(gameObject)
    self.transform = gameObject.transform
    self.spriteRenderer = gameObject:GetComponent("SpriteRenderer")
    self:reg(module.event.onUpdate, self.onUpdate)
    self:reg(module.event.onMouseButtonDown, self.onMouseButtonDown)
end

function Player:onUpdate(dt)
    local inputDirX = 0
    local inputDirY = 0

    if Input.GetKey(KeyCode.W) then
        inputDirY = 1
    end
    if Input.GetKey(KeyCode.A) then
        inputDirX = -1
    end
    if Input.GetKey(KeyCode.S) then
        inputDirY = -1
    end
    if Input.GetKey(KeyCode.D) then
        inputDirX = 1
    end

    local playerInfo = self:getPlayerInfo()
    local VxMax = playerInfo.VxMax
    local VyMax = playerInfo.VyMax
    local v_add = playerInfo.v_add
    local dv_release = playerInfo.dv_release
    local dv_press = playerInfo.dv_press

    self.vx = playerUtils.calc(self.vx, inputDirX, VxMax, v_add * dt, dv_release * dt, dv_press * dt)
    self.vy = playerUtils.calc(self.vy, inputDirY, VyMax, v_add * dt, dv_release * dt, dv_press * dt)
    movePosition(self.transform, self.vx * dt, self.vy * dt, 0)
    if self.vx > 0 then
        self.spriteRenderer.flipX = false
    elseif self.vx < 0 then
        self.spriteRenderer.flipX = true
    end
end

function Player:onMouseButtonDown(mouseId, x, y, z)
    local bullet = module.poolMgr.objPool:getOrCreate(Bullet) ---@type Bullet
    local bx, by, bz = getWorldPosFromScreen(Camera.main, x, y, z)
    bullet:setAssetInfo("Assets/Res/Common/bullet.prefab")
    bullet:setPos(bx, by, 0)
    bullet:show()
end

function Player:getPlayerInfo()
    return require("player.PlayerInfo")
end

return Player