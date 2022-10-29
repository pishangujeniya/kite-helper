﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KiteConnect
{
    public class Constants
    {

        // Products
        public const string PRODUCT_MIS = "MIS";
        public const string PRODUCT_CNC = "CNC";
        public const string PRODUCT_NRML = "NRML";

        // Order types
        public const string ORDER_TYPE_MARKET = "MARKET";
        public const string ORDER_TYPE_LIMIT = "LIMIT";
        public const string ORDER_TYPE_SLM = "SL-M";
        public const string ORDER_TYPE_SL = "SL";

        // Order status
        public const string ORDER_STATUS_COMPLETE = "COMPLETE";
        public const string ORDER_STATUS_CANCELLED = "CANCELLED";
        public const string ORDER_STATUS_REJECTED = "REJECTED";

        // Varities
        public const string VARIETY_REGULAR = "regular";
        public const string VARIETY_BO = "bo";
        public const string VARIETY_CO = "co";
        public const string VARIETY_AMO = "amo";
        public const string VARIETY_ICEBERG = "iceberg";

        // Transaction type
        public const string TRANSACTION_TYPE_BUY = "BUY";
        public const string TRANSACTION_TYPE_SELL = "SELL";

        // Validity
        public const string VALIDITY_DAY = "DAY";
        public const string VALIDITY_IOC = "IOC";
        public const string VALIDITY_TTL = "TTL";

        // Exchanges
        public const string EXCHANGE_NSE = "NSE";
        public const string EXCHANGE_BSE = "BSE";
        public const string EXCHANGE_NFO = "NFO";
        public const string EXCHANGE_CDS = "CDS";
        public const string EXCHANGE_BFO = "BFO";
        public const string EXCHANGE_MCX = "MCX";

        // Margins segments
        public const string MARGIN_EQUITY = "equity";
        public const string MARGIN_COMMODITY = "commodity";

        // Margin modes
        public const string MARGIN_MODE_COMPACT = "compact";

        // Ticker modes
        public const string MODE_FULL = "full";
        public const string MODE_QUOTE = "quote";
        public const string MODE_LTP = "ltp";

        // Positions
        public const string POSITION_DAY = "day";
        public const string POSITION_OVERNIGHT = "overnight";

        // Historical intervals
        public const string INTERVAL_MINUTE = "minute";
        public const string INTERVAL_3MINUTE = "3minute";
        public const string INTERVAL_5MINUTE = "5minute";
        public const string INTERVAL_10MINUTE = "10minute";
        public const string INTERVAL_15MINUTE = "15minute";
        public const string INTERVAL_30MINUTE = "30minute";
        public const string INTERVAL_60MINUTE = "60minute";
        public const string INTERVAL_DAY = "day";

        // GTT status
        public const string GTT_ACTIVE = "active";
        public const string GTT_TRIGGERED = "triggered";
        public const string GTT_DISABLED = "disabled";
        public const string GTT_EXPIRED = "expired";
        public const string GTT_CANCELLED = "cancelled";
        public const string GTT_REJECTED = "rejected";
        public const string GTT_DELETED = "deleted";


        // GTT trigger type
        public const string GTT_TRIGGER_OCO = "two-leg";
        public const string GTT_TRIGGER_SINGLE = "single";
    }
}
