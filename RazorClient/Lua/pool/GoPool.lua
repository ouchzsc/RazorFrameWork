local GoPool = require("obj.Abstract.Object")
local resUtils = require("res.resUtils")
local GameObject = CS.UnityEngine.GameObject

function GoPool:onNew()
    self.size = 1
    self.cnt = 0
    self.assetPaths = {}
    self.gameObjects = {}
    self.go2Free = {}
end

function GoPool:setSize(size)
    self.size = size
end

function GoPool:put(assetPath, go)
    table.insert(self.assetPaths, assetPath)
    table.insert(self.gameObjects, go)
    self.cnt = self.cnt + 1
    self:purge()
end

---@private
function GoPool:get(key)
    local keys = self.assetPaths
    local objects = self.gameObjects
    for i = 1, self.cnt do
        local index = self.cnt - i + 1
        if keys[index] == key then
            local obj = objects[index]
            table.remove(keys, index)
            table.remove(objects, index)
            self.cnt = self.cnt - 1
            return obj
        end
    end
end

function GoPool:purge(size)
    size = size or self.size
    local num = self.cnt - size
    if num <= 0 then
        return
    end
    for i = 1, num do
        table.remove(self.assetPaths)
        local gameObject = table.remove(self.gameObjects)
        local free = self.go2Free[gameObject]
        GameObject.Destroy(gameObject)
        free()
    end
    self.cnt = self.cnt - num
end

function GoPool:loadGo(assetPath, callBack)
    local go = self:get(assetPath)
    if go then
        callBack(go)
    else
        resUtils.loadAssetByPath(assetPath, function(prefab, free)
            local go = GameObject.Instantiate(prefab)
            GameObject.DontDestroyOnLoad(go)
            self.go2Free[go] = free
            callBack(go)
        end)
    end
end

return GoPool