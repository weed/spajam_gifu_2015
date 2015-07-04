﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Mvc;
using SpajamHonsen.Models;
using SpajamHonsen.Models.JsonResponse;
using SpajamHonsen.Utilities;

namespace SpajamHonsen.Controllers
{
    public class TweetDetailController : ApiController
    {
        private spajamhonsenEntities db = new spajamhonsenEntities();

        // GET: api/GetTweetDetail/5
        [ResponseType(typeof(void))]
        public async Task<List<HcvInformation>> GetTweetDetail(string id, string lan)
        {
            var results = new List<HcvInformation>();
            foreach (var model in db.HVCLog.Where(item => item.SpotID == id && item.TweetID != null))
            {
                var result = new HcvInformation()
                {
                    Age = model.Age,
                    Sex = model.Sex,
                    Expression = model.Expression,
                    SpotID = model.SpotID,
                    CreateDateTime = model.CreateDateTime,
                    Language = model.Language,
                    LogID = model.LogID,
                    TweetID = model.TweetID
                };

                var tweet = db.Tweet.First(item => item.SpotID == model.SpotID && item.TweetID == model.TweetID);
                if (tweet != null)
                {
                    if (lan != "ja")
                    {
                        result.TweetText = await BingUtil.RequestMicrosoftTranslatorAPIAsync(tweet.TweetText, "ja", lan);
                        result.TweetURL = tweet.TweetURL;
                    }
                    else
                    {
                        result.TweetText = tweet.TweetText;
                        result.TweetURL = tweet.TweetURL;
                    }
                }
                results.Add(result);
            }
            return results;
        }
    }
}