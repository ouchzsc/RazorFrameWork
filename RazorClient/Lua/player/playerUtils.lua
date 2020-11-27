local playerUtils = {}

local function getVDir(v)
    local vDir = 0
    if v > 0 then
        vDir = 1
    elseif v < 0 then
        vDir = -1
    end
    return vDir
end

local function acc(v, vMax, v_add, inputDir)
    local vDir = getVDir(v)
    if vDir == 0 then
        return vMax * inputDir * 0.3
    end
    if math.abs(v) < vMax then
        local v_added = v + v_add * vDir
        if math.abs(v_added) > vMax then
            v_added = vMax * vDir
        end
        return v_added
    else
        return v
    end
end

local function dcc(v, v_dcc)
    local abs_v = math.abs(v)
    local vDir = getVDir(v)
    if abs_v < v_dcc then
        return 0
    else
        return v - vDir * v_dcc
    end
end

function playerUtils.calc(v, inputDir, vMax, v_add, dv_release, dv_press)
    local abs_v = math.abs(v)
    local vDir = getVDir(v)
    if inputDir == 0 then
        if abs_v > 0 then
            return dcc(v, dv_release)
        else
            return 0
        end
    else
        if inputDir == vDir or vDir == 0 then
            --相同方向，按键加速
            return acc(v, vMax, v_add, inputDir)
        else
            if abs_v > 0 then
                --按键减速
                return dcc(v, dv_press)
            else
                --启动加速
                return acc(v, vMax, v_add, inputDir)
            end
        end
    end
end

return playerUtils