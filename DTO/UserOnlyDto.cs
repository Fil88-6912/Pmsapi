using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PmsApi.DTO
{
    public record UserOnlyDto(
        string Id,
        string UserName,
        string FirstName,
        string LastName,
        string PhoneNumber,
        string Email
    );
}