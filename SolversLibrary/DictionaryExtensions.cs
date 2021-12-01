using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solvers
{
    public static class DictionaryExtensions
    {
        public static IEnumerable<TValue> GetValues<TKey, TValue>(this Dictionary<TKey, TValue> dict, IEnumerable<TKey> keys)
        {
            var values = new List<TValue>();
            foreach (var k in keys)
                values.Add(dict[k]);
            return values.AsEnumerable();
        }
    }
}
