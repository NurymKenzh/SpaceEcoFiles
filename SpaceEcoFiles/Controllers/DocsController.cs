﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using SpaceEcoFiles.Data;
using SpaceEcoFiles.Models;

namespace SpaceEcoFiles.Controllers
{
    public class DocsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostEnvironment _hostingEnvironment;
        private readonly IStringLocalizer<SharedResources> _sharedLocalizer;

        public DocsController(ApplicationDbContext context,
            IHostEnvironment hostingEnvironment,
            IStringLocalizer<SharedResources> sharedLocalizer)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _sharedLocalizer = sharedLocalizer;
        }

        // GET: Docs
        [Authorize]
        public async Task<IActionResult> Index(string SortOrder,
            string TitleFilter,
            DateTime? DateFilter,
            Language? LanguageFilter,
            int? DocTypeIdFilter,
            int? DocFormatIdFilter,
            string FileFilter,
            int? DownloadsCountFilter,
            int? PageNumber)
        {
            var docs = _context.Doc
                .Include(d => d.DocFormat)
                .Include(d => d.DocType)
                .ToList();

            ViewBag.TitleFilter = TitleFilter;
            ViewBag.DateFilter = DateFilter;
            ViewBag.LanguageFilter = LanguageFilter;
            ViewBag.DocTypeIdFilter = DocTypeIdFilter;
            ViewBag.DocFormatIdFilter = DocFormatIdFilter;
            ViewBag.FileFilter = FileFilter;
            ViewBag.DownloadsCountFilter = DownloadsCountFilter;

            ViewBag.TitleSort = SortOrder == "Title" ? "TitleDesc" : "Title";
            ViewBag.DateSort = SortOrder == "Date" ? "DateDesc" : "Date";
            ViewBag.LanguageSort = SortOrder == "Language" ? "LanguageDesc" : "Language";
            ViewBag.DocTypeNameSort = SortOrder == "DocTypeName" ? "DocTypeNameDesc" : "DocTypeName";
            ViewBag.DocFormatNameSort = SortOrder == "DocFormatName" ? "DocFormatNameDesc" : "DocFormatName";
            ViewBag.FileSort = SortOrder == "File" ? "FileDesc" : "File";
            ViewBag.DownloadsCountSort = SortOrder == "DownloadsCount" ? "DownloadsCountDesc" : "DownloadsCount";

            if (!string.IsNullOrEmpty(TitleFilter))
            {
                docs = docs.Where(b => b.Title.ToLower().Contains(TitleFilter.ToLower())).ToList();
            }
            if (DateFilter != null)
            {
                docs = docs.Where(b => b.Date == DateFilter).ToList();
            }
            if (LanguageFilter != null)
            {
                docs = docs.Where(b => b.Language == LanguageFilter).ToList();
            }
            if (DocTypeIdFilter != null)
            {
                docs = docs.Where(b => b.DocTypeId == DocTypeIdFilter).ToList();
            }
            if (DocFormatIdFilter != null)
            {
                docs = docs.Where(b => b.DocFormatId == DocFormatIdFilter).ToList();
            }
            if (!string.IsNullOrEmpty(FileFilter))
            {
                docs = docs.Where(b => b.File.ToLower().Contains(FileFilter.ToLower())).ToList();
            }
            if (DownloadsCountFilter != null)
            {
                docs = docs.Where(b => b.DownloadsCount == DownloadsCountFilter).ToList();
            }

            switch (SortOrder)
            {
                case "Title":
                    docs = docs.OrderBy(b => b.Title).ToList();
                    break;
                case "TitleDesc":
                    docs = docs.OrderByDescending(b => b.Title).ToList();
                    break;
                case "Date":
                    docs = docs.OrderBy(b => b.Date).ToList();
                    break;
                case "DateDesc":
                    docs = docs.OrderByDescending(b => b.Date).ToList();
                    break;
                case "Language":
                    docs = docs.OrderBy(b => b.Language).ToList();
                    break;
                case "LanguageDesc":
                    docs = docs.OrderByDescending(b => b.Language).ToList();
                    break;
                case "DocTypeName":
                    docs = docs.OrderBy(b => b.DocType.Name).ToList();
                    break;
                case "DocTypeNameDesc":
                    docs = docs.OrderByDescending(b => b.DocType.Name).ToList();
                    break;
                case "DocFormatName":
                    docs = docs.OrderBy(b => b.DocFormat.Name).ToList();
                    break;
                case "DocFormatNameDesc":
                    docs = docs.OrderByDescending(b => b.DocFormat.Name).ToList();
                    break;
                case "File":
                    docs = docs.OrderBy(b => b.File).ToList();
                    break;
                case "FileDesc":
                    docs = docs.OrderByDescending(b => b.File).ToList();
                    break;
                case "DownloadsCount":
                    docs = docs.OrderBy(b => b.DownloadsCount).ToList();
                    break;
                case "DownloadsCountDesc":
                    docs = docs.OrderByDescending(b => b.DownloadsCount).ToList();
                    break;
                default:
                    docs = docs.OrderByDescending(b => b.Date).ToList();
                    break;
            }

            ViewBag.SortOrder = SortOrder;

            var pager = new Pager(docs.Count(), PageNumber);

            var viewModel = new DocIndexPageViewModel
            {
                Items = docs.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };

            ViewData["DocFormatId"] = new SelectList(_context.DocFormat.ToList().OrderBy(d => d.Name), "Id", "Name");
            ViewData["DocTypeId"] = new SelectList(_context.DocType.ToList().OrderBy(d => d.Name), "Id", "Name");

            return View(viewModel);
        }

        // GET: Docs/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doc = await _context.Doc
                .Include(d => d.DocFormat)
                .Include(d => d.DocType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (doc == null)
            {
                return NotFound();
            }

            return View(doc);
        }

        // GET: Docs/Create
        [Authorize(Roles = "Administrator, Moderator")]
        public IActionResult Create()
        {
            ViewData["DocFormatId"] = new SelectList(_context.DocFormat.ToList().OrderBy(d => d.Name), "Id", "Name");
            ViewData["DocTypeId"] = new SelectList(_context.DocType.ToList().OrderBy(d => d.Name), "Id", "Name");
            return View();
        }

        // POST: Docs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Create([Bind("Id,Title,Date,Language,DocTypeId,DocFormatId,FormFile")] Doc doc)
        {
            string path = "";
            if (doc.FormFile != null)
            {
                path = Path.Combine(_hostingEnvironment.ContentRootPath, "Files", doc.FormFile.FileName);
                if (System.IO.File.Exists(path))
                {
                    ModelState.AddModelError("FormFile", string.Format(_sharedLocalizer["TheFileAlreadyExists"], doc.FormFile.FileName));
                }
            }
            else
            {
                ModelState.AddModelError("FormFile", string.Format(_sharedLocalizer["TheFieldIsRequired"], _sharedLocalizer["File"]));
            }

            if (ModelState.IsValid)
            {
                doc.DownloadsCount = 0;
                using (Stream fileStream = new FileStream(path, FileMode.Create))
                {
                    await doc.FormFile.CopyToAsync(fileStream);
                    doc.File = doc.FormFile.FileName;
                }

                _context.Add(doc);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DocFormatId"] = new SelectList(_context.DocFormat.ToList().OrderBy(d => d.Name), "Id", "Name", doc.DocFormatId);
            ViewData["DocTypeId"] = new SelectList(_context.DocType.ToList().OrderBy(d => d.Name), "Id", "Name", doc.DocTypeId);
            return View(doc);
        }

        // GET: Docs/Edit/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doc = await _context.Doc.FindAsync(id);
            if (doc == null)
            {
                return NotFound();
            }
            ViewData["DocFormatId"] = new SelectList(_context.DocFormat.ToList().OrderBy(d => d.Name), "Id", "Name", doc.DocFormatId);
            ViewData["DocTypeId"] = new SelectList(_context.DocType.ToList().OrderBy(d => d.Name), "Id", "Name", doc.DocTypeId);
            return View(doc);
        }

        // POST: Docs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Date,Language,DocTypeId,DocFormatId,File,FormFile,DownloadsCount")] Doc doc)
        {
            if (id != doc.Id)
            {
                return NotFound();
            }

            if (doc.FormFile != null)
            {
                string path = Path.Combine(_hostingEnvironment.ContentRootPath, "Files", doc.FormFile.FileName);
                if (System.IO.File.Exists(path))
                {
                    ModelState.AddModelError("FormFile", string.Format(_sharedLocalizer["TheFileAlreadyExists"], doc.FormFile.FileName));
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (doc.FormFile != null)
                    {
                        doc.DownloadsCount = 0;
                        Doc _doc = await _context.Doc.AsNoTracking().FirstOrDefaultAsync(d => d.Id == doc.Id);
                        string path = Path.Combine(_hostingEnvironment.ContentRootPath, "Files", _doc.File);
                        System.IO.File.Delete(path);

                        path = Path.Combine(_hostingEnvironment.ContentRootPath, "Files", doc.FormFile.FileName);
                        using (Stream fileStream = new FileStream(path, FileMode.Create))
                        {
                            await doc.FormFile.CopyToAsync(fileStream);
                            doc.File = doc.FormFile.FileName;
                        }
                    }

                    _context.Update(doc);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DocExists(doc.Id))
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
            ViewData["DocFormatId"] = new SelectList(_context.DocFormat.ToList().OrderBy(d => d.Name), "Id", "Name", doc.DocFormatId);
            ViewData["DocTypeId"] = new SelectList(_context.DocType.ToList().OrderBy(d => d.Name), "Id", "Name", doc.DocTypeId);
            return View(doc);
        }

        // GET: Docs/Delete/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doc = await _context.Doc
                .Include(d => d.DocFormat)
                .Include(d => d.DocType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (doc == null)
            {
                return NotFound();
            }

            return View(doc);
        }

        // POST: Docs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var doc = await _context.Doc.FindAsync(id);

            string path = Path.Combine(_hostingEnvironment.ContentRootPath, "Files", doc.File);
            System.IO.File.Delete(path);

            _context.Doc.Remove(doc);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Download(string FileName)
        {
            var doc = _context.Doc.FirstOrDefault(d => d.File == FileName);
            doc.DownloadsCount++;
            _context.Update(doc);
            _context.SaveChanges();

            var fileName = Path.Combine(_hostingEnvironment.ContentRootPath, "Files", FileName);
            var net = new System.Net.WebClient();
            var data = net.DownloadData(fileName);
            var content = new System.IO.MemoryStream(data);
            var contentType = "application/octet-stream";
            return File(content, contentType, FileName);
        }

        [HttpGet]
        public IActionResult DownloadShow(string FileName)
        {
            var fileName = Path.Combine(_hostingEnvironment.ContentRootPath, "Files", FileName);
            var net = new System.Net.WebClient();
            var data = net.DownloadData(fileName);
            var content = new System.IO.MemoryStream(data);
            var contentType = "application/octet-stream";
            return File(content, contentType, FileName);
        }

        [HttpGet]
        [Authorize]
        public IActionResult Show(string FileName)
        {
            var fileName = Path.Combine(_hostingEnvironment.ContentRootPath, "Files", FileName);
            var contentType = "application/octet-stream";
            if (Path.GetExtension(FileName).ToLower() == ".jpg" || Path.GetExtension(FileName).ToLower() == ".jpeg")
            {
                contentType = "image/jpeg";
                return new PhysicalFileResult(fileName, contentType);
            }
            if (Path.GetExtension(FileName).ToLower() == ".png")
            {
                contentType = "image/png";
                return new PhysicalFileResult(fileName, contentType);
            }
            if (Path.GetExtension(FileName).ToLower() == ".pdf")
            {
                contentType = "application/pdf";
                return new PhysicalFileResult(fileName, contentType);
            }
            return new PhysicalFileResult(fileName, contentType);
        }

        private bool DocExists(int id)
        {
            return _context.Doc.Any(e => e.Id == id);
        }
    }
}
