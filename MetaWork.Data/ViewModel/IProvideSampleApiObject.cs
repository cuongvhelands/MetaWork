using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaWork.Data.ViewModel
{
    public interface IProvideSampleApiObject
    {
        KeyValuePair<Type, object> CreateSampleObjectEntry();
        KeyValuePair<Type, object> CreateSampleObjectsEntry();
    }
}
