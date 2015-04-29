﻿using SpajamMadobenWebAPI.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.WindowsAzure.Storage;
using System.Configuration;
using Microsoft.WindowsAzure.Storage.Blob;

namespace SpajamMadobenWebAPI.Controllers
{
    public class ServiceController : ApiController
    {
        // GET: api/Services
        public async Task<string> GetServiceAsync()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://spajammadobenwebapi.azurewebsites.net/");
                HttpResponseMessage response = await client.GetAsync("api/CardInfoes");
                if (response.IsSuccessStatusCode)
                {
                    var products = await response.Content.ReadAsAsync<List<Service>>();
                    return string.Empty;
                }
                else
                {
                    return null;
                }
            }
        }

        // GET: api/Services/5
        [ResponseType(typeof(CardInfo))]
        public async Task<string> GetServiceAsync(string id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://spajammadobenwebapi.azurewebsites.net/");
                HttpResponseMessage response = await client.GetAsync("api/CardInfoes");

                if (response.IsSuccessStatusCode)
                {
                    String text = await response.Content.ReadAsStringAsync();
                    return text;
                }
                else
                {
                    return null;
                }
            }

        }

        public HttpResponseMessage GetAudio(String fileName) 
        {
            // アカウントを取得
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                @"DefaultEndpointsProtocol=https;AccountName=spajammadobenstrage;AccountKey=007q7do8gs4w3BFp3vWIGLO7XXqJKquhKaqZ9vWuAUZzawL/teMWwyNgCgTLf5X9oGVZVVpu0VXe/WbN19wgvQ==");

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            
            // コンテナを作成
            CloudBlobContainer container = blobClient.GetContainerReference("audios");

            container.CreateIfNotExists();

            // Blobを作成
            CloudBlockBlob blockBlob = container.GetBlockBlobReference("test" + fileName + ".flac");

            // Blobにアップロードする
            var filePath = HttpContext.Current.Server.MapPath("~/App_Data/" + fileName + ".flac");

            using (var fileStream = System.IO.File.OpenRead(filePath))
            {
                blockBlob.UploadFromStream(fileStream);
            } 

            return null;
        }


        // GET: api/GoogleSpeechTexts/flacName
        public string GetGoogleSpeechText(String flacName)
        {
            var key = "AIzaSyBlwhF2pGCf472kxOMCGk1-4ODWtInjjGk";
            var url = "https://www.google.com/speech-api/v2/recognize?output=json&lang=ja-jp&key=";
            var postUrl = url + key;
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(postUrl);

            request.Method = "POST";
            var filePath = HttpContext.Current.Server.MapPath("~/App_Data/" + flacName + ".flac");
            byte[] byteArray = File.ReadAllBytes(filePath);
            var sampleRate = 16000;
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
        }
    }
}
