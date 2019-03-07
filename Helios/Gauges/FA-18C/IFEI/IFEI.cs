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

namespace GadrocsWorkshop.Helios.Gauges.FA18C
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Controls;
    using System;
    using System.Windows.Media;
    using System.Windows;

    [HeliosControl("Helios.FA18C.IFEI", "IFEI", "F/A-18C", typeof(FA18CDeviceRenderer))]
    class IFEI_FA18C : FA18CDevice
    {
        private static readonly Rect SCREEN_RECT = new Rect(0, 0, 1, 1);
        private Rect _scaledScreenRect = SCREEN_RECT;
        private string _interfaceDeviceName = "IFEI";

        private String _font = "Hornet IFEI Mono"; // "Segment7 Standard"; //"Seven Segment";
        private Color _textColor = Color.FromArgb(0xff,220, 220, 220);
        private Color _backGroundColor = Color.FromArgb(100, 100, 20, 50);
        private string _imageLocation = "{Helios}/Gauges/FA-18C/IFEI/";
        private bool _useBackGround = false;

        public IFEI_FA18C()
            : base("IFEI_Gauge", new Size(779, 702))
        {
            double spacing = 70;
            double start = 64;
            double left = 400;
            // adding the control buttons
            AddButton("MODE", left, start, new Size(87, 62), _interfaceDeviceName, "IFEI Mode Button");
            AddButton("QTY", left, start + spacing, new Size(87, 62), _interfaceDeviceName, "IFEI QTY Button");
            AddButton("UP", left, start + 2 * spacing, new Size(87, 62), _interfaceDeviceName, "IFEI Up Arrow Button");
            AddButton("DOWN", left, start + 3 * spacing, new Size(87, 62), _interfaceDeviceName, "IFEI Down Arrow Button");
            AddButton("ZONE", left, start + 4 * spacing, new Size(87, 62), _interfaceDeviceName, "IFEI ZONE Button");
            AddButton("ET", left, start + 5 * spacing, new Size(87, 62), _interfaceDeviceName, "IFEI ET Button");

            // adding the text displays
            double dispHeight = 50;
            double fontSize = 50;

            double clockDispWidth = 50;
            double clockSpreadWidth = 3;
            double clockX = 530;
            double clockY = 352;
            AddTextDisplay("Clock HH", clockX, clockY, new Size(clockDispWidth, dispHeight), fontSize, "10", _interfaceDeviceName, "Clock Hours");
            AddTextDisplay("Clock MM", clockX + clockDispWidth + clockSpreadWidth, clockY, new Size(clockDispWidth, dispHeight), fontSize, "11", _interfaceDeviceName, "Clock Minutes");
            AddTextDisplay("Clock SS", clockX + 2* (clockDispWidth + clockSpreadWidth), clockY, new Size(clockDispWidth, dispHeight), fontSize, "12", _interfaceDeviceName, "Clock Seconds");
            dispHeight = 32;
            AddTextDisplay("Elapsed Time", 554, 418, new Size(128, dispHeight), fontSize, "01:59:59", _interfaceDeviceName, "Elapsed Time");

            // Fuel info

            AddTextDisplay("Bingo", 545, 255, new Size(133, dispHeight), fontSize, "2000", _interfaceDeviceName, "Bingo Value");

            double fuelX = 530;
            double fuelWidth = 154;
            AddTextDisplay("Fuel Total", fuelX, 97, new Size(fuelWidth, dispHeight), fontSize, "10780T", _interfaceDeviceName, "Total Fuel Amount");



            AddTextDisplay("Fuel Internal", fuelX, 163, new Size(fuelWidth, dispHeight), fontSize, "10780I", _interfaceDeviceName, "Internal Fuel Amount");

            double RPMWidth = 60;
            AddTextDisplay("RPM Left", 110, 91, new Size(RPMWidth, dispHeight), fontSize, "65", _interfaceDeviceName, "Left RPM Value");
            AddTextDisplay("RPM Right", 261, 91, new Size(RPMWidth, dispHeight), fontSize, "65", _interfaceDeviceName, "Left RPM Value");
            
            double TempWidth = 92;
            AddTextDisplay("Temp Left", 80, 148, new Size(TempWidth, dispHeight), fontSize, "330", _interfaceDeviceName, "Left Temperature Value");
            AddTextDisplay("Temp Right", 261, 148, new Size(TempWidth, dispHeight), fontSize, "330", _interfaceDeviceName, "Right Temperature Value");

            AddTextDisplay("FF Left", 80, 203, new Size(TempWidth, dispHeight), fontSize, "6", _interfaceDeviceName, "Left Fuel Flow Value");
            AddTextDisplay("FF Right", 261, 203, new Size(TempWidth, dispHeight), fontSize, "6", _interfaceDeviceName, "Right fuel Flow Value");

            double oilWidth = 64;
            AddTextDisplay("Oil Left", 107, 438, new Size(oilWidth, dispHeight), fontSize, "60", _interfaceDeviceName, "Left Oil Temperature");
            AddTextDisplay("Oil Right", 262, 438, new Size(oilWidth, dispHeight), fontSize, "60", _interfaceDeviceName, "Right Oil Temperature");

            AddIFEIParts("Gauges", 0, 0, new Size(779, 702), _interfaceDeviceName, "IFEI Needles & Flags");

            AddPot(
                name: "Brightness Control",
                posn: new Point(82, 630),
                size: new Size(60, 60),
                knobImage: "{Helios}/Images/AV-8B/Common Knob.png",
                initialRotation: 219,
                rotationTravel: 291,
                minValue: 0,
                maxValue: 1,
                initialValue: 0,
                stepValue: 0.1,
                interfaceDeviceName: _interfaceDeviceName,
                interfaceElementName: "IFEI Brightness Control Knob",
                fromCenter: true
                );
            Size ThreeWayToggleSize = new Size(70, 140);
            Add3PosnToggle(
                name: "Video Record DDI",
                posn: new Point(236, 570),
                size: ThreeWayToggleSize,
                image: "{Helios}/Images/Toggles/orange-round-",
                interfaceDevice: _interfaceDeviceName,
                interfaceElement: "IFEI Video Record Switch DDI",
                fromCenter: false
                );

            Add3PosnToggle(
                name: "Video Record HUD",
                posn: new Point(395, 570),
                size: ThreeWayToggleSize,
                image: "{Helios}/Images/Toggles/orange-round-",
                interfaceDevice: _interfaceDeviceName,
                interfaceElement: "IFEI Video Record Switch HUD",
                fromCenter: false
                );

            Add3PosnToggle(
                name: "Video Record Control",
                posn: new Point(584, 570),
                size: ThreeWayToggleSize,
                image: "{Helios}/Images/Toggles/orange-round-",
                interfaceDevice: _interfaceDeviceName,
                interfaceElement: "IFEI Video Record Switch Control",
                fromCenter: false
                );


        }

        protected override void OnProfileChanged(HeliosProfile oldProfile) {
            base.OnProfileChanged(oldProfile);
        }

        public override string BezelImage
        {
            get { return _imageLocation + "IFEI.png"; }
        }

        private void AddTextDisplay(string name, double x, double y, Size size, double baseFontsize, string testDisp,
            string interfaceDevice, string interfaceElement)
        {
            TextDisplay display = AddTextDisplay(
                name: name,
                pos: new Point(x, y),
                size: size,
                font: _font,
                baseFontsize: baseFontsize,
                horizontalAlignment: TextHorizontalAlignment.Right,
                verticalAligment: TextVerticalAlignment.Top,
                testTextDisplay: testDisp,
                textColor: _textColor,
                backgroundColor: _backGroundColor,
                useBackground: _useBackGround,
                interfaceDeviceName: interfaceDevice,
                interfaceElementName: interfaceElement
                );
            display.TextFormat.FontWeight = FontWeights.Heavy;
        }

        private void AddButton(string name, double x, double y, Size size, string interfaceDevice, string interfaceElement)
        {
            Point pos = new Point(x, y);
            AddButton(
                name: name,
                posn: pos,
                size: size,
                image: _imageLocation + "IFEI_" + name + ".png",
                pushedImage: _imageLocation + "IFEI_" + name + "_DN.png",
                buttonText: "",
                interfaceDeviceName: interfaceDevice,
                interfaceElementName: interfaceElement,
                fromCenter: false
                );
        }
        private void Add3PosnToggle(string name, Point posn, Size size, string image, string interfaceDevice, string interfaceElement, bool fromCenter)
        {
            Add3PosnToggle(
                name: name,
                posn: posn,
                size: size,
                imageUp: image + "up.png",
                imageNorm: image + "norm.png",
                imageDown: image + "down.png",
                interfaceDeviceName: interfaceDevice,
                interfaceElementName: interfaceElement,
                fromCenter: false
                );
        }
        private void AddIFEIParts(string name, double x, double y, Size size, string interfaceDevice, string interfaceElement)
        {
            IFEI_Gauges IFEI_gauges = new IFEI_Gauges
            {
                Top = y,
                Left = x,
                Height = size.Height,
                Width = size.Width,
                Name = name
            };

            Children.Add(IFEI_gauges);
            foreach (IBindingTrigger trigger in IFEI_gauges.Triggers)
            {
                AddTrigger(trigger, trigger.Name);
            }
            foreach (IBindingAction action in IFEI_gauges.Actions)
            {
                AddAction(action, action.Name);
            }
            //AddDefaultOutputBinding(
            //    childName: componentName,
            //    deviceTriggerName: "encoder.incremented",
            //    interfaceActionName: interfaceDeviceName + ".increment." + interfaceElementName
            //);
            //AddDefaultOutputBinding(
            //    childName: componentName,
            //    deviceTriggerName: "encoder.decremented",
            //    interfaceActionName: interfaceDeviceName + ".decrement." + interfaceElementName
            //    );
        }

        public override bool HitTest(Point location)
        {
            if (_scaledScreenRect.Contains(location))
            {
                return false;
            }

            return true;
        }

        public override void MouseDown(Point location)
        {
            // No-Op
        }

        public override void MouseDrag(Point location)
        {
            // No-Op
        }

        public override void MouseUp(Point location)
        {
            // No-Op
        }

    }
}
