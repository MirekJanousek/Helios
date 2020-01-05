helios_mock.test = {
    [1] = function()
        helios_mock.setSelf("FA-18C_hornet")
        -- now wait for event loop to notice change before we load
    end,
    [11] = function()
        assert(helios.selfName() == "FA-18C_hornet", "plane not active")
        helios_mock.loadDriver("FA-18C_hornet")
        assert(helios_mock.driverName() == "FA-18C_hornet", "profile not active")
    end,
    [12] = function()
        helios_mock.loader.reload()
    end,
    [13] = function()
        helios_mock.loader.reload()
    end,
    [14] = function()
        helios_mock.setSelf("FA-18C_hornet")
    end,
    [24] = function()
        assert(helios.selfName() == "FA-18C_hornet", "plane not active")
        helios_mock.loadDriver("FA-18C_hornet", "my_F18_profile_name")
        assert(helios_mock.driverName() == "FA-18C_hornet", "driver not active")
    end
}
