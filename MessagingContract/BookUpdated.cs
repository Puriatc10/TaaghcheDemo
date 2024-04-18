using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessagingContract
{
    public record BookUpdated {
        public int BookId { get; set; }
        public DateTime EventTime { get; set; }
    }
}
