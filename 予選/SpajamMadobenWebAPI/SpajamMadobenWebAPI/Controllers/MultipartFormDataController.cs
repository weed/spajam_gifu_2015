﻿using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;

namespace SpajamMadobenWebAPI.Controllers
{
    /// <summary>
    /// multipart-formでPOSTされた音声ファイルを登録するAPI
    /// </summary>
    public class MultipartFormDataController : ApiController
    {
        // POST: api/Service
        /// <summary>
        /// multipart-formでPOSTされた音声ファイルを登録する
        /// </summary>
        /// <returns></returns>
        public async Task<string> PostMultipartAudioAsync()
        {
            // リクエスト内容を取得
            if (Request.Content.IsMimeMultipartContent() == false)
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            var provider = await Request.Content.ReadAsMultipartAsync();
            var fileContent = provider.Contents.First(x => x.Headers.ContentDisposition.Name == JsonConvert.SerializeObject("buffer"));

            // バイト配列に戻す
            byte[] byteArray = await fileContent.ReadAsByteArrayAsync();
            
            // AzureBLOBStrageに保存
            var fileName = Guid.NewGuid().ToString();

            // アカウントを取得
            var appSettings = ConfigurationManager.AppSettings;
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(appSettings["CloudStorageAccount"]);

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // コンテナを作成
            CloudBlobContainer container = blobClient.GetContainerReference("audios");

            container.CreateIfNotExists();

            // Blobを作成
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName + ".flac");

            // byte配列をMemoryStreamに変換
            using (MemoryStream ms = new MemoryStream(byteArray, 0, byteArray.Length))
            {
                // Blobにアップロードする
                await blockBlob.UploadFromStreamAsync(ms);
            }

            // GoogleSpeechAPIに送信
            /*
            var httpClient = new HttpClient();

            // POST 送信先の Uri
            var key = "AIzaSyBlwhF2pGCf472kxOMCGk1-4ODWtInjjGk";
            var urlString = "https://www.google.com/speech-api/v2/recognize?output=json&lang=ja-jp&key=";
            var uri = new Uri(urlString + key);

            MultipartFormDataContent multiContent = new MultipartFormDataContent();
            multiContent.Add(fileContent, JsonConvert.SerializeObject("buffer"));

            var result = await httpClient.PostAsync(uri, multiContent);

            return null;
            */

            var key = appSettings["GoogleSpeechAPIKey"];
            var url = "https://www.google.com/speech-api/v2/recognize?output=json&lang=ja-jp&key=";
            var postUrl = url + key;
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(postUrl);
            var sampleRate = 16000;

            request.Method = "POST";
            request.ContentType = "audio/x-flac; rate=" + sampleRate.ToString();
            request.ContentLength = byteArray.Length;

            Stream sendStream = request.GetRequestStream();
            sendStream.Write(byteArray, 0, byteArray.Length);
            sendStream.Close();

            string responseFromServer;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            var reader = new StreamReader(response.GetResponseStream());
            responseFromServer = reader.ReadToEnd();
            reader.Close();
            response.Close();
            return responseFromServer;
            // return JsonConvert.DeserializeObject<SpajamMadobenWebAPI.Models.GoogleSpeechAPIResponseModel.Rootobject>(responseFromServer);
        }

    }
}
