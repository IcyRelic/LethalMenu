using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System.Linq;
using System.IO;
using Newtonsoft.Json.Linq;
using LethalMenu.Util;

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

                GetLanguages().ToList().ForEach(x => Debug.Log($"Loaded Language {x}"));
            } 
            catch (Exception e)
            {
                Debug.LogError($"Failed to initialize localization: {e.Message}");
                Debug.LogException(e);
            }
        }

        private static void LoadLanguage()
        {
            Assembly.GetExecutingAssembly().GetManifestResourceNames().Where(x => x.StartsWith("LethalMenu.Resources.Language.") && x.EndsWith(".json")).ToList().ForEach(x =>
            {
                Debug.Log($"Loading localization file => {x}");
                var jsonStr = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(x)).ReadToEnd();

                JObject json = JObject.Parse(jsonStr);

                Dictionary<string, string> localization = new Dictionary<string, string>();

                if(!json.ContainsKey("LANGUAGE") || !json.ContainsKey("TRANSLATOR"))
                {
                    Debug.LogError($"Failed to load localization file => {x}");
                    return;
                }

                string language = json["LANGUAGE"].ToString();
                string translator = json["TRANSLATOR"].ToString();

                json.Properties().ToList().ForEach(p =>
                {
                    var name = p.Name;
                    var value = p.Value.ToString();

                    localization.Add(name, value);

                    //Debug.Log($"Loaded localization {name} = {value}");
                });



                _languages.Add(language, new Language(language, translator, localization));
                Debug.Log($"Loaded Language {language} by {translator}");
            });
        }

        public static string[] GetLanguages() => _languages.Keys.ToArray();
        public static void SetLanguage(string name) => Language = LanguageExists(name) ? _languages[name] : _languages["English"];
        public static bool LanguageExists(string name) => _languages.ContainsKey(name);
        public static string Localize(string key) => Language.Has(key) ? Language.Localize(key) : LocalizeEnglish(key);
        public static string[] LocalizeArray(string[] keys) => keys.Select(key => Localize(key)).ToArray();
        public static string Localize(string[] keys) => keys.Aggregate("", (current, key) => current + " " + Localize(key));
        private static string LocalizeEnglish(string key) => _languages["English"].Localize(key);
    }
}
