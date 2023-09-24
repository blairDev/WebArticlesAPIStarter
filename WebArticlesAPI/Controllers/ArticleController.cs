using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc;
using System;
using WebArticlesAPI.Data;
using WebArticlesAPI.Models;


namespace WebArticlesAPI.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ArticleController : ControllerBase
    {
        // GET: api/v1/Article/all/{key}
        [HttpGet("all/{key}")]
        public ActionResult<List<Article?>> Get(string key)
        {
            try
            {
                throw new Exception("Not yet implemented");
            }
            catch (Exception err)
            {
                return new ObjectResult(err.Message);
                //return new StatusCodeResult(StatusCodes.Status500InternalServerError); //StatusCode(500);
            }

        }

        // GET api/v1/Article/{5}/{key}
        [HttpGet("{id}/{key}")]
        public ActionResult<ArticleDTO?> Get(int id, string key)
        {

            try
            {
                DataLayer dl = new();
                ArticleUserDTO? userDTO = dl.GetUserLevelByKey(key);
                if (userDTO == null)
                {
                    return Unauthorized($"{key} is unauthorized to access the database"); //status 401;
                }
                if (userDTO.UserLevel is not "admin" and not "user")
                {
                    return Unauthorized($"{key} is unauthorized to access the database"); //status 401;
                }

                //return the article or null
                ArticleDTO? article = dl.GetArticleById(id);
                if(article == null)
                {
                    return NotFound($"Article {id} not found."); //status 404
                }

                //all is good, return the article
                return Ok(article);
            }
            catch (ArgumentNullException)
            {
                return BadRequest("The user key can not be null."); //status 400
            }
            catch (Exception)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError); //StatusCode(500);
            }
        }

        // POST api/v1/Article/{key}
        [HttpPost("{key}")]
        public ActionResult<Article> Post([FromBody] ArticleDTO articleDTO, string key)
        {            
            try
            {
                DataLayer dl = new();
                ArticleUserDTO? userDTO = dl.GetUserLevelByKey(key);
                if (userDTO == null)
                {
                    return Unauthorized($"{key} is unauthorized to access the database"); //status 401;
                }
                if (userDTO.UserLevel is not "admin" and not "user")
                {
                    return Unauthorized($"{key} is unauthorized to access the database"); //status 401;
                }

                //validate article information
                ArticleDTO tempArticleDTO = ValidateArticleDTO(articleDTO);
                if(tempArticleDTO.ArticleUrl == "Not a valid URL")
                {
                    return BadRequest("The article URL is not valid."); //status 400
                }

                Article tempArticle = new Article()
                {
                    UserId = userDTO.Id,
                    Title = tempArticleDTO.Title,
                    UserComment = tempArticleDTO.UserComment,
                    ArticleUrl = tempArticleDTO.ArticleUrl
                };
                
                //return the article or null
                Article? article = dl.InsertArticle(tempArticle);
                if(article == null)
                {
                    return new StatusCodeResult(StatusCodes.Status500InternalServerError); //StatusCode(500);
                }   

                //all is good, return the article showing the new id
                return Ok(article);
            }
            catch (ArgumentNullException)
            {
                return BadRequest("The user key can not be null."); //status 400
            }
            catch (Exception)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError); //StatusCode(500);
            }
        }

        // PUT api/v1/Article/{5}/{adminKey}
        [HttpPut("{Id}")]
        public ActionResult<ArticleDTO> Put(int Id, string adminKey, [FromBody] ArticleDTO articleDTO)
        {
            try
            {
                throw new Exception("Not yet implemented");
            }
            catch (ArgumentNullException)
            {
                return BadRequest("The user key can not be null."); //status 400
            }
            catch (Exception err)
            {
                return new ObjectResult(err.Message);
                //return new StatusCodeResult(StatusCodes.Status500InternalServerError); //StatusCode(500);
            }

        }

        // DELETE api/v1/Article/{5}/{adminKey}
        [HttpDelete("{Id}")]
        public ActionResult<string> Delete(int Id, string adminKey)
        {
            try
            {
                throw new Exception("Not yet implemented");
            }
            catch (ArgumentNullException)
            {
                return BadRequest("The user key can not be null."); //status 400
            }
            catch (Exception err)
            {
                return new ObjectResult(err.Message);
                //return new StatusCodeResult(StatusCodes.Status500InternalServerError); //StatusCode(500);
            }

        }

        /// <summary>
        /// Validates the article information
        /// </summary>
        /// <param name="articleDTO">ArticleDTO</param>
        /// <returns>ArticleDTO</returns>
        private ArticleDTO ValidateArticleDTO(ArticleDTO articleDTO)
        {
            articleDTO.Title = articleDTO.Title.Trim();
            if(articleDTO.Title.Length > 250)
            {
                articleDTO.Title = articleDTO.Title.Substring(0, 250);
            }
            articleDTO.UserComment = articleDTO.UserComment.Trim();
            if (articleDTO.UserComment.Length > 1024)
            {
                articleDTO.Title = articleDTO.Title.Substring(0, 1024);
            }
            articleDTO.ArticleUrl = articleDTO.ArticleUrl.Trim();
            if (articleDTO.ArticleUrl.Length > 250)
            {
                articleDTO.Title = articleDTO.Title.Substring(0, 250);
            }
            if (!(articleDTO.ArticleUrl.StartsWith("https://www.") || articleDTO.ArticleUrl.StartsWith("http://www.") || articleDTO.ArticleUrl.StartsWith("https://") || articleDTO.ArticleUrl.StartsWith("http://")))
            {
                articleDTO.ArticleUrl = "Not a valid URL";
            }
            return articleDTO;
        }
    }
}
