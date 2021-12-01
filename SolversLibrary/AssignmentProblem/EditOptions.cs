using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solvers.AssignmentProblem
{
    public struct EditOptions<T>
    {
        #region Cost functions
        public Func<T, T, double> SubstitutionCost
        {
            get;
            set;
        }
        public Func<T, double> InsertionCost
        {
            get;
            set;
        }
        public Func<T, double> DeletionCost
        {
            get;
            set;
        }
        #endregion Cost functions
    }
}
