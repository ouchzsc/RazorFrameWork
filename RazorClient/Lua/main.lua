local Time = CS.UnityEngine.Time
local module = require("module")

local main = {}

function main.onStart()
    module.init()

    local sceneCfg = require("cfgGen.asset.asset").get("Assets/Res/Scene1/s1.unity")
    module.sceneMgr.switch(sceneCfg.bundleName, sceneCfg.assetName)
    module.ui.panelTip:show()
end

function main.onUpdate()
    local dt = Time.deltaTime
    module.event.onUpdate:trigger(dt)
    module.timerMgr.onUpdate(dt)
end

return main