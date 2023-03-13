using Domain.Entities;
using Persistance;
using static Google.Apis.Auth.GoogleJsonWebSignature;

namespace SocialApp.Services
{
    public class AuthServices
    {
        private readonly ApplicationDbContext _context;

        public AuthServices(ApplicationDbContext context)
        {
            _context = context;
        }

        public User Authenticate(Payload payload)
        {
            return FindUserOrAdd(payload);
        }

        private User FindUserOrAdd(Payload payload)
        {
            var user = _context.Users.Where(x => x.Email == payload.Email).FirstOrDefault();
            if (user == null)
            {
                user = new User()
                {
                    FirstName = payload.Name,
                    Email = payload.Email,
                    RegisteredAt = DateTime.UtcNow,
                    LastName = payload.FamilyName,
                    Username = payload.Email,
                    Picture = payload.Picture,
                };
                _context.Users.Add(user);
            }
            _context.SaveChanges();
            return user;
        }
    }
}
