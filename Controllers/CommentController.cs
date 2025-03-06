using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ssd_authorization_solution.DTOs;
using ssd_authorization_solution.Entities;

namespace MyApp.Namespace;

[Route("api/[controller]")]
[Authorize]
[ApiController]
public class CommentController : ControllerBase
{
    private readonly AppDbContext db;

    public CommentController(AppDbContext ctx)
    {
        this.db = ctx;
    }

    [HttpGet]
    [Authorize(Roles = "Writer,Editor,Subscriber")]
    public IEnumerable<CommentDto> Get([FromQuery] int? articleId)
    {
        var query = db.Comments.Include(x => x.Author).AsQueryable();
        if (articleId.HasValue)
            query = query.Where(c => c.ArticleId == articleId);
        return query.Select(CommentDto.FromEntity);
    }

    [HttpGet(":id")]
    [Authorize(Roles = "Writer,Editor,Subscriber")]
    public CommentDto? GetById(int id)
    {
        return db
            .Comments.Include(x => x.Author)
            .Select(CommentDto.FromEntity)
            .SingleOrDefault(x => x.Id == id);
    }

    [HttpPost]
    [Authorize(Roles = "Subscriber")]
    public CommentDto Post([FromBody] CommentFormDto dto)
    {
        var userName = HttpContext.User.Identity?.Name;
        var author = db.Users.Single(x => x.UserName == userName);
        var article = db.Articles.Single(x => x.Id == dto.ArticleId);
        var entity = new Comment
        {
            Content = dto.Content,
            Article = article,
            Author = author,
        };
        var created = db.Comments.Add(entity).Entity;
        db.SaveChanges();
        return CommentDto.FromEntity(created);
    }

    [HttpPut(":id")]
    [Authorize(Roles = "Subscriber,Editor")]
    public IActionResult Put(int id, [FromBody] CommentFormDto dto)
    {
        var userName = HttpContext.User.Identity?.Name;
        var entity = db
            .Comments.Include(x => x.Author)
            .Where(x => x.Author.UserName == userName)
            .Single(x => x.Id == id);
        
        var author = db.Users.Single(x => x.UserName == userName);
        
        if (entity == null)
        {
            return NotFound();
        }

        if (User.IsInRole(Roles.Subscriber) && entity.AuthorId != author.Id)
        {
           
            return Forbid(); 
        }
        
        entity.Content = dto.Content;
        var updated = db.Comments.Update(entity).Entity;
        db.SaveChanges();
        return Ok(CommentDto.FromEntity(updated));
    }
    
    [HttpDelete("{id}")]  
    [Authorize(Roles = "Subscriber,Editor")]  
    public IActionResult DeleteComment(int id)
    {
        var entity = db.Comments.Include(x => x.Author).SingleOrDefault(x => x.Id == id);
        
        var userName = HttpContext.User.Identity?.Name;
        var author = db.Users.Single(x => x.UserName == userName);

        
        if (entity == null)
        {
            return NotFound();
        }
        
        if (User.IsInRole(Roles.Subscriber) && entity.AuthorId != author.Id)
        {
           
            return Forbid(); 
        }
        
        db.Comments.Remove(entity);
        db.SaveChanges();

        return Ok(new { message = "Comment deleted successfully" });
    }
}
