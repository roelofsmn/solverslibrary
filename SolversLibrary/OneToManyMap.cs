using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using System.Diagnostics;
using Base;

namespace Solvers
{
    [DataContract]
    public class OneToManyMap<TKey, Tvalue> : Dictionary<Tvalue, Tvalue> where TKey : class
    {
        [DataMember]
        public TKey A { get; private set; }
        [DataMember]
        public TKey B { get; private set; }
        public OneToManyMap(TKey a, TKey b, IEqualityComparer<Tvalue> comparer = null)
        {
            if (comparer == null)
                comparer = EqualityComparer<Tvalue>.Default;
            A = a;
            B = b;
        }

        public static OneToOneMap<TKey, Tvalue> FromDictionary(Dictionary<Tvalue, Tvalue> dict, TKey a, TKey b)
        {
            var oom = new OneToOneMap<TKey, Tvalue>(a, b);
            if (oom.Add(dict))
                return oom;
            throw new ArgumentException("Non unique values in dictionary!");
        }

        public void Add(IEnumerable<Tuple<Tvalue, Tvalue>> items)
        {
            foreach (var t in items)
                Add(t.Item1, t.Item2);
        }
        public void Add(IEnumerable<KeyValuePair<Tvalue, Tvalue>> items)
        {
            foreach (var t in items)
                Add(t.Key, t.Value);
        }

        public void Remove(IEnumerable<Tvalue> items)
        {
            foreach (var t in items)
                Remove(t);
        }

        public virtual IEnumerable<Tvalue> MapFrom(TKey from, IEnumerable<Tvalue> values)
        {
            if (from == A)
                return values.Select(v => MapForward(v));
            else if (from == B)
                return values.SelectMany(v => MapBackward(v));
            else
                throw new InvalidOperationException();
        }
        public virtual IEnumerable<Tvalue> MapTo(TKey to, IEnumerable<Tvalue> values)
        {
            if (to == B)
                return values.Select(v => MapForward(v));
            else if (to == A)
                return values.SelectMany(v => MapBackward(v));
            else
                throw new InvalidOperationException();
        }
        public virtual Tvalue MapForward(Tvalue value)
        {
            try
            {
                return this[value];
            }
            catch (KeyNotFoundException)
            {
                throw new ArgumentException("Map does not contain value");
            }
        }
        public virtual IEnumerable<Tvalue> MapBackward(Tvalue value)
        {
            return this.Where(kv => kv.Value.Equals(value)).Select(kv => kv.Key);
        }
        public IEnumerable<Tuple<Tvalue, Tvalue>> GetMapsAB()
        {
            return this.Select(kv => new Tuple<Tvalue, Tvalue>(kv.Key, kv.Value));
        }
        public bool Contains(Tvalue item)
        {
            Debug.WriteLine("OneToOneMap<T>.Contains(T item) uses default equality comparar!!!");
            return this.ContainsKey(item);
        }

        public IEnumerable<Tvalue> ACollection
        {
            get { return this.Keys; }
        }
        public IEnumerable<Tvalue> BCollection
        {
            get { return this.Values; }
        }
    }

    public class ManyMapEqualityComparer<T1, T2> : IEqualityComparer<OneToManyMap<T1, T2>> where T1 : class
    {
        public bool Equals(OneToManyMap<T1, T2> x, OneToManyMap<T1, T2> y)
        {
            foreach (var a in x.ACollection)
                if (!y.Contains(a) || !x.MapForward(a).Equals(y.MapForward(a)))
                    return false;
            return true;
        }

        public int GetHashCode(OneToManyMap<T1, T2> obj)
        {
            return HashcodeHelper.GetCollectionHashcode(obj.GetMapsAB());
        }
    }
}