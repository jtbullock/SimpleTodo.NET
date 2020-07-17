using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SimpleTodo.Mvc.Data;
using SimpleTodo.Mvc.Extensions;
using SimpleTodo.Mvc.Models;
using SimpleTodo.Mvc.Models.Todos;
using SimpleTodo.Mvc.ViewModels;

namespace SimpleTodo.Mvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<IdentityUser> _userManager;

        public HomeController(ILogger<HomeController> logger,
            UserManager<IdentityUser> userManager,
            ApplicationDbContext context)
        {
            _logger = logger;
            _userManager = userManager;
            _context = context;
        }

        [Authorize]
        [HttpGet]
        public IActionResult Index()
        {
            var currentUserId = _userManager.GetUserId(User);

            var viewModel = new HomeViewModel
            {
                Todos = GetTodosForToday(currentUserId),
                LastFiveDaysTodos = GetLastFiveDaysTodos()
            };

            return View(viewModel);
        }


        [HttpPost]
        public IActionResult Index(HomeViewModel viewModel)
        {
            var currentUserId = _userManager.GetUserId(User);

            if (!viewModel.NewTodoDescription.IsNullOrEmpty())
            {
                _context.Todos.Add(new Todo
                {
                    Description = viewModel.NewTodoDescription,
                    CreatedDate = DateTime.Now,
                    OwnerUserId = currentUserId
                });

                _context.SaveChanges();
            }

            var returnViewModel = new HomeViewModel
            {
                Todos = GetTodosForToday(currentUserId),
                LastFiveDaysTodos = GetLastFiveDaysTodos()
            };

            return View(returnViewModel);
        }

        [HttpPost]
        [Authorize]
        public IActionResult SetTaskComplete([FromBody] SetTaskCompleteRequest request)
        {
            var currentUserId = _userManager.GetUserId(User);
            var todo = _context.Todos.FirstOrDefault(t => t.Id == request.Id);

            if (todo is null || todo.OwnerUserId != currentUserId)
                throw new UnauthorizedAccessException("You are not authorized to access this task.");

            todo.IsComplete = request.IsComplete;
            todo.CompletedDate = request.IsComplete ? DateTime.Now : (DateTime?) null;

            if (todo.IsRecurring && request.IsComplete)
                _context.Add(CreateFutureTodo(todo));

            if (todo.IsRecurring && !request.IsComplete)
                RemoveFutureRecurringTodos(todo.Id);

            _context.SaveChanges();

            return Ok();
        }

        [HttpPost]
        [Authorize]
        public IActionResult SetTaskRecurring([FromBody] SetTaskRecurringRequest request)
        {
            var currentUserId = _userManager.GetUserId(User);
            var todo = _context.Todos.FirstOrDefault(t => t.Id == request.Id);

            if (todo is null || todo.OwnerUserId != currentUserId)
                throw new UnauthorizedAccessException("You are not authorized to access this task.");

            if (todo.IsRecurring)
                RemoveFutureRecurringTodos(todo.Id);
            else if (todo.IsComplete) _context.Todos.Add(CreateFutureTodo(todo));

            todo.IsRecurring = !todo.IsRecurring;

            _context.SaveChanges();

            return Ok();
        }

        private IEnumerable<IGrouping<DateTime, Todo>> GetLastFiveDaysTodos()
        {
            var todaysDate = DateTime.Now.Date;
            var fiveDaysAgoDate = DateTime.Now.AddDays(-7);

            return _context.Todos
                .Where(t => t.IsComplete && t.CompletedDate.HasValue &&
                            t.CompletedDate < todaysDate &&
                            t.CompletedDate >= fiveDaysAgoDate)
                .OrderByDescending(t => t.CompletedDate)
                .ToList()
                .GroupBy(t => t.CompletedDate.Value.Date);
        }

        private List<Todo> GetTodosForToday(string userId)
        {
            return _context.Todos
                .Where(t =>
                    t.OwnerUserId == userId &&
                    (!t.IsComplete || t.CompletedDate.HasValue && t.CompletedDate.Value.Date == DateTime.Now.Date) &&
                    t.StartDate.Date <= DateTime.Now.Date)
                .OrderByDescending(t => t.CreatedDate)
                .ToList();
        }

        private void RemoveFutureRecurringTodos(int createdFromId)
        {
            var futureTasks = _context.Todos
                .Where(t => t.CreatedFrom == createdFromId &&
                            t.StartDate > DateTime.Now)
                .ToList();

            if (futureTasks.Any()) _context.Todos.RemoveRange(futureTasks);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }

        private Todo CreateFutureTodo(Todo sourceTodo)
        {
            return new Todo
            {
                CreatedFrom = sourceTodo.Id,
                Description = sourceTodo.Description,
                IsRecurring = true,
                OwnerUserId = sourceTodo.OwnerUserId,
                StartDate = DateTime.Now.Date.AddDays(1)
            };
        }

        public class SetTaskCompleteRequest
        {
            public int Id { get; set; }
            public bool IsComplete { get; set; }
        }
    }

    public class SetTaskRecurringRequest
    {
        public int Id { get; set; }
    }
}