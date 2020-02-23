using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalsAndTransforms.Interfaces
{
    // Common interface between filter types in the UI
    public interface IFilterIdentifier
    {
        long Id { get; set; }
        string Name { get; set; }
    }
}
