using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace pet_adoption_system.Models.View_Model
{
    public class AdopterVM
    {
        //public AdopterVM()
        //{
        //    this.PetList = new List<int>();
        //}
        public int AdopterId { get; set; }
        [Display(Name = "Adopt Name"), Required]
        public string AdopterName { get; set; }
        [Display(Name = "Birthdate"), Required, Column(TypeName = "date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime BirthDate { get; set; }
        [Display(Name = "Age"), Required]
        public int Age { get; set; }
        public string Picture { get; set; }
        public HttpPostedFileBase PictureFile { get; set; }
        [Display(Name = "Marital Status"), Required]
        public bool Maritalstatus { get; set; }
        public List<int> PetList { get; set; } = new List<int>();
        
    }
}