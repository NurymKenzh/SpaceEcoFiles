﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpaceEcoFiles.Data;
using SpaceEcoFiles.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpaceEcoFiles.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IServiceProvider _serviceProvider;

        public UsersController(
            UserManager<IdentityUser> userManager,
            ApplicationDbContext context,
            IServiceProvider serviceProvider)
        {
            _userManager = userManager;
            _context = context;
            _serviceProvider = serviceProvider;
        }

        [Authorize(Roles = "Administrator")]
        public IActionResult Index(string SortOrder, string Email, int? Page)
        {
            List<ApplicationUserViewModel> users = new List<ApplicationUserViewModel>();
            foreach (var iuser in _userManager.Users.ToList())
            {
                ApplicationUserViewModel user = ApplicationUserViewModel.CopyToApplicationUserViewModel(iuser);
                user.RoleNames = new List<string>() { };
                foreach (var role in _userManager.GetRolesAsync(iuser).Result)
                {
                    string srole = _context.Roles
                        .FirstOrDefault(r => r.Name == role)
                        .Name;
                    user.RoleNames.Add(srole);
                }
                users.Add(user);
            }
            users = users.ToList();

            ViewBag.EmailFilter = Email;

            ViewBag.EmailSort = SortOrder == "Email" ? "EmailDesc" : "Email";

            if (!string.IsNullOrEmpty(Email))
            {
                users = users.Where(u => u.Email.ToLower().Contains(Email.ToLower())).ToList();
            }

            switch (SortOrder)
            {
                case "Email":
                    users = users.OrderBy(u => u.Email).ToList();
                    break;
                case "EmailDesc":
                    users = users.OrderByDescending(u => u.Email).ToList();
                    break;
                default:
                    users = users.OrderBy(u => u.Id).ToList();
                    break;
            }
            ViewBag.SortOrder = SortOrder;

            var pager = new Pager(users.Count(), Page);

            var viewModel = new ApplicationUserIndexPageViewModel
            {
                Items = users.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };

            return View(viewModel);
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var iuser = await _context.Users.SingleOrDefaultAsync(u => u.Id == id);
            if (iuser == null)
            {
                return NotFound();
            }
            ApplicationUserViewModel user = ApplicationUserViewModel.CopyToApplicationUserViewModel(iuser);
            foreach (var role in await _userManager.GetRolesAsync(iuser))
            {
                user.RoleNames.Add(role);
            }
            ViewBag.Roles = _context.Roles
                .OrderBy(r => r.Name)
                .Select(r => r.Name)
                .ToList();
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(string id, [Bind("Id,RoleNames")] ApplicationUserViewModel user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                IdentityUser auser = await _context.Users.SingleOrDefaultAsync(u => u.Id == user.Id);
                await _userManager.RemoveFromRolesAsync(auser, await _userManager.GetRolesAsync(auser));
                await _userManager.AddToRolesAsync(auser, user.RoleNames == null ? new List<string>() : user.RoleNames);

                return RedirectToAction("Index");
            }
            return View(user);
        }

        private bool UserExists(string id)
        {
            return _context.Users.Any(u => u.Id == id);
        }
    }
}
