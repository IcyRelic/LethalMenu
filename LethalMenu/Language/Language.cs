using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace LethalMenu.Language
{
    public class Language
    {
        public string Name { get; private set; }
        public string Translator { get; private set; }
        private Dictionary<string, string> _language;
        
        public Language(string name, string translator, Dictionary<string, string> language)
        {
            Name = name;
            Translator = translator;
            _language = language;
        }
        public string Localize(string key) => _language.ContainsKey(key) ? _language[key] : key;
        public bool Has(string key) => _language.ContainsKey(key);
    }

    public class Localization
    {
        public static Language Language { get; private set; }
        private static Dictionary<string, Language> _languages = new Dictionary<string, Language>();
        
        private static bool _initialized = false;
        public static void Initialize()
        {
            try
            {
                if (_initialized) return;

                LoadLanguage();
                SetLanguage("English");
                _initialized = true;
            } 
            catch (Exception e)
            {
                UnityEngine.Debug.LogError($"Failed to initialize localization: {e.Message}");
                UnityEngine.Debug.LogException(e);
            }
        }

        private static void LoadLanguage()
        {
            Assembly.GetExecutingAssembly().GetManifestResourceNames().Where(x => x.StartsWith("LethalMenu.Resources.Language.") && x.EndsWith(".json")).ToList().ForEach(x =>
            {
                var jsonStr = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(x)).ReadToEnd();

                JObject json = JObject.Parse(jsonStr);

                if (!json.TryGetValue("DETAILS", out JToken detailsToken)) return;
                if (!json.TryGetValue("LANGUAGE", out JToken langToken)) return;

 
                var details = detailsToken.ToObject<Dictionary<string, string>>();
                var lang = JObject.Parse(langToken.ToString());

                if(!details.ContainsKey("LANGUAGE") || !details.ContainsKey("TRANSLATOR")) return;

                Dictionary<string, string> localization = new Dictionary<string, string>();


                lang.Properties().ToList().ForEach(sectionProperty =>
                {
                    
                    var section = JObject.Parse(sectionProperty.Value.ToString());
                    
                    //foreach y key value pair
                    foreach (var prop in section.Properties())
                    {
                        var key = $"{sectionProperty.Name}.{prop.Name}";
                        localization.Add(key, prop.Value.ToString());
                        UnityEngine.Debug.Log($"Loaded localization {key} = {prop.Value}");
                    }
                });

                _languages.Add(details["LANGUAGE"], new Language(details["LANGUAGE"], details["TRANSLATOR"], localization));
                UnityEngine.Debug.Log($"Loaded Language {details["LANGUAGE"]} by {details["TRANSLATOR"]}");
            });
        }

        public static void SetLanguage(string name) => Language = LanguageExists(name) ? _languages[name] : _languages["English"];
        public static bool LanguageExists(string name) => _languages.ContainsKey(name);
        public static string Localize(string key) => Language.Has(key) ? Language.Localize(key) : LocalizeEnglish(key);
        private static string LocalizeEnglish(string key) => _languages["English"].Localize(key);
    }
}
