---@class Pool:Object
local Pool = require("obj.Abstract.Object"):extends()

---@protected
function Pool:onNew()
    self.size = 1
    self.cnt = 0
    self.keys = {}
    self.objects = {}
end

function Pool:setSize(size)
    self.size = size
end

function Pool:put(key, obj)
    table.insert(self.keys, key)
    table.insert(self.objects, obj)
    self.cnt = self.cnt + 1
    self:purge()
end

function Pool:get(key)
    local keys = self.keys
    local objects = self.objects
    for i = self.cnt, 1 do
        if keys[i] == key then
            local obj = objects[i]
            table.remove(keys, i)
            table.remove(objects, i)
            self.cnt = self.cnt - 1
            return obj
        end
    end
end

function Pool:purge(size)
    size = size or self.size
    local num = self.cnt - size
    if num <= 0 then
        return
    end
    for i = 1, num do
        table.remove(self.keys)
        local obj = table.remove(self.objects)
        if obj.onDispose then
            obj:onDispose()
        end
    end
    self.cnt = self.cnt - num
end

return Pool