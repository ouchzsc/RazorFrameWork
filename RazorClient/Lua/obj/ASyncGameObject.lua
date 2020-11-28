---@class ASyncGameObject:ASyncObject
local ASyncGameObject = require("obj.Abstract.ASyncObject"):new()
local module = require("module")
local pool = module.poolMgr.defaultGoPool

function ASyncGameObject:setAssetInfo(assetPath)
    self.assetPath = assetPath
end

function ASyncGameObject:loadRes(callBack)
    self:getPool():loadGo(self.assetPath, callBack)
end

function ASyncGameObject:unloadRes(go)
    self:getPool():put(self.assetPath, go)
end

function ASyncGameObject:getPool()
    return pool
end

function ASyncGameObject:onASyncObjectEnable(res)
    self:onEnable(res)
end

function ASyncGameObject:onASyncObjectDisable()
    self:onDisable()
end

function ASyncGameObject:onEnable(gameObject)
    gameObject:SetActive(true)
    self.go = gameObject
end

function ASyncGameObject:onDisable()
    self.go:SetActive(false)
    self.go = nil
end

return ASyncGameObject