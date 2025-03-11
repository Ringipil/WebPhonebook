using System.ComponentModel.DataAnnotations;

namespace WebPhonebook.Models
{
    public class UploadViewModel
    {
        [Required]
        [DataType(DataType.Upload)]
        public IFormFile FirstNamesFile { get; set; }

        [Required]
        [DataType(DataType.Upload)]
        public IFormFile MiddleNamesFile { get; set; }

        [Required]
        [DataType(DataType.Upload)]
        public IFormFile LastNamesFile { get; set; }
    }
}
