using Microsoft.EntityFrameworkCore;
using FinalExam.Models;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
namespace FinalExam.Controllers
{
    [Route("Logins")]
    public class LoginsController : Controller
    {
        private FinalContext dbContext;
    
        public LoginsController(FinalContext context)
        {
            dbContext = context;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if(UserId == null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("ShowUser");
            }
        }

        [HttpPost("AddUser")]
        public IActionResult AddUser(User NewUser)
        {
            if(ModelState.IsValid)
            {
                if(dbContext.Users.Any(u => u.email == NewUser.email))
                {
                    ModelState.AddModelError("email", "email exists!");
                    return View("Index");
                }

                PasswordHasher<User> hasher = new PasswordHasher<User>();
                NewUser.password = hasher.HashPassword(NewUser, NewUser.password);

                dbContext.Users.Add(NewUser);
                dbContext.SaveChanges();

                HttpContext.Session.SetInt32("UserId", NewUser.user_id);
                return RedirectToAction("Index", "Final");
            }
            else
            {
                return View("Index");
            }
        }

        [HttpGet("Login")]
        public IActionResult Login()
        {
            int? UserId = HttpContext.Session.GetInt32("UserId");
            if(UserId == null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Final");
            }
        }

        [HttpPost("LoginUser")]
        public IActionResult LoginUser(LoginUser User)
        {
            if(ModelState.IsValid)
            {
                User FoundUser = dbContext.Users.FirstOrDefault(u => u.email == User.email);
                if(FoundUser == null)
                {
                    ModelState.AddModelError("email", "Invalid Email/Password");
                    return View("Login");
                }

                PasswordHasher<LoginUser> hasher = new PasswordHasher<LoginUser>();
                var result = hasher.VerifyHashedPassword(User, FoundUser.password, User.password);
                if(result == PasswordVerificationResult.Failed)
                {
                    ModelState.AddModelError("email", "Invalid Email/Password");
                    return View("Login");
                }

                HttpContext.Session.SetInt32("UserId", FoundUser.user_id);
                return RedirectToAction("Index", "Final");
            }
            else
            {
                return View("Login");
            }
        }

        [HttpGet("Logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Final");
        }

        
        [HttpGet("AllUsers")]
        public IActionResult AllUsers()
        {
            List<User> AllUsers = dbContext.Users.ToList();
            ViewBag.Users = AllUsers;
            return View("AllUsers");
        }
    }
}