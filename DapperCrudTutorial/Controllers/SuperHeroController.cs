using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace DapperCrudTutorial.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SuperHeroController : ControllerBase
    {
        private readonly IConfiguration _config;

        public SuperHeroController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        public async Task<ActionResult<List<SuperHero>>> GetAllSuperHeroes()
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            IEnumerable<SuperHero> heroes = await SelectAllHeroes(connection);
            return Ok(heroes);
        }



        [HttpGet("{heroId}")]
        public async Task<ActionResult<List<SuperHero>>> GetSuperHero(int heroId)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            var hero = await connection.QueryFirstAsync<SuperHero>("[dbo].[GetAllHeroesById] @Id", 
                new { Id = heroId});
            return Ok(hero);
        }

        [HttpPost]
        public async Task<ActionResult<List<SuperHero>>> CreateHero(SuperHero hero)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("[dbo].[CreateSuperHero] @Name,@FirstName, @LastName,@Place", hero);
            return Ok(await SelectAllHeroes(connection));
        }
        private static async Task<IEnumerable<SuperHero>> SelectAllHeroes(SqlConnection connection)
        {
            return await connection.QueryAsync<SuperHero>("[dbo].[GetAllHeroes]");
        }

        [HttpPut]
        public async Task<ActionResult<List<SuperHero>>> UpdateHero(SuperHero hero)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("[dbo].[UpdateSuperHero] @Name,@FirstName, @LastName,@Place,@Id", hero);
            return Ok(await SelectAllHeroes(connection));
        }

        [HttpDelete("{heroId}")]
        public async Task<ActionResult<List<SuperHero>>> DeleteSuperHero(int heroId)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("[dbo].[DeleteHeroesById] @Id",
                new { Id = heroId });
            return Ok(await SelectAllHeroes(connection));
        }
    }
}
