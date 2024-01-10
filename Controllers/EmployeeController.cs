using AutoMapper;
using Demo.BLL.Interfaces;
using Demo.BLL.Repositories;
using Demo.DAL.Models;
using Demo.PL.Helper;
using Demo.PL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Demo.PL.Controllers
{

    public class EmployeeController : Controller
    {
        //private readonly IEmployeeRepository _EmployeeRepository;
        //private readonly IDepartmentRepository _departmentRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public EmployeeController(IUnitOfWork unitOfWork, IMapper mapper) // Ask CLR to create an object from class implementing this interface
        {
            //_EmployeeRepository = EmployeeRepository;
            //_departmentRepository = departmentRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<IActionResult> Index(string SearchValue)
        {
            /// Binding is a one way Binding in MVC
            /// ViewData is a Dictionary Object while ViewBag is a dynamic Object and both are used to send data from controller to view
            //ViewData
            ViewData["Message"] = "Hello View Data!";
            IEnumerable<Employee> employees;

            //ViewBag
            ViewBag.Message = "Hello ViewBag";
            if (string.IsNullOrEmpty(SearchValue))
            {
                employees = await _unitOfWork.EmployeeRepository.GetAll();
            }
            else
            {
                employees = _unitOfWork.EmployeeRepository.GetEmployeesByName(SearchValue);
            }
            
            var mappedEmp = _mapper.Map<IEnumerable<Employee>, IEnumerable<EmployeeViewModel>>(employees);

            return View(mappedEmp);
        }
        //[HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Department =await _unitOfWork.DepartmentRepository.GetAll();
            return View();
        }
        [HttpPost]
        public  async Task<IActionResult> Create(EmployeeViewModel EmployeeViewModel)
        {
            if (ModelState.IsValid) //Server side validation
            {
                ///Manual Mapping
                ///var employee = new Employee()
                ///{
                ///    Name = EmployeeViewModel.Name,
                ///    Address = EmployeeViewModel.Address,
                ///    Age = EmployeeViewModel.Age,
                ///    Salary = EmployeeViewModel.Salary,
                ///    HireDate = EmployeeViewModel.HireDate,
                ///    IsActive = EmployeeViewModel.IsActive,
                ///    DepartmentsID = EmployeeViewModel.DepartmentsID,
                ///    Email = EmployeeViewModel.Email,
                ///    ID = EmployeeViewModel.ID,
                ///    PhoneNumber = EmployeeViewModel.PhoneNumber,                    
                ///};

                //Employee employee = (Employee)EmployeeViewModel;

                EmployeeViewModel.ImageName = await DocumentSettings.UploadFileAsync(EmployeeViewModel.Image, "Images");

                var mappedEmp = _mapper.Map<EmployeeViewModel,Employee>(EmployeeViewModel);

                   await _unitOfWork.EmployeeRepository.Add(mappedEmp);
                
                int count = await _unitOfWork.Complete();
                if (count > 0)
                    TempData["Message"] = "Employee Has Been Added Successfully!";
                //Update
                //Delete      
                return RedirectToAction("Index");
            }
            return View(EmployeeViewModel);
        }

        public async Task<IActionResult> Details(int? id, string viewName = "Details")
        {
            if (id is null)
                return BadRequest();//400
            var Employee = await _unitOfWork.EmployeeRepository.GetById(id.Value);
            if (Employee is null)
                return NotFound();//404
            var mappedEmp = _mapper.Map<Employee, EmployeeViewModel>(Employee);
            return View(viewName, mappedEmp);
        }


        public async Task<IActionResult> Edit(int? id)
        {
            ///if (id is null)
            ///    return BadRequest();
            ///var Employee = _EmployeeRepository.GetById(id.Value);
            ///if (Employee is null)
            ///    return NotFound();
            ///return View(Employee);
            ///
            return await Details(id, "Edit");

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromRoute] int id, EmployeeViewModel employeeVM)
        {
            if (id != employeeVM.ID)
                return BadRequest();
            if (ModelState.IsValid)
            {
                try
                {
                    var mappedEmp = _mapper.Map<EmployeeViewModel, Employee>(employeeVM);
                    _unitOfWork.EmployeeRepository.Update(mappedEmp);
                    await _unitOfWork.Complete();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    //1.Log Exception
                    //2.Friendly Message
                    ModelState.AddModelError(string.Empty, ex.Message);

                }
            }
            return View(employeeVM);
        }
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            return await Details(id, "Delete");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([FromRoute] int id, EmployeeViewModel employeeVM)
        {
            if (id != employeeVM.ID)
                return BadRequest();
            try
            {
                var mappedEmp = _mapper.Map<EmployeeViewModel, Employee>(employeeVM);
                _unitOfWork.EmployeeRepository.Delete(mappedEmp);
               int count =await _unitOfWork.Complete();
                if (count > 0 )
                {
                    DocumentSettings.DeleteFile(mappedEmp.ImageName, "Images");
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                //Log Exception and show friendly message
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(employeeVM);
            }
        }
    }
}
