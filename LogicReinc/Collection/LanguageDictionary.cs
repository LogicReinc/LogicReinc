using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicReinc.Collections
{
    public class LanguageDictionary
    {
        private Dictionary<string, Dictionary<string, string>> collections;

        public LanguageDictionary()
        {
            collections = new Dictionary<string, Dictionary<string, string>>();
        }

        public Dictionary<string,string> GetLangauge(string languageName)
        {
            if (!collections.ContainsKey(languageName))
                return null;
            return collections[languageName];
        }

        public void AddLanguage(string name, Dictionary<string,string> language)
        {
            collections.Add(name, language);
        }
        public void RemoveLanguage(string name)
        {
            collections.Remove(name);
        }

        public string GetLiteral(string language, string literalName, string defaultLanguage = null)
        {
            Dictionary<string, string> l = GetLangauge(language);

            if (l != null && l.ContainsKey(literalName))
                return l[literalName];

            if (l == null && defaultLanguage != null)
                l = GetLangauge(defaultLanguage);

            if (l != null && l.ContainsKey(literalName))
                return l[literalName];

            return "";
        }
    }
}
