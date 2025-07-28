using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using RBacServer.Data;
using RBacServer.Models;
using SQLitePCL;

public class TokenService
{
    private readonly IConfiguration _config;
    private readonly ApplicationDbContext _context;


    public TokenService(IConfiguration config, ApplicationDbContext context)
    {
        _config = config;
        _context = context;
    }

   public virtual string CreateToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username)
        };

        foreach (var userRole in user.UserRoles)
        {
            var role = _context.Roles.FirstOrDefault(r => r.Id == userRole.RoleId);
            if (role != null)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.name));
            }
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            _config["Jwt:Issuer"],
            _config["Jwt:Issuer"],
            claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

}