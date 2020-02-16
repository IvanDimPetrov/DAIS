using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAIS.Models;
using DAIS.Models.Repositories.Interfaces;
using DAIS.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DAIS.Controllers
{
    [Authorize]
    public class VotesController : Controller
    {
        private readonly IVotesRepository _votesRepository;
        private readonly IEmployeeRepository _employeeRepository;

        public VotesController(IVotesRepository voteRepository, IEmployeeRepository employeeRepository)
        {
            this._votesRepository = voteRepository;
            this._employeeRepository = employeeRepository;
        }

        [HttpGet]
        public async Task<IActionResult> ActiveVotes()
        {
            var loggedUser = this.HttpContext.User.Identity.Name;

            var votes = await this._votesRepository.GetAllActiveVotes(loggedUser);
            return View(votes);
        }

        [HttpGet]
        public async Task<IActionResult> MyVotes()
        {
            var loggedUser = this.HttpContext.User.Identity.Name;

            var votes = await this._votesRepository.GetAllInitiatedFromEmployeeVotes(loggedUser);

            return View(votes);
        }

        [HttpGet]
        public async Task<IActionResult> SuspendedVotes()
        {
            var loggedUser = this.HttpContext.User.Identity.Name;

            var votes = await this._votesRepository.GetAllSuspendedVotes(loggedUser);

            return View(votes);
        }


        public async Task<IActionResult> Details(int id)
        {
            var details = await this._votesRepository.GetVoteDetails(id);

            var votingDetails = new Dictionary<string, List<string>>();

            var giftsDeatils = new Dictionary<string, int>();

            if (details[0][1] != "" && details[0][1] != "")
            {
                foreach (var row in details)
                {
                    votingDetails.Add(row[1], new List<string>() { row[2], row[3] });
                }

                var grp = votingDetails.Values.ToList().GroupBy(x => x[1]);

                foreach (var gr in grp)
                {
                    giftsDeatils.Add(gr.Key, gr.Count());
                }
            }
            
            var model = new DetailsVoteViewModel()
            {
                ReceiverName = details[0][0],
                VotingDetails = votingDetails,
                GiftsStatistic = giftsDeatils.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value)
            };
            

            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new CreateVoteViewModel()
            {
                EmployeeList = new SelectList(await this.GetEmployeeWithoutVote(), "UserName", "Name"),
                Owner = this.User.Identity.Name,
            };

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Create(CreateVoteViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                model.EmployeeList = new SelectList(await this.GetEmployeeWithoutVote(), "UserName", "Name");
                return View(model);
            }

            var voteToInsert = new Vote()
            {
                Owner = model.Owner,
                Receiver = model.Reciever,
                IsActive = true
            };

            await this._votesRepository.InsertVote(voteToInsert);

            return RedirectToAction("ActiveVotes", "Votes");
        }


        [HttpGet]
        public async Task<IActionResult> Voting(int id)
        {       
            var gifts = await this._votesRepository.GetAllGifts();

            var model = new VotingViewModel()
            {
                Gifts = new SelectList(gifts, "Id", "Name"),
                LoggedUser = this.HttpContext.User.Identity.Name,
                VoteId = id
            };

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Voting(VotingViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                model.Gifts = new SelectList(await this._votesRepository.GetAllGifts(), "Id", "Name");
                return View(model);
            }

            await this._votesRepository.VoteForGift(model);

            return RedirectToAction("ActiveVotes", "Votes");
        }

        [HttpPost]
        public async Task<IActionResult> SuspendVote(string id)
        {
            await this._votesRepository.SuspendVote(int.Parse(id));

            return Json("/Votes/MyVotes");
        }


        private async Task<List<Employee>> GetEmployeeWithoutVote()
        {
            bool validDate (DateTime birthdate)
            {
                var tempDate = new DateTime(DateTime.Now.Year, birthdate.Month, birthdate.Day);

                var daysDiff = (tempDate - DateTime.Now).TotalDays;

                if (daysDiff > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            };

            var loggedUserName = this.HttpContext.User.Identity.Name;

            var employees = await this._employeeRepository.GetEmployeesWithoutVote();
                                
            return employees.Where(x => x.UserName != loggedUserName && validDate(x.BirthDate))
                            .ToList();
        }
    }
}
