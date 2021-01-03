using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly string _connectionStrings;

        public UserController(ILogger<UserController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _connectionStrings = configuration.GetSection("ConnectionStrings").GetSection("MesherContext").Value;
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            using (var connection = new SqlConnection(_connectionStrings))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();
                string sql = @"Select * from [User].AppUsers where Id=@id";
                var result = await connection.QueryFirstAsync<AppUser>(sql, new { id = id });
                return Ok(result);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            var users = new List<AppUser>();
            using (var connection = new SqlConnection(_connectionStrings))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();
                string sql = @"Select * from [User].AppUsers";
                var result = await connection.QueryAsync<AppUser>(sql);
                users = result.ToList();
            }
            return Ok(users);
        }
        [HttpGet("UserWithRoles")]
        public async Task<IActionResult> GetUserWithUserRole()
        {
            var users = new List<AppUser>();
            using (var connection = new SqlConnection(_connectionStrings))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();
                string sql = @"Select * from [User].AppUsers U inner join [User].AppUserRoles R on R.UserId=U.Id";
                var result = await connection.QueryAsync<AppUser, AppUserRole, AppUser>(sql, (appUser, appUserRole) => { appUser.AppUserRoles.Add(appUserRole); return appUser; }, splitOn: "UserId");
                users = result.ToList();
            }
            return Ok(users);
        }
        [HttpPost]
        public async Task<IActionResult> Add(AppUser appUser)
        {
            using (var connection = new SqlConnection(_connectionStrings))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();
                string sql = @"Insert Into [User].AppUsers (Name,Surname,Email) Values (@Name,@Surname,@Email) Select Cast(Scope_Identity() as int)";
                var result = await connection.ExecuteAsync(sql, appUser);

                return Ok(appUser);
            }
        }
        [HttpPut]
        public async Task<IActionResult> Update(AppUser appUser)
        {
            using (var connection = new SqlConnection(_connectionStrings))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();
                string sql = @"Update [User].AppUsers Set Name=@Name,Surname=@Surname,Email=@Email Where Id=@Id";
                var result = await connection.ExecuteAsync(sql, appUser);
                if (result != 1) return BadRequest("Güncelleme işlemi başarısız.");
                return Ok(appUser);
            }
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            using (var connection = new SqlConnection(_connectionStrings))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();
                string sql = @"Delete From [User].AppUsers Where Id=@Id";
                var result = await connection.ExecuteAsync(sql, new { Id = id });
                if (result != 1) return NotFound();
                return Ok("Silme işlemi başarılı");
            }
        }
    }
}
