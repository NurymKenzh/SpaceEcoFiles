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
    public class DocFormatsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DocFormatsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: DocFormats
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Index(string SortOrder,
            string NameFilter,
            int? PageNumber)
        {
            var docFormats = _context.DocFormat
                .ToList();

            ViewBag.NameFilter = NameFilter;

            ViewBag.NameSort = SortOrder == "Name" ? "NameDesc" : "Name";

            if (!string.IsNullOrEmpty(NameFilter))
            {
                docFormats = docFormats.Where(b => b.Name.ToLower().Contains(NameFilter.ToLower())).ToList();
            }

            switch (SortOrder)
            {
                case "Name":
                    docFormats = docFormats.OrderBy(b => b.Name).ToList();
                    break;
                case "NameDesc":
                    docFormats = docFormats.OrderByDescending(b => b.Name).ToList();
                    break;
                default:
                    docFormats = docFormats.OrderBy(b => b.Id).ToList();
                    break;
            }

            ViewBag.SortOrder = SortOrder;

            var pager = new Pager(docFormats.Count(), PageNumber);

            var viewModel = new DocFormatIndexPageViewModel
            {
                Items = docFormats.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };

            return View(viewModel);
        }

        // GET: DocFormats/Details/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var docFormat = await _context.DocFormat
                .FirstOrDefaultAsync(m => m.Id == id);
            if (docFormat == null)
            {
                return NotFound();
            }

            return View(docFormat);
        }

        // GET: DocFormats/Create
        [Authorize(Roles = "Administrator, Moderator")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: DocFormats/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Administrator, Moderator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,NameRU,NameKK,NameEN")] DocFormat docFormat)
        {
            if (ModelState.IsValid)
            {
                _context.Add(docFormat);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(docFormat);
        }

        // GET: DocFormats/Edit/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var docFormat = await _context.DocFormat.FindAsync(id);
            if (docFormat == null)
            {
                return NotFound();
            }
            return View(docFormat);
        }

        // POST: DocFormats/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Administrator, Moderator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NameRU,NameKK,NameEN")] DocFormat docFormat)
        {
            if (id != docFormat.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(docFormat);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DocFormatExists(docFormat.Id))
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
            return View(docFormat);
        }

        // GET: DocFormats/Delete/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var docFormat = await _context.DocFormat
                .FirstOrDefaultAsync(m => m.Id == id);
            if (docFormat == null)
            {
                return NotFound();
            }

            return View(docFormat);
        }

        // POST: DocFormats/Delete/5
        [Authorize(Roles = "Administrator, Moderator")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var docFormat = await _context.DocFormat.FindAsync(id);
            _context.DocFormat.Remove(docFormat);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DocFormatExists(int id)
        {
            return _context.DocFormat.Any(e => e.Id == id);
        }
    }
}
