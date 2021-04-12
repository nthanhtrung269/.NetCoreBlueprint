using Blueprint.CustomJwtMiddleware.Entities;
using Blueprint.CustomJwtMiddleware.Models;
using System.Collections.Generic;

namespace Blueprint.CustomJwtMiddleware.Services
{
    public interface IUserService
    {
        AuthenticateResponse Authenticate(AuthenticateRequest model);
        IEnumerable<User> GetAll();
        User GetById(int id);
        int? ValidateJwtToken(string token);
        string GenerateJwtToken(User user);
    }
}
