
using DAIS.Models.Repositories.Interfaces;
using DAIS.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DAIS.Controllers
{
    public class AccountController : Controller
    {
        public IEmployeeRepository _employeeRepository;

        public AccountController(IEmployeeRepository employeeRepository)
        {
            this._employeeRepository = employeeRepository;
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginEmployeeViewModel model, string returnUrl)
        {
            if (!this.ModelState.IsValid)
            {
                return View(model);
            }

            var employee = await this._employeeRepository.GetEmployeeByUserNameAndPassword(model.UserName, model.Password);

            if (employee != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, model.UserName),
                    new Claim("EmployeeName", employee.Name)
                };

                var userIdentity = new ClaimsIdentity(claims, "Login");

                ClaimsPrincipal principal = new ClaimsPrincipal(userIdentity);
                await this.HttpContext.SignInAsync(principal);

                if (!String.IsNullOrEmpty(returnUrl) && this.Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }

            }
            else
            {
                this.ModelState.AddModelError("", "Login Failed");
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {

            await this.HttpContext.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }
    }
}
