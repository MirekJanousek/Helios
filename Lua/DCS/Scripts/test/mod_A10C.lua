helios_mock.test = {
    [1] = function()
        helios_mock.setSelf("A-10C")
        helios_mock.sleepRatio = 0.9
    end,
    -- wait until we notice vehicle change
    [11] = function()
        helios_mock.loadModule("A-10C", "Helios_A10C")
    end,
    [10000] = function()
    end
}