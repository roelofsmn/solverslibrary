﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolversLibrary.Search.Traversal
{
    public interface ITraversalStrategy<T>
    {
        T NextState();
        void AddCandidateState(T state);
        void ReplaceCandidateState(T current, T replacement);

        bool ContainsCandidates();
        bool Contains(T state);
        void Clear();
        
    }
}