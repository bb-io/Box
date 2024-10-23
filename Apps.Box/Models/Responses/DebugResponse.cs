using Blackbird.Applications.Sdk.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Box.Models.Responses
{
    public class DebugResponse
    {
        [Display("Token")]
        public string AccessToken { get; set; }
    }
}
