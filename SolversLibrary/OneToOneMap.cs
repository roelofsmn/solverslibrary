using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using System.Diagnostics;
using Base;

namespace Solvers
{
    [DataContract]
    public class OneToOneMap<TKey, Tvalue> where TKey : class
    {
        [DataMember]
        public TKey A { get; private set; }
        [DataMember]
        public TKey B { get; private set; }
        public OneToOneMap(TKey a, TKey b, IEqualityComparer<Tvalue> comparer = null)
        {
            if (comparer == null)
                comparer = EqualityComparer<Tvalue>.Default;
            A = a;
            B = b;
            AtoB = new Dictionary<Tvalue, Tvalue>(comparer);
            BtoA = new Dictionary<Tvalue, Tvalue>(comparer);
        }

        public static OneToOneMap<TKey, Tvalue> FromDictionary(Dictionary<Tvalue, Tvalue> dict, TKey a, TKey b)
        {
            var oom = new OneToOneMap<TKey, Tvalue>(a, b);
            if (oom.Add(dict))
                return oom;
            throw new ArgumentException("Non unique values in dictionary!");
        }
        [DataMember]
        private Dictionary<Tvalue, Tvalue> AtoB;
        [DataMember]
        private Dictionary<Tvalue, Tvalue> BtoA;

        [IgnoreDataMember]
        public int Count { get { return AtoB.Count; } }

        public bool Add(Tvalue itemA, Tvalue itemB)
        {
            if (AtoB.ContainsKey(itemA) || BtoA.ContainsKey(itemB))
                return false;

            AtoB.Add(itemA, itemB);
            BtoA.Add(itemB, itemA);

            return true;
        }

        public void Add(IEnumerable<Tuple<Tvalue, Tvalue>> items)
        {
            //bool allTrue = true;
            foreach (var t in items)
                Add(t.Item1, t.Item2);
            //return allTrue;
        }
        public bool Add(IEnumerable<KeyValuePair<Tvalue, Tvalue>> items)
        {
            bool allTrue = true;
            foreach (var t in items)
                allTrue = allTrue && Add(t.Key, t.Value);
            return allTrue;
        }

        public void Remove(Tvalue item)
        {
            if (AtoB.ContainsKey(item))
            {
                var other = AtoB[item];
                AtoB.Remove(item);
                BtoA.Remove(other);
            }
            else if (BtoA.ContainsKey(item))
            {
                var other = BtoA[item];
                AtoB.Remove(other);
                BtoA.Remove(item);
            }
        }

        public void Remove(IEnumerable<Tvalue> items)
        {
            foreach (var t in items)
                Remove(t);
        }

        public virtual IEnumerable<Tvalue> MapFrom(TKey from, IEnumerable<Tvalue> values)
        {
            if (from == A)
                return AtoB.GetValues(values);
            else if (from == B)
                return BtoA.GetValues(values);
            else
                throw new ArgumentException("This map does not map to/from provided key");
        }
        public virtual IEnumerable<Tvalue> MapTo(TKey to, IEnumerable<Tvalue> values)
        {
            if (to == A)
                return BtoA.GetValues(values);
            else if (to == B)
                return AtoB.GetValues(values);
            else
                throw new ArgumentException("This map does not map to/from provided key");
        }

        public virtual Tvalue MapFrom(TKey from, Tvalue value)
        {
            try
            {
                if (from == A)
                    return AtoB[value];
                else if (from == B)
                    return BtoA[value];
                else
                    throw new ArgumentException("This map does not map to/from provided key");
            }
            catch (KeyNotFoundException)
            {
                if (AtoB.ContainsKey(value) || BtoA.ContainsKey(value))
                    return value;
                else
                    throw new ArgumentException("Map does not contain value");
            }
        }
        public virtual Tvalue MapTo(TKey to, Tvalue value)
        {
            if (to == A)
                return MapFrom(B, value);
            else if (to == B)
                return MapFrom(A, value);
            else
                throw new ArgumentException("This map does not map to/from provided key");
        }
        public IEnumerable<Tuple<Tvalue, Tvalue>> GetMapsAB()
        {
            return AtoB.Select(kv => new Tuple<Tvalue, Tvalue>(kv.Key, kv.Value));
        }
        public bool Contains(Tvalue item)
        {
            Debug.WriteLine("OneToOneMap<T>.Contains(T item) uses default equality comparar!!!");
            return AtoB.ContainsKey(item) || BtoA.ContainsKey(item);
        }

        public IEnumerable<Tvalue> ACollection
        {
            get { return AtoB.Keys; }
        }
        public IEnumerable<Tvalue> BCollection
        {
            get { return BtoA.Keys; }
        }
    }

    public class MapEqualityComparer<T1, T2> : IEqualityComparer<OneToOneMap<T1, T2>> where T1 : class
    {
        public bool Equals(OneToOneMap<T1, T2> x, OneToOneMap<T1, T2> y)
        {
            foreach (var a in x.ACollection)
                if (!y.Contains(a) || !x.MapTo(x.B, a).Equals(y.MapTo(y.B, a)))
                    return false;
            return true;
        }

        public int GetHashCode(OneToOneMap<T1, T2> obj)
        {
            return HashcodeHelper.GetCollectionHashcode(obj.GetMapsAB());
        }
    }
}