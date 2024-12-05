namespace TimeTaskTracking.Models.Entities
{
    public class ToDoListModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public DateTime? DateTime { get; set; }
        public string? To { get; set; }
        public string? ApplicationUsersId { get; set; }

    }
}
