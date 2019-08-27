using Movies.Client.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Movies.Client.Services
{
    public class CRUDService : IIntegrationService
    {
        private static HttpClient _httpClient = new HttpClient();

        public CRUDService()
        {
            _httpClient.BaseAddress = new Uri("http://localhost:57863");
            _httpClient.Timeout = new TimeSpan(0, 0, 30);
            _httpClient.DefaultRequestHeaders.Clear();
            //_httpClient.DefaultRequestHeaders.Accept.Add(
            //    new MediaTypeWithQualityHeaderValue("application/json"));
            //_httpClient.DefaultRequestHeaders.Accept.Add(
            //   new MediaTypeWithQualityHeaderValue("application/xml", 0.9));
        }
        public async Task Run()
        {
            //await GetResource();
            //await GetResourceThroughHttpRequestMessage();
            //await CreateResource();
            //await UpdateResource();
            await DeleteResource();
        }


        public async Task GetResource()
        {
            var response = await _httpClient.GetAsync("api/movies");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var movies = new List<Movie>();

            if (response.Content.Headers.ContentType.MediaType == "application/json")
            {
                movies = JsonConvert.DeserializeObject<List<Movie>>(content);
            }

            if (response.Content.Headers.ContentType.MediaType == "application/xml")
            {
                var serializer = new XmlSerializer(typeof(List<Movie>));
                movies = (List<Movie>)serializer.Deserialize(new StringReader(content));
            }
        }

        public async Task GetResourceThroughHttpRequestMessage()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "api/movies");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var response = await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var movies = JsonConvert.DeserializeObject<List<Movie>>(content);
        }

        public async Task CreateResource()
        {
            var movieToCreate = new MovieForCreation()
            {
                Title = "Reservoir Dogs",
                Description = "After a simple jewelry heist goes terribly wrong, the " +
                "surviving criminals begin to suspect that one of them is a police informant.",
                DirectorId = Guid.Parse("d28888e9-2ba9-473a-a40f-e38cb54f9b35"),
                ReleaseDate = new DateTimeOffset(new DateTime(1992, 9, 2)),
                Genre = "Crime, Drama"
            };

            var serializedMovieToCreate = JsonConvert.SerializeObject(movieToCreate);
            var request = new HttpRequestMessage(HttpMethod.Post, "api/movies");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Content = new StringContent(serializedMovieToCreate);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var createdMovie = JsonConvert.DeserializeObject<Movie>(content);
        }

        private async Task UpdateResource()
        {
            var movieToUpdate = new MovieForUpdate()
            {
                Title = "Pulp Fiction",
                Description = "The movie with Zed.",
                DirectorId = Guid.Parse("d28888e9-2ba9-473a-a40f-e38cb54f9b35"),
                ReleaseDate = new DateTimeOffset(new DateTime(1992, 9, 2)),
                Genre = "Crime, Drama"
            };

            var serializedMovieToUpdate = JsonConvert.SerializeObject(movieToUpdate);

            var request = new HttpRequestMessage(HttpMethod.Put,
                "api/movies/5b1c2b4d-48c7-402a-80c3-cc796ad49c6b");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Content = new StringContent(serializedMovieToUpdate);
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var updatedMovie = JsonConvert.DeserializeObject<Movie>(content);
        }

        private async Task DeleteResource()
        {
            var request = new HttpRequestMessage(HttpMethod.Delete,
                "api/movies/5b1c2b4d-48c7-402a-80c3-cc796ad49c6b");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
        }
    }
}
