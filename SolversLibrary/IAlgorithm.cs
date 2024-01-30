using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolversLibrary
{
    public interface IAlgorithm<T>
    {
        T Run();

        event Action<T>? ProgressUpdated;
    }
}
