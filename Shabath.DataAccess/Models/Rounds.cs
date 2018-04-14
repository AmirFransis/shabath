using System;
using System.Collections.Generic;

namespace Shabath.DataAccess.Models
{
    public partial class Rounds
    {
        public int Id { get; set; }
        public int CurrentRoundNumber { get; set; }
        public DateTime EventDate { get; set; }
        public string DayOfWeek { get; set; }
    }
}
