using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Authentication.Lab.Entity;
using Microsoft.IdentityModel.Tokens;

public class JWTTokenRepository {
    private IConfiguration iconfiguration { get; set; }
    public JWTTokenRepository (IConfiguration configuration) {
        iconfiguration = configuration;
    }

    public string GenerateToken (Users users) {
        // generate token that is valid for 7 days
        var tokenHandler = new JwtSecurityTokenHandler ();
        var key = Encoding.ASCII.GetBytes (iconfiguration["JWT:Key"]);
        var tokenDescriptor = new SecurityTokenDescriptor {
            Subject = new ClaimsIdentity (new [] { new Claim ("id", users.Name.ToString ()) }),
            Expires = DateTime.UtcNow.AddMinutes (1),
            SigningCredentials = new SigningCredentials (new SymmetricSecurityKey (key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken (tokenDescriptor);
        return tokenHandler.WriteToken (token);
    }

    public string? ValidateToken (string token) {
        if (token == null)
            return null;

        var tokenHandler = new JwtSecurityTokenHandler ();
        var key = Encoding.ASCII.GetBytes (iconfiguration["JWT:Key"]);
        try {
            tokenHandler.ValidateToken (token, new TokenValidationParameters {
                ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey (key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken) validatedToken;
            var userId = jwtToken.Claims.First (x => x.Type == "id").Value;

            // return user id from JWT token if validation successful
            return userId;
        } catch {
            // return null if validation fails
            return null;
        }
    }
}