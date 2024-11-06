using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Box.Dtos
{
    public class ErrorDto
    {
        public string Type { get; set; }
        public int Status { get; set; }
        public string Code { get; set; }
        public ContextInfo ContextInfo { get; set; }
        public string HelpUrl { get; set; }
        public string Message { get; set; }
        public string RequestId { get; set; }
        
    }
    public class Conflict
    {
        public string Type { get; set; }
        public string Id { get; set; }
        public string SequenceId { get; set; }
        public string Etag { get; set; }
        public string Name { get; set; }
    }

    public class ContextInfo
    {
        public Conflict[] Conflicts { get; set; }
    }

    
 }
