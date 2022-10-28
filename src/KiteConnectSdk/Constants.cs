using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiteConnectSdk
{
    public class Constants : KiteConnect.Constants
    {
        public const string KiteBrowserOms = "/oms";
        public const string KiteBrowserApiRoot = "https://kite.zerodha.com" + KiteBrowserOms;
        public const string KiteInstrumentsCsvUrl = "https://api.kite.trade/instruments";
    }
}
