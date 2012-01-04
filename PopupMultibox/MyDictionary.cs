using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace Henderson.Util.MyDictionary
{
    /// <summary>
    /// <para>
    /// My implementation of a jagged-depth multi-dimensional key-value pair association list.
    /// It supports keys of type string and int, and values of type string, int, double, and bool (as well as another level).
    /// It will automatically create a new level when you try to use it.
    /// </para>
    /// <para>
    /// Example usage:
    /// </para>
    /// <example>
    /// <para>
    ///     MyDictionary dict = new MyDictionary();
    /// </para>
    /// <para>
    ///     dict["hi"] = 5;
    /// </para>
    /// <para>
    ///     dict["hi2"]["hi3"] = "hi";
    /// </para>
    /// <para>
    ///     dict["hi2"]["hi4"]["hi5"] = 1;
    /// </para>
    /// <para>
    ///     dict["hi2"]["hi4"]["hi6"] = "hi2";
    /// </para>
    /// <para>
    ///     dict[1] = "hi1";
    /// </para>
    /// <para>
    ///     dict[dict["hi"]] = false;
    /// </para>
    /// <para>
    ///     int dicthi = dict["hi"]; // 5
    /// </para>
    /// <para>
    ///     string dicthi2 = dict["hi"]; // "5"
    /// </para>
    /// <para>
    ///     string dict1 = dict[1]; // "hi1"
    /// </para>
    /// <para>
    ///     bool dict5 = dict[5]; // false
    /// </para>
    /// <para>
    ///     string dict52 = dict[5]; // "False"
    /// </para>
    /// </example>
    /// </summary>
    class MyDictionary : IEnumerable<MyDictionary>
    {
        Dictionary<MyKey, MyDictionary> dict;
        object val = null;
        MyKey myKey = null;

        /// <summary>
        /// Gets the value associated with the given key
        /// </summary>
        /// <param name="key">Either an int or a string</param>
        /// <returns>The value associated with the given key</returns>
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
                    catch
                    {
                        return null;
                    }
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

        /// <summary>
        /// Converts a MyDictionary object to a KeyValuePair object (used to allow the iterator to be of type MyDictionary or KeyValuePair)
        /// </summary>
        /// <param name="m">The MyDictionary object to convert</param>
        /// <returns>The MyDictionary object represented as a KeyValuePair</returns>
        public static implicit operator KeyValuePair<MyKey, MyDictionary>(MyDictionary m)
        {
            try
            {
                return new KeyValuePair<MyKey, MyDictionary>(m.myKey, m);
            }
            catch
            {
                return default(KeyValuePair<MyKey, MyDictionary>);
            }
        }

        /// <summary>
        /// Converts a KeyValuePair object to a MyDictionary object.
        /// </summary>
        /// <param name="o">The KeyValuePair object to convert</param>
        /// <returns>The KeyValuePair object represented as a MyDictionary</returns>
        public static implicit operator MyDictionary(KeyValuePair<MyKey, MyDictionary> o)
        {
            try
            {
                MyDictionary d = o.Value;
                d.myKey = o.Key;
                return d;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Converts the value of a MyDictionary object to a string (used to retrieve strings from the dictionary)
        /// </summary>
        /// <param name="m">The MyDictionary object to convert</param>
        /// <returns>The value of the MyDictionary object represented as a string</returns>
        public static implicit operator string(MyDictionary m)
        {
            try
            {
                return m.val + "";
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// Converts a string to a MyDictionary object (used to store strings in the dictionary)
        /// </summary>
        /// <param name="o">The string to convert</param>
        /// <returns>A MyDictionary object that stores the given string as its value</returns>
        public static implicit operator MyDictionary(string o)
        {
            MyDictionary d = new MyDictionary();
            try
            {
                d.val = o;
                return d;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Converts the value of a MyDictionary object to an int (used to retrieve ints from the dictionary)
        /// </summary>
        /// <param name="m">The MyDictionary object to convert</param>
        /// <returns>The value of the MyDictionary object represented as an int</returns>
        public static implicit operator int(MyDictionary m)
        {
            try
            {
                return (int)((double)m.val);
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Converts an int to a MyDictionary object (used to store ints in the dictionary)
        /// </summary>
        /// <param name="o">The int to convert</param>
        /// <returns>A MyDictionary object that stores the given int as its value</returns>
        public static implicit operator MyDictionary(int o)
        {
            MyDictionary d = new MyDictionary();
            try
            {
                d.val = (double)o;
                return d;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Converts the value of a MyDictionary object to a double (used to retrieve doubles from the dictionary)
        /// </summary>
        /// <param name="m">The MyDictionary object to convert</param>
        /// <returns>The value of the MyDictionary object represented as a double</returns>
        public static implicit operator double(MyDictionary m)
        {
            try
            {
                return (double)m.val;
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Converts a double to a MyDictionary object (used to store doubles in the dictionary)
        /// </summary>
        /// <param name="o">The double to convert</param>
        /// <returns>A MyDictionary object that stores the given double as its value</returns>
        public static implicit operator MyDictionary(double o)
        {
            MyDictionary d = new MyDictionary();
            try
            {
                d.val = o;
                return d;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Converts the value of a MyDictionary object to a bool (used to retrieve bools from the dictionary)
        /// </summary>
        /// <param name="m">The MyDictionary object to convert</param>
        /// <returns>The value of the MyDictionary object represented as a double</returns>
        public static implicit operator bool(MyDictionary m)
        {
            try
            {
                return (bool)m.val;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Converts a bool to a MyDictionary object (used to store bools in the dictionary)
        /// </summary>
        /// <param name="o">The bool to convert</param>
        /// <returns>A MyDictionary object that stores the given bool as its value</returns>
        public static implicit operator MyDictionary(bool o)
        {
            MyDictionary d = new MyDictionary();
            try
            {
                d.val = o;
                return d;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Converts a MyKey object to a MyDictionary object
        /// </summary>
        /// <param name="m">The MyKey object to convert</param>
        /// <returns>A MyDictionary object that stores the value from the MyKey object</returns>
        public static implicit operator MyDictionary(MyKey m)
        {
            try
            {
                MyDictionary d = new MyDictionary();
                d.val = m.Key;
                return d;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the value stored in this MyDictionary object
        /// </summary>
        public object Value
        {
            get
            {
                return val;
            }
        }

        /// <summary>
        /// Gets the values stored under this level (including other levels)
        /// </summary>
        public MyDictionary[] Values
        {
            get
            {
                return dict.Values.ToArray();
            }
        }

        /// <summary>
        /// Gets the values stored under this level (including other levels) as objects
        /// </summary>
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

        /// <summary>
        /// Gets the values stored under this level (including other levels) as strings
        /// Other levels will be represented as empty strings
        /// </summary>
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

        /// <summary>
        /// Gets the keys defined at this level as MyKey objects, which can be converted to either ints or strings
        /// </summary>
        public MyKey[] Keys
        {
            get
            {
                return dict.Keys.ToArray();
            }
        }

        /// <summary>
        /// Gets the keys defined at this level as strings
        /// </summary>
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

        /// <summary>
        /// Gets the keys defined at this level as ints
        /// </summary>
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
                    //lst.Add(k);
                }
                return lst.ToArray();
            }
        }

        /// <summary>
        /// Creates a new MyDictionary object.  This should only be called once, to create the top level MyDictionary object.
        /// All other MyDictionary objects are created automatically.
        /// </summary>
        public MyDictionary()
        {
            dict = new Dictionary<MyKey, MyDictionary>(0);
        }

        /// <summary>
        /// Determines if this MyDictionary object is equal to the given object
        /// </summary>
        /// <param name="obj">The object to check equality with</param>
        /// <returns>true if they are equal, false otherwise</returns>
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
                catch
                {
                    return false;
                }
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

        /// <summary>
        /// Gets the hash code for this MyDictionary object.  This is only here to make the Equals method work.
        /// </summary>
        /// <returns>The hash code</returns>
        public override int GetHashCode()
        {
            try
            {
                return val.GetHashCode();
            }
            catch
            {
                return base.GetHashCode();
            }
        }

        #region IEnumerable Members

        /// <summary>
        /// Returns an enumerator that goes through the elements at this level (including other level objects)
        /// </summary>
        /// <returns>The iterator</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new MyValueIterator(dict);
        }

        #endregion

        #region IEnumerable<MyDictionary> Members

        /// <summary>
        /// Returns an enumerator that goes through the elements at this level (including other level objects)
        /// </summary>
        /// <returns>The iterator</returns>
        IEnumerator<MyDictionary> IEnumerable<MyDictionary>.GetEnumerator()
        {
            return new MyValueIterator(dict);
        }

        #endregion

        /// <summary>
        /// Creates a string representation of this MyDictionary instance and its contents with the default indentAmount of 4.
        /// </summary>
        /// <returns>A string representation of this MyDictionary instance</returns>
        public string makeRepresentationString()
        {
            return this.makeRepresentationString(4);
        }

        /// <summary>
        /// Creates a string representation of this MyDictionary instance and its contents with an indentation amount as given (minimum 1)
        /// </summary>
        /// <param name="indentAmount">The amount to indent each level beyond the containing level</param>
        /// <returns>A string representation of this MyDictionary instance</returns>
        public string makeRepresentationString(int indentAmount)
        {
            if (indentAmount <= 0)
                indentAmount = 1;
            return makeKeyValuePairsString(this, 0, indentAmount);
        }

        /// <summary>
        /// Helper method for makeRepresentationString
        /// </summary>
        /// <param name="dict">The MyDictionary object to iterate through</param>
        /// <param name="lv">The current level of indentation</param>
        /// <param name="indentAmount">The amount to indent each level</param>
        /// <returns>A string representation of the given MyDictionary instance with the given base indent level/amount</returns>
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

    /// <summary>
    /// Represents a key for a MyDictionary object.  Allows ints and strings.
    /// </summary>
    class MyKey
    {
        object key = null;

        /// <summary>
        /// Gets the key as an object
        /// </summary>
        public object Key
        {
            get
            {
                return key;
            }
        }

        /// <summary>
        /// Creates a MyKey instance with the given key
        /// </summary>
        /// <param name="key">The key to store in the MyKey instance</param>
        private MyKey(object key)
        {
            this.key = key;
        }

        /// <summary>
        /// Converts the key stored in the MyKey instance to a string (used for retrieving the value of a key and for comparison)
        /// </summary>
        /// <param name="m">The MyKey instance to convert</param>
        /// <returns>The key stored in the MyKey instance represented as a string</returns>
        public static implicit operator string(MyKey m)
        {
            try
            {
                return m.key + "";
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// Converts the given string to a MyKey instance (used for string indicies in a MyDictionary)
        /// </summary>
        /// <param name="o">The string to convert</param>
        /// <returns>The given string represented as a MyKey instance</returns>
        public static implicit operator MyKey(string o)
        {
            return new MyKey(o);
        }

        /// <summary>
        /// Converts the key stored in the MyKey instance to an int (used for retrieving the value of a key and for comparison)
        /// </summary>
        /// <param name="m">The MyKey instance to convert</param>
        /// <returns>The key stored in the Mykey instance represented as an int</returns>
        public static implicit operator int(MyKey m)
        {
            try
            {
                return (int)((double)m.key);
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Converts the given int to a MyKey instance (used for int indicies in a MyDictionary)
        /// </summary>
        /// <param name="o">The int to convert</param>
        /// <returns>The given int represented as a MyKey instance</returns>
        public static implicit operator MyKey(int o)
        {
            return new MyKey((double)o);
        }

        /// <summary>
        /// Converts the given MyDictionary object to a MyKey instance (used for accessing an index specified by another MyDictionary entry)
        /// </summary>
        /// <param name="o">The MyDictionary object to convert</param>
        /// <returns>The MyDictionary object represented as a MyKey instance</returns>
        public static implicit operator MyKey(MyDictionary o)
        {
            try
            {
                return new MyKey(o.Value);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Compares this MyKey instance to the given object for equality (used for recognizing existing indicies in a MyDicitonary)
        /// </summary>
        /// <param name="obj">The object to compare for equality</param>
        /// <returns>true if the given object is equal to this MyKey instance, false otherwise</returns>
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

        /// <summary>
        /// Gets the hash code for this MyKey instance.  This is only here to make Equals work.
        /// </summary>
        /// <returns>The hash code</returns>
        public override int GetHashCode()
        {
            try
            {
                return key.GetHashCode();
            }
            catch
            {
                return base.GetHashCode();
            }
        }
    }

    /// <summary>
    /// An iterator for a MyDictionary
    /// </summary>
    class MyValueIterator : IEnumerator<MyDictionary>
    {
        private Dictionary<MyKey, MyDictionary> dict;
        private IEnumerator<MyDictionary> iter;

        /// <summary>
        /// Creates a new iterator using the given dictionary to get the iterator that actually does the work
        /// </summary>
        /// <param name="dict"></param>
        public MyValueIterator(Dictionary<MyKey, MyDictionary> dict)
        {
            this.dict = dict;
            this.iter = dict.Values.GetEnumerator();
        }

        #region IEnumerator<MyDictionary> Members

        /// <summary>
        /// Gets the current element
        /// </summary>
        public MyDictionary Current
        {
            get { return iter.Current; }
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Disposes this iterator (actually the contained iterator)
        /// </summary>
        public void Dispose()
        {
            iter.Dispose();
        }

        #endregion

        #region IEnumerator Members

        /// <summary>
        /// Gets the current element
        /// </summary>
        object IEnumerator.Current
        {
            get { return iter.Current; }
        }

        /// <summary>
        /// Moves to the next element
        /// </summary>
        /// <returns>True if successful, false otherwise</returns>
        public bool MoveNext()
        {
            return iter.MoveNext();
        }

        /// <summary>
        /// Resets this iterator back to the beginning
        /// </summary>
        public void Reset()
        {
            iter.Reset();
        }

        #endregion
    }
}