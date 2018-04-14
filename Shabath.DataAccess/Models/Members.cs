using System;
using System.Collections.Generic;

namespace Shabath.DataAccess.Models
{
    public partial class Members
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int RoundNumber { get; set; }
        public bool IsChosenForThisWeek { get; set; }
        public bool IsActive { get; set; }
        
    }
}
