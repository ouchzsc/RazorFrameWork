local pairs = pairs
local setmetatable = setmetatable

--------------------------------------------------------
--- lua 的table在foreach 遍历的时候可以删除更改（具体请查看next的说明），但不能增加
--- 这里的结构，可以foreach时候增加

local Stream = {}

function Stream:New()
    local instance = {}
    setmetatable(instance, self)
    self.__index = self
    instance.list = {}
    instance.add = nil
    instance.lastid = 0
    instance.depth = 0
    return instance
end

function Stream:Add(e)
    local id = self.lastid + 1
    self.lastid = id
    if self.depth == 0 then
        self.list[id] = e
    else
        if self.add == nil then
            self.add = {}
        end
        self.add[#self.add + 1] = { id = id, e = e }
    end
    return id
end

function Stream:Delete(id)
    local old = self.list[id]
    if old ~= nil then
        self.list[id] = nil
        return old
    end

    if self.add then
        for i = 1, #self.add do
            local a = self.add[i]
            if id == a.id then
                if a.del then
                    return nil
                else
                    local olde = a.e
                    a.del = true
                    a.e = nil
                    return olde
                end
            end
        end
    end
    return nil
end

function Stream:ForEach(func, ...)
    self.depth = self.depth + 1

    local addlen
    if self.add then
        addlen = #self.add
        if self.depth == 1 and addlen > 0 then
            --- compact
            for i = 1, addlen do
                local a = self.add[i]
                if not a.del then
                    self.list[a.id] = a.e
                end
            end
            self.add = nil
        end
    end

    for id, e in pairs(self.list) do
        func(e, id, ...)
    end

    if self.add and self.depth > 1 and addlen then
        local addlen2 = #self.add
        if addlen > addlen2 then
            addlen = addlen2
        end
        for i = 1, addlen do
            local a = self.add[i]
            if not a.del then
                func(a.e, a.id, ...)
            end
        end
    end

    self.depth = self.depth - 1
end

local cnt
local function Counter()
    cnt = cnt + 1
end

function Stream:Size()
    cnt = 0
    self:ForEach(Counter)
    return cnt
end

function Stream:Clear()
    self.list = {}
    self.add = nil
end

return Stream
