local testMgr = {}
local KeyCode = CS.UnityEngine.KeyCode

local function noFileMsg()
    print("test目录下有个test_template.lua，把“_template”删除，创建test.lua文件，即可在运行期修改代码并执行其中的f5 f6 f7 f8")
end

local function err(msg, lvl)
    loggers.default:error(msg)

end

local function tryInvokeTest(funcName)
    print(string.format("test %s", funcName))
    package.loaded["test.test"] = nil
    local ok, test = xpcall(require, noFileMsg, "test.test")
    if ok then
        local func = test[funcName]
        if func then
            xpcall(func, err)
        end
    end
end

local function onKeyDown(key)
    if key == KeyCode.F5 then
        tryInvokeTest("f5")
    end
    if key == KeyCode.F6 then
        tryInvokeTest("f6")
    end
    if key == KeyCode.F7 then
        tryInvokeTest("f7")
    end
    if key == KeyCode.F8 then
        tryInvokeTest("f8")
    end
end

function testMgr.init()
    event.onKeyDown:reg(onKeyDown)
end

return testMgr