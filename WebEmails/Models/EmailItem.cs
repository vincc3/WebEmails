using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebEmails.Models
{
    public class EmailItem
    {
        public int ID { get; set; }
        public string EmailTO { get; set; }
        public string EmailCC { get; set; }
        public string EmailBCC { get; set; }
        public string Subject { get; set; }
        public string Contents { get; set; }
    }
}