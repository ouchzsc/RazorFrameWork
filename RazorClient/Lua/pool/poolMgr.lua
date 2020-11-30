local GoPool = require("pool.GoPool")
local ObjPool = require("pool.ObjPool")
local TablePool = require("pool.TablePool")
local poolMgr = {}

function poolMgr.init0()
    poolMgr.defaultGoPool = GoPool:new()
    poolMgr.defaultGoPool:setSize(50)

    poolMgr.objPool = ObjPool:new()
    poolMgr.objPool:setSize(50)

    poolMgr.timerPool = TablePool:new()
    poolMgr.timerPool:setSize(50)
end

return poolMgr