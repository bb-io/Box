using Blackbird.Applications.Sdk.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Box
{
    public class BoxApplication : IApplication
    {
        private string _name;

        public BoxApplication()
        {
            _name = "Box";
        }

        public string Name
        {
            get => _name;
            set => _name = value;
        }

        public T GetInstance<T>()
        {
            throw new NotImplementedException();
        }
    }
}
