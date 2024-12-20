using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http.Json;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
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
        private List<int> issues = new();
        private List<int> notes = new();
        private List<int> clinicVisits = new();
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

        [JsonPropertyName("issues")]
        public List<int> Issues { get => issues; set => issues = value; }

        [JsonPropertyName("notes")]
        public List<int> Notes { get => notes; set => notes = value; }

        [JsonPropertyName("clinicVisits")]
        public List<int> ClinicVisits { get => clinicVisits; set => clinicVisits = value; }
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
            string baseUri = Preferences.Get("uri", "https://example.com/api/RobotManager/");
            string apiUri = baseUri + "CreateEdit/nao";
            try
            {
                HttpResponseMessage response = await client.PostAsJsonAsync(apiUri, nao);
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
            string apiUri = baseUri + $"GetSingle/nao/{id}";
            try
            {
                Nao nao = await client.GetFromJsonAsync<Nao>(apiUri);

                if (nao != null)
                {
                    this.name = nao.name;
                    this.headID = nao.headID;
                    this.bodyID = nao.bodyID;
                    this.warrantyExtension = nao.warrantyExtension;
                    this.purchased = nao.purchased;
                    this.issues = nao.issues;
                    this.notes = nao.notes;
                    this.clinicVisits = nao.clinicVisits;
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

    }

    public class Issue
    {
        int id;
        int nao;

        private string title = String.Empty;
        private string description = String.Empty;
        private string author = String.Empty;
        private DateTime date = new();
        private Dictionary<int, DateTime> replicated = new(); // (int user, DateTime dateTime)?
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
        public string Author { get => author; set => author = value; }

        [JsonPropertyName("date")]
        public DateTime Date { get => date; set => date = value; }

        [JsonPropertyName("replicated")]
        public Dictionary<int, DateTime> Replicated { get => replicated; set => replicated = value; }
        public bool IsReplicated { get => replicated != null; }

        [JsonPropertyName("solved")]
        public (int user, DateTime dateTime)? Solved { get => solved; set => solved = value; }
        public bool IsSolved { get => solved != null; }

        [JsonPropertyName("solvedReport")]
        public string SolvedReport { get => solvedReport; set => solvedReport = value; }

        public async Task<bool> Post()
        {
            Issue issue = this;
            using HttpClient client = new();
            string baseUri = Preferences.Get("uri", "https://example.com/api/RobotManager/");
            string apiUri = baseUri + "CreateEdit/issue";
            try
            {
                HttpResponseMessage response = await client.PostAsJsonAsync(apiUri, issue);
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
    }

    public class Note
    {
        int id;
        int nao;

        private string title = String.Empty;
        private string description = String.Empty;
        private string author = String.Empty;
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
        public string Author { get => author; set => author = value; }

        [JsonPropertyName("date")]
        public DateTime Date { get => date; set => date = value; }

        public async Task<bool> Post()
        {
            DateTime dateTime = DateTime.Now;
            var userSecret = GenerateUserSecret(dateTime);
            var userId = Preferences.Get("userId", -1);
            var deviceId = Preferences.Get("deviceId", null);

            if (userId == -1 || deviceId == null) { return false; }

            Note note = this;
            
            using HttpClient client = new();
            string baseUri = Preferences.Get("uri", "https://example.com/api/RobotManager/");
            string apiUri = baseUri+"CreateEdit/note";
            try
            {
                HttpResponseMessage response = await client.PostAsJsonAsync(apiUri, note);
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
        private bool isBack;
        private string notes = String.Empty;
        private string backReport = String.Empty;
        private string author = String.Empty;

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

        [JsonPropertyName("isBack")]
        public bool IsBack { get => isBack; set => isBack = value; }

        [JsonPropertyName("notes")]
        public string Notes { get => notes; set => notes = value; }

        [JsonPropertyName("backReport")]
        public string BackReport { get => backReport; set => backReport = value; }

        [JsonPropertyName("author")]
        public string Author { get => author; set => author = value; }

        public string DisplayID { get => id.ToString().PadLeft(4, '0'); }
        public int IssueCount { get => issues.Count; }

        public async Task<bool> Post()
        {
            ClinicVisit visit = this;
            using HttpClient client = new();
            string baseUri = Preferences.Get("uri", "https://example.com/api/RobotManager/");
            string apiUri = baseUri + "CreateEdit/clinicVisit";
            try
            {
                HttpResponseMessage response = await client.PostAsJsonAsync(apiUri, visit);
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
                    this.isBack = visit.isBack;
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

        
    }

    public enum Status
    {
        Free,
        Game,
        Clinic
    }
}
