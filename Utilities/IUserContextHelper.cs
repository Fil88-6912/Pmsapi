using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PmsApi.Utilities
{
    public interface IUserContextHelper
    {
        string GetUserId();
        bool IsAdmin();
    }
}