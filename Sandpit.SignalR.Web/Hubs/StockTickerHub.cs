using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;


namespace Sandpit.SignalR.Web
{
    [HubName("stockTickerMini")]
    public class StockTickerHub : Hub
    {
        private readonly StockTickerBroadcast _stockTicker;


        public StockTickerHub() : this(StockTickerBroadcast.Instance) { }

        public StockTickerHub(StockTickerBroadcast stockTicker)
        {
            _stockTicker = stockTicker;
        }


        public IEnumerable<StockModel> GetAllStocks()
        {
            return _stockTicker.GetAllStocks();
        }
    }
}