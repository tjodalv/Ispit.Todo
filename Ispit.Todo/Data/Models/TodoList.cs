namespace Ispit.Todo.Data.Models
{
    public class TodoList
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string UserId { get; set; }
        public AspNetUser User { get; set; }
        public ICollection<Task> Tasks { get; set; }
    }
}
