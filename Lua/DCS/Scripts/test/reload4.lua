helios_mock.test = {
    [1] = function()
        helios_mock.loader.impl.setSimID("ORIGINAL*")
        helios_mock.setSelf("FA-18C_hornet")
    end,
    [15] = function()
        helios_mock.loadDriver("FA-18C_hornet")
        assert(helios_mock.driverName() == "FA-18C_hornet", "driver not loaded")
        helios_mock.loader.reload()
    end,
    [16] = function()
        assert(helios_mock.driverName() == "", "state not reset")
        helios_mock.loader.impl.setSimID("SECOND*")
        helios_mock.loader.reload()
        assert(helios_mock.driverName() == "", "state not reset")
    end,
    [17] = function()
    end
}
