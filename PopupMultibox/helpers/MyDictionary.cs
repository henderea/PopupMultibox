using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace Henderson.Util.MyDictionary
{
    class MyDictionary : IEnumerable<MyDictionary>
    {
        Dictionary<MyKey, MyDictionary> dict;
        object val = null;
        MyKey myKey = null;

        public MyDictionary this[MyKey key]
        {
            get
            {
                try
                {
                    this.val = null;
                    return dict[key];
                }
                catch
                {
                    try
                    {
                        dict[key] = new MyDictionary();
                        dict[key].myKey = key;
                        return dict[key];
                    }
                    catch { }
                    return null;
                }
            }
            set
            {
                try
                {
                    dict[key] = value;
                    dict[key].myKey = key;
                }
                catch { }
            }
        }

        public static implicit operator KeyValuePair<MyKey, MyDictionary>(MyDictionary m)
        {
            try
            {
                return new KeyValuePair<MyKey, MyDictionary>(m.myKey, m);
            }
            catch { }
            return default(KeyValuePair<MyKey, MyDictionary>);
        }

        public static implicit operator MyDictionary(KeyValuePair<MyKey, MyDictionary> o)
        {
            try
            {
                MyDictionary d = o.Value;
                d.myKey = o.Key;
                return d;
            }
            catch { }
            return null;
        }

        public static implicit operator string(MyDictionary m)
        {
            try
            {
                return m.val + "";
            }
            catch { }
            return "";
        }

        public static implicit operator MyDictionary(string o)
        {
            MyDictionary d = new MyDictionary();
            try
            {
                d.val = o;
                return d;
            }
            catch { }
            return null;
        }

        public static implicit operator int(MyDictionary m)
        {
            try
            {
                return (int)((double)m.val);
            }
            catch { }
            return 0;
        }

        public static implicit operator MyDictionary(int o)
        {
            MyDictionary d = new MyDictionary();
            try
            {
                d.val = (double)o;
                return d;
            }
            catch { }
            return null;
        }

        public static implicit operator double(MyDictionary m)
        {
            try
            {
                return (double)m.val;
            }
            catch { }
            return 0;
        }

        public static implicit operator MyDictionary(double o)
        {
            MyDictionary d = new MyDictionary();
            try
            {
                d.val = o;
                return d;
            }
            catch { }
            return null;
        }

        public static implicit operator bool(MyDictionary m)
        {
            try
            {
                return (bool)m.val;
            }
            catch { }
            return false;
        }

        public static implicit operator MyDictionary(bool o)
        {
            MyDictionary d = new MyDictionary();
            try
            {
                d.val = o;
                return d;
            }
            catch { }
            return null;
        }

        public static implicit operator MyDictionary(MyKey m)
        {
            try
            {
                MyDictionary d = new MyDictionary();
                d.val = m.Key;
                return d;
            }
            catch { }
            return null;
        }

        public object Value
        {
            get
            {
                return val;
            }
        }

        public MyDictionary[] Values
        {
            get
            {
                return dict.Values.ToArray();
            }
        }

        public object[] OValues
        {
            get
            {
                List<object> lst = new List<object>(0);
                foreach (MyDictionary d in dict.Values)
                {
                    if (d.Value != null)
                        lst.Add(d.Value);
                    lst.Add(d);
                }
                return lst.ToArray();
            }
        }

        public string[] SValues
        {
            get
            {
                List<string> lst = new List<string>(0);
                foreach (MyDictionary d in dict.Values)
                {
                    lst.Add(d);
                }
                return lst.ToArray();
            }
        }

        public MyKey[] Keys
        {
            get
            {
                return dict.Keys.ToArray();
            }
        }

        public string[] SKeys
        {
            get
            {
                List<string> lst = new List<string>(0);
                foreach (MyKey k in dict.Keys)
                {
                    lst.Add(k);
                }
                return lst.ToArray();
            }
        }

        public int[] IKeys
        {
            get
            {
                List<int> lst = new List<int>(0);
                foreach (MyKey k in dict.Keys)
                {
                    try
                    {
                        lst.Add(int.Parse(k+""));
                    }
                    catch { }
                }
                return lst.ToArray();
            }
        }

        public MyDictionary()
        {
            dict = new Dictionary<MyKey, MyDictionary>(0);
        }

        public override bool Equals(object obj)
        {
            if (obj is MyKey)
                return ((MyKey)obj).Equals(this);
            if (obj is MyDictionary && ((MyDictionary)obj).Value != null)
                return ((MyDictionary)obj).Value.Equals(this.Value);
            else if (obj is MyDictionary)
            {
                try
                {
                    return ((MyDictionary)obj).dict.Equals(this.dict);
                }
                catch { }
                return false;
            }
            if (obj is string)
                return ((string)obj).Equals((string)this);
            if (obj is int)
                return ((int)obj).Equals((int)this);
            if (obj is double)
                return ((double)obj).Equals((double)this);
            if (obj is bool)
                return ((bool)obj).Equals((bool)this);
            return false;
        }

        public override int GetHashCode()
        {
            try
            {
                return val.GetHashCode();
            }
            catch { }
            return base.GetHashCode();
        }

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new MyValueIterator(dict);
        }

        #endregion

        #region IEnumerable<MyDictionary> Members

        IEnumerator<MyDictionary> IEnumerable<MyDictionary>.GetEnumerator()
        {
            return new MyValueIterator(dict);
        }

        #endregion

        public string makeRepresentationString()
        {
            return this.makeRepresentationString(4);
        }

        public string makeRepresentationString(int indentAmount)
        {
            if (indentAmount <= 0)
                indentAmount = 1;
            return makeKeyValuePairsString(this, 0, indentAmount);
        }

        private string makeKeyValuePairsString(MyDictionary dict, int lv, int indentAmount)
        {
            string rval = "";
            foreach (KeyValuePair<MyKey, MyDictionary> d in dict)
            {
                for (int i = 0; i < lv * indentAmount; i++)
                {
                    rval += " ";
                }
                rval += d.Key + ": " + d.Value + "\n";
                if (d.Value.Value == null)
                    rval += makeKeyValuePairsString(d.Value, lv + 1, indentAmount);
            }
            return rval;
        }
    }

    class MyKey
    {
        object key = null;

        public object Key
        {
            get
            {
                return key;
            }
        }

        private MyKey(object key)
        {
            this.key = key;
        }

        public static implicit operator string(MyKey m)
        {
            try
            {
                return m.key + "";
            }
            catch { }
            return "";
        }

        public static implicit operator MyKey(string o)
        {
            return new MyKey(o);
        }

        public static implicit operator int(MyKey m)
        {
            try
            {
                return (int)((double)m.key);
            }
            catch { }
            return 0;
        }

        public static implicit operator MyKey(int o)
        {
            return new MyKey((double)o);
        }

        public static implicit operator MyKey(MyDictionary o)
        {
            try
            {
                return new MyKey(o.Value);
            }
            catch { }
            return null;
        }

        public override bool Equals(object obj)
        {
            if (obj is MyKey)
                return ((MyKey)obj).key.Equals(this.key);
            if (obj is MyDictionary && ((MyDictionary)obj).Value != null)
                return ((MyDictionary)obj).Value.Equals(this.key);
            if (obj is string)
                return ((string)obj).Equals((string)this);
            if (obj is int)
                return ((int)obj).Equals((int)this);
            return false;
        }

        public override int GetHashCode()
        {
            try
            {
                return key.GetHashCode();
            }
            catch { }
            return base.GetHashCode();
        }
    }

    class MyValueIterator : IEnumerator<MyDictionary>
    {
        private Dictionary<MyKey, MyDictionary> dict;
        private IEnumerator<MyDictionary> iter;

        public MyValueIterator(Dictionary<MyKey, MyDictionary> dict)
        {
            this.dict = dict;
            this.iter = dict.Values.GetEnumerator();
        }

        #region IEnumerator<MyDictionary> Members

        public MyDictionary Current
        {
            get
            {
                return iter.Current;
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            iter.Dispose();
        }

        #endregion

        #region IEnumerator Members

        object IEnumerator.Current
        {
            get
            {
                return iter.Current;
            }
        }

        public bool MoveNext()
        {
            return iter.MoveNext();
        }

        public void Reset()
        {
            iter.Reset();
        }

        #endregion
    }
}