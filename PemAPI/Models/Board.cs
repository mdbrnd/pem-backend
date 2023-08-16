namespace PemAPI.Models
{
    public class Board
    {
        public int Id { get; set; }

        public int ProjectId { get; set; }

        public string Name { get; set; }

    }

    public class CreateBoardModel
    {
        public int ProjectId { get; set; }

        public string Name { get; set; }
    }
}
