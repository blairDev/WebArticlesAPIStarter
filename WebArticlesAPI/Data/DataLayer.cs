using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System.Data;
using System.Data.SqlClient;
using WebArticlesAPI.Models;

/// File: DataLayer.cs
/// Name: Dave Blair
/// Class: CITC 1317
/// Semester: Fall 2023
/// Project: WebArticlesAPI
namespace WebArticlesAPI.Data
{
    /// <summary>
    /// The data DataLayer class is used to hide implementation details of
    /// connecting to the database doing standard CRUD operations.
    /// 
    /// IMPORTANT NOTES:
    /// On the serverside, any input-output operations should be done asynchronously. This includes
    /// file and database operations. In doing so, the thread is freed up for the entire time a request
    /// is in flight. When a request executes the await code, the request thread is returned back to the
    /// thread pool. When the request is satisfied, the thread is taken from the thread pool and continues.
    /// This is all built into the .NET Core Framework making it very easy to implement into our code.
    /// 
    /// When throwing an exception from an ASYNC function the exception is never thrown back to the calling entity. 
    /// This makes sense because the function could possibly block and cause strange and unexpected 
    /// behavior. Instead, we will LOG the exception.
    /// </summary>
    internal class DataLayer
    {

        #region "Properties"

        /// <summary>
        /// This variable holds the connection details
        /// such as name of database server, database name, username, and password.
        /// The ? makes the property nullable
        /// </summary>
        private readonly string? connectionString = null;

        #endregion

        #region "Constructors"

        /// <summary>
        /// This is the default constructor and has the default 
        /// connection string specified 
        /// </summary>
        public DataLayer()
        {
            //preprocessor directives can help by using a debug build database environment for testing
            // while using a production database environment for production build.
#if (DEBUG)
            connectionString = @"Data Source=(localdb)\ProjectModels;Initial Catalog=WebArticles;Integrated Security=True;Connect Timeout=30";
#else
            connectionString = @"Production Server Connection Information";
#endif
        }

        /// <summary>
        /// Parameterized Constructor passing in a connection string
        /// </summary>
        /// <param name="connectionString"></param>
        public DataLayer(string connectionString)
        {
            this.connectionString = connectionString;
        }

        #endregion

        #region "Database Operations"

        /// <summary>
        /// Get all article in the database and return in a List
        /// </summary>
        /// <param>None</param>
        /// <returns>List of Articles</returns>
        /// <exception cref="Exception"></exception>
        public List<Article> GetArticles()
        {
            List<Article> articles = new();            

            try
            {

                //using guarentees the release of resources at the end of scope 
                using SqlConnection conn = new(connectionString);

                // open the database connection
                conn.Open();

                // create a command object identifying the stored procedure
                using SqlCommand cmd = new SqlCommand("spGetArticles", conn);

                // set the command object so it knows to execute a stored procedure
                cmd.CommandType = CommandType.StoredProcedure;               

                // execute the command which returns a data reader object
                // usually we should use ExecuteReaderAsync() but for this example we will use ExecuteReader()
                using SqlDataReader rdr = (SqlDataReader)cmd.ExecuteReader();

                // if the reader contains a data set, convert to article objects
                while (rdr.Read())
                {
                    Article article = new Article();
                    
                    article.Id = (int)rdr.GetValue(0);
                    article.UserId = (int)rdr.GetValue(1);
                    article.Title = (string)rdr.GetValue(2);
                    article.UserComment = (string)rdr.GetValue(3);
                    article.ArticleUrl = (string)rdr.GetValue(4);

                    articles.Add(article);
                }
            }
            catch (Exception)
            {
                //normally we would write to a log here
                //rethrow exception
                throw;
            }
            finally
            {
                // no clean up because the 'using' statements guarantees closing resources
            }

            //check for articles length to be zero after returned from database
            return articles;

        } // end function GetArticles

        /// <summary>
        /// Get a user by key (GUID)
        /// returns a single User object or a null User
        /// </summary>
        /// <param name="key"></param>
        /// <returns>ArticleUserDTO</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="Exception"></exception>
        public ArticleUserDTO? GetUserLevelByKey(string key)
        {

            ArticleUserDTO? userDTO = null;

            try
            {
                if (key == null)
                {
                    throw new ArgumentNullException("Username or Password can not be null.");
                }

                //using guarentees the release of resources at the end of scope 
                using SqlConnection conn = new SqlConnection(connectionString);

                // open the database connection
                conn.Open();

                // create a command object identifying the stored procedure
                using SqlCommand cmd = new SqlCommand("spGetUserLevel", conn);

                // set the command object so it knows to execute a stored procedure
                cmd.CommandType = CommandType.StoredProcedure;

                // add parameters to command, which will be passed to the stored procedure
                cmd.Parameters.Add(new SqlParameter("UserKey", key));
                
                // execute the command which returns a data reader object
                using SqlDataReader rdr = (SqlDataReader) cmd.ExecuteReader();

                // if the reader contains a data set, load a local user object
                if (rdr.Read())
                {       
                    userDTO = new();
                    userDTO.UserLevel = (string)rdr.GetValue(0);
                    userDTO.Id = (int)rdr.GetValue(1);
                }
            }
            catch (ArgumentNullException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                // no clean up because the 'using' statements guarantees closing resources
            }

            return userDTO;

        } // end function GetUserLevelByKey

        /// <summary>
        /// Gets an article in the database by article ID and returns an ArticleDTO or null
        /// </summary>
        /// <param>Id</param>
        /// <returns>ArticleDTO</returns>
        /// <exception cref="Exception"></exception>
        public ArticleDTO? GetArticleById(int Id)
        {
            ArticleDTO? articleDTO = null;

            try
            {

                //using guarentees the release of resources at the end of scope 
                using SqlConnection conn = new(connectionString);

                // open the database connection
                conn.Open();

                // create a command object identifying the stored procedure
                using SqlCommand cmd = new SqlCommand("spGetAnArticle", conn);

                // set the command object so it knows to execute a stored procedure
                cmd.CommandType = CommandType.StoredProcedure;

                // add parameters to command, which will be passed to the stored procedure
                cmd.Parameters.Add(new SqlParameter("@Id", Id));

                // execute the command which returns a data reader object
                // usually we should use ExecuteReaderAsync() but for this example we will use ExecuteReader()
                using SqlDataReader rdr = (SqlDataReader)cmd.ExecuteReader();

                // if the reader contains a data set, convert to article objects
                if (rdr.Read())
                {
                    //article is null so create a new instance
                    articleDTO = new ArticleDTO();

                    articleDTO.Title = (string)rdr.GetValue(2);
                    articleDTO.UserComment = (string)rdr.GetValue(3);
                    articleDTO.ArticleUrl = (string)rdr.GetValue(4);
                    
                }
            }
            catch (Exception)
            {
                //normally we would write to a log here
                //rethrow exception
                throw;
            }
            finally
            {
                // no clean up because the 'using' statements guarantees closing resources
            }

            //check for articles length to be zero after returned from database
            return articleDTO;

        } // end function GetArticleById

