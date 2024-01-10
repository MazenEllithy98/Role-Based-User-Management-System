using AutoMapper;
using Demo.BLL.Interfaces;
using Demo.BLL.Repositories;
using Demo.DAL.Models;
using Demo.PL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Demo.PL.Controllers
{
    
    public class DepartmentController : Controller
    {
        //private readonly IDepartmentRepository _departmentRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public DepartmentController(IUnitOfWork unitOfWork, IMapper mapper) // Ask CLR to create an object from class implementing this interface
        {
            _unitOfWork =unitOfWork;
            _mapper = mapper;
        }
        public async Task<IActionResult> Index()
        {
            var departments = await _unitOfWork.DepartmentRepository.GetAll();

            var mappedDept = _mapper.Map<IEnumerable<Departments>, IEnumerable<DepartmentViewModel>>(departments);
            return View(mappedDept);
        }
        //[HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(DepartmentViewModel departmentVM)
        {
            
            if (ModelState.IsValid) //Server side validation

            {
                var mappedDept = _mapper.Map<DepartmentViewModel, Departments>(departmentVM);

                await _unitOfWork.DepartmentRepository.Add(mappedDept);
                
                int count = await _unitOfWork.Complete();

                //TempData
                if (count > 0)
                TempData["Message"] = "Department is Created Successfuly!";
                //Update
                //Delete
                //_unitOfWork.Complete();
                return RedirectToAction(nameof(Index));
            }
            return View(departmentVM);
        }

        public async Task<IActionResult> Details(int? id , string viewName = "Details")
        {
            if (id is null)
                return BadRequest();
            var department = await _unitOfWork.DepartmentRepository.GetById(id.Value);
            if (department is null)
                return NotFound();

            var mappedDept = _mapper.Map<Departments,DepartmentViewModel>(department);
            return View(viewName , mappedDept);
        }

        
        public async Task<IActionResult> Edit(int? id)
        {
            ///if (id is null)
            ///    return BadRequest();
            ///var department = _departmentRepository.GetById(id.Value);
            ///if (department is null)
            ///    return NotFound();
            ///return View(department);
            ///
            return await Details(id, "Edit");

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromRoute]int id , DepartmentViewModel departmentVM)
        {
            if(id!=departmentVM.ID) 
                return BadRequest();
            if (ModelState.IsValid)
            {
                try
                {

                    var mappedDept = _mapper.Map<DepartmentViewModel, Departments>(departmentVM);

                    _unitOfWork.DepartmentRepository.Update(mappedDept);

                    await _unitOfWork.Complete();

                    return RedirectToAction("Index");
                }
                catch(Exception ex)
                {
                    //1.Log Exception
                    //2.Friendly Message
                    ModelState.AddModelError(string.Empty , ex.Message);
                    
                }
            }
            return View(departmentVM);
        }
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            return await Details(id, "Delete");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([FromRoute] int id , DepartmentViewModel departmentVM)
        {
            if (id != departmentVM.ID)
                return BadRequest();
            try
            {

                var mappedDept = _mapper.Map<DepartmentViewModel, Departments>(departmentVM);

                _unitOfWork.DepartmentRepository.Delete(mappedDept);
                await _unitOfWork.Complete();
                return RedirectToAction("Index");
            }
            catch (Exception ex) 
            {
                //Log Exception and show friendly message
                ModelState.AddModelError (string.Empty , ex.Message);
                return View(departmentVM);
            }
        }
    }
}
