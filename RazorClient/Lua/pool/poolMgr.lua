local Pool = require("pool.Pool")
local GoPool = require("pool.GoPool")
local poolMgr = {}

function poolMgr.init()
    poolMgr.defaultGoPool = GoPool:new()
    poolMgr.defaultGoPool:setSize(0)
end

return poolMgr