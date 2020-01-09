
-- load modules from our containing directory
package.path = package.path..';./exmock/?.lua;'
package.cpath = package.cpath..';./exmock/?.dll;'

-- bunch of globals set to appear like DCS Lua environment
local exmock = loadfile("exmock/mock_dcs.lua")()

-- load export16 and additional interface for tests to reach into export16
helios_mock = loadfile("exmock/mock_export16.lua")(exmock, arg[2])

-- configure test, if any
if arg[1] then
    loadfile(string.format("test/%s.lua", arg[1]))()
else
    helios_mock.test = {
        [10] = function() end
    }
end

local eventFrames = {}
if helios_mock ~= nil and  helios_mock.test ~= nil then
    -- create sorted work list of events in test
    for eventFrame, _ in pairs(helios_mock.test)  do
        table.insert(eventFrames, eventFrame)
    end
    table.sort(eventFrames)
else
    -- run for 1000 frames
    eventFrames[1000] = function() end
end

helios_mock.run(eventFrames)