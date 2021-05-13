using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicsCRMResourceSynchronization.Core.Business.Models
{
    public class UsuarioModel
    {
        public Guid systemuserid { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string fullname { get; set; }
    }
}
