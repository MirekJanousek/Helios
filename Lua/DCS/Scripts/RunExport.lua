
-- load modules from our containing directory
package.path = package.path..';./exmock/?.lua;'
package.cpath = package.cpath..';./exmock/?.dll;'

-- bunch of globals set to appear like DCS Lua environment
local exmock = loadfile("exmock/mock_dcs.lua")()
exmock.selfName = arg[1];
log.write('MOCK', log.INFO, string.format("aircraft set to '%s'", exmock.selfName));

-- change to test directory, if any
if arg[2] ~= nil then
    lfs.chdir(arg[2])
end

-- load Export.lua we are testing
local success, result, errorMessage = pcall(loadfile, "Export.lua")
if not success then
    log.write('MOCK', log.ERROR, result);
    return 1
end
if result == nil then
    log.write('MOCK', log.ERROR, string.format("'Export.lua' not found in '%s'", lfs.currentdir()));
    log.write('MOCK', log.ERROR, errorMessage);
    return 2
end

-- default implementations that not all export modules will have.  for now, we choose to crash if someone does
-- dot implement the other export functions, because this is most likely a test setup error
function LuaExportBeforeNextFrame() end
function LuaExportAfterNextFrame() end

-- execute script to register callbacks
result()

-- run like DCS
exmock.run()