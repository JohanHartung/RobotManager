using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static RobotManager.Classes.Helpers;

namespace RobotManager.Classes
{
    public class Nao : INotifyPropertyChanged
    {
        int id;
        private string name = String.Empty;
        private string headID = String.Empty;
        private string bodyID = String.Empty;
        private int warrantyExtension = 0;
        private DateTime purchased;
        private Status status = Status.Free;

        [JsonPropertyName("id")]
        public int Id { get => id; set => id = value; }

        [JsonPropertyName("name")]
        public string Name { get => name; set => name = value; }
        public string Ip { get => $"10.0.4.{name}"; }

        [JsonPropertyName("headID")]
        public string HeadID { get => headID; set => headID = value; }

        [JsonPropertyName("bodyID")]
        public string BodyID { get => bodyID; set => bodyID = value; }

        [JsonPropertyName("warrantyExtension")]
        public int WarrantyExtension { get => warrantyExtension; set => warrantyExtension = value; }

        [JsonPropertyName("purchased")]
        public DateTime Purchased { get => purchased; set => purchased = value; }
        public DateTime Warranty { get => purchased.AddYears(2 + WarrantyExtension); }

        [JsonPropertyName("status")]
        public Status Status
        {
            get => status;
            set
            {
                status = value;
                OnPropertyChanged(nameof(Status));
            }
        }
        public string StatusText { get => status.ToString(); }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public async Task<bool> Post()
        {
            Nao nao = this;
            using HttpClient client = new();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            string baseUri = Preferences.Get("uri", "https://example.com/api/RobotManager/");
            string apiUri = baseUri + "CreateEdit/nao";

            User user = new();
            var dateTime = DateTime.UtcNow;
            var userSecret = GenerateUserSecret(dateTime.ToString());

            var request = new CreateEditNaoRequest
            {
                Nao = nao,
                UserId = user.Id,
                DeviceId = user.DeviceId,
                DateTime = dateTime.ToString(),
                UserSecret = userSecret
            };

            var content = JsonContent.Create(request);
            // Serialize JSON manually and log it
            string jsonBody = JsonSerializer.Serialize(request);
            Console.WriteLine("Sending JSON: " + jsonBody);

            try
            {
                HttpResponseMessage response = await client.PostAsync(apiUri, content);
                var result = await response.Content.ReadFromJsonAsync<CreateNaoResponse>();
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }

        }

        public async Task<bool> InitializeFromCloud(int naoID)
        {
            id = naoID;
            return await Get();
        }

        public async Task<bool> Get()
        {
            using HttpClient client = new();
            string baseUri = Preferences.Get("uri", "https://example.com/api/RobotManager/");
            string apiUri = baseUri + $"GetSingle/nao/{id}";
            try
            {
                HttpResponseMessage response = await client.GetAsync(apiUri).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();

                Nao? nao = await response.Content.ReadFromJsonAsync<Nao>();
                

                if (nao != null)
                { 
                    this.name = nao.name;
                    this.headID = nao.headID;
                    this.bodyID = nao.bodyID;
                    this.warrantyExtension = nao.warrantyExtension;
                    this.purchased = nao.purchased;
                    this.status = nao.status;
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> Delete()
        {
            using HttpClient client = new();
            string baseUri = Preferences.Get("uri", "https://example.com/api/RobotManager/");
            string apiUri = baseUri + $"Delete/nao/{id}";
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

        public async Task<bool> SetStatus(Status status)
        {
            using HttpClient client = new();
            string baseUri = Preferences.Get("uri", "https://example.com/api/RobotManager/");
            string apiUri = baseUri + $"SetStatus/{id}/{(int)status}";
            try
            {
                HttpResponseMessage response = await client.PostAsync(apiUri, new StringContent(""));
                response.EnsureSuccessStatusCode();
                this.status = status;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

    }

    public class Issue
    {
        int id;
        int nao;

        private string title = String.Empty;
        private string description = String.Empty;
        private int author;
        private DateTime date = new();
        private Dictionary<int, DateTime> replicated = new(); // (int user, DateTime dateTime)
        private (int user, DateTime dateTime)? solved = new();
        private string solvedReport = String.Empty;

        [JsonPropertyName("id")]
        public int Id { get => id; set => id = value; }

        [JsonPropertyName("nao")]
        public int Nao { get => nao; set => nao = value; }

        [JsonPropertyName("title")]
        public string Title { get => title; set => title = value; }

        [JsonPropertyName("description")]
        public string Description { get => description; set => description = value; }

        [JsonPropertyName("author")]
        public int Author { get => author; set => author = value; }

        [JsonPropertyName("date")]
        public DateTime Date { get => date; set => date = value; }

        [JsonPropertyName("replicated")]
        public Dictionary<int, DateTime> Replicated { get => replicated; set => replicated = value ?? new(); }
        public bool IsReplicated { get => replicated.Count > 0; }

        [JsonPropertyName("solved")]
        public (int user, DateTime dateTime)? Solved { get => solved; set => solved = value; }
        public bool IsSolved { get => solved != default; }

        [JsonPropertyName("solvedReport")]
        public string SolvedReport { get => solvedReport; set => solvedReport = value; }

        public async Task<bool> InitializeFromCloud(int issueID)
        {
            id = issueID;
            return await Get();
        }

        public async Task<bool> Post()
        {
            var dateTime = DateTime.UtcNow;
            var userSecret = GenerateUserSecret(dateTime.ToString());
            var userId = Preferences.Get("UserId", -1);
            var deviceId = Preferences.Get("DeviceId", null);

            if (userId == -1 || deviceId == null) { return false; }
            Issue issue = this;

            var request = new CreateEditIssueRequest
            {
                Issue = issue,
                UserId = userId,
                DeviceId = deviceId,
                DateTime = dateTime.ToString(),
                UserSecret = userSecret
            };

            using HttpClient client = new();
            string baseUri = Preferences.Get("uri", "https://example.com/api/RobotManager/");
            string apiUri = baseUri + "CreateEdit/issue";
            try
            {
                HttpResponseMessage response = await client.PostAsJsonAsync(apiUri, request);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> Get()
        {
            using HttpClient client = new();
            string baseUri = Preferences.Get("uri", "https://example.com/api/RobotManager/");
            string apiUri = baseUri + $"GetSingle/issue/{id}";
            Issue issue = await client.GetFromJsonAsync<Issue>(apiUri);
            if (issue != null)
            {
                this.nao = issue.nao;
                this.title = issue.title;
                this.description = issue.description;
                this.author = issue.author;
                this.date = issue.date;
                this.replicated = issue.replicated;
                this.solved = issue.solved;
                this.solvedReport = issue.solvedReport;
                return true;
            }
            return false;
        }

        public async Task<bool> Delete()
        {
            using HttpClient client = new();
            string baseUri = Preferences.Get("uri", "https://example.com/api/RobotManager/");
            string apiUri = baseUri + $"Delete/issue/{id}";
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

        public async Task<bool> ReplicateIssue()
        {
            var userId = Preferences.Get("UserId", -1);
            var dateTime = DateTime.Now;
            if (userId == -1) { return false; }

            using HttpClient client = new();
            string baseUri = Preferences.Get("uri", "https://example.com/api/RobotManager/");
            string apiUri = baseUri + $"ReplicateIssue/{this.Id}/{dateTime}/{userId}";
            try
            {
                HttpResponseMessage response = await client.PostAsync(apiUri, new StringContent(""));
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> SolveIssue()
        {
            var userId = Preferences.Get("UserId", -1);
            var dateTime = DateTime.Now;
            if (userId == -1) { return false; }

            var request = new SolveIssueRequest
            {
                Id = this.Id,
                UserId = userId,
                DateTime = dateTime.ToString(),
                Report = this.SolvedReport
            };
            using HttpClient client = new();
            string baseUri = Preferences.Get("uri", "https://example.com/api/RobotManager/");
            string apiUri = baseUri + "SolveIssue";
            try
            {
                HttpResponseMessage response = await client.PostAsJsonAsync(apiUri, request);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }

    public class Note
    {
        int id;
        int nao;

        private string title = String.Empty;
        private string description = String.Empty;
        private int author;
        private User authorUser = new();
        private DateTime date;

        [JsonPropertyName("id")]
        public int Id { get => id; set => id = value; }

        [JsonPropertyName("nao")]
        public int Nao { get => nao; set => nao = value; }

        [JsonPropertyName("title")]
        public string Title { get => title; set => title = value; }

        [JsonPropertyName("description")]
        public string Description { get => description; set => description = value; }

        [JsonPropertyName("author")]
        public int Author { get => author; set => author = value; }

        public User AuthorUser { get => authorUser; set => authorUser = value; }
        public string? AuthorName { get => authorUser.Name; }

        [JsonPropertyName("date")]
        public DateTime Date { get => date; set => date = value; }

        public async Task<bool> InitializeFromCloud(int noteID)
        {
            id = noteID;
            return await Get();
        }

        public async Task<bool> Post()
        {
            var dateTime = DateTime.UtcNow;
            var userSecret = GenerateUserSecret(dateTime.ToString());
            var userId = Preferences.Get("UserId", -1);
            var deviceId = Preferences.Get("DeviceId", null);

            if (userId == -1 || deviceId == null) { return false; }

            Note note = this;

            var request = new CreateEditNoteRequest
            {
                Note = note,
                UserId = userId,
                DeviceId = deviceId,
                DateTime = dateTime.ToString(),
                UserSecret = userSecret
            };

            using HttpClient client = new();
            string baseUri = Preferences.Get("uri", "https://example.com/api/RobotManager/");
            string apiUri = baseUri + "CreateEdit/note";
            try
            {
                HttpResponseMessage response = await client.PostAsJsonAsync(apiUri, request);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> Get()
        {
            using HttpClient client = new();

            string baseUri = Preferences.Get("uri", "https://example.com/api/RobotManager/");
            string apiUri = baseUri + $"GetSingle/note/{id}";
            try
            {
                Note note = await client.GetFromJsonAsync<Note>(apiUri);
                if (note != null)
                {
                    this.nao = note.nao;
                    this.title = note.title;
                    this.description = note.description;
                    this.author = note.author;
                    this.date = note.date;
                    await this.authorUser.GetUser(note.author);

                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> Delete()
        {
            using HttpClient client = new();
            string baseUri = Preferences.Get("uri", "https://example.com/api/RobotManager/");
            string apiUri = baseUri + $"Delete/note/{id}";
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
    }
    public class ClinicVisit
    {
        int id;
        int nao;

        private DateTime date;
        private DateTime backDate;
        private List<int> issues = new();
        private string notes = String.Empty;
        private string backReport = String.Empty;
        private int author;
        private int collector;

        [JsonPropertyName("id")]
        public int Id { get => id; set => id = value; }

        [JsonPropertyName("nao")]
        public int Nao { get => nao; set => nao = value; }

        [JsonPropertyName("date")]
        public DateTime Date { get => date; set => date = value; }

        [JsonPropertyName("backDate")]
        public DateTime BackDate { get => backDate; set => backDate = value; }

        [JsonPropertyName("issues")]
        public List<int> Issues { get => issues; set => issues = value; }

        public bool IsBack { get => BackReport != String.Empty; }

        [JsonPropertyName("notes")]
        public string Notes { get => notes; set => notes = value; }

        [JsonPropertyName("backReport")]
        public string BackReport { get => backReport; set => backReport = value; }

        [JsonPropertyName("author")]
        public int Author { get => author; set => author = value; }
        [JsonPropertyName("collector")]
        public int Collector { get => collector; set => collector = value; }

        public string DisplayID { get => id.ToString().PadLeft(4, '0'); }
        public int IssueCount { get => issues.Count; }

        public async Task<bool> InitializeFromCloud(int clinicVisitID)
        {
            id = clinicVisitID;
            return await Get();
        }

        public async Task<bool> Post()
        {
            var dateTime = DateTime.UtcNow;
            var userSecret = GenerateUserSecret(dateTime.ToString());
            var userId = Preferences.Get("UserId", -1);
            var deviceId = Preferences.Get("DeviceId", null);

            if (userId == -1 || deviceId == null) { return false; }

            ClinicVisit visit = this;

            var request = new CreateEditClinicVisitRequest
            {
                ClinicVisit = visit,
                UserId = userId,
                DeviceId = deviceId,
                DateTime = dateTime.ToString(),
                UserSecret = userSecret
            };
            using HttpClient client = new();
            string baseUri = Preferences.Get("uri", "https://example.com/api/RobotManager/");
            string apiUri = baseUri + "CreateEdit/clinicVisit";
            try
            {
                HttpResponseMessage response = await client.PostAsJsonAsync(apiUri, request);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> Get()
        {
            using HttpClient client = new();
            string baseUri = Preferences.Get("uri", "https://example.com/api/RobotManager/");
            string apiUri = baseUri + $"GetSingle/clinicVisit/{id}";
            try
            {
                ClinicVisit visit = await client.GetFromJsonAsync<ClinicVisit>(apiUri);
                if (visit != null)
                {
                    this.nao = visit.nao;
                    this.date = visit.date;
                    this.issues = visit.issues;
                    this.notes = visit.notes;
                    this.backReport = visit.backReport;
                    this.author = visit.author;
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> Delete()
        {
            using HttpClient client = new();
            string baseUri = Preferences.Get("uri", "https://example.com/api/RobotManager/");
            string apiUri = baseUri + $"Delete/clinicVisit/{id}";
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

        public async Task<bool> EndClinicVisit()
        {
            var userId = Preferences.Get("UserId", -1);
            var dateTime = DateTime.Now;
            if (userId == -1) { return false; }

            var request = new EndClinicVisitRequest
            {
                Id = this.Id,
                UserId = userId,
                BackDate = dateTime.ToString(),
                BackReport = this.BackReport
            };
            using HttpClient client = new();
            string baseUri = Preferences.Get("uri", "https://example.com/api/RobotManager/");
            string apiUri = baseUri + "EndClinicVisit";
            try
            {
                HttpResponseMessage response = await client.PostAsJsonAsync(apiUri, request);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

    }

    public enum Status
    {
        Free,
        Game,
        Clinic
    }
    public class CreateEditNaoRequest
    {
        public Nao Nao { get; set; }

        // Nao Data
        //public int Id { get; set; }
        //public string Name { get; set; }
        //public string HeadID { get; set; }
        //public string BodyID { get; set; }
        //public int WarrantyExtension { get; set; }
        //public string Purchased { get; set; }
        //public List<int> Issues { get; set; }
        //public List<int> Notes { get; set; }
        //public List<int> ClinicVisits { get; set; }
        //public int Status { get; set; }

        // User Data for Authentication
        public int UserId { get; set; }
        public string DeviceId { get; set; }
        public string DateTime { get; set; }
        public string UserSecret { get; set; }
    }

    public class CreateNaoResponse
    {
        public int id { get; set; }
        public string name { get; set; }
        public string headID { get; set; }
        public string bodyID { get; set; }
        public int warrantyExtension { get; set; }
        public string purchased { get; set; }
        //public List<int> issues { get; set; }
        //public List<int> notes { get; set; }
        //public List<int> clinicVisits { get; set; }
        public int status { get; set; }
    }

    public class CreateEditIssueRequest
    {
        public Issue Issue { get; set; }
        public int UserId { get; set; }
        public string DeviceId { get; set; }
        public string DateTime { get; set; }
        public string UserSecret { get; set; }
    }

    public class CreateEditNoteRequest
    {
        public Note Note { get; set; }
        public int UserId { get; set; }
        public string DeviceId { get; set; }
        public string DateTime { get; set; }
        public string UserSecret { get; set; }
    }

    public class CreateEditClinicVisitRequest
    {
        public ClinicVisit ClinicVisit { get; set; }
        public int UserId { get; set; }
        public string DeviceId { get; set; }
        public string DateTime { get; set; }
        public string UserSecret { get; set; }
    }

    public class SolveIssueRequest
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string DateTime { get; set; }
        public string Report { get; set; }
    }

    public class EndClinicVisitRequest
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string BackDate { get; set; }
        public string BackReport { get; set; }
    }

}
