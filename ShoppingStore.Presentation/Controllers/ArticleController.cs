using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ShoppingStore.Domain.Interfaces;
using ShoppingStore.Domain.Entities;
using System.Data;

namespace ShoppingStore.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ArticleController(IArticleService articleService, Serilog.ILogger logger) : ControllerBase
    {

        [HttpGet]
        public async Task<IActionResult> GetAllArticles()
        {
            try
            {
                var articles = await articleService.GetAllArticles();
                logger.Information("Successfully retrieved all articles.");
                return Ok(articles);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Failed to get all articles.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetArticleById(Guid id)
        {
            try
            {
                var article = await articleService.GetArticleById(id);
                logger.Information($"Successfully retrieved article with ID {id}");
                return Ok(article);
            }
            catch (KeyNotFoundException)
            {
                logger.Warning($"Article with ID {id} not found.");
                return NotFound();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Failed to get article by ID {Id}.", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateArticle([FromBody] Article article)
        {
            try
            {
                var createdArticle = await articleService.CreateArticle(article);
                logger.Information($"Successfully created article with ID {createdArticle.Id}.");
                return CreatedAtAction(nameof(GetArticleById), new { id = createdArticle.Id }, createdArticle);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Failed to create article.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateArticle(Guid id, [FromBody] Article article)
        {
            if (id != article.Id)
            {
                logger.Warning("Article ID mismatch: {Id} != {ArticleId}", id, article.Id);
                return BadRequest("Article ID mismatch.");
            }

            try
            {
                await articleService.UpdateArticle(article);
                logger.Information("Successfully updated article with ID {Id}.", id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                logger.Warning("Article with ID {Id} not found.", id);
                return NotFound();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Failed to update article with ID {Id}.", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArticle(Guid id)
        {
            try
            {
                await articleService.DeleteArticle(id);
                logger.Information($"Successfully deleted article with ID {id}.");
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                logger.Warning("Article with ID {Id} not found.", id);
                return NotFound();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Failed to delete article with ID {Id}.", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }
    }
}
