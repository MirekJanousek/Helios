local mock_device = {}
local mock_counters = ...

function mock_device.update_arguments()
end

function mock_device.get_argument_value(self, index)  --luacheck: no unused
    -- value goes from 0 to 100 and then wraps repeatedly
    return mock_counters.makeValue(-1, 1, 0.01)
end

function mock_device.get_frequency(self)  --luacheck: no unused
    return mock_counters.makeValue(100, 200, 0.1)
end

function mock_device.performClickableAction(self, action, value) -- luacheck: no unused
    log.write('MOCK', log.INFO, string.format("clickable action %s, %s", tostring(action), tostring(value)))
end

return mock_device