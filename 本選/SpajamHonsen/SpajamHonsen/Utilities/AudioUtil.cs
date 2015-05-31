﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SpajamHonsen.Utilities
{
    /// <summary>
    /// Spajam予選で使用した音声関連WEBAPIのユーティリティークラス
    /// </summary>
    /// <remarks>
    /// Spajam予選で使用した音声関連WEBAPIのユーティリティークラス
    /// </remarks>
    public class AudioUtil
    {
        /// <summary>
        /// VoiceTextAPIにリクエスト送信
        /// </summary>
        /// <param name="voiceText"></param>
        /// <param name="appSettings"></param>
        /// <returns>ファイルIDのstring</returns>
        public async Task<string> RequestVoiceTextAPI(string voiceText, System.Collections.Specialized.NameValueCollection appSettings)
        {
            var username = appSettings["VoiceTextAPIUser"];
            var url = "https://api.voicetext.jp/v1/tts";

            var authHeader = new AuthenticationHeaderValue("Basic", System.Convert.ToBase64String(System.Text.UTF8Encoding.UTF8.GetBytes(string.Format("{0}:{1}", username, ""))));

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = authHeader;

            var uri = new Uri(url);

            // Request設定
            var param = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "text", voiceText },
                    { "speaker", "haruka" },
                });

            var result = await client.PostAsync(uri, param);
            var stream = await result.Content.ReadAsStreamAsync();

            var fileName = Guid.NewGuid().ToString();
            var containerName = "audios";

            var azureStorageUtil = new AzureStorageUtil();
            await azureStorageUtil.UploadBlobStrage(stream, fileName, containerName);
            return fileName;
        }

        /// <summary>
        /// Google日本語入力APIにリクエスト送信
        /// </summary>
        /// <param name="kanaText"></param>
        /// <returns>ファイルIDのstring</returns>
        public async Task<string> RequestGoogleJapaneseAPI(string kanaText)
        {
            var url = "http://www.google.com/transliterate";

            var client = new HttpClient();

            var uri = new Uri(url);

            // Request設定
            var param = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "langpair", "ja-Hira|ja" },
                    { "text", kanaText },
                });

            var result = await client.PostAsync(uri, param);
            var responseStream = await result.Content.ReadAsStreamAsync();
            using (StreamReader sr = new StreamReader(responseStream, Encoding.GetEncoding("utf-8")))
            {
                var resultString = sr.ReadToEnd();
                return resultString;
            }
        }


    }
}