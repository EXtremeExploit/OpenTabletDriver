using System.Linq;
using TabletDriverLib.Tablet;
using TabletDriverPlugin.Tablet;

namespace TabletDriverLib.Vendors
{
    public class SkipByteTabletReportParser : TabletReportParser
    {
        public override IDeviceReport Parse(byte[] data)
        {
            return base.Parse(data[1..^0]);
        }
    }
}