--接口和回调：
--ASyncObject只提供onEnable和onDisable回调，而没有onAwake 和 onDestroy 回调。
--原因是为了复用被disable的资源
--如果提供了onAwake，必须让用户知道 onAwake和 onEnable中处理的可能是不同的资源
--ASyncObject直接不提供onAwake和onDestroy，带来的缺点是绑定资源的费时操作必须写在onASyncObjectEnable里，它们的调用次数更多了

--how to use：
--继承ASyncObject类进行扩展
--实现loadRes 和 unloadRes, 添加传参接口。
--重载onASyncObjectEnable和onASyncObjectDisable，可以对资源进行预处理，提供名为onEnable和onDisable的回调。
--创建对象, 传入参数，比如bundleName assetName，或其他自定义参数
--调用show hide 方法，可触发 onASyncObjectEnable,onShow,onASyncObjectDisable回调，回调函数中可以访问：传入的参数，Module中的数据，loadRes加载的资源
--onEnable中监听事件触发onShow，事件在onASyncObjectDisable后会统一注销。
--可以调用定时工具，定时工具在onASyncObjectDisable后会统一注销

--资源加载：
--目前这套框架下，资源加载接口分普通资源和FairyGUI资源
--普通资源加载调用AssetManager():Spawn(assetCfg.bundleName,assetCfg.assetName, Asset.EHierarchyGroup.DEFAULT),ListenChainLoadDone
--FairyGUI调用CS.World.Core.Asset.AssetManager.Instance:Instantiate(assetCfg.packageName,assetCfg.uiName,onLoadedCallback)

--嵌套：
--为了框架简单，ASyncObject这层不支持自动嵌套，用户自己创建子对象，调用子对象的show hide,需要确保show hide 的一致性。
--要封装的话去下层封装
local eventUtils = require("event.eventUtils")
local timerUtils = require("time.timerUtils")

---@class ASyncObject:Object
local ASyncObject = require("obj.Abstract.Object"):extends()

function ASyncObject:onNew()
    self.__res = nil
    self.__isLoading = nil
    self.__wishEnable = nil
    self.__isEnabled = nil
end

---@public
function ASyncObject:show()
    if self.__wishEnable and not self.__isEnabled then
        return
    end
    self.__wishEnable = true

    if self.__isLoading then
        return
    end

    if self.__res then
        self:__showWithRes()
    else
        self.__isLoading = true
        self:loadRes(function(res, self)
            self:__onLoadedDone(res)
        end, self)
    end
end

---@public
function ASyncObject:hide()
    if not self.__wishEnable then
        return
    end
    self.__wishEnable = nil
    if not self.__res then
        return
    else
        self:__hideWithRes()
    end
end

function ASyncObject:__onLoadedDone(res)
    self.__isLoading = nil
    self.__res = res
    if self.__wishEnable then
        self:__showWithRes()
    else
        self:__hideWithRes()
    end
end

function ASyncObject:__showWithRes()
    if not self.__isEnabled then
        self.__isEnabled = true
        if self.onASyncObjectEnable then
            self:onASyncObjectEnable(self.__res)
        end
    end
    if self.onShow then
        self:onShow()
    end
end

function ASyncObject:__hideWithRes()
    if self.__isEnabled then
        self.__isEnabled = nil
        if self.onASyncObjectDisable then
            self:onASyncObjectDisable()
        end
    end
    self:unloadRes(self.__res)
    self.__res = nil
    eventUtils.unRegAllEvent(self)
    timerUtils.unScheduleAllTimer(self)
end

---@public
function ASyncObject:reg(simpleevt, handler)
    eventUtils.reg(self, simpleevt, handler)
end

---@public
function ASyncObject:scheduleTimer(fixid, delay, task, ...)
    timerUtils.scheduleTimer(self, fixid, delay, task, ...)
end

---@public
function ASyncObject:scheduleTimerAtFixedRate(fixid, delay, period, task, ...)
    timerUtils.scheduleTimerAtFixedRate(self, fixid, delay, period, task, ...)
end

---@public
function ASyncObject:unRegAllEvent()
    eventUtils.unRegAllEvent(self)
end

---@public
function ASyncObject:unScheduleAllTimer()
    timerUtils.unScheduleAllTimer(self)
end

---@protected
function ASyncObject:loadRes(callBack, param)
    error("loadRes 方法需要实现")
end

---@protected
function ASyncObject:unloadRes(res)
    error("unloadRes 方法需要实现")
end

---@protected
function ASyncObject:onASyncObjectEnable(res)
    error("建议重载，提供名为onEnable的回调")
end

---@protected
function ASyncObject:onASyncObjectDisable()
    error("建议重载，提供名为onDisable的回调")
end

return ASyncObject