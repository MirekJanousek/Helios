﻿//  Copyright 2014 Craig Courtney
//    
//  Helios is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  Helios is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

namespace GadrocsWorkshop.Helios.Interfaces.DCS.AV8B
{
    using GadrocsWorkshop.Helios.Interfaces.DCS.Common;
    using GadrocsWorkshop.Helios.UDPInterface;
    using GadrocsWorkshop.Helios.Windows.Controls;
    using Microsoft.Win32;
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    /// <summary>
    /// Interaction logic for AV8BInterfaceEditor.xaml
    /// </summary>
    public partial class AV8BInterfaceEditor : HeliosInterfaceEditor
    {
        static AV8BInterfaceEditor()
        {
            Type ownerType = typeof(AV8BInterfaceEditor);

            CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(DCSConfigurator.AddDoFile, AddDoFile_Executed));
            CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(DCSConfigurator.RemoveDoFile, RemoveDoFile_Executed));
        }

        private static void AddDoFile_Executed(object target, ExecutedRoutedEventArgs e)
        {
            AV8BInterfaceEditor editor = target as AV8BInterfaceEditor;
            string file = e.Parameter as string;
            if (editor != null && !string.IsNullOrWhiteSpace(file) && !editor.Configuration.DoFiles.Contains(file))
            {
                editor.Configuration.DoFiles.Add((string)e.Parameter);
                editor.NewDoFile.Text = "";
            }
        }

        private static void RemoveDoFile_Executed(object target, ExecutedRoutedEventArgs e)
        {
            AV8BInterfaceEditor editor = target as AV8BInterfaceEditor;
            string file = e.Parameter as string;
            if (editor != null && !string.IsNullOrWhiteSpace(file) && editor.Configuration.DoFiles.Contains(file))
            {
                editor.Configuration.DoFiles.Remove(file);
            }
        }

        private string _dcsPath = null;
        private uint _bestDCSInstallType = 0;

        public AV8BInterfaceEditor()
        {
            InitializeComponent();
            _bestDCSInstallType = 3;
            Configuration = new DCSConfigurator("DCS AV-8B", DCSPath);
            Configuration.ExportConfigPath = "Config\\Export";
            switch (_bestDCSInstallType)
            {
                case 3:
                    Configuration.DCSInstallType = "GA";
                    Configuration.InstallTypeGA = true;
                    break;
                case 2:
                    Configuration.DCSInstallType = "OpenBeta";
                    Configuration.InstallTypeBeta = true;
                    break;
                case 1:
                    Configuration.DCSInstallType = "OpenAlpha";
                    Configuration.InstallTypeAlpha = true;
                    break;
                default:
                    Configuration.DCSInstallType = "";
                    Configuration.InstallTypeGA = false;
                    Configuration.InstallTypeBeta = false;
                    Configuration.InstallTypeAlpha = false;
                    break;
            }
            Configuration.ExportFunctionsPath = "pack://application:,,,/Helios;component/Interfaces/DCS/AV8B/ExportFunctions.lua";

        }

        #region Properties

        public DCSConfigurator Configuration
        {
            get { return (DCSConfigurator)GetValue(ConfigurationProperty); }
            set { SetValue(ConfigurationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Configuration.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ConfigurationProperty =
            DependencyProperty.Register("Configuration", typeof(DCSConfigurator), typeof(AV8BInterfaceEditor), new PropertyMetadata(null));

        public string DCSPath
        {
            get
            {

                if (_dcsPath == null)
                {
                    RegistryKey pathKey = Registry.CurrentUser.OpenSubKey(@"Software\Eagle Dynamics\DCS World");
                    if (pathKey == null)
                    {
                        --_bestDCSInstallType;
                        pathKey = Registry.CurrentUser.OpenSubKey(@"Software\Eagle Dynamics\DCS World OpenBeta");
                    }
                    if (pathKey == null)
                    {
                        --_bestDCSInstallType;
                        pathKey = Registry.CurrentUser.OpenSubKey(@"Software\Eagle Dynamics\DCS World OpenAlpha");
                    }
                    if (pathKey == null)
                    {
                        --_bestDCSInstallType;
                        pathKey = Registry.CurrentUser.OpenSubKey(@"Software\Eagle Dynamics\DCS AV8B");
                    }

                    if (pathKey != null)
                    {
                        _dcsPath = (string)pathKey.GetValue("Path");
                        pathKey.Close();
                        ConfigManager.LogManager.LogDebug("DCS AV-8B Interface Editor - Found DCS Path (Path=\"" + _dcsPath + "\")");
                    }
                    else
                    {
                        ConfigManager.LogManager.LogDebug("DCS AV-8B Interface Editor - No DCS Installation Paths Found in registry");
                        _bestDCSInstallType = 0;
                        _dcsPath = "";
                    }
                }
                return _dcsPath;
            }
            set
            {
                _dcsPath = value;
            }
        }
        public void ForceDCSPath()
        {
            RegistryKey pathKey = null;
            if (Configuration.InstallTypeGA)
            {
                pathKey = Registry.CurrentUser.OpenSubKey(@"Software\Eagle Dynamics\DCS World");
            }
            else if (Configuration.InstallTypeBeta)
            {
                pathKey = Registry.CurrentUser.OpenSubKey(@"Software\Eagle Dynamics\DCS World OpenBeta");
            }
            else if (Configuration.InstallTypeAlpha)
            {
                pathKey = Registry.CurrentUser.OpenSubKey(@"Software\Eagle Dynamics\DCS World OpenAlpha");
            }

            if (pathKey != null)
            {
                _dcsPath = (string)pathKey.GetValue("Path");
                Configuration.AppPath = _dcsPath;
                pathKey.Close();
                ConfigManager.LogManager.LogDebug("DCS AV-8B Interface Editor - Found DCS Path (Path=\"" + _dcsPath + "\")");
            }
            else
            {
                _dcsPath = "";
                Configuration.AppPath = "";
                ConfigManager.LogManager.LogDebug("DCS AV-8B Interface Editor - Forced DCS Install Type Path not found (Installation Type=\"" + Configuration.DCSInstallType + "\")");
            }
        }

        #endregion

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == InterfaceProperty)
            {
                Configuration.UDPInterface = Interface as BaseUDPInterface;
            }

            base.OnPropertyChanged(e);
        }

        private void Configure_Click(object sender, RoutedEventArgs e)
        {
            if (Configuration.UpdateExportConfig())
            {
                MessageBox.Show(Window.GetWindow(this), "DCS AV-8B has been configured.");
            }
            else
            {
                MessageBox.Show(Window.GetWindow(this), "Error updating DCS AV-8B configuration.  Please do one of the following and try again:\n\nOption 1) Run Helios as Administrator\nOption 2) Install DCS outside the Program Files Directory\nOption 3) Disable UAC.");
            }
        }

        private void ResetPath(object sender, RoutedEventArgs e)
        {
            if (Configuration != null)
            {
                Configuration.AppPath = DCSPath;
            }
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            Configuration.RestoreConfig();
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton _rb = (RadioButton)sender;
            if (_rb.GroupName == "DCSInstallTypeGroup")
            {
                // an override for the installation type has been declared 
                Configuration.DCSInstallType = (string)_rb.Tag;
                ForceDCSPath();
            }
        }

    }
}