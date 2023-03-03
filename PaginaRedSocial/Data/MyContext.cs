using Microsoft.EntityFrameworkCore;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


using Microsoft.AspNetCore.Authentication.Cookies;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using System.Reflection;

using Microsoft.Extensions.Options;
using PaginaRedSocial.Models;
using PaginaRedSocial;
using PaginaRedSocial.Helpers;

namespace PaginaRedSocial.Data
{
    public class MyContext : DbContext
    {
        public MyContext(DbContextOptions<MyContext> optionsBuilder) : base(optionsBuilder) { }


        public DbSet<Post> Posts { get; set; }
        public DbSet<User> Usuarios { get; set; }
        public DbSet<Reaccion> reacciones { get; set; }
        public DbSet<Tag> tags { get; set; }
        
        public DbSet<TipoReaccion> tiposReacciones { get; set; }

        public DbSet<Comentario> comentarios { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {


            modelBuilder.Entity<User>(usuario =>
            {
                usuario.Property(u => u.Nombre).HasColumnType("varchar(80)").IsRequired(true);
                usuario.Property(u => u.Dni).HasColumnType("int").IsRequired(true);
                usuario.HasIndex(u => u.Dni).IsUnique();
                usuario.Property(u => u.Email).HasColumnType("varchar(30)").IsRequired(true);
                usuario.HasIndex(u => u.Email).IsUnique();
                usuario.Property(u => u.Password).HasColumnType("varchar(200)").IsRequired(true);
                usuario.Property(u => u.Intentos).HasColumnType("int");
            });

            modelBuilder.Entity<Reaccion>()
                .ToTable("Reacciones")
                .HasKey(r => r.Id);

            modelBuilder.Entity<Comentario>()
                .ToTable("Comentarios")
                .HasKey(c => c.Id);

            modelBuilder.Entity<Tag>()
                .ToTable("Tags")
                .HasKey(t => t.Id);

            modelBuilder.Entity<TipoReaccion>()
                .ToTable("TipoReacciones")
                .HasKey(tr => tr.Id);

            modelBuilder.Entity<Post>()
            .HasOne(D => D.user)
            .WithMany(U => U.posts)
            .HasForeignKey(D => D.UserId)
            .OnDelete(DeleteBehavior.Cascade);

            //propiedades de los datos de Post
            modelBuilder.Entity<Post>(
                post =>
                {
                    post.Property(p => p.Contenido).HasColumnType("longtext");
                    post.Property(p => p.Fecha).HasColumnType("datetime");
                });


            //DEFINICIÓN DE LA RELACIÓN ONE TO MANY USUARIO -> COMENTARIO
            modelBuilder.Entity<Comentario>()
            .HasOne(comentario => comentario.Usuario)
            .WithMany(usuario => usuario.MisComentarios)
            .HasForeignKey(comentario => comentario.UsuarioId)
            .OnDelete(DeleteBehavior.NoAction);

            //DEFINICIÓN DE LA RELACIÓN ONE TO MANY POST -> COMENTARIO
            modelBuilder.Entity<Comentario>()
            .HasOne(comentario => comentario.Post)
            .WithMany(post => post.Comentarios)
            .HasForeignKey(comentario => comentario.PostId)
            .OnDelete(DeleteBehavior.Cascade);

            //DEFINICIÓN DE LA RELACIÓN ONE TO MANY POST -> REACCION
            modelBuilder.Entity<Reaccion>()
            .HasOne(R => R.Post)
            .WithMany(P => P.Reacciones)
            .HasForeignKey(R => R.PostId)
            .OnDelete(DeleteBehavior.Cascade);

            //DEFINICIÓN DE LA RELACIÓN ONE TO MANY USUARIO -> REACCION
            modelBuilder.Entity<Reaccion>()
            .HasOne(R => R.User)
            .WithMany(U => U.MisReacciones)
            .HasForeignKey(R => R.UsuarioId)
            .OnDelete(DeleteBehavior.NoAction);

            //DEFINICIÓN DE LA RELACIÓN ONE TO MANY TIPO_REACCION -> REACCION
            modelBuilder.Entity<Reaccion>()
            .HasOne(reaccion => reaccion.TipoReaccion)
            .WithMany(tipoReaccion => tipoReaccion.Reacciones)
            .HasForeignKey(reaccion => reaccion.TipoReaccionId)
            .OnDelete(DeleteBehavior.Cascade);

            //DEFINICIÓN DE LA RELACIÓN MANY TO MANY POST -> TAG
            modelBuilder.Entity<Post>()
                .HasMany(post => post.Tags)
                .WithMany(tag => tag.Posts)
                .UsingEntity<PostTag>(
                    ept => ept.HasOne(pt => pt.Tag).WithMany(p => p.PostTags).HasForeignKey(p => p.TagId),
                    ept => ept.HasOne(up => up.Post).WithMany(u => u.PostTags).HasForeignKey(t => t.PostId),
                    ept => ept.HasKey(k => new { k.TagId, k.PostId })
                );

            //DEFINICIÓN DE LA RELACIÓN MANY TO MANY  USUARIOS -> USUARIOS (AMIGOS)
            modelBuilder.Entity<UsuarioAmigo>()
                .HasOne(UA => UA.Usuario)
                .WithMany(U => U.misAmigos)
                .HasForeignKey(u => u.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<UsuarioAmigo>()
                .HasOne(UA => UA.Amigo)
                .WithMany(U => U.amigosMios)
                .HasForeignKey(u => u.AmigoId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<UsuarioAmigo>()
                .HasKey(k => new { k.UsuarioId, k.AmigoId });


            modelBuilder.Entity<User>()
            .HasData(
            new User { Id = 1, Nombre = "Admin", Dni = 123123, Email = "admin@gmail.com", Bloqueado = false, IsAdmin = true, Password = Utils.Encriptar("123"), Intentos = 0 }
            ,
            new User { Id = 2, Nombre = "Juan", Dni = 12345678, Email = "juan@gmail.com", Bloqueado = false, IsAdmin = false, Password = Utils.Encriptar("123"), Intentos = 0 }
            );
            modelBuilder.Entity<Post>()
               .HasData(
               new Post { Id = 1, Contenido = "post de Juan", Fecha = DateTime.Now, UserId = 2 }
               );
            modelBuilder.Entity<TipoReaccion>().HasData(
                new { Id = 1, Palabra = "Me gusta" },
                new { Id = 2, Palabra = "Me encanta" },
                new { Id = 3, Palabra = "Me divierte" },
                new { Id = 4, Palabra = "Me entristece" }
            );
        }


        public DbSet<PaginaRedSocial.Models.PostTag> PostTag { get; set; }


        public DbSet<PaginaRedSocial.Models.UsuarioAmigo> UsuarioAmigo { get; set; }
    }
}

