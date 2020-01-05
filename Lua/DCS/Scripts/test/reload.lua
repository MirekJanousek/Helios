helios_mock.test = {
    [1] = function()
        helios_mock.setSelf("FA-18C_hornet")
        helios_mock.loadDriver("FA-18C_hornet")
        assert(helios_mock.driverName() == "FA-18C_hornet", "driver not active")
    end,
    [2] = function()
        -- luacheck: globals tostring LuaExportActivityNextEvent helios_loader
        helios_mock.loader.impl.setSimID("ORIGINAL*")
        log.write('RELOAD', log.DEBUG, string.format("global LuaExportActivityNextEvent '%s'", tostring(LuaExportActivityNextEvent)))
        helios_mock.loader.reload()
        assert(helios_mock.driverName() == "", "state not reset")
        log.write('RELOAD', log.DEBUG, string.format("global LuaExportActivityNextEvent '%s'", tostring(LuaExportActivityNextEvent)))
    end,
    [3] = function()
        helios_mock.setSelf("FA-18C_hornet")
        helios_mock.loadDriver("FA-18C_hornet")
        assert(helios_mock.driverName() == "FA-18C_hornet", "driver not active")
    end
}
