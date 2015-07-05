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
                    TweetID = model.TweetID,
                    IconURL = TweetDetailController.GetAconUrl(lan, model.Sex, model.Age, model.Expression)
                };

                var tweet = db.Tweet.First(item => item.SpotID == model.SpotID && item.TweetID == model.TweetID);
                if (tweet != null)
                {
                    if (lan != "jp")
                    {
                        result.TweetText = await BingUtil.RequestMicrosoftTranslatorAPIAsync(tweet.TweetText, "jp", lan);
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

        public static string GetAconUrl(string lan, string sex, int age, int expression)
        {
            var result = "https://spajamhonsenstorage.blob.core.windows.net/faceicons/";
            if (lan == "cn")
            {
                result = "cn_";
            } 
            else if (lan == "en")
            {
                result = "cn_";
            }
            else
            {
                result = "ja_";
            }

            if (sex == "f")
            {
                result += "f_";
            }
            else
            {
                result += "m_";
            }

            if (age > 0 && age <= 15)
            {
                result += "00-15_";
            } 
            else if (age > 16 && age <= 19) 
            {
                result += "16-19_";
            }
            else if (age > 20 && age <= 29)
            {
                result += "20-29_";
            }
            else if (age > 30 && age <= 45)
            {
                result += "30-45_";
            }
            else 
            {
                result += "46-60_";
            }
            
            result += expression + ".png";
            return result;
        }
    }
}