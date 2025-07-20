namespace Catalog.Infrastructure.Repositories.Identity
{
    public class JWTConfiguration
    {
        public string SecretKey { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
    }
}
