using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YelpSharp;
using MTLSpotScraper.Helper;
using MTLSpotScraper.Repos;
using MTLSpotScraper.Interfaces;

namespace MTLSpotScraper
{
    class YelpConfig
    {

        private static Options _options;

        public static Options Options
        {
            get
            {
                if (_options == null)
                {

                    MongoHelper.SetInfos("MTLSpot");
                    _options = new Options()
                    {
                        AccessToken = Environment.GetEnvironmentVariable(MongoHelper.Infos.CLientId, EnvironmentVariableTarget.Machine),
                        AccessTokenSecret = Environment.GetEnvironmentVariable("YELP_ACCESS_TOKEN_SECRET", EnvironmentVariableTarget.Machine),
                        ConsumerKey = Environment.GetEnvironmentVariable("YELP_CONSUMER_KEY", EnvironmentVariableTarget.Machine),
                        ConsumerSecret = Environment.GetEnvironmentVariable("YELP_CONSUMER_SECRET", EnvironmentVariableTarget.Machine)
                    };

                    if (String.IsNullOrEmpty(_options.AccessToken) ||
                        String.IsNullOrEmpty(_options.AccessTokenSecret) ||
                        String.IsNullOrEmpty(_options.ConsumerKey) ||
                        String.IsNullOrEmpty(_options.ConsumerSecret))
                    {
                        throw new InvalidOperationException("No OAuth info available.  Please modify Config.cs to add your YELP API OAuth keys");
                    }
                }
                return _options;
            }
        }

    }
}
