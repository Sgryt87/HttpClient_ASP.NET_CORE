using Movies.Client.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Client.Services
{
    public class StreamService : IIntegrationService
    {
        private static HttpClient _httpClient = new HttpClient();

        public StreamService()
        {
            _httpClient.BaseAddress = new Uri("http://localhost:57863");
            _httpClient.Timeout = new TimeSpan(0, 0, 30);
            _httpClient.DefaultRequestHeaders.Clear();
        }

        public async Task Run()
        {
            //GetPosterWithStream();
            //GetPosterWithStreamAndCompletionMode();
            await TestGetPostWithoutStream();
            await TestGetPostWithStream();
            await TestGetPostWithAndCompletionMode();
        }

        private async Task GetPosterWithStream()
        {
            var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"api/movies/d8663e5e-7494-4f81-8739-6e0de1bea7ee/posters/{Guid.NewGuid()}");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using (var response = await _httpClient.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var stream = await response.Content.ReadAsStreamAsync();
                var poster = stream.ReadAndDeserializeFromJson<Poster>();
                //using (var streamReader = new StreamReader(stream))
                //{
                //    using (var jsonTextReader = new JsonTextReader(streamReader))
                //    {
                //        var jsonSerializer = new JsonSerializer();
                //        var poster = jsonSerializer.Deserialize<Poster>(jsonTextReader);

                //        // do something with the poster
                //    }
                //}
            }
        }

        private async Task GetPosterWithStreamAndCompletionMode()
        {
            var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"api/movies/d8663e5e-7494-4f81-8739-6e0de1bea7ee/posters/{Guid.NewGuid()}");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using (var response = await _httpClient.SendAsync(request,
                HttpCompletionOption.ResponseHeadersRead))
            {
                response.EnsureSuccessStatusCode();
                var stream = await response.Content.ReadAsStreamAsync();
                var poster = stream.ReadAndDeserializeFromJson<Poster>();
                //using (var streamReader = new StreamReader(stream))
                //{
                //    using (var jsonTextReader = new JsonTextReader(streamReader))
                //    {
                //        var jsonSerializer = new JsonSerializer();
                //        var poster = jsonSerializer.Deserialize<Poster>(jsonTextReader);

                //        // do something with the poster
                //    }
                //}
            }
        }

        private async Task GetPosterWIthoutStream()
        {
            var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"api/movies/d8663e5e-7494-4f81-8739-6e0de1bea7ee/posters/{Guid.NewGuid()}");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var posters = JsonConvert.DeserializeObject<Poster>(content);
        }

        public async Task TestGetPostWithoutStream()
        {
            // warmup
            await GetPosterWIthoutStream();

            var stopWatch = Stopwatch.StartNew();

            for (int i = 0; i < 200; i++)
            {
                await GetPosterWIthoutStream();
            }

            stopWatch.Stop();
            Console.WriteLine($"Elapsed milliseconds without stream: " +
                $"{stopWatch.ElapsedMilliseconds}, " +
                $"averaging {stopWatch.ElapsedMilliseconds / 200} milliseconds/request");
        }

        public async Task TestGetPostWithStream()
        {
            // warmup
            await GetPosterWithStream();

            var stopWatch = Stopwatch.StartNew();

            for (int i = 0; i < 200; i++)
            {
                await GetPosterWithStream();
            }

            stopWatch.Stop();
            Console.WriteLine($"Elapsed milliseconds witht stream: " +
                $"{stopWatch.ElapsedMilliseconds}, " +
                $"averaging {stopWatch.ElapsedMilliseconds / 200} milliseconds/request");
        }

        public async Task TestGetPostWithAndCompletionMode()
        {
            // warmup
            await GetPosterWithStreamAndCompletionMode();

            var stopWatch = Stopwatch.StartNew();

            for (int i = 0; i < 200; i++)
            {
                await GetPosterWithStreamAndCompletionMode();
            }

            stopWatch.Stop();
            Console.WriteLine($"Elapsed milliseconds witht stream and completion: " +
                $"{stopWatch.ElapsedMilliseconds}, " +
                $"averaging {stopWatch.ElapsedMilliseconds / 200} milliseconds/request");
        }
    }
}
