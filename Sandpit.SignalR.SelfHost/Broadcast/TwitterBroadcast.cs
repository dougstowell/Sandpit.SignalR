﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using LinqToTwitter;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.IO;


namespace Sandpit.SignalR.SelfHost
{
    public class TwitterBroadcast
    {
        // Singleton instance
        private readonly static Lazy<TwitterBroadcast> _instance =
            new Lazy<TwitterBroadcast>(() => new TwitterBroadcast(GlobalHost.ConnectionManager.GetHubContext<TwitterHub>().Clients));

        private readonly ConcurrentDictionary<string, TweetModel> _tweets = new ConcurrentDictionary<string, TweetModel>();

        private readonly object _updateTweetsLock = new object();

        private readonly TimeSpan _updateInterval = TimeSpan.FromMilliseconds(10000);

        private readonly Timer _timer;
        private volatile bool _updatingTweets = false;


        public static TwitterBroadcast Instance
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

        private String SearchTerm
        {
            get
            {
                FileInfo fi = new FileInfo("searchTerm.txt");
                return File.ReadAllText(fi.FullName);
            }
        }


        private TwitterBroadcast(IHubConnectionContext clients)
        {
            Clients = clients;

            PopulateLatestTweets();

            _timer = new Timer(UpdateTweets, null, _updateInterval, _updateInterval);
        }


        private void PopulateLatestTweets()
        {

            _tweets.Clear();

            var auth = new SingleUserAuthorizer
            {
                Credentials = new SingleUserInMemoryCredentials
                {
                    ConsumerKey =
                        ConfigurationManager.AppSettings["twitterConsumerKey"],
                    ConsumerSecret =
                        ConfigurationManager.AppSettings["twitterConsumerSecret"],
                    TwitterAccessToken =
                        ConfigurationManager.AppSettings["twitterAccessToken"],
                    TwitterAccessTokenSecret =
                        ConfigurationManager.AppSettings["twitterAccessTokenSecret"]
                }
            };
            var twitterCtx = new TwitterContext(auth);

            String geoCode = ConfigurationManager.AppSettings["geoCode"];
            String term = SearchTerm;

            //public int Count { get; set; }
            //public string GeoCode { get; set; }
            //public bool IncludeEntities { get; set; }
            //public string Locale { get; set; }
            //public ulong MaxID { get; set; }
            //public string Query { get; set; }
            //public ResultType ResultType { get; set; }
            //public string SearchLanguage { get; set; }
            //public SearchMetaData SearchMetaData { get; set; }
            //public ulong SinceID { get; set; }
            //public List<Status> Statuses { get; set; }
            //public SearchType Type { get; set; }
            //public DateTime Until { get; set; }

            var queryResults =
                from search in twitterCtx.Search
                where
                    search.Type == SearchType.Search &&
                    search.Query == term &&
                    search.GeoCode == geoCode &&
                    search.Locale == "en"
                select search;

            Search srch = queryResults.Single();

            var tweetResults =
                (from status in srch.Statuses
                 select new TweetModel
                 {
                     Id = status.StatusID,
                     UserName = status.User.Identifier.ScreenName,
                     Text = status.Text,
                     Source = status.Source,
                     ImageSource = status.User.ProfileImageUrl,
                     Longitude = status.Coordinates.Longitude,
                     Latitude = status.Coordinates.Latitude,
                     CreatedAt = status.CreatedAt
                 })
                .ToList();

            _tweets.Clear();
            tweetResults.ForEach(t => _tweets.TryAdd(t.Id, t));
        }

        private void UpdateTweets(object state)
        {
            lock (_updateTweetsLock)
            {
                if (!_updatingTweets)
                {
                    _updatingTweets = true;

                    PopulateLatestTweets();

                    Broadcast(_tweets.Values);

                    _updatingTweets = false;
                }
            }
        }

        private void Broadcast(IEnumerable<TweetModel> tweets)
        {
            Clients.All.refreshTweets(tweets);
        }

        public IEnumerable<TweetModel> GetAllTweets()
        {
            return _tweets.Values;
        }
    }
}