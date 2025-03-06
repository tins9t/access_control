using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ssd_authorization_solution.DTOs;
using ssd_authorization_solution.Entities;

namespace MyApp.Namespace;

[Route("api/[controller]")]
[ApiController]
public class ArticleController : ControllerBase
{
    private readonly AppDbContext db;

    public ArticleController(AppDbContext ctx)
    {
        this.db = ctx;
    }

    [HttpGet]
    public IEnumerable<ArticleDto> Get()
    {
        return db.Articles.Include(x => x.Author).Select(ArticleDto.FromEntity);
    }

    [HttpGet(":id")]
    public ArticleDto? GetById(int id)
    {
        return db
            .Articles.Include(x => x.Author)
            .Where(x => x.Id == id)
            .Select(ArticleDto.FromEntity)
            .SingleOrDefault();
    }

    [HttpPost]
    [Authorize(Roles = "Writer")]
    public ArticleDto Post([FromBody] ArticleFormDto dto)
    {
        var userName = HttpContext.User.Identity?.Name;
        var author = db.Users.Single(x => x.UserName == userName);
        var entity = new Article
        {
            Title = dto.Title,
            Content = dto.Content,
            Author = author,
            CreatedAt = DateTime.Now
        };
        var created = db.Articles.Add(entity).Entity;
        db.SaveChanges();
        return ArticleDto.FromEntity(created);
    }

    [HttpPut(":id")]
    [Authorize(Roles = "Writer,Editor")]
    public IActionResult Put(int id, [FromBody] ArticleFormDto dto)
    {
        var userName = HttpContext.User.Identity?.Name;
        var entity = db
            .Articles
            .Include(x => x.Author)
            .Single(x => x.Id == id);
        
        var author = db.Users.Single(x => x.UserName == userName);

        if (entity == null)
        {
            return NotFound();
        }

        if (User.IsInRole(Roles.Writer) && entity.AuthorId != author.Id)
        {
            return Forbid(); 
        }
        
        entity.Title = dto.Title;
        entity.Content = dto.Content;
        var updated = db.Articles.Update(entity).Entity;
        db.SaveChanges();
        
        return Ok(ArticleDto.FromEntity(updated));
    }
    
    [HttpDelete("{id}")] 
    [Authorize(Roles = "Writer,Editor")] 
    public IActionResult Delete(int id)
    {
        var entity = db.Articles.SingleOrDefault(x => x.Id == id);
        
        var userName = HttpContext.User.Identity?.Name;
        var author = db.Users.Single(x => x.UserName == userName);

        
        if (entity == null)
        {
            return NotFound();
        }
        
        if (User.IsInRole(Roles.Writer) && entity.AuthorId != author.Id)
        {
            return Forbid(); 
        }

        db.Articles.Remove(entity);
        db.SaveChanges();

        return Ok(new { message = "Article deleted successfully" });
    }
}
