using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Threading.Tasks;
using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PaginaRedSocial.Data;
using PaginaRedSocial.Models;
using static System.Net.Mime.MediaTypeNames;

namespace PaginaRedSocial.Controllers
{
    public class PostsController : Controller
    {
        private readonly MyContext _context;
        private SoundPlayer _soundPlayer;

        public PostsController(MyContext context)
        {
            _context = context;
        }

        // GET: Posts
        public async Task<IActionResult> Index()
        {
            if (!this._context.Usuarios.Find(int.Parse(User.Identity.Name)).IsAdmin)
                return Redirect("/Posts/all?message=No-tenes-permiso-de-administrador");
            var myContext = _context.Posts.Include(p => p.user);
            return View(await myContext.ToListAsync());
        }

        // GET: Posts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Posts == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.user)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // GET: Posts/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Usuarios, "Id", "Email");
            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Contenido,Fecha,UserId")] Post post)
        {
            if (ModelState.IsValid)
            {
                _context.Add(post);
                await _context.SaveChangesAsync();
                _soundPlayer = new SoundPlayer("Resources/SuccessSound.wav");
                _soundPlayer.Play();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Usuarios, "Id", "Email", post.UserId);
            return View(post);
        }

        // GET: Posts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Posts == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Usuarios, "Id", "Email", post.UserId);
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Contenido,Fecha,UserId")] Post post)
        {
            if (id != post.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(post);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.Id))
                    {
                        _soundPlayer = new SoundPlayer("Resources/SuccessSound.wav");
                        _soundPlayer.Play();
                        return NotFound();
                    }
                    else
                    {
                        _soundPlayer = new SoundPlayer("Resources/ErrorSound.wav");
                        _soundPlayer.Play();
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Usuarios, "Id", "Email", post.UserId);
            return View(post);
        }

        // GET: Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Posts == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.user)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Posts == null)
            {
                return Problem("Entity set 'MyContext.Posts'  is null.");
            }
            var post = await _context.Posts.FindAsync(id);
            if (post != null)
            {
                _soundPlayer = new SoundPlayer("Resources/SuccessSound.wav");
                _soundPlayer.Play();
                _context.Posts.Remove(post);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }

        [HttpPost]
        public IActionResult CreatePost(Microsoft.AspNetCore.Http.IFormCollection collection)
        {
            int userId = int.Parse(@User.Identity.Name);
            var userActual = this._context.Usuarios.Include(u => u.misAmigos)
                .Where(user => user.Id == userId)
                .FirstOrDefault();
            Post newPost = new Post();
            newPost.Contenido = collection["postContent"];
            newPost.user = userActual;
            newPost.Fecha = DateTime.Now;

            List<Tag> postTags = new List<Tag>();
            postTags = this.getTags(collection["postContent"]);
            if (postTags.Count > 0)
            {
                this.setTagsToPost(newPost, postTags);
            }

            this._context.Posts.Add(newPost);
            this._context.SaveChanges();

            return Redirect("/Home/MisPosts");
        }

        private void setTagsToPost(Post post, List<Tag> tags)
        {
            foreach (Tag tag in tags)
            {
                Tag tagEncontrado = this._context.tags
                    .Where(t => t.Palabra == tag.Palabra)
                    .FirstOrDefault();

                if (tagEncontrado != null)
                {
                    post.Tags.Add(tagEncontrado);
                }
                else
                {
                    post.Tags.Add(tag);
                }
            }
        }

        private List<Tag> getTags(String text)
        {
            List<Tag> postTags = new List<Tag>();
            if (!string.IsNullOrEmpty(text))
            {
                string[] firstFilter = text.Split(' ');
                foreach (string filter in firstFilter)
                {
                    if (filter.Contains("#"))
                    {
                        string[] tags = filter.Split('#');
                        for (int i = 1; i < tags.Length; i++)
                        {
                            Console.WriteLine("tag: " + tags[i]);
                            Tag newTag = new Tag { Palabra = tags[i].Trim() };
                            postTags.Add(newTag);
                        }
                    }
                }
            }

            return postTags;
        }

        public async Task<IActionResult> EliminarPost()
        {
            string idQuery = HttpContext.Request.Query["id"].ToString();
            int postId = Int16.Parse(idQuery);

            var post = this._context.Posts.Where(post => post.Id == postId).FirstOrDefault();

            this._context.Posts.Remove(post);
            this._context.SaveChanges();

            return Redirect("/Home/MisPosts");
        }

        public string SetReaction()
        {
            int postId = Int16.Parse(HttpContext.Request.Query["postId"].ToString());
            int reactionId = Int16.Parse(HttpContext.Request.Query["reactionId"].ToString());
            int fromUserId = Int16.Parse(HttpContext.Request.Query["fromUserId"].ToString());

            User fromUser = this._context.Usuarios.Where(u => u.Id == fromUserId).FirstOrDefault();
            Post post = this._context.Posts.Where(post => post.Id == postId)
                                .Include(post => post.Reacciones)
                                .FirstOrDefault();

            Reaccion existingReaction = post.Reacciones
                                        .Where(reaction => reaction.PostId == postId)
                                        .Where(reaction => reaction.UsuarioId == fromUserId)
                                        .FirstOrDefault();

            if (existingReaction != null )
            {
                if (existingReaction.TipoReaccionId == reactionId)
                {
                    // Si el nuevo tipo de reacción es igual a la que ya existía
                    // entonces se elimina la reacción
                    post.Reacciones.Remove(existingReaction);
                }
                else
                {
                    // Si es distinta entonces solamente se cambia el tipo de reacción
                    existingReaction.TipoReaccionId = reactionId;
                }
            }
            else
            {
                // Si no existe la reacción entonces se agrega
                Reaccion newReaccion = new Reaccion();
                newReaccion.TipoReaccionId = reactionId;
                newReaccion.User = fromUser;
                newReaccion.Post = post;
                post.Reacciones.Add(newReaccion);
            }
           
            this._context.Posts.Update(post);
            this._context.SaveChanges();

            return "ok";
        }

        public async Task<IActionResult> EditarPost()
        {
            string idQuery = HttpContext.Request.Query["id"].ToString();
            int postId = Int16.Parse(idQuery);

            var post = this._context.Posts.Where(post => post.Id == postId).FirstOrDefault();

            return View("/Views/Home/EditarMiPost/index.cshtml", post);
        }


        [HttpPost]
        public IActionResult EditMyPost(Microsoft.AspNetCore.Http.IFormCollection collection)
        {

            int postId = Int16.Parse(collection["postId"]);

            var post = this._context.Posts.Where(post => post.Id == postId)
                .Include(p => p.Tags)
                .Include(p => p.PostTags)
                .FirstOrDefault();
            string text = collection["postContent"];
            List<Tag> postTags = new List<Tag>();

            postTags = this.getTags(collection["postContent"].ToString());

            if (postTags.Count > 0)
            {
                post.Tags.Clear();
                this.setTagsToPost(post, postTags);
            }
            else
            {
                post.Tags.Clear();
            }

            post.Contenido = collection["postContent"];

            this._context.Posts.Update(post);
            this._context.SaveChanges();

            return Redirect("/Home/MisPosts");
        }

    }
}
