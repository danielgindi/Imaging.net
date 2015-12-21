using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections;
using System.Runtime.InteropServices;
using System.Drawing.Drawing2D;

namespace Imaging.net
{
    [Flags]
    public enum Corner
    {
        None = 0,
        TopLeft = 0x01,
        TopRight = 0x02,
        BottomRight = 0x04,
        BottomLeft = 0x08,
        AllCorners = TopLeft | TopRight | BottomRight | BottomLeft
    }
}
