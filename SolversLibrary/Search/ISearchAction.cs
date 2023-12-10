using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolversLibrary.Search
{
    public interface ISearchAction<T>
    {
        /// <summary>
        /// Apply the action to a given state.
        /// </summary>
        /// <param name="state">The search state to apply the action to.</param>
        /// <returns>A collection of states resulting from the action.</returns>
        T Apply(T state);
    }

    public interface ICostSearchAction<T> : ISearchAction<T>
    {
        double Cost(T state);
    }
}
