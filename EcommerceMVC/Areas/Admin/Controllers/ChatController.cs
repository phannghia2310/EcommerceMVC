using AutoMapper;
using EcommerceMVC.Areas.Admin.Models;
using EcommerceMVC.Data;
using EcommerceMVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EcommerceMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(AuthenticationSchemes = "AdminAuth")]
    public class ChatController : Controller
    {
        private readonly Ecommerce2024Context _context;
        private readonly IMapper _mapper;

        public ChatController(Ecommerce2024Context context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetUser()
        {
            var name = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
            return Ok(new { name = name });
        }

        [HttpPost]
        [Route("/Admin/Chat/SaveMessage")]
        public async Task<IActionResult> SaveMessage([FromBody] MessageVM model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var message = _mapper.Map<Message>(model);
                    _context.Messages.Add(message);
                    await _context.SaveChangesAsync();

                    return Ok();
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Lỗi: {ex.Message}");
                }
            }

            return View(model);
        }

        [HttpGet]
        [Route("/Admin/Chat/GetUnreadMessagesCount")]
        public async Task<IActionResult> GetUnreadMessagesCount()
        {
            var unreadMessagesCount = _context.Messages.Count(m => m.FromUser != "Admin" && m.IsRead == 0);
            return Ok(unreadMessagesCount);
        }

        [HttpGet]
        [Route("/Admin/Chat/GetUnreadMessages")]
        public async Task<IActionResult> GetUnreadMessages()
        {
            var unreadMessages = _context.Messages.Where(m => m.FromUser != "Admin" && m.IsRead == 0).ToList();
            return Ok(unreadMessages);
        }

        [HttpGet]
        [Route("/Admin/Chat/GetContactList")]
        public async Task<IActionResult> GetContactList()
        {
            var contacts = _context.Messages
                .Where(m => m.ToUser == "Admin")
                .GroupBy(m => m.FromUser)
                .Select(g => new
                {
                    Contact = g.Key,
                    LastMessage = g.OrderByDescending(m => m.Timestamp).FirstOrDefault().Message1,
                    LastMessageTimestamp = g.Max(m => m.Timestamp),
                    UnreadMessagesCount = g.Count(m => m.IsRead == 0)
                })
                .ToList();

            return Ok(contacts);
        }

        [HttpPost]
        [Route("/Admin/Chat/MarkAsRead")]
        public async Task<IActionResult> MarkAsRead([FromBody] MessageModel message)
        {
            try
            {
                var messages = _context.Messages.Where(m => m.FromUser == message.FromUser);

                foreach (var msg in messages)
                {
                    msg.IsRead = 1;
                }

                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        [Route("/Admin/Chat/GetMessages")]
        public IActionResult GetMessages([FromBody] MessageModel message)
        {
            try
            {
                var messages = _context.Messages
                        .Where(m => (m.FromUser == message.FromUser && m.ToUser == "Admin") 
                                        || (m.FromUser == "Admin" && m.ToUser == message.FromUser))
                        .OrderBy(m => m.Timestamp)
                        .ToList();
                return Json(messages);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
