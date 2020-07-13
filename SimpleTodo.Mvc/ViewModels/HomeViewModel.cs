using System;
using System.Collections.Generic;
using System.Linq;
using SimpleTodo.Mvc.Models.Todos;

namespace SimpleTodo.Mvc.ViewModels
{
    public class HomeViewModel
    {
        public List<Todo> Todos { get; set; }
        public IEnumerable<IGrouping<DateTime, Todo>> LastFiveDaysTodos { get; set; }
        public string NewTodoDescription { get; set; }
    }
}