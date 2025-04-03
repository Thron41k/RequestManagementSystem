using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RequestManagement.Client.Models;
using RequestManagement.Client.Services;
using RequestManagement.Common.Models;
using System.Security.Claims;
using System.Threading.Tasks;
using RequestManagement.Server.Controllers;
using Item = RequestManagement.Common.Models.Item;

namespace RequestManagement.Client.Controllers
{
    [Authorize] // Требуется аутентификация для всех действий
    public class RequestController : Controller
    {
        private readonly GrpcRequestService _requestService;

        public RequestController(GrpcRequestService requestService)
        {
            _requestService = requestService;
        }

        // GET: /Request/
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var requests = await _requestService.GetAllRequestsAsync();
            var viewModel = requests.Select(r => new RequestViewModel
            {
                Id = r.Id,
                Number = r.Number,
                CreationDate = r.CreationDate,
                DueDate = r.DueDate,
                Comment = r.Comment,
                ExecutionComment = r.ExecutionComment,
                Status = r.Status,
                EquipmentId = r.EquipmentId
            }).ToList();

            return View(viewModel);
        }

        // GET: /Request/Create
        [HttpGet]
        [Authorize(Roles = "Administrator")] // Только администратор может создавать заявки
        public IActionResult Create()
        {
            return View(new RequestViewModel());
        }

        // POST: /Request/Create
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RequestViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var request = new Request
            {
                Number = model.Number,
                CreationDate = model.CreationDate,
                DueDate = model.DueDate,
                Comment = model.Comment,
                ExecutionComment = model.ExecutionComment,
                Status = model.Status,
                EquipmentId = model.EquipmentId,
                Items = model.Items?.Select(i => new Item
                {
                    Name = i.Name,
                    Article = i.Article,
                    Quantity = i.Quantity,
                    Note = i.Note,
                    Status = i.Status
                }).ToList() ?? new List<Item>()
            };

            await _requestService.CreateRequestAsync(request);
            return RedirectToAction(nameof(Index));
        }

        // GET: /Request/Edit/{id}
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var request = await _requestService.GetRequestByIdAsync(id);
            if (request == null)
            {
                return NotFound();
            }

            var viewModel = new RequestViewModel
            {
                Id = request.Id,
                Number = request.Number,
                CreationDate = request.CreationDate,
                DueDate = request.DueDate,
                Comment = request.Comment,
                ExecutionComment = request.ExecutionComment,
                Status = request.Status,
                EquipmentId = request.EquipmentId,
                Items = request.Items.Select(i => new ItemViewModel
                {
                    Id = i.Id,
                    Name = i.Name,
                    Article = i.Article,
                    Quantity = i.Quantity,
                    Note = i.Note,
                    Status = i.Status
                }).ToList()
            };

            return View(viewModel);
        }

        // POST: /Request/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(RequestViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var request = new Request
            {
                Id = model.Id,
                Number = model.Number,
                CreationDate = model.CreationDate,
                DueDate = model.DueDate,
                Comment = model.Comment,
                ExecutionComment = model.ExecutionComment,
                Status = model.Status,
                EquipmentId = model.EquipmentId,
                Items = model.Items?.Select(i => new Item
                {
                    Id = i.Id,
                    Name = i.Name,
                    Article = i.Article,
                    Quantity = i.Quantity,
                    Note = i.Note,
                    Status = i.Status
                }).ToList() ?? new List<Item>()
            };

            // Ограничения по ролям
            if (userRole == "Observer")
            {
                ModelState.AddModelError("", "Наблюдатели не могут редактировать заявки.");
                return View(model);
            }
            else if (userRole == "User")
            {
                // Пользователь может изменять только ExecutionComment и Status
                var originalRequest = await _requestService.GetRequestByIdAsync(model.Id);
                request.Number = originalRequest.Number;
                request.CreationDate = originalRequest.CreationDate;
                request.DueDate = originalRequest.DueDate;
                request.Comment = originalRequest.Comment;
                request.EquipmentId = originalRequest.EquipmentId;
                request.Items = originalRequest.Items; // Пользователь не может менять Items
            }

            await _requestService.UpdateRequestAsync(request);
            return RedirectToAction(nameof(Index));
        }

        // POST: /Request/Delete/{id}
        [HttpPost]
        [Authorize(Roles = "Administrator")] // Только администратор может удалять
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _requestService.DeleteRequestAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}