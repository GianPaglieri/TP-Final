using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PaginaRedSocial.Data;
using PaginaRedSocial.Models;

namespace PaginaRedSocial.Controllers
{
    public class PostTagsController : Controller
    {
        private readonly MyContext _context;

        public PostTagsController(MyContext context)
        {
            _context = context;
        }

        // GET: PostTags
        public async Task<IActionResult> Index()
        {
            var myContext = _context.PostTag.Include(p => p.Post).Include(p => p.Tag);
            return View(await myContext.ToListAsync());
        }

        // GET: PostTags/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.PostTag == null)
            {
                return NotFound();
            }

            var postTag = await _context.PostTag
                .Include(p => p.Post)
                .Include(p => p.Tag)
                .FirstOrDefaultAsync(m => m.TagId == id);
            if (postTag == null)
            {
                return NotFound();
            }

            return View(postTag);
        }

        // GET: PostTags/Create
        public IActionResult Create()
        {
            ViewData["PostId"] = new SelectList(_context.Posts, "Id", "Id");
            ViewData["TagId"] = new SelectList(_context.tags, "Id", "Id");
            return View();
        }

        // POST: PostTags/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PostId,TagId")] PostTag postTag)
        {
            if (ModelState.IsValid)
            {
                _context.Add(postTag);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PostId"] = new SelectList(_context.Posts, "Id", "Id", postTag.PostId);
            ViewData["TagId"] = new SelectList(_context.tags, "Id", "Id", postTag.TagId);
            return View(postTag);
        }

        // GET: PostTags/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.PostTag == null)
            {
                return NotFound();
            }

            var postTag = await _context.PostTag.FindAsync(id);
            if (postTag == null)
            {
                return NotFound();
            }
            ViewData["PostId"] = new SelectList(_context.Posts, "Id", "Id", postTag.PostId);
            ViewData["TagId"] = new SelectList(_context.tags, "Id", "Id", postTag.TagId);
            return View(postTag);
        }

        // POST: PostTags/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PostId,TagId")] PostTag postTag)
        {
            if (id != postTag.TagId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(postTag);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostTagExists(postTag.TagId))
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
            ViewData["PostId"] = new SelectList(_context.Posts, "Id", "Id", postTag.PostId);
            ViewData["TagId"] = new SelectList(_context.tags, "Id", "Id", postTag.TagId);
            return View(postTag);
        }

        // GET: PostTags/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.PostTag == null)
            {
                return NotFound();
            }

            var postTag = await _context.PostTag
                .Include(p => p.Post)
                .Include(p => p.Tag)
                .FirstOrDefaultAsync(m => m.TagId == id);
            if (postTag == null)
            {
                return NotFound();
            }

            return View(postTag);
        }

        // POST: PostTags/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.PostTag == null)
            {
                return Problem("Entity set 'MyContext.PostTag'  is null.");
            }
            var postTag = await _context.PostTag.FindAsync(id);
            if (postTag != null)
            {
                _context.PostTag.Remove(postTag);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostTagExists(int id)
        {
            return _context.PostTag.Any(e => e.TagId == id);
        }
    }
}
