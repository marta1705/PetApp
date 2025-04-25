namespace PetApp.Models
{
    public class ActivityListViewModel
    {
        public List<Activity> Activities { get; set; }
        public DateTime SelectedMonth { get; set; }
        public List<DateTime> AvailableMonths { get; set; }
        public string TotalDuration { get; set; }
        public string AverageDailyDuration { get; set; }
        public float TotalDistance { get; set; }
        public float ChangeFromPreviousMonth { get; set; }
        public Dictionary<string, float> DurationByCategory { get; set; }
        public Dictionary<string, float> DurationByDate  { get; set; }
        public Dictionary<string, float> DistanceByDate { get; set; }
        public Dictionary<string, float> MonthlyActivity { get; set; }
        public Dictionary<string, float> ActivityByPet { get; set; }
        public string SortBy { get; set; }
        public string SortDirection { get; set; }
    }
}
