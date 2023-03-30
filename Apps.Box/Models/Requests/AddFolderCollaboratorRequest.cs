using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Box.Models.Requests
{
    public class AddFolderCollaboratorRequest
    {
        public string CollaboratorId { get; set; }

        public string FolderId { get; set; }

        public string Role { get; set; }  //editor, viewer, previewer, uploader, previewer uploader, viewer uploader, co-owner,  owner
    }
}
