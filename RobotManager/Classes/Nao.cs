using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RobotManager.Classes
{
    public class Nao : INotifyPropertyChanged
    {
        private string name;
        private string headID;
        private string bodyID;
        private DateTime purchased;
        private List<Issue> issues;
        private List<Note> notes;
        private List<ClinicVisit> clinicVisits;
        private Status status;

        public string Name { get => name; set => name = value; }
        public string Ip { get => $"10.0.4.{name}"; }
        public string HeadID { get => headID; set => headID = value; }
        public string BodyID { get => bodyID; set => bodyID = value; }
        public DateTime Purchased { get => purchased; set => purchased = value; }
        public DateTime Waranty { get => purchased.AddYears(2); }

        public List<Issue> Issues { get => issues; set => issues = value; }
        public List<Note> Notes { get => notes; set => notes = value; }
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
        private string title;
        private string description;
        private bool replicated;
        private bool solved;
        private DateTime date;
        private DateTime replicatedDate;
        private DateTime solvedDate;

        public string Title { get => title; set => title = value; }
        public string Description { get => description; set => description = value; }
        public bool Replicated { get => replicated; set => replicated = value; }
        public bool Solved { get => solved; set => solved = value; }
        public DateTime Date { get => date; set => date = value; }
        public DateTime ReplicatedDate { get => replicatedDate; set => replicatedDate = value; }
        public DateTime SolvedDate { get => solvedDate; set => solvedDate = value; }
    }

    public class Note
    {
        private string title;
        private string description;
        private DateTime date;

        public string Title { get => title; set => title = value; }
        public string Description { get => description; set => description = value; }
        public DateTime Date { get => date; set => date = value; }
    }

    public class ClinicVisit
    {
        private DateTime date;
        private List<Issue> issues;
        private bool isBack;
        private string backReport;

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
