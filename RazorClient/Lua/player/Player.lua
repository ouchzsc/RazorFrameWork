local ASyncGameObject = require("obj.ASyncGameObject")
local module = require("module")
local Input = CS.UnityEngine.Input
local KeyCode = CS.UnityEngine.KeyCode

local playerUtils = require("player.playerUtils")
local pos = {x=1,y=1,z=0}
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
    --print("Player:onEnable(gameObject)")
    self.transform = gameObject.transform
    self:reg(module.event.onUpdate, self.onUpdate)
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

    local pos = self.transform.position
    pos.x = pos.x + self.vx * dt
    pos.y = pos.y + self.vy * dt
    self.transform.position = pos
    --module.loggers.default:info("%s,%s,%s,%s,%s", inputDirX, inputDirY, self.vx, self.vy, pos)
end

function Player:getPlayerInfo()
    return require("player.PlayerInfo")
end

return Player