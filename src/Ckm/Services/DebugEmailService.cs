namespace Ckm.Services
{
    using System;
    using System.Diagnostics;

    public class DebugMailService : IMailService
    {
        public bool SendMail(string to, string from, string subject, string body)
        {
            Debug.WriteLine( $"Sending mail : TO {to}  : from {from} " ) ;
            return true;
        }
    }
}