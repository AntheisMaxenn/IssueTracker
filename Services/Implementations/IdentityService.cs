using IssueTracker.Contracts.V1.Requests;
using IssueTracker.Data;
using IssueTracker.Domain;
using IssueTracker.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace IssueTracker.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<IdentityUser> UserManager;
        private readonly JwtSettings jwtSettings;
        private readonly ILogger<IdentityService> logger;
        private readonly TokenValidationParameters tokenValidationParameters;
        private readonly IssueTrackerDbContext context;
        private readonly RoleManager<IdentityRole> RoleManager;

        public IdentityService(UserManager<IdentityUser> userManager, JwtSettings jwtSettings, ILogger<IdentityService> logger, TokenValidationParameters tokenValidationParameters, IssueTrackerDbContext context, RoleManager<IdentityRole> roleManager)
        {
            UserManager = userManager;
            this.jwtSettings = jwtSettings;
            this.logger = logger;
            this.tokenValidationParameters = tokenValidationParameters;
            this.context = context;
            RoleManager = roleManager;
        }

        public async Task<AuthenticationResult> Register(EmployeeRegistrationRequest registerUserRequest)
        {
            var alreadyUser = await UserManager.FindByEmailAsync(registerUserRequest.Email);

            if (alreadyUser != null)
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "A User with this Email Already exists" }
                };
            }

            var user = new IdentityUser
            {
                Email = registerUserRequest.Email,
                UserName = registerUserRequest.Email,

            };

            var createdUser = await UserManager.CreateAsync(user, registerUserRequest.Password);
            {
                if (!createdUser.Succeeded)
                {
                    return new AuthenticationResult
                    { Errors = createdUser.Errors.Select(x => x.Description) };
                }
            };

            var newUser = await UserManager.FindByEmailAsync(registerUserRequest.Email);

            Data.Employee employee = new Data.Employee
            {
                FirstName = registerUserRequest.FirstName,
                LastName = registerUserRequest.LastName,
                Email = registerUserRequest.Email,
                Position = registerUserRequest.Position,
                EmployeeId = newUser.Id

            };

            await context.Employees.AddAsync(employee);

            try
            {
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                await UserManager.DeleteAsync(await UserManager.FindByEmailAsync(registerUserRequest.Email));

                logger.LogError($"There was an error SavingChanges to Employee Table. Error: {e}");

                return new AuthenticationResult
                {
                    Errors = new[] { "Can't create new Employee in table." }
                };
            }

            return await GenerateAuthenticationResponseForUser(user);

        }

        public async Task<AuthenticationResult> Login(EmployeeLoginRequest employeeLoginRequest)
        {
            var user = await UserManager.FindByEmailAsync(employeeLoginRequest.Email.ToString());
            //logger.LogInformation("AlreadyUser object" + alreadyUser.ToString());

            if (user == null)
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "Cannot find that user" }
                };
            }

            //var user = new IdentityUser
            //{
            //    Email = registerUserRequest.Email,
            //    UserName = registerUserRequest.FirstName,

            //};

            //var createdUser = await UserManager.CreateAsync(user);
            //{
            //    if (!createdUser.Succeeded)
            //    {
            //        return new AuthenticationResult
            //        { Errors = createdUser.Errors.Select(x => x.Description) };
            //    }
            //};

            var hasValidPassword = await UserManager.CheckPasswordAsync(user, employeeLoginRequest.Password.ToString());

            if (!hasValidPassword)
            {
                return new AuthenticationResult
                {
                    Errors = new[] { $"Incorrect Password. : {hasValidPassword}" }
                };
            }

            return await GenerateAuthenticationResponseForUser(user);
        }

        public async Task<AuthenticationResult> Refresh(RefreshTokenRequest refreshTokenRequest)
        {
            var validatedToken = GetPrincipalFromToken(refreshTokenRequest.Token);

            if (validatedToken == null)
            {
                return new AuthenticationResult()
                {
                    Errors = new[] { "There was an Error" }
                };
            }

            var expiryDateUnix = long.Parse(validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Exp).Value );

            var expiryDateTimeUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(expiryDateUnix);

            if (expiryDateTimeUtc > DateTime.Now)
            {
                return new AuthenticationResult()
                {
                    Errors = new[] { "Still Valid Token!" }
                };
            }

            var jti = validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

            var storedRefreshToken = await context.RefreshTokens.SingleOrDefaultAsync(x => x.Token == refreshTokenRequest.RefreshToken);

            if (storedRefreshToken == null)
            {
                return new AuthenticationResult()
                {
                    Errors = new[] { "Refresh Token doesn't exist" }
                };
            }

            if (storedRefreshToken.ExpiryDate < DateTime.Now)
            {
                return new AuthenticationResult()
                {
                    Errors = new[] { "Refresh token has expired" }
                };
            }

            if (storedRefreshToken.Invalidated)
            {
                return new AuthenticationResult()
                {
                    Errors = new[] { "Refresh Token is Invalidated" }
                };
            }

            if (storedRefreshToken.Used)
            {
                return new AuthenticationResult()
                {
                    Errors = new[] { "Refresh has been used" }
                };
            }

            if (storedRefreshToken.JwtId != jti)
            {
                return new AuthenticationResult()
                {
                    Errors = new[] { "Token and Refresh don't match." }
                };
            }

            storedRefreshToken.Used = true;

            context.RefreshTokens.Update(storedRefreshToken);
            await context.SaveChangesAsync();

            var user = await UserManager.FindByIdAsync(validatedToken.Claims.Single(x => x.Type == "id").Value);
            return await GenerateAuthenticationResponseForUser(user);

        } 

        private ClaimsPrincipal GetPrincipalFromToken(string token) 
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var principle = tokenHandler.ValidateToken(token, tokenValidationParameters, out var validationToken);
                if (!IsJwtWithValidSecurityAlgorithm(validationToken))
                {
                    return null;
                }
                return principle;
            }
            catch
            {
                return null;
            }

        }

        private async Task<AuthenticationResult> GenerateAuthenticationResponseForUser(IdentityUser user)
                {
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var key = Encoding.ASCII.GetBytes(jwtSettings.Secret);

                    var claims = new List<Claim>
                        {
                            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                            new Claim(JwtRegisteredClaimNames.Email, user.Email),
                            new Claim("id", user.Id),
                        };

                    // Get user claims -> AddRange(userClaims) the claims from db.
                    claims.AddRange(await UserManager.GetClaimsAsync(user));
                    // Get user Roles from db
                    var userRoles = await UserManager.GetRolesAsync(user);
                    // foreach role get each claim within and add to the claims 
                    foreach(var userRole in userRoles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, userRole));

                        var role = await RoleManager.FindByNameAsync(userRole);
                        if (role == null) continue;
                        var roleClaims = await RoleManager.GetClaimsAsync(role);

                        foreach(var roleClaim in roleClaims)
                        {
                            if (claims.Contains(roleClaim)) continue;
                            claims.Add(roleClaim);
                        }
                    }


            
                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(claims),
                        Expires = DateTime.UtcNow.AddHours(12),
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                    };

                    var token = tokenHandler.CreateToken(tokenDescriptor);
                    var refreshToken = new RefreshToken()
                    {
                        JwtId = token.Id,
                        UserId = user.Id,
                        CreationDate = DateTime.UtcNow,
                        ExpiryDate = DateTime.UtcNow.AddMonths(1)

                    };


                    await context.RefreshTokens.AddAsync(refreshToken);
                    await context.SaveChangesAsync();

                    return new AuthenticationResult
                    {
                        Success = true,
                        Token = tokenHandler.WriteToken(token),
                        RefreshToken = refreshToken.Token
                    };
                }

        private bool IsJwtWithValidSecurityAlgorithm(SecurityToken validationToken)
        {
            return (validationToken is JwtSecurityToken jwtSecurityToken) &&
                jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}