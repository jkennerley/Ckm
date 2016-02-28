namespace Ckm.ViewModels
{
    using Models;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class LoginViewModel
    {
        [Required]
        //[StringLength(255,MinimumLength=5)]
        public string Username { get; set; }

        [Required]
        public string Password{ get; set; }

    }
}