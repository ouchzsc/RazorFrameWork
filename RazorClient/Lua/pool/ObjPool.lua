---@class ObjPool:Object
local ObjPool = require("obj.Object"):extends()

---@protected
function ObjPool:onNew()
    self.size = 1
    self.cnt = 0
    self.keys = {}
    self.objects = {}
end

function ObjPool:setSize(size)
    self.size = size
end

function ObjPool:put(key, obj)
    table.insert(self.keys, key)
    table.insert(self.objects, obj)
    self.cnt = self.cnt + 1
    self:purge()
end

function ObjPool:getOrCreate(key)
    local keys = self.keys
    local objects = self.objects
    for i = self.cnt, 1,-1 do
        if keys[i] == key then
            local obj = objects[i]
            table.remove(keys, i)
            table.remove(objects, i)
            self.cnt = self.cnt - 1
            return obj
        end
    end
    return key:new()
end

function ObjPool:purge(size)
    size = size or self.size
    local num = self.cnt - size
    if num <= 0 then
        return
    end
    for i = 1, num do
        table.remove(self.keys)
        table.remove(self.objects)
    end
    self.cnt = self.cnt - num
end

return ObjPool