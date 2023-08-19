namespace PemAPI.Models
{
    public class Project
    {
        public int Id { get; set; }

        public int OwnerUserId { get; set; }

        public List<int> MemberUserIds { get; set; } = new List<int>();

        public string Name { get; set; }

        public string Description { get; set; }

        public Project(int id, int ownerUserId,List<int> memberUserIds, string name, string description)
        {
            Id = id;
            OwnerUserId = ownerUserId;
            MemberUserIds = memberUserIds;
            Name = name;
            Description = description;
        }

        public Project(int ownerUserId, string name, string description)
        {
            OwnerUserId = ownerUserId;
            MemberUserIds.Add(ownerUserId);
            Name = name;
            Description = description;
        }
    }

    public class CreateProjectModel
    {
        public string Name { get; set; }

        public string Description { get; set; }
    }
}
