using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreeAds.API.Dtos
{
    public class MessageDto
    {
        public Guid Id { get; set; }
        public int SenderId { get; set; }
        public string SenderUsername { get; set; }
        public int RecipientId { get; set; }
        public string RecipientUsername { get; set; }
        public string Content { get; set; }
        public DateTime? DateRead { get; set; }
        public DateTime MessageSent { get; set; }
    }
}
