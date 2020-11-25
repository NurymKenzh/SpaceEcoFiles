using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SpaceEcoFiles.Models
{
    public enum Language
    {
        Russian,
        Kazakh,
        English
    }

    public class Doc
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Title")]
        public string Title { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Date")]
        [Required(ErrorMessageResourceType = typeof(Resources.Controllers.SharedResources), ErrorMessageResourceName = "TheFieldIsRequired")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime Date { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "Language")]
        public Language Language { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "DocType")]
        public int DocTypeId { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "DocType")]
        public DocType DocType { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "DocFormat")]
        public int DocFormatId { get; set; }
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "DocFormat")]
        public DocFormat DocFormat { get; set; }

        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "File")]
        public string File { get; set; }

        [NotMapped]
        [Display(ResourceType = typeof(Resources.Controllers.SharedResources), Name = "File")]
        public IFormFile FormFile { get; set; }
    }

    public class DocIndexPageViewModel
    {
        public IEnumerable<Doc> Items { get; set; }
        public Pager Pager { get; set; }
    }
}
