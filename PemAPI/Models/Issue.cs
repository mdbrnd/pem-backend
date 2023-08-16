namespace PemAPI.Models
{
    public class Issue
    {
        public int Id { get; set; }

        public int ProjectId { get; set; }

        public int BoardId { get; set; }

        public bool Completed { get; set; } = false;

        public string Name { get; set; }

        public string Description { get; set; }

        
    }
    public class CreateIssueModel
    {
        public int BoardId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
    }
}
