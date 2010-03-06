using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CM.Deployer
{
    /// <summary>
    /// The sort order on a collection of environment properties is important, since downstream
    /// property values may depend on properties already defined.  Therefore, a dictionary
    /// isn't really an appropriate absraction...
    /// </summary>
    public class PropertyList : IList<KeyValuePair<string, string>>
    {
        private readonly List<KeyValuePair<string, string>> properties = new List<KeyValuePair<string, string>>();

        public virtual PropertyList Add(string key, string value)
        {
            properties.Add(new KeyValuePair<string, string>(key, value));
            return this;
        }

        public override bool Equals(object obj)
        {
            var other = obj as PropertyList;
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.ToString(), ToString());
        }

        public override int GetHashCode()
        {
            return (properties != null ? properties.GetHashCode() : 0);
        }

        public override string ToString()
        {
            var lines = properties.Select(prop => string.Format("[{0}: {1}]", prop.Key, prop.Value)).ToArray();
            return string.Join(", ", lines);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return properties.GetEnumerator();
        }

        public void Add(KeyValuePair<string, string> item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(KeyValuePair<string, string> item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<string, string> item)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        public int IndexOf(KeyValuePair<string, string> item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, KeyValuePair<string, string> item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public KeyValuePair<string, string> this[int index]
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }
    }
}