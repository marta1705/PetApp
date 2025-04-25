using PetApp.DAL;

namespace PetApp.Services
{
    public class ReminderService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public ReminderService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    var now = DateTime.Now;
                    var oneHourLater = now.AddHours(1);
                    var reminders = db.Reminders
                        .Where(r => r.DueDate >= now && r.DueDate <= oneHourLater && !r.IsSent)
                        .ToList();


                    foreach (var reminder in reminders)
                    {
                        reminder.IsSent = true;
                        db.Reminders.Update(reminder);
                    }

                    await db.SaveChangesAsync();
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}