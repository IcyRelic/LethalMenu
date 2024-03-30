using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace LethalMenu.Language;

public class Language
{
    private readonly Dictionary<string, string> _language;

    public Language(string name, string translator, Dictionary<string, string> language)
    {
        Name = name;
        Translator = translator;
        _language = language;
    }

    public int Rank { get; set; }
    public string Name { get; }
    public string Translator { get; private set; }

    public string Localize(string key)
    {
        return _language.ContainsKey(key) ? _language[key] : key;
    }

    public bool Has(string key)
    {
        return _language.ContainsKey(key);
    }

    public int Count()
    {
        return _language.Count;
    }
}

public class Localization
{
    private static readonly Dictionary<string, Language> _languages = new();
    private static bool _initialized;
    public static Language Language { get; private set; }

    public static void Initialize()
    {
        if (_initialized) return;
        LoadLanguage();
        SetLanguage("English");
        _initialized = true;
    }

    private static void LoadLanguage()
    {
        Assembly.GetExecutingAssembly().GetManifestResourceNames()
            .Where(x => x.StartsWith("LethalMenu.Resources.Language.") && x.EndsWith(".json")).ToList().ForEach(x =>
            {
                string jsonStr = null;
                try
                {
                    using (var reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(x)))
                    {
                        jsonStr = reader.ReadToEnd();
                    }

                    var json = JObject.Parse(jsonStr);

                    var localization = new Dictionary<string, string>();

                    if (!json.ContainsKey("LANGUAGE") || !json.ContainsKey("TRANSLATOR"))
                    {
                        Debug.LogError($"Failed to load localization file => {x}");
                        return;
                    }

                    var language = json["LANGUAGE"].ToString();
                    var translator = json["TRANSLATOR"].ToString();

                    json.Properties().ToList().ForEach(p =>
                    {
                        var name = p.Name;
                        var value = p.Value.ToString();

                        localization.Add(name, value);
                    });

                    _languages.Add(language, new Language(language, translator, localization));
                    Debug.Log($"Loaded Language {language} by {translator}");
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Json Error: Loading language file {x} because {ex.Message}");

                    var lineNumber = 0;
                    using (var reader = new StringReader(jsonStr))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            lineNumber++;
                            if (line.Contains(ex.Message))
                            {
                                Debug.LogError($"Error occurred at line {lineNumber} in file {x}");
                                break;
                            }
                        }
                    }

                    if (ex is JsonReaderException jsonEx)
                        Debug.LogError(
                            $"Json parsing error: Line {jsonEx.LineNumber}, Position {jsonEx.LinePosition} [{jsonEx.Path}]");
                }
            });

        var englishCount = _languages["English"].Count();
        _languages.Values.ToList().ForEach(x =>
        {
            if (x.Count() != englishCount)
            {
                var percentComplete = x.Count() / (double)englishCount * 100;
                Debug.LogWarning($"Language {x.Name} is missing {englishCount - x.Count()} keys");

                if (percentComplete < 80.00)
                {
                    _languages.Remove(x.Name);
                    Debug.LogError($"{x.Name} is too far behind. Unloading it.");
                }
            }
        });
    }

    public static string[] GetLanguages()
    {
        _languages.Values.ToList().ForEach(x => x.Rank = Language == x ? 1 : 999);
        return _languages.Values.OrderBy(x => x.Rank).ThenBy(x => x.Name).Select(x => x.Name).ToArray();
    }

    public static void SetLanguage(string name)
    {
        Language = LanguageExists(name) ? _languages[name] : _languages["English"];
    }

    public static bool LanguageExists(string name)
    {
        return _languages.ContainsKey(name);
    }

    public static string Localize(string key)
    {
        return Language.Has(key) ? Language.Localize(key) : LocalizeEnglish(key);
    }

    public static string[] LocalizeArray(string[] keys)
    {
        return keys.Select(key => Localize(key)).ToArray();
    }

    public static string Localize(string[] keys, bool newLine = false)
    {
        return keys.Aggregate("", (current, key) => current + (newLine ? "\n" : " ") + Localize(key)).Substring(1);
    }

    private static string LocalizeEnglish(string key)
    {
        return _languages["English"].Localize(key);
    }
}