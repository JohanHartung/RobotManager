using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RobotManager.Classes
{
    public static class Helpers
    {
        public static string Hash(string password, string? challenge = null)
        {
            using var sha256 = SHA256.Create();

            string clearText = challenge == null ? password : Hash(password) + challenge;
            var bytes = Encoding.UTF8.GetBytes(clearText);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        public static string? GenerateUserSecret(string dateTime)
        {
            using var sha256 = SHA256.Create();
            string? token = Preferences.Get("Token", null);
            if (token == null)
            {
                return null;
            }
            return Hash(token, dateTime);
        }

        public static async Task<List<Note>?> GetAllnotes()
        {
            List<Note>? notes = new();
            using HttpClient client = new();
            string baseUri = Preferences.Get("uri", "https://example.com/api/RobotManager/");
            string apiUri = baseUri + $"GetAll/notes";

            var response = await client.GetFromJsonAsync<List<Note>>(apiUri);
            if (response != null)
            {
                notes = response;
            }


            return notes;
        }

        public static async Task<List<Note>?> GetGroupNotes(int robotId)
        {
            List<Note>? notes = new();
            using HttpClient client = new();
            string baseUri = Preferences.Get("uri", "https://example.com/api/RobotManager/");
            string apiUri = baseUri + $"GetGroup/Note/{robotId}";

            var response = await client.GetFromJsonAsync<List<Note>>(apiUri);
            if (response != null)
            {
                notes = response;
            }

            return notes;
        }

        public static async Task<List<Issue>?> GetAllIssues()
        {
            List<Issue>? issues = new();
            using HttpClient client = new();
            string baseUri = Preferences.Get("uri", "https://example.com/api/RobotManager/");
            string apiUri = baseUri + $"GetAll/issues";

            var response = await client.GetFromJsonAsync<List<Issue>>(apiUri);
            if (response != null)
            {
                issues = response;
            }

            return issues;
        }

        public static async Task<List<Issue>?> GetGroupIssues(int robotId)
        {
            List<Issue>? issues = new();
            using HttpClient client = new();
            string baseUri = Preferences.Get("uri", "https://example.com/api/RobotManager/");
            string apiUri = baseUri + $"GetGroup/issue/{robotId}";
            try
            {

                var response = await client.GetFromJsonAsync<List<Issue>>(apiUri);
                if (response != null)
                {
                    foreach (var issue in response)
                    {
                        if (issue.Solved?.user == 0 && issue.Solved?.dateTime == DateTime.MinValue)
                        {
                            issue.Solved = null;
                        }
                    }
                    issues = response;
                }
            }
            catch (Exception ex)
            {
                issues = new List<Issue>();
            }
            return issues;
        }

        public static async Task<List<ClinicVisit>?> GetAllClinicVisits()
        {
            List<ClinicVisit>? clinicVisits = new();
            using HttpClient client = new();
            string baseUri = Preferences.Get("uri", "https://example.com/api/RobotManager/");
            string apiUri = baseUri + $"GetAll/clinicVisits";

            var response = await client.GetFromJsonAsync<List<ClinicVisit>>(apiUri);
            if (response != null)
            {
                clinicVisits = response;
            }


            return clinicVisits;
        }

        public static async Task<List<ClinicVisit>?> GetGroupClinicVisits(int robotId)
        {
            List<ClinicVisit>? clinicVisits = new();
            using HttpClient client = new();
            string baseUri = Preferences.Get("uri", "https://example.com/api/RobotManager/");
            string apiUri = baseUri + $"GetGroup/clinicVisit/{robotId}";

            var response = await client.GetFromJsonAsync<List<ClinicVisit>>(apiUri);
            if (response != null)
            {
                clinicVisits = response;
            }

            return clinicVisits;
        }

        public static async Task<List<Game>?> GetAllGames()
        {
            List<Game>? games = new();
            using HttpClient client = new();
            string baseUri = Preferences.Get("uri", "https://example.com/api/RobotManager/");
            string apiUri = baseUri + $"GetAll/games";

            var response = await client.GetFromJsonAsync<List<Game>>(apiUri);
            if (response != null)
            {
                games = response;
            }


            return games;
        }



    }
}
