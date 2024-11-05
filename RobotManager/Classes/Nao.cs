using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http.Json;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

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
            string apiUrl = "https://skakominor.de/api/RobotManager/CreateEdit/nao";
            try
            {
                HttpResponseMessage response = await client.PostAsJsonAsync(apiUrl, nao);
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
            string apiUrl = $"https://skakominor.de/api/RobotManager/GetSingle/nao/{id}";
            try
            {
                Nao nao = await client.GetFromJsonAsync<Nao>(apiUrl);

                if (nao != null)
                {
                    this.id = nao.id;
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
            string apiUrl = $"https://skakominor.de/api/RobotManager/Delete/nao/{id}";
            try
            {
                HttpResponseMessage response = await client.DeleteAsync(apiUrl);
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
        private bool replicated;
        private bool solved;
        private DateTime date;
        private DateTime replicatedDate;
        private DateTime solvedDate;
        private string solvedReport = String.Empty;

        public int Id { get => id; set => id = value; }
        public int Nao { get => nao; set => nao = value; }

        public string Title { get => title; set => title = value; }
        public string Description { get => description; set => description = value; }
        public bool Replicated { get => replicated; set => replicated = value; }
        public bool Solved { get => solved; set => solved = value; }
        public DateTime Date { get => date; set => date = value; }
        public DateTime ReplicatedDate { get => replicatedDate; set => replicatedDate = value; }
        public DateTime SolvedDate { get => solvedDate; set => solvedDate = value; }
        public string SolvedReport { get => solvedReport; set => solvedReport = value; }

        public async Task<bool> Post()
        {
            Issue issue = this;
            using HttpClient client = new();
            string apiUrl = "https://skakominor.de/api/RobotManager/CreateEdit/issue";
            try
            {
                HttpResponseMessage response = await client.PostAsJsonAsync(apiUrl, issue);
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
            string apiUrl = $"https://skakominor.de/api/RobotManager/GetSingle/issue/{id}";
            Issue issue = await client.GetFromJsonAsync<Issue>(apiUrl);
            if (issue != null)
            {
                this.id = issue.id;
                this.nao = issue.nao;
                this.title = issue.title;
                this.description = issue.description;
                this.replicated = issue.replicated;
                this.solved = issue.solved;
                this.date = issue.date;
                this.replicatedDate = issue.replicatedDate;
                this.solvedDate = issue.solvedDate;
                this.solvedReport = issue.solvedReport;
                return true;
            }
            return false;
        }

        public async Task<bool> Delete()
        {
            using HttpClient client = new();
            string apiUrl = $"https://skakominor.de/api/RobotManager/Delete/issue/{id}";
            try
            {
                HttpResponseMessage response = await client.DeleteAsync(apiUrl);
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
        private DateTime date;

        public int Id { get => id; set => id = value; }
        public int Nao { get => nao; set => nao = value; }

        public string Title { get => title; set => title = value; }
        public string Description { get => description; set => description = value; }
        public DateTime Date { get => date; set => date = value; }

        public async Task<bool> Post()
        {
            Note note = this;
            using HttpClient client = new();
            string apiUrl = "https://skakominor.de/api/RobotManager/CreateEdit/note";
            try
            {
                HttpResponseMessage response = await client.PostAsJsonAsync(apiUrl, note);
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
            string apiUrl = $"https://skakominor.de/api/RobotManager/GetSingle/note/{id}";
            try
            {
                Note note = await client.GetFromJsonAsync<Note>(apiUrl);
                if (note != null)
                {
                    this.id = note.id;
                    this.nao = note.nao;
                    this.title = note.title;
                    this.description = note.description;
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
            string apiUrl = $"https://skakominor.de/api/RobotManager/Delete/note/{id}";
            try
            {
                HttpResponseMessage response = await client.DeleteAsync(apiUrl);
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

        public int Id { get => id; set => id = value; }
        public int Nao { get => nao; set => nao = value; }

        public DateTime Date { get => date; set => date = value; }
        public DateTime BackDate { get => backDate; set => backDate = value; }
        public List<int> Issues { get => issues; set => issues = value; }
        public bool IsBack { get => isBack; set => isBack = value; }
        public string Notes { get => notes; set => notes = value; }
        public string BackReport { get => backReport; set => backReport = value; }

        public string DisplayID { get => id.ToString().PadLeft(4, '0'); }
        public int IssueCount { get => issues.Count; }

        public async Task<bool> Post()
        {
            ClinicVisit visit = this;
            using HttpClient client = new();
            string apiUrl = "https://skakominor.de/api/RobotManager/CreateEdit/clinicVisit";
            try
            {
                HttpResponseMessage response = await client.PostAsJsonAsync(apiUrl, visit);
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
            string apiUrl = $"https://skakominor.de/api/RobotManager/GetSingle/clinicVisit/{id}";
            try
            {
                ClinicVisit visit = await client.GetFromJsonAsync<ClinicVisit>(apiUrl);
                if (visit != null)
                {
                    this.id = visit.id;
                    this.nao = visit.nao;
                    this.date = visit.date;
                    this.issues = visit.issues;
                    this.isBack = visit.isBack;
                    this.notes = visit.notes;
                    this.backReport = visit.backReport;
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
            string apiUrl = $"https://skakominor.de/api/RobotManager/Delete/clinicVisit/{id}";
            try
            {
                HttpResponseMessage response = await client.DeleteAsync(apiUrl);
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
