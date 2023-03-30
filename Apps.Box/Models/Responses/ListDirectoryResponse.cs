using Apps.Box.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Box.Models.Responses
{
    public class ListDirectoryResponse
    {
        public IEnumerable<DirectoryItemDto> DirectoriesItems { get; set; }
    }
}
