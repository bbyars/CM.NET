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
    public class PropertyList : IEnumerable<KeyValuePair<string, string>>
    {
        private readonly List<KeyValuePair<string, string>> properties = new List<KeyValuePair<string, string>>();

        public virtual PropertyList Add(string key, string value)
        {
            properties.Add(new KeyValuePair<string, string>(key, value));
            return this;
        }

        public override string ToString()
        {
            var lines = properties.Select(prop => string.Format("[{0}: {1}]", prop.Key, prop.Value)).ToArray();
            return string.Join(", ", lines);
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

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return properties.GetEnumerator();
        }
    }
}