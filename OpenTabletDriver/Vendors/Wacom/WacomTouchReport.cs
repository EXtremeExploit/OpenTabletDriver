﻿using System.Numerics;
using OpenTabletDriver.Plugin.Tablet;
using OpenTabletDriver.Plugin.Tablet.Touch;

namespace OpenTabletDriver.Vendors.Wacom
{
    public struct WacomTouchReport : ITouchReport, IAuxReport
    {
        public WacomTouchReport(byte[] report)
        {
            Raw = report;
            AuxButtons = new bool[0];
            Touches = prevTouches ?? new TouchPoint[MAX_POINTS];
            if (report[2] == 0x81)
            {
                ApplyTouchMask((ushort)(Raw[3] | (Raw[4] << 8)));
                prevTouches = (TouchPoint[])Touches.Clone();
                return;
            }

            var nChunks = Raw[1];
            for (var i = 0; i < nChunks; i++)
            {
                var offset = (i << 3) + 2;
                var touchID = Raw[offset];
                if (touchID == 0x80)
                {
                    AuxButtons = new bool[]
                    {
                        (report[1 + offset] & (1 << 0)) != 0,
                        (report[1 + offset] & (1 << 1)) != 0,
                        (report[1 + offset] & (1 << 2)) != 0,
                        (report[1 + offset] & (1 << 3)) != 0
                    };
                    continue;
                }
                touchID -= 2;
                if (touchID >= MAX_POINTS)
                    continue;
                var touchState = Raw[1 + offset];
                if (touchState == 0x20)
                    Touches[touchID] = null;
                else
                {
                    Touches[touchID] = new TouchPoint
                    {
                        TouchID = touchID,
                        Position = new Vector2
                        {
                            X = (Raw[2 + offset] << 4) | (Raw[4 + offset] >> 4),
                            Y = (Raw[3 + offset] << 4) | (Raw[4 + offset] & 0xF)
                        },
                    };
                }
            }
            prevTouches = (TouchPoint[])Touches.Clone();
        }

        private void ApplyTouchMask(ushort mask)
        {
            for (var i = 0; i < MAX_POINTS; i++)
            {
                if ((mask & 1) == 0)
                {
                    Touches[i] = null;
                }
                mask >>= 1;
            }
        }
        private static TouchPoint[] prevTouches;
        public const int MAX_POINTS = 16;
        public byte[] Raw { set; get; }
        public bool[] AuxButtons { set; get; }
        public TouchPoint[] Touches { set; get; }
        public bool ShouldSerializeTouches() => true;
    }
}
