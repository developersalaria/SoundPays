using System;
using System.Collections.Generic;
using System.Text;

namespace SoundpaysAdd.Data.DTO.Email
{
    public class EmailRequest
    {
        public string To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string From { get; set; }
    }
}
