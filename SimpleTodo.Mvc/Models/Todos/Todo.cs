using System;

namespace SimpleTodo.Mvc.Models.Todos
{
    public class Todo
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public bool IsComplete { get; set; }
        public DateTime? CompletedDate { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string OwnerUserId { get; set; }
        public bool IsRecurring { get; set; }
        public int? CreatedFrom { get; set; }
        public DateTime StartDate { get; set; } = DateTime.Now;
    }
}