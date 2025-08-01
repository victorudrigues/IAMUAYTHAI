using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAMUAYTHAI.Application.Abstractions.Features.Class.Request
{
    public class CreateClassRequest
    {
        public DateTime DateTime { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}