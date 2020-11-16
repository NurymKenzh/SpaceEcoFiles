using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SpaceEcoFiles.Data;
using SpaceEcoFiles.Models;

namespace SpaceEcoFiles.Controllers
{
    public class DocTypesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DocTypesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: DocTypes
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Index(string SortOrder,
            string NameFilter,
            int? PageNumber)
        {
            var docTypes = _context.DocType
                //.Where(b => true);
                .ToList();

            ViewBag.NameFilter = NameFilter;

            ViewBag.NameSort = SortOrder == "Name" ? "NameDesc" : "Name";

            if (!string.IsNullOrEmpty(NameFilter))
            {
                docTypes = docTypes.Where(b => b.Name.ToLower().Contains(NameFilter.ToLower())).ToList();
            }

            switch (SortOrder)
            {
                case "Name":
                    docTypes = docTypes.OrderBy(b => b.Name).ToList();
                    break;
                case "NameDesc":
                    docTypes = docTypes.OrderByDescending(b => b.Name).ToList();
                    break;
                default:
                    docTypes = docTypes.OrderBy(b => b.Id).ToList();
                    break;
            }

            ViewBag.SortOrder = SortOrder;

            var pager = new Pager(docTypes.Count(), PageNumber);

            var viewModel = new DocTypeIndexPageViewModel
            {
                Items = docTypes.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };

            return View(viewModel);
        }

        // GET: DocTypes/Details/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var docType = await _context.DocType
                .FirstOrDefaultAsync(m => m.Id == id);
            if (docType == null)
            {
                return NotFound();
            }

            return View(docType);
        }

        // GET: DocTypes/Create
        [Authorize(Roles = "Administrator, Moderator")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: DocTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Administrator, Moderator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,NameRU,NameKK,NameEN")] DocType docType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(docType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(docType);
        }

        // GET: DocTypes/Edit/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var docType = await _context.DocType.FindAsync(id);
            if (docType == null)
            {
                return NotFound();
            }
            return View(docType);
        }

        // POST: DocTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Administrator, Moderator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NameRU,NameKK,NameEN")] DocType docType)
        {
            if (id != docType.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(docType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DocTypeExists(docType.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(docType);
        }

        // GET: DocTypes/Delete/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var docType = await _context.DocType
                .FirstOrDefaultAsync(m => m.Id == id);
            if (docType == null)
            {
                return NotFound();
            }

            return View(docType);
        }

        // POST: DocTypes/Delete/5
        [Authorize(Roles = "Administrator, Moderator")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var docType = await _context.DocType.FindAsync(id);
            _context.DocType.Remove(docType);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DocTypeExists(int id)
        {
            return _context.DocType.Any(e => e.Id == id);
        }
    }
}
