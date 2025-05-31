using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EffortlessQA.Data.Dtos
{
    public class LoginResponseDto
    {
        public string Data { get; set; } // The JWT token

        //public MetaDto Meta { get; set; }
        public object Error { get; set; }
    }
}
