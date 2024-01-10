using Demo.DAL.Models;
using Demo.PL.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace Demo.PL.Controllers
{
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;

        public RoleController(RoleManager<IdentityRole>roleManager , IMapper mapper)
        {
            _roleManager = roleManager;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index(string Name)
        {
            if (string.IsNullOrEmpty(Name))
            {
                var roles = await _roleManager.Roles.Select(R => new RoleViewModel()
                {
                    ID = R.Id,
                    RoleName = R.Name,
                    
                }).ToListAsync();
                return View(roles);

            }
            else
            {
                var roles = await _roleManager.FindByNameAsync(Name);

                var mappedRole = new RoleViewModel()
                {
                    ID = roles?.Id,
                    RoleName = roles?.Name,    
                };

                return View(new List<RoleViewModel>() { mappedRole });
            }
            
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(RoleViewModel roleVM)
        {
            if(ModelState.IsValid)
            {
                var mappedRole = _mapper.Map<RoleViewModel , IdentityRole >(roleVM);
                await _roleManager.CreateAsync(mappedRole);
                return RedirectToAction("Index");
            }
            return View(roleVM);
        }

        public async Task<IActionResult> Details(string id, string viewName = "Details")
        {
            if (id is null)
                return BadRequest();//400
            var role = await _roleManager.FindByIdAsync(id);
            if (role is null)
                return NotFound();//404
            var mappedRole = _mapper.Map<IdentityRole, RoleViewModel>(role);
            return View(viewName, mappedRole);
        }
        public async Task<IActionResult> Edit(string id)
        {
            return await Details(id, "Edit");

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, RoleViewModel roleVM)
        {
            if (id != roleVM.ID)
                return BadRequest();
            if (ModelState.IsValid)
            {
                try
                {
                    var role = await _roleManager.FindByIdAsync(id);
                    role.Name = roleVM.RoleName;
                    
                    await _roleManager.UpdateAsync(role);

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    //1.Log Exception
                    //2.Friendly Message
                    ModelState.AddModelError(string.Empty, ex.Message);

                }
            }
            return View(roleVM);
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
                var role = await _roleManager.FindByIdAsync(id);
                await _roleManager.DeleteAsync(role);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                //Log Exception and show friendly message
                ModelState.AddModelError(string.Empty, ex.Message);
                return RedirectToAction("Error", "Home");
            }
        }

    }
}
