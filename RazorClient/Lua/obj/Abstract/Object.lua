---@class Object
local Object = {}

function Object:new(o)
    o = o or {}
    setmetatable(o, self)
    self.__index = self
    if o.onNew then
        o:onNew()
    end
    return o
end

function Object:extends()
    local o = {}
    setmetatable(o, self)
    self.__index = self
    return o
end

return Object