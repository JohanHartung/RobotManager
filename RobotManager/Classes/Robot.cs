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
    public class Robot : INotifyPropertyChanged
    {
        int id;
        private int headNumber;
        private string model = String.Empty;
        private string bodySerial = String.Empty;
        private string headSerial = String.Empty;
        private string version = String.Empty;
        private DateTime? purchased;
        private DateTime? warrantyEnd;
        private string comment = String.Empty;
        private Status status = Status.Free;

        [JsonPropertyName("id")]
        public int Id { get => id; set => id = value; }


        [JsonPropertyName("head_number")]
        public int HeadNumber { get => headNumber; set => headNumber = value; }
        public string Ip { get => $"10.0.4.{headNumber}"; }

        [JsonPropertyName("model")]
        public string Model { get => model; set => model = value; }

        [JsonPropertyName("body_serial")]
        public string BodySerial { get => bodySerial; set => bodySerial = value; }

        [JsonPropertyName("head_serial")]
        public string HeadSerial { get => headSerial; set => headSerial = value; }

        [JsonPropertyName("purchased")]
        public DateTime? Purchased { get => purchased; set => purchased = value; }

        [JsonPropertyName("warranty_end")]
        public DateTime? WarrantyEnd { get => warrantyEnd; set => warrantyEnd = value; }

        public string WarrantyInfo
        {
            get
            {
                if (warrantyEnd == null)
                {
                    return "No warranty info";
                }
                else
                {
                    return warrantyEnd > DateTime.Now ? $"Under warranty" : "Not under warranty";
                }
            }
        }

        [JsonPropertyName("version")]
        public string Version { get => version; set => version = value; }

        [JsonPropertyName("comment")]
        public string Comment { get => comment; set => comment = value; }

        //[JsonPropertyName("status")]
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

        //public async Task<bool> Post()
        //{
        //    Robot robot = this;
        //    using HttpClient client = new();
        //    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        //    string baseUri = Preferences.Get("uri", "https://vat.berlin-united.com/api/");
        //    string apiUri = baseUri + "CreateEdit/robot";

        //    User user = new();
        //    var dateTime = DateTime.UtcNow;
        //    var userSecret = GenerateUserSecret(dateTime.ToString());

        //    var request = new CreateEditRobotRequest
        //    {
        //        Robot = robot,
        //        UserId = user.Id,
        //        DeviceId = user.DeviceId,
        //        DateTime = dateTime.ToString(),
        //        UserSecret = userSecret
        //    };

        //    var content = JsonContent.Create(request);
        //    // Serialize JSON manually and log it
        //    string jsonBody = JsonSerializer.Serialize(request);
        //    Console.WriteLine("Sending JSON: " + jsonBody);

        //    try
        //    {
        //        HttpResponseMessage response = await client.PostAsync(apiUri, content);
        //        var result = await response.Content.ReadFromJsonAsync<CreateRobotResponse>();
        //        return response.IsSuccessStatusCode;
        //    }
        //    catch
        //    {
        //        return false;
        //    }

        //}

        public async Task<bool> InitializeFromCloud(int robotID)
        {
            id = robotID;
            return await Get();
        }

        public async Task<bool> Get()
        {
            using HttpClient client = new();
            string baseUri = Preferences.Get("uri", "https://vat.berlin-united.com/api/");
            string apiUri = baseUri + $"robots/{id}/";
            //Preferences.Set("apiKey", "...");
            string apiKey = Preferences.Get("apiKey", "");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", apiKey);

            try
            {
                var response = await client.GetAsync(apiUri);
                response.EnsureSuccessStatusCode();

                Robot? robot = await response.Content.ReadFromJsonAsync<Robot>();
                

                if (robot != null)
                { 
                    this.headNumber = robot.headNumber;
                    this.model = robot.model;
                    this.bodySerial = robot.bodySerial;
                    this.headSerial = robot.headSerial;
                    this.version = robot.version;
                    this.purchased = robot.purchased;
                    this.warrantyEnd = robot.warrantyEnd;
                    this.comment = robot.comment;

                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        //public async Task<bool> Delete()
        //{
        //    using HttpClient client = new();
        //    string baseUri = Preferences.Get("uri", "https://vat.berlin-united.com/api/");
        //    string apiUri = baseUri + $"Delete/robot/{id}";
        //    try
        //    {
        //        HttpResponseMessage response = await client.DeleteAsync(apiUri);
        //        return response.IsSuccessStatusCode;
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}

        public async Task<bool> SetStatus(Status status)
        {
            using HttpClient client = new();
            string baseUri = Preferences.Get("uri", "https://vat.berlin-united.com/api/");
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
    public enum IssueStatus
    {
        Noticed,
        Verified,
        InClinic,
        Solved
    }
    public class Issue
    {
        int id;
        int robot;
        private string description = String.Empty;
        private int author;
        private string status = String.Empty;
        private DateTime created = new();
        private DateTime modified = new();
        private string solvedReport = String.Empty;

        [JsonPropertyName("id")]
        public int Id { get => id; set => id = value; }

        [JsonPropertyName("robot")]
        public int Robot { get => robot; set => robot = value; }

        [JsonPropertyName("description")]
        public string Description { get => description; set => description = value; }

        //[JsonPropertyName("author")]
        public int Author { get => author; set => author = value; }

        [JsonPropertyName("created")]
        public DateTime Created { get => created; set => created = value; }

        [JsonPropertyName("modified")]
        public DateTime Modified { get => modified; set => modified = value; }

        [JsonPropertyName("status")]
        public string Status { get => status; set => status = value; }

        public bool IsSolved { get => status == IssueStatus.Solved.ToString(); }

        //[JsonPropertyName("solvedReport")]
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
            string baseUri = Preferences.Get("uri", "https://vat.berlin-united.com/api/");
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
            string baseUri = Preferences.Get("uri", "https://vat.berlin-united.com/api/");
            string apiUri = baseUri + $"health-issues/{id}";
            Issue issue = await client.GetFromJsonAsync<Issue>(apiUri);
            if (issue != null)
            {
                this.robot = issue.robot;
                this.description = issue.description;
                //this.author = issue.author;
                this.status = issue.status;
                this.created = issue.created;
                this.modified = issue.modified;
                //this.solvedReport = issue.solvedReport;
                return true;
            }
            return false;
        }

        public async Task<bool> Delete()
        {
            using HttpClient client = new();
            string baseUri = Preferences.Get("uri", "https://vat.berlin-united.com/api/");
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
            string baseUri = Preferences.Get("uri", "https://vat.berlin-united.com/api/");
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
            string baseUri = Preferences.Get("uri", "https://vat.berlin-united.com/api/");
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
        int robot;

        private string title = String.Empty;
        private string description = String.Empty;
        private int author;
        private User authorUser = new();
        private DateTime date;

        [JsonPropertyName("id")]
        public int Id { get => id; set => id = value; }

        [JsonPropertyName("robot")]
        public int Robot { get => robot; set => robot = value; }

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
            string baseUri = Preferences.Get("uri", "https://vat.berlin-united.com/api/");
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

            string baseUri = Preferences.Get("uri", "https://vat.berlin-united.com/api/");
            string apiUri = baseUri + $"GetSingle/note/{id}";
            try
            {
                Note note = await client.GetFromJsonAsync<Note>(apiUri);
                if (note != null)
                {
                    this.robot = note.robot;
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
            string baseUri = Preferences.Get("uri", "https://vat.berlin-united.com/api/");
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
        int robot;

        private DateTime date;
        private DateTime backDate;
        private List<int> issues = new();
        private string notes = String.Empty;
        private string backReport = String.Empty;
        private int author;
        private int collector;

        [JsonPropertyName("id")]
        public int Id { get => id; set => id = value; }

        [JsonPropertyName("robot")]
        public int Robot { get => robot; set => robot = value; }

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
            string baseUri = Preferences.Get("uri", "https://vat.berlin-united.com/api/");
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
            string baseUri = Preferences.Get("uri", "https://vat.berlin-united.com/api/");
            string apiUri = baseUri + $"GetSingle/clinicVisit/{id}";
            try
            {
                ClinicVisit visit = await client.GetFromJsonAsync<ClinicVisit>(apiUri);
                if (visit != null)
                {
                    this.robot = visit.robot;
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
            string baseUri = Preferences.Get("uri", "https://vat.berlin-united.com/api/");
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
            string baseUri = Preferences.Get("uri", "https://vat.berlin-united.com/api/");
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
    public class CreateEditRobotRequest
    {
        public Robot Robot { get; set; }

        // Robot Data
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

    public class CreateRobotResponse
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
