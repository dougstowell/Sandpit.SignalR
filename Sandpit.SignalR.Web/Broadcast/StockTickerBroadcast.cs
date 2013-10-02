using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;


namespace Sandpit.SignalR.Web
{
    public class StockTickerBroadcast
    {
        // Singleton instance
        private readonly static Lazy<StockTickerBroadcast> _instance = 
            new Lazy<StockTickerBroadcast>(() => new StockTickerBroadcast(GlobalHost.ConnectionManager.GetHubContext<StockTickerHub>().Clients));

        private readonly ConcurrentDictionary<string, StockModel> _stocks = new ConcurrentDictionary<string, StockModel>();

        private readonly object _updateStockPricesLock = new object();

        //stock can go up or down by a percentage of this factor on each change
        private readonly double _rangePercent = .002;

        private readonly TimeSpan _updateInterval = TimeSpan.FromMilliseconds(250);
        private readonly Random _updateOrNotRandom = new Random();

        private readonly Timer _timer;
        private volatile bool _updatingStockPrices = false;


        public static StockTickerBroadcast Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        private IHubConnectionContext Clients
        {
            get;
            set;
        }


        private StockTickerBroadcast(IHubConnectionContext clients)
        {
            Clients = clients;

            _stocks.Clear();
            var stocks = new List<StockModel>
            {
                new StockModel { Symbol = "MSFT", Price = 30.31m },
                new StockModel { Symbol = "APPL", Price = 578.18m },
                new StockModel { Symbol = "GOOG", Price = 570.30m }
            };
            stocks.ForEach(stock => _stocks.TryAdd(stock.Symbol, stock));

            _timer = new Timer(UpdateStockPrices, null, _updateInterval, _updateInterval);

        }


        public IEnumerable<StockModel> GetAllStocks()
        {
            return _stocks.Values;
        }

        private void UpdateStockPrices(object state)
        {
            lock (_updateStockPricesLock)
            {
                if (!_updatingStockPrices)
                {
                    _updatingStockPrices = true;

                    foreach (var stock in _stocks.Values)
                    {
                        if (TryUpdateStockPrice(stock))
                        {
                            Broadcast(stock);
                        }
                    }

                    _updatingStockPrices = false;
                }
            }
        }

        private bool TryUpdateStockPrice(StockModel stock)
        {
            // Randomly choose whether to udpate this stock or not
            var r = _updateOrNotRandom.NextDouble();
            if (r > .1)
            {
                return false;
            }

            // Update the stock price by a random factor of the range percent
            var random = new Random((int)Math.Floor(stock.Price));
            var percentChange = random.NextDouble() * _rangePercent;
            var pos = random.NextDouble() > .51;
            var change = Math.Round(stock.Price * (decimal)percentChange, 2);
            change = pos ? change : -change;

            stock.Price += change;
            return true;
        }

        private void Broadcast(StockModel stock)
        {
            Clients.All.updateStockPrice(stock);
        }
    }
}