using Microsoft.AspNet.Identity.EntityFramework;
using System;

namespace Ckm.Models
{
    public class CkmUser : IdentityUser
    {
        public DateTime FirstTrip { get; set; }
    }
}