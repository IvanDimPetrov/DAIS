using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DAIS.Models;
using DAIS.DataBase;
using DAIS.Models.Repositories;
using Microsoft.AspNetCore.Authorization;
using DAIS.Models.Repositories.Interfaces;

namespace DAIS.Controllers
{
    public class HomeController : Controller
    {
        private IEmployeeRepository _employeeRepository;
        public HomeController(IEmployeeRepository employeeRepository)
        {
            this._employeeRepository = employeeRepository;
        }

        public async Task<IActionResult> Index()
        {
            var employees = await this._employeeRepository.GetAllEmployees();
            return View(employees);
        }

    }
}
