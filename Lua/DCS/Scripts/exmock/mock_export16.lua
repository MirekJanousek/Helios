-- mock testing interface, only for test/*.lua
local helios_mock = {}
helios_mock.fps = 60.0
helios_mock.sleepRatio = 0.9

-- our mock dcs and nohook option
local exmock, hotload = ...
helios_mock.exmock = exmock
helios_mock.selfName = exmock.selfName

-- incrementing counters for mock device values
local counters = exmock.counters

if hotload == "hotload" then
    -- load export script as if hot reload was the main Export.lua running under DCS, and gain privileged access
    helios_mock.loader = loadfile(lfs.writedir().."Scripts\\Helios\\HeliosReload.lua")()
    helios_mock.loader.hotload(lfs.writedir().."Scripts\\Helios\\HeliosExport16.lua")
else
    -- load export script as if Helios Export was the main Export.lua running under DCS, and gain privileged access
    helios_mock.loader = {}
    helios_mock.loader.impl = loadfile(lfs.writedir().."Scripts\\Helios\\HeliosExport16.lua")()
end

-- set name of vehicle/aircraft to be reported by mock DCS
function helios_mock.setSelf(name)
    log.write('MOCK', log.ERROR, string.format("changing vehicle from '%s' to '%s'", helios_mock.selfName, name))
    helios_mock.exmock.selfName = name

    -- existing test scripts expect this here
    helios_mock.selfName = name
end

function helios_mock.driverName()
    return helios_mock.loader.impl.driverName
end

function helios_mock.moduleName()
    return helios_mock.loader.impl.moduleName
end

function helios_mock.loadDriver(driverName)
    -- NOTE: this isn't part of the helios API, enforced by luacheck
    helios_mock.loader.impl.loadDriver(driverName)
end

function helios_mock.receiveLoadDriver(driverName)
    helios_mock.loader.impl.dispatchCommand(string.format("D%s", driverName))
end

function helios_mock.receiveLoadModule()
    helios_mock.loader.impl.dispatchCommand(string.format("M"))
end

-- test loading module in compatibility mode
function helios_mock.loadModule(selfName, moduleName)
    local driver = helios_mock.loader.impl.createModuleDriver(selfName, moduleName)
    if driver == nil then
        log.write('MOCK', log.ERROR, string.format("failed to create module driver %s for %s", moduleName, selfName))
        return
    end
    helios_mock.loader.impl.installDriver(driver, moduleName)
    helios_mock.loader.impl.notifyLoaded()
end

-- LuaSocket
local socket = require("socket")

-- these must be defined by export scripts we test
-- luacheck: globals LuaExportStart LuaExportBeforeNextFrame LuaExportAfterNextFrame LuaExportActivityNextEvent LuaExportStop

-- run scenario from test case
function helios_mock.run(eventFrames)
    LuaExportStart()
    local frame = 0
    local nextEvent = 0;
    for _, eventFrame in ipairs(eventFrames) do
        local event = helios_mock.test[eventFrame]
        log.write('MOCK', log.INFO, string.format("executing until next test event at frame %d", eventFrame))
        for progress=frame,eventFrame do
            log.write('MOCK', log.DEBUG, string.format("frame %d", progress))
            LuaExportBeforeNextFrame()
            frame = progress
            -- sleep to slow down
            counters.nextValue = counters.nextValue + 1
            socket.select(nil, nil, helios_mock.sleepRatio / helios_mock.fps)
            LuaExportAfterNextFrame()

            local clock = frame / helios_mock.fps
            if clock >= nextEvent then
                log.write('MOCK', log.DEBUG, string.format("export activity at clock %f", clock))
                nextEvent = LuaExportActivityNextEvent(nextEvent)
            end
        end
        -- now we have just processed a frame after which we have a test event
        log.write('MOCK', log.INFO, string.format("test event at frame %d", eventFrame))
        event()
    end
    LuaExportStop()
end

return helios_mock