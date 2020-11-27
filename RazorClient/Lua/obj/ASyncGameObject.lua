---@class ASyncGameObject:ASyncGameObject
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
    self.go = res
    res:SetActive(true)
    if self.onEnable then
        self:onEnable(res)
    end
end

function ASyncGameObject:onASyncObjectDisable()
    self.go:SetActive(false)
    self.go = nil
end

return ASyncGameObject