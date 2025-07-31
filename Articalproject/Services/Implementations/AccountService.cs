

using Articalproject.Models.Identity;
using Articalproject.Resources;
using Articalproject.Services.InterFaces;
using Azure.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace Articalproject.Services.Implementations
{
    public class AccountService : IAccountService
    {
            private readonly IMemoryCache _cache;
        private readonly UserManager<User> _userManager;
        public AccountService(IMemoryCache cache, UserManager<User> userManager)
            {
                _cache = cache;
                _userManager = userManager;
        }

            public (bool canResend, TimeSpan? remainingTime) CanUserResend(string userId,string type)
            {
            string key = $"resend_{userId}{type}";

            if (_cache.TryGetValue(key, out (int count, DateTime lastSent) data))
                {
                    var now = DateTime.UtcNow;
                    var nextAvailable = data.count == 1 ? data.lastSent.AddMinutes(5) : data.lastSent.AddHours(24);

                    if (now < nextAvailable)
                        return (false, nextAvailable - now);
                }

                return (true, null);
            }

            public void RecordResend(string userId, string type)
            {
                string key = $"resend_{userId}{type}";
              if (_cache.TryGetValue(key, out (int count, DateTime lastSent) data))
                {
                    _cache.Set(key, (data.count + 1, DateTime.UtcNow));
                }
                else
                {
                    _cache.Set(key, (1, DateTime.UtcNow));
                }
            }


    }
}
