using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Ispit.Todo.Data;
using Ispit.Todo.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace Ispit.Todo.Controllers
{
    public class TodoController : Controller
    {
        private readonly UserManager<AspNetUser> _userManager;
        private readonly ApplicationDbContext _context;

        public IList<TodoList>? TodoLists { get; set; } = new List<TodoList>();

        public TodoController(UserManager<AspNetUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // GET: Todo
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound("Unable to load user.");
            }

            TodoLists = await _context.TodoLists
                .Include(t => t.Tasks)
                .Where(t => t.UserId == user.Id)
                .ToListAsync();

            return View(TodoLists);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTodoList(string name)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound("Unable to load user.");
            }

            if (string.IsNullOrEmpty(name))
            {
                ModelState.AddModelError("name", "The name field is required.");
                return RedirectToAction(nameof(Index));
            }

            var todoList = new TodoList
            {
                Name = name,
                UserId = user.Id
            };

            _context.TodoLists.Add(todoList);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
