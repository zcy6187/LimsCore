using System.ComponentModel.DataAnnotations;

namespace LIMS.Users.Dto
{
    public class ChangeUserLanguageDto
    {
        [Required]
        public string LanguageName { get; set; }
    }
}