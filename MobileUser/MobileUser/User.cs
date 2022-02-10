using System;

namespace MobileUser
{
    public class User
    {
        public string FirstName  { get; set; }
        
        public string SurName { get; set; }

        public int Age { get; set; }

        public DateTime CreationTime { get; set; }

        public Guid Id { get; set; }
    }
}
