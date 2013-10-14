using System;
using System.Collections.Generic;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;


namespace Sandpit.SignalR.SelfHost
{
    [HubName("twitterMap")]
    public class TwitterHub : Hub
    {
        private readonly TwitterBroadcast _twitter;


        public TwitterHub() : this(TwitterBroadcast.Instance) { }

        public TwitterHub(TwitterBroadcast twitter)
        {
            _twitter = twitter;
        }


        public IEnumerable<TweetModel> GetAllTweets()
        {
            return _twitter.GetAllTweets();
        }

        public void SetSearchTerm(String searchTerm)
        {
            _twitter.SearchTerm = searchTerm;
        }

        public String GetSearchTerm()
        {
            return _twitter.SearchTerm;
        }
    }
}
