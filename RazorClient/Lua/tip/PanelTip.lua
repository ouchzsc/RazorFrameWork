local ASyncGameObject = require("obj.ASyncGameObject")
local module = require("module")

---@class PanelTip:ASyncGameObject
local PanelTip = ASyncGameObject:extends()

function PanelTip:onEnable(gameObject)
    gameObject.transform:SetParent(module.ui.canvas.transform,false)
    self.container = gameObject:GetComponent("Container")
    self.Text = self.container.objects[0]
    self.tipText = ""

    self:reg(module.event.onSendTip, self.onSendTip)
end

function PanelTip:onShow()
    self.Text.text = self.tipText
end

function PanelTip:onSendTip(text)
    self.tipText = text
    self:onShow()
    self:scheduleTimer("clear", 0.4, function(self)
        self.tipText = ""
        self:onShow()
    end)
end

function PanelTip:onDisable()

end

return PanelTip