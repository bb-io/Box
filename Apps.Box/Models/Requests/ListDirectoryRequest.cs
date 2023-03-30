using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Box.Models.Requests
{
    public class ListDirectoryRequest
    {
        public string FolderId { get; set; }

        public int Limit { get; set; }
    }
}
