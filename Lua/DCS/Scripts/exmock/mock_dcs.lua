local exmock = {}
exmock.selfName = ""
exmock.sleepRatio = 0.9
exmock.fps = 60.0

-- we implement a bunch of DCS globals, so we write them:
-- luacheck: ignore 121
-- luacheck: ignore 111

-- mock implementation, only used when testing in LUA 5.1 runtime without DCS
-- luacheck globals log
log = require("mock_log")

-- lfs is always provided by DCS
-- luacheck: globals lfs
lfs = require("lfs")

-- extra function provided by DCS
-- luacheck: globals lfs
function lfs.writedir()
    return "..\\"
end

exmock.counters = loadfile("exmock/mock_counters.lua")()

-- mock DCS device (singleton for now)
local mock_device = loadfile("exmock/mock_device.lua")(exmock.counters)

function list_indication(indicator_id) -- luacheck: no unused
    return ""
end

function GetDevice(name) -- luacheck: no unused
    return mock_device
end

function LoGetSelfData()
    local info = {}
    info.Name = exmock.selfName
    info.Heading = exmock.counters.makeValue(-1, 1, 0.02)
    return info
end

function LoGetAltitudeAboveSeaLevel()
    return exmock.counters.makeValue(10000, 20000, 10)
end

function LoGetAltitudeAboveGroundLevel()
    -- luacheck: globals LoGetAltitudeAboveSeaLevel
    return LoGetAltitudeAboveSeaLevel() - 1000;
end

function LoGetADIPitchBankYaw()
    return exmock.counters.makeValue(-10, 10, 0.1), exmock.counters.makeValue(-10, 10, 0.1), exmock.counters.makeValue(-10, 10, 0.1)
end

function LoGetEngineInfo()
    return {
        RPM = {
           left = exmock.counters.makeValue(1, 100, 1),
           right = exmock.counters.makeValue(1, 100, 1)
        },
        Temperature = {
            left = exmock.counters.makeValue(1, 100, 1),
            right = exmock.counters.makeValue(1, 100, 1)
        },
        FuelConsumption = {
            left = exmock.counters.makeValue(1, 100, 1),
            right = exmock.counters.makeValue(1, 100, 1)
        },
        fuel_internal = exmock.counters.makeValue(1000, 2000, 1),
        fuel_external = exmock.counters.makeValue(1000, 2000, 1)
    }
end

function LoGetControlPanel_HSI()
    return {
        ADR_raw = exmock.counters.makeValue(-10, 10, 0.1),
        RMI_raw = exmock.counters.makeValue(-10, 10, 0.1)
    }
end

function LoGetVerticalVelocity()
    return exmock.counters.makeValue(-10, 10, 0.1)
end

function LoGetIndicatedAirSpeed()
    return exmock.counters.makeValue(100, 500, 1.0)
end

function LoGetRoute()
    return nil
end

function LoGetAngleOfAttack()
    return exmock.counters.makeValue(-10, 10, 0.1)
end

function LoGetAccelerationUnits()
    return { x = exmock.counters.makeValue(-10, 10, 0.1), y = exmock.counters.makeValue(-10, 10, 0.1), z = exmock.counters.makeValue(-10, 10, 0.1) }
end

function LoGetGlideDeviation()
    return exmock.counters.makeValue(-10, 10, 0.1);
end

function LoGetSideDeviation()
    return exmock.counters.makeValue(-10, 10, 0.1);
end

function LoGetNavigationInfo()
    return nil
end

function LoGeoCoordinatesToLoCoordinates(x1, z1)
    return { x = x1, y = 0.0, z = z1};
end

function LoGetPlayerPlaneId()
    return "player"
end

-- LuaSocket
local socket = require("socket")

-- these must be defined by export scripts we test
-- luacheck: globals LuaExportStart LuaExportBeforeNextFrame LuaExportAfterNextFrame LuaExportActivityNextEvent LuaExportStop

-- run forever
function exmock.mainLoop()
    local frame = 0
    local nextEvent = 0;

    while true do
        frame = frame + 1
        exmock.counters.nextValue = exmock.counters.nextValue + 1
        LuaExportBeforeNextFrame()
        -- sleep to slow down
        socket.select(nil, nil, exmock.sleepRatio / exmock.fps)
        LuaExportAfterNextFrame()
        local clock = frame / exmock.fps
        if clock >= nextEvent then
            nextEvent = LuaExportActivityNextEvent(nextEvent)
        end
    end
end

-- run forever
function exmock.run()
    log.write('MOCK', log.INFO, "LuaExportStart");
    LuaExportStart()
    local _, error = pcall(exmock.mainLoop);
    log.write('MOCK', log.INFO, error);
    log.write('MOCK', log.INFO, "exiting");
    log.write('MOCK', log.INFO, "LuaExportStop");
    LuaExportStop()
end

return exmock