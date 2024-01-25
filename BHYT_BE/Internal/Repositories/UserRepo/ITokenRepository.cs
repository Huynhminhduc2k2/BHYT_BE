using Microsoft.AspNetCore.Identity;

namespace BHYT_BE.Internal.Repositories.UserRepo
{
    public interface ITokenRepository
    {
        string CreateJWTToken(IdentityUser user, List<string> roles);
    }
}
