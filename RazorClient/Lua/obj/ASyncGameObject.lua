---@class ASyncGameObject:ASyncObject
local ASyncGameObject = require("obj.Abstract.ASyncObject"):new()
local module = require("module")
local pool = module.poolMgr.defaultGoPool

function ASyncGameObject:setAssetInfo(assetPath)
    self.assetPath = assetPath
end

function ASyncGameObject:loadRes(callBack, param)
    self:getPool():loadGo(self.assetPath, callBack, param)
end

function ASyncGameObject:unloadRes(go)
    self:getPool():put(self.assetPath, go)
end

function ASyncGameObject:getPool()
    return pool
end

function ASyncGameObject:onASyncObjectEnable(res)
    self.__go = res
    res:SetActive(true)
    if self.onEnable then
        self:onEnable(res)
    end
end

function ASyncGameObject:onASyncObjectDisable()
    if self.onDisable then
        self:onDisable()
    end
    self.__go:SetActive(false)
    self.__go = nil
end

return ASyncGameObject