using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
        private List<Issue> issues = new();
        private List<Note> notes = new();
        private List<ClinicVisit> clinicVisits = new();
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
        public List<Issue> Issues { get => issues; set => issues = value; }

        [JsonPropertyName("notes")]
        public List<Note> Notes { get => notes; set => notes = value; }
        
        [JsonPropertyName("clinicVisits")]
        public List<ClinicVisit> ClinicVisits { get => clinicVisits; set => clinicVisits = value; }
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

    }

    public class Issue
    {
        int id;
        int nao;

        private string title;
        private string description;
        private bool replicated;
        private bool solved;
        private DateTime date;
        private DateTime replicatedDate;
        private DateTime solvedDate;
        private string solvedReport;

        int Id { get => id; set => id = value; }
        public int Nao { get => nao; set => nao = value; }

        public string Title { get => title; set => title = value; }
        public string Description { get => description; set => description = value; }
        public bool Replicated { get => replicated; set => replicated = value; }
        public bool Solved { get => solved; set => solved = value; }
        public DateTime Date { get => date; set => date = value; }
        public DateTime ReplicatedDate { get => replicatedDate; set => replicatedDate = value; }
        public DateTime SolvedDate { get => solvedDate; set => solvedDate = value; }
        public string SolvedReport { get => solvedReport; set => solvedReport = value; }
    }

    public class Note
    {
        int id;
        int nao;

        private string title;
        private string description;
        private DateTime date;

        int Id { get => id; set => id = value; }
        public int Nao { get => nao; set => nao = value; }

        public string Title { get => title; set => title = value; }
        public string Description { get => description; set => description = value; }
        public DateTime Date { get => date; set => date = value; }
    }

    public class ClinicVisit
    {
        int id;
        int nao;

        private DateTime date;
        private List<Issue> issues;
        private bool isBack;
        private string backReport;

        int Id { get => id; set => id = value; }
        public int Nao { get => nao; set => nao = value; }

        public DateTime Date { get => date; set => date = value; }
        public List<Issue> Issues { get => issues; set => issues = value; }
        public bool IsBack { get => isBack; set => isBack = value; }
        public string BackReport { get => backReport; set => backReport = value; }
    }

    public enum Status
    {
        Free,
        Game,
        Clinic
    }
}
