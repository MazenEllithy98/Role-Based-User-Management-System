using Demo.DAL.Models;
using Org.BouncyCastle.Asn1.Cmp;

namespace Demo.PL.Helper
{
    public interface IEmailSettings
    {
        public void SendEmail(Email email);
        
    }
}
