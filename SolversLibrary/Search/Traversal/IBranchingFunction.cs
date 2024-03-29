﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolversLibrary.Search.Traversal
{
    public interface IBranchingFunction<T>
    {
        IEnumerable<ITraversal<T>> Branch(T state);
    }
}
