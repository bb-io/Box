using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Box.Models.Requests
{
    public class RenameFileRequest
    {
        public string FileId { get; set; }

        public string NewFilename { get; set; }
    }
}
