namespace PetApp.Models
{
    public class ExpenseListViewModel
    {
        public List<Expense> Expenses { get; set; }
        public DateTime SelectedMonth { get; set; }
        public List<DateTime> AvailableMonths { get; set; } = new List<DateTime>();
        public float TotalAmount { get; set; }
        public float AverageDailyAmount { get; set; }
        public string? MostFrequentCategory { get; set; }
        public float ChangeFromPreviousMonth { get; set; }
        public Dictionary<string, float> ExpensesByCategory { get; set; } = new();
        public Dictionary<string, float> ExpensesByDate { get; set; } = new();

        public string SortBy { get; set; }
        public string SortDirection { get; set; }
    }
}