        /// <summary>
        /// Insert an article into the database and return the article with the new ID
        /// </summary>
        /// <param>Article</param>
        /// <returns>Article</returns>
        /// <exception cref="Exception"></exception>
        /// <exception cref="ArgumentNullException"></exception>"
        public Article? InsertArticle(Article article)
        {
            
            Article? tempArticle = null;
            try
            {
                if (article == null)
                {
                    throw new ArgumentNullException("Article can not be null.");
                }

                //using guarentees the release of resources at the end of scope 
                using SqlConnection conn = new(connectionString);

                // open the database connection
                conn.Open();

                // create a command object identifying the stored procedure
                using SqlCommand cmd = new SqlCommand("spInsertArticle", conn);

                // set the command object so it knows to execute a stored procedure
                cmd.CommandType = CommandType.StoredProcedure;

                // add parameters to command, which will be passed to the stored procedure
                cmd.Parameters.Add(new SqlParameter("@UserId", article.UserId));
                cmd.Parameters.Add(new SqlParameter("@Title", article.Title));
                cmd.Parameters.Add(new SqlParameter("@UserComment", article.UserComment));
                cmd.Parameters.Add(new SqlParameter("@ArticleUrl", article.ArticleUrl));

                //create a parameter to hold the output value
                SqlParameter IdValue = new SqlParameter("@Id", SqlDbType.Int);
                IdValue.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(IdValue);

                // execute the command which returns a data reader object
                // usually we should use ExecuteReaderAsync() but for this example we will use ExecuteReader()
                int count = cmd.ExecuteNonQuery();

                // if the reader contains a data set, convert to article objects
                if (count > 0)
                {
                    //article is null so create a new instance
                    tempArticle = new Article();

                    tempArticle.Id = (int)IdValue.Value;

                    tempArticle.UserId = article.UserId;
                    tempArticle.Title = article.Title;
                    tempArticle.UserComment = article.UserComment;
                    tempArticle.ArticleUrl = article.ArticleUrl;

                }
            }
            catch(ArgumentNullException)
            {
                throw;
            }
            catch (Exception)
            {
                //normally we would write to a log here
                //rethrow exception
                throw;
            }
            finally
            {
                // no clean up because the 'using' statements guarantees closing resources
            }

            //article is null if there was an error
            return tempArticle;

        } // end function GetArticleById

        /// <summary>
        /// Update an article in the database and return row count affected
        /// </summary>
        /// <param>Id, ArticleDTO</param>
        /// <returns>int</returns>
        /// <exception cref="Exception"></exception>
        public int UpdateArticle(int Id, ArticleDTO articleDTO)
        {
            int count;

            try
            {
                if(articleDTO == null)
                {
                    throw new ArgumentNullException("Article can not be null.");
                }

                //using guarentees the release of resources at the end of scope 
                using SqlConnection conn = new(connectionString);

                // open the database connection
                conn.Open();

                // create a command object identifying the stored procedure
                using SqlCommand cmd = new SqlCommand("spUpdateArticle", conn);

                // set the command object so it knows to execute a stored procedure
                cmd.CommandType = CommandType.StoredProcedure;

                // add parameters to command, which will be passed to the stored procedure
                cmd.Parameters.Add(new SqlParameter("@Id", Id));
                cmd.Parameters.Add(new SqlParameter("@Title", articleDTO.Title));
                cmd.Parameters.Add(new SqlParameter("@UserComment", articleDTO.UserComment));
                cmd.Parameters.Add(new SqlParameter("@ArticleUrl", articleDTO.ArticleUrl));                

                // execute the command which returns a data reader object
                // usually we should use ExecuteReaderAsync() but for this example we will use ExecuteReader()
                count = cmd.ExecuteNonQuery();
                                
            }
            catch (ArgumentNullException)
            {
                throw;
            }
            catch (Exception)
            {
                //normally we would write to a log here
                //rethrow exception
                throw;
            }
            finally
            {
                // no clean up because the 'using' statements guarantees closing resources
            }

            //article is null if there was an error
            return count;

        } // end function GetArticleById

        /// <summary>
        /// Delete an article in the database and return row count affected
        /// </summary>
        /// <param>Id</param>
        /// <returns>int</returns>
        /// <exception cref="Exception"></exception>
        public int DeleteArticle(int Id)
        {
            int count;
            try
            {
                throw new Exception("Not yet implemented.");             
            }
            catch (Exception)
            {
                //normally we would write to a log here
                //rethrow exception
                throw;
            }
            finally
            {
                // no clean up because the 'using' statements guarantees closing resources
            }

            //article is null if there was an error
            return count;

        } // end function GetArticleById

        #endregion

    } // end class DataLayer

} // end namespace