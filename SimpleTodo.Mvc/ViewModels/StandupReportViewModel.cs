using System.Collections.Generic;
using SimpleTodo.Mvc.Models.Todos;

namespace SimpleTodo.Mvc.ViewModels
{
    public class StandupReportViewModel
    {
        public List<Todo> YesterdaysTodos { get; set; }
        public List<Todo> TodaysTodos { get; set; }
    }
}