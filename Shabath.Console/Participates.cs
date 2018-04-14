using Shabath.DataAccess;
using Shabath.DataAccess.Models;
using System;
using System.Collections.Generic;

namespace Shabath.Console
{
    internal class Participates
    {
        public List<Members> Members = new List<Members>(new Members[2]);
        public DateTime EventDate;
    }
}