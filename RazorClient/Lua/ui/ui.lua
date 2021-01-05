local ui = {}

function ui.init()
    ui.canvas = CS.UnityEngine.GameObject.Find("Canvas")
    ui.panelTip = ui.create(require("tip.PanelTip"), "Assets/Res/Tip/PanelTip.prefab")
end

function ui.create(cls, assetKey)
    local panel = cls:new()
    panel:setAssetInfo(assetKey)
    return panel
end

return ui