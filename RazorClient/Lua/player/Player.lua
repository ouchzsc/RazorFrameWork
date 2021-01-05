local ASyncGameObject = require("obj.ASyncGameObject")
local module = require("module")
local Input = CS.UnityEngine.Input
local KeyCode = CS.UnityEngine.KeyCode
local playerUtils = require("player.playerUtils")
local movePosition = CS.TransformUtils.movePosition

local SimpleEvent = require("event.SimpleEvent")
local Ability = require("ability.Ab_Shoot")

---@class Player:ASyncGameObject
local Player = ASyncGameObject:extends()

function Player:onNew()
    ASyncGameObject.onNew(self)
    self.x = 0
    self.y = 0
    self.vx = 0
    self.vy = 0
    self.evt_attack = SimpleEvent:new()
    self.ability = Ability:new()
    self.ability:setPlayer(self)
end

function Player:onEnable(gameObject)
    self.transform = gameObject.transform
    self.spriteRenderer = gameObject:GetComponent("SpriteRenderer")
    self:reg(module.event.onUpdate, self.onUpdate)
    self:reg(module.event.onMouseButtonDown, self.onMouseButtonDown)
    self:reg(self.evt_attack, self.onAttack)
    self.ability:show()
end

function Player:onDisable()
    self.ability:hide()
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
    self.x, self.y = movePosition(self.transform, self.vx * dt, self.vy * dt, 0)
    if self.vx > 0 then
        self.spriteRenderer.flipX = false
    elseif self.vx < 0 then
        self.spriteRenderer.flipX = true
    end
end

function Player:onMouseButtonDown(mouseId, x, y, z)
    if mouseId ~= 0 then
        return
    end
    self.ability.event_cast:trigger(x, y)
end

function Player:getPlayerInfo()
    return require("player.PlayerInfo")
end

return Player