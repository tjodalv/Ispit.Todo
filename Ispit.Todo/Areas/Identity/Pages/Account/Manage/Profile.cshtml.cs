using Ispit.Todo.Data.Models;
using Ispit.Todo.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ispit.Todo.Areas.Identity.Pages.Account.Manage
{
    public class ProfileModel : PageModel
    {
        private readonly UserManager<AspNetUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProfileModel> _logger;

        public ProfileModel(UserManager<AspNetUser> userManager, ApplicationDbContext context, ILogger<ProfileModel> logger)
        {
            _userManager = userManager;
            _context = context;
            _logger = logger;
        }

        public IList<TodoList>? TodoLists { get; set; } = new List<TodoList>();

        public async Task<IActionResult> OnGet()
        {
            var user = await _userManager.GetUserAsync(User);

            Console.WriteLine("Jebem ti mater");

            if (user == null)
            {
                return NotFound("Unable to load user.");
            }

            TodoLists = await _context.TodoLists
                .Include(t => t.Tasks)
                .Where(t => t.UserId == user.Id)
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostCreateTodoList(string name)
        {
            Console.WriteLine("Jebem ti mater iz POST metode");

            _logger.LogInformation($"Received name: {name}");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Model state is invalid.");
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        _logger.LogError($"ModelState Error: {error.ErrorMessage}");
                    }
                }
                return Page();
            }

            try
            {
                var user = await _userManager.GetUserAsync(User);
                _logger.LogInformation($"User is {user}");
                if (user == null)
                {
                    _logger.LogError("User not found.");
                    return NotFound("Unable to load user.");
                }

                if (string.IsNullOrEmpty(name))
                {
                    _logger.LogWarning("Name is null or empty.");
                    ModelState.AddModelError("name", "The name field is required.");
                    return Page();
                }

                var todoList = new TodoList
                {
                    Name = name,
                    UserId = user.Id
                };

                _context.TodoLists.Add(todoList);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Todo list created successfully.");
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        public async Task<IActionResult> OnPostCreateTask(int todoListId, string description)
        {
            var todoList = await _context.TodoLists.FindAsync(todoListId);

            if (todoList == null)
            {
                return NotFound("Unable to load todo list.");
            }

            var task = new Data.Models.Task
            {
                Description = description,
                TodoListId = todoListId,
                Status = false
            };

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }
    }
}
