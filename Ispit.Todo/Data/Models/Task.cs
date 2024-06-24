﻿namespace Ispit.Todo.Data.Models
{
    public class Task
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public bool Status { get; set; }
        public int TodoListId { get; set; }
        public TodoList TodoList { get; set; }
    }
}
