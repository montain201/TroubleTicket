using Core.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Services
{
    public interface IUserService
    {
        Task<string> Login(LoginUser loginUse);
    }
}
