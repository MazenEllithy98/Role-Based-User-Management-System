using AutoMapper;
using Demo.DAL.Models;
using Demo.PL.Helper;
using Demo.PL.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Demo.PL.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IMapper _mapper;

        public UserController(UserManager<ApplicationUser> userManager,SignInManager<ApplicationUser> signInManager , IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
        }
        public async Task<IActionResult> Index(string email)
        {
            if(string.IsNullOrEmpty(email))
            {
                var users = await _userManager.Users.Select(U => new UserViewModel()
                {
                    ID = U.Id,
                    FName = U.FName,
                    LName = U.LName,
                    Email = U.Email,
                    PhoneNumber = U.PhoneNumber,
                    Roles = _userManager.GetRolesAsync(U).Result
                }).ToListAsync();
                return View(users);

            }
            else
            {
                var users = await _userManager.FindByEmailAsync(email);

                var mappedUser = new UserViewModel()
                {
                    ID = users.Id,
                    FName = users.FName,
                    LName = users.LName,
                    Email = users.Email,
                    PhoneNumber = users.PhoneNumber,
                    Roles = _userManager.GetRolesAsync(users).Result
                };

                return View(new List<UserViewModel>() { mappedUser });
            }
        }


        public async Task<IActionResult> Details(string id, string viewName = "Details")
        {
            if (id is null)
                return BadRequest();//400
            var user = await _userManager.FindByIdAsync(id);
            if (user is null)
                return NotFound();//404
            var mappedUser= _mapper.Map<ApplicationUser, UserViewModel>(user);
            return View(viewName, mappedUser);
        }
        public async Task<IActionResult> Edit(string id)
        {
            return await Details(id, "Edit");

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, UserViewModel UserVM)
        {
            if (id != UserVM.ID)
                return BadRequest();
            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userManager.FindByIdAsync(id);
                    user.FName = UserVM.FName;
                    user.LName = UserVM.LName;
                    user.PhoneNumber = UserVM.PhoneNumber;
                    //user.Email = UserVM.Email;
                    //user.SecurityStamp = Guid.NewGuid().ToString();

                    await _userManager.UpdateAsync(user);

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    //1.Log Exception
                    //2.Friendly Message
                    ModelState.AddModelError(string.Empty, ex.Message);

                }
            }
            return View(UserVM);
        }


        public async Task<IActionResult> Delete(string id)
        {
            return await Details(id, "Delete");
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmDelete(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync (id);
                await _userManager.DeleteAsync(user);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                //Log Exception and show friendly message
                ModelState.AddModelError(string.Empty, ex.Message);
                return RedirectToAction("Error" , "Home");
            }
        }

    }
}
