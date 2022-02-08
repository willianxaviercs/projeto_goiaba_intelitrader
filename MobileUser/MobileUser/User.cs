using System;
//using System.ComponentModel.DataAnnotations;

namespace MobileUser
{
    public class User
    {
  //      [Required]
        public string FirstName  { get; set; }
        
        public string SurName { get; set; }

    //    [Range(1, 128)]
      //  [Required]
        public int Age { get; set; }
    }
}
