namespace tech_project_back_end.Models
{
    public class AuditLogs
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string EntityName { get; set; }
        public string Action { get; set; }
        public string Role { get; set; }
        public required DateTime Timestamp { get; set; }
        public required string Changes { get; set; }
    }
}
