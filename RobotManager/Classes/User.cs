using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RobotManager.Classes
{
    public class User
    {
        [JsonPropertyName("userId")]
        public int Id { get => Preferences.Get("UserId", -1); set => Preferences.Set("UserId", value); }

        [JsonPropertyName("userName")]
        public string? Name { get => Preferences.Get("UserName", null); set => Preferences.Set("UserName", value); }

        [JsonPropertyName("deviceId")]
        public string? DeviceId { get => Preferences.Get("DeviceId", null); set => Preferences.Set("DeviceId", value); }

        [JsonPropertyName("token")]
        public string? Token { get => Preferences.Get("Token", null); set => Preferences.Set("Token", value); }

        public void Initialize((int userId, string deviceId, string token)? userdata)
        {
            if (userdata == null) { return; }
            this.Id = userdata.Value.userId;
            this.DeviceId = userdata.Value.deviceId;
            this.Token = userdata.Value.token;
        }

        public bool Exists()
        {
            return Id != -1 && Name != null && DeviceId != null && Token != null;
        }

        public async Task<(int userId, string deviceId, string token)> CreateUser(string username, string password)
        {
            using HttpClient client = new();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            string baseUri = Preferences.Get("uri", "https://example.com/api/RobotManager/");
            string apiUri = baseUri + "Create/user";
            var content = JsonContent.Create(new { username, password });

            try
            {
                HttpResponseMessage response = await client.PostAsync(apiUri, content);
                response.EnsureSuccessStatusCode();
                string responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Response Content: {responseContent}");
                var result = await response.Content.ReadFromJsonAsync<CreateUserResponse>();
                return result != null ? (result.UserId, result.DeviceId, result.Token) : (-1, "", "");
            }
            catch
            {
                return (-1, "", "");
            }
        }

        public async Task<bool> EditUser(int id, string username, string token, string? password = null)
        {
            using HttpClient client = new();
            string baseUri = Preferences.Get("uri", "https://example.com/api/RobotManager/");
            string apiUri = baseUri + $"Edit/user/{id}";
            var content = new { username, token, password };
            try
            {
                HttpResponseMessage response = await client.PostAsJsonAsync(apiUri, content);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<User?> GetUser(int id)
        {
            using HttpClient client = new();
            string baseUri = Preferences.Get("uri", "https://example.com/api/RobotManager/");
            string apiUri = baseUri + $"GetSingle/user/{id}";
            try
            {
                HttpResponseMessage response = await client.GetAsync(apiUri);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<User>();
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> DeleteUser(int id)
        {
            using HttpClient client = new();
            string baseUri = Preferences.Get("uri", "https://example.com/api/RobotManager/");
            string apiUri = baseUri + $"Delete/user/{id}";
            try
            {
                HttpResponseMessage response = await client.DeleteAsync(apiUri);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool?> ValidUsername(string username)
        {
            using HttpClient client = new();
            string baseUri = Preferences.Get("uri", "https://example.com/api/RobotManager/");
            string apiUri = baseUri + $"ValidUsername/{username}";
            try
            {
                HttpResponseMessage response = await client.GetAsync(apiUri);
                string responseContent = await response.Content.ReadAsStringAsync();
                if (responseContent == "{\"statusCode\":200}") 
                { 
                    return !string.IsNullOrEmpty(responseContent); 
                }
                else if(responseContent == "{\"statusCode\":204}")
                {
                    return false;
                }
                else { return null; }
            }
            catch
            {
                return null;
            }
        }

        public async Task<(int processId, string secret)?> LoginChallenge(string username)
        {
            using HttpClient client = new();
            string baseUri = Preferences.Get("uri", "https://example.com/api/RobotManager/");
            string apiUri = baseUri + $"LoginChallenge/{username}";
            try
            {
                HttpResponseMessage response = await client.GetAsync(apiUri);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<dynamic>();
                    return (result.processId, result.secret);
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<(int userId, string deviceId, string token)?> LoginResponse(int processId, string response)
        {
            using HttpClient client = new();
            string baseUri = Preferences.Get("uri", "https://example.com/api/RobotManager/");
            string apiUri = baseUri + $"LoginResponse/{processId},{response}";
            try
            {
                HttpResponseMessage responseMessage = await client.GetAsync(apiUri);
                if (responseMessage.IsSuccessStatusCode)
                {
                    var result = await responseMessage.Content.ReadFromJsonAsync<dynamic>();
                    return (result.userId, result.deviceId, result.token);
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> ValidRegistrationCode(string code)
        {
            using HttpClient client = new();
            string baseUri = Preferences.Get("uri", "https://example.com/api/RobotManager/");
            string apiUri = baseUri + $"ValidRegistrationCode/{code}";
            try
            {
                HttpResponseMessage response = await client.GetAsync(apiUri);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync() == "{\"statusCode\":200}";
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
    }

    public class CreateUserResponse
    {
        [JsonPropertyName("id")]
        public int UserId { get; set; }

        [JsonPropertyName("deviceId")]
        public string DeviceId { get; set; }

        [JsonPropertyName("token")]
        public string Token { get; set; }
    }
}
