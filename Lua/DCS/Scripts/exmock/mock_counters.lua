local mock_counters = {}
mock_counters.nextValue = 0

function mock_counters.makeValue(min, max, resolution)
    local iteration = mock_counters.nextValue % ((max - min) / resolution)
    local value = min + (resolution * iteration)
    return value
end

return mock_counters