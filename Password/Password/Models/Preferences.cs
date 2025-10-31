using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Password.Models
{
    public class Preferences
    {
        public string? UserName { get; set; }
        public int UserId { get; set; }
        public DateFormat DFormat { get; set; } = DateFormat.YYYYMMDD;
    }
    public enum DateFormat
    {
        YYYYMMDD,
        DDMMYYYY,
        MMDDYYYY
    }
}
