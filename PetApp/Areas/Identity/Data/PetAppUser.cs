using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace PetApp.Areas.Identity.Data;

public class PetAppUser : IdentityUser
{
    public string FirstName { get; set; }
}

