using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;

namespace Catalog.Infrastructure.Repositories.Identity.Models
{
    public class OtherParamUser : ParamUser
    {
        public string UserName { get; set; }
        public bool SeniorManager { get; set; }

        [Required]
        public Professions Profession { get; set; }
    }
}
