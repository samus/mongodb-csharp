using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace MongoDB.Driver.Util
{
    /// <summary>
    /// This was ripped out of SubSonic I believe.
    /// </summary>
    public static class Inflector
    {
        private static readonly List<InflectorRule> _plurals = new List<InflectorRule>();
        private static readonly List<InflectorRule> _singulars = new List<InflectorRule>();
        private static readonly List<string> _uncountables = new List<string>();

        /// <summary>
        /// Initializes the <see cref="Inflector"/> class.
        /// </summary>
        static Inflector()
        {
            AddPluralRule("$", "s");
            AddPluralRule("s$", "s");
            AddPluralRule("(ax|test)is$", "$1es");
            AddPluralRule("(octop|vir)us$", "$1i");
            AddPluralRule("(alias|status)$", "$1es");
            AddPluralRule("(bu)s$", "$1ses");
            AddPluralRule("(buffal|tomat)o$", "$1oes");
            AddPluralRule("([ti])um$", "$1a");
            AddPluralRule("sis$", "ses");
            AddPluralRule("(?:([^f])fe|([lr])f)$", "$1$2ves");
            AddPluralRule("(hive)$", "$1s");
            AddPluralRule("([^aeiouy]|qu)y$", "$1ies");
            AddPluralRule("(x|ch|ss|sh)$", "$1es");
            AddPluralRule("(matr|vert|ind)ix|ex$", "$1ices");
            AddPluralRule("([m|l])ouse$", "$1ice");
            AddPluralRule("^(ox)$", "$1en");
            AddPluralRule("(quiz)$", "$1zes");

            AddSingularRule("s$", String.Empty);
            AddSingularRule("ss$", "ss");
            AddSingularRule("(n)ews$", "$1ews");
            AddSingularRule("([ti])a$", "$1um");
            AddSingularRule("((a)naly|(b)a|(d)iagno|(p)arenthe|(p)rogno|(s)ynop|(t)he)ses$", "$1$2sis");
            AddSingularRule("(^analy)ses$", "$1sis");
            AddSingularRule("([^f])ves$", "$1fe");
            AddSingularRule("(hive)s$", "$1");
            AddSingularRule("(tive)s$", "$1");
            AddSingularRule("([lr])ves$", "$1f");
            AddSingularRule("([^aeiouy]|qu)ies$", "$1y");
            AddSingularRule("(s)eries$", "$1eries");
            AddSingularRule("(m)ovies$", "$1ovie");
            AddSingularRule("(x|ch|ss|sh)es$", "$1");
            AddSingularRule("([m|l])ice$", "$1ouse");
            AddSingularRule("(bus)es$", "$1");
            AddSingularRule("(o)es$", "$1");
            AddSingularRule("(shoe)s$", "$1");
            AddSingularRule("(cris|ax|test)es$", "$1is");
            AddSingularRule("(octop|vir)i$", "$1us");
            AddSingularRule("(alias|status)$", "$1");
            AddSingularRule("(alias|status)es$", "$1");
            AddSingularRule("^(ox)en", "$1");
            AddSingularRule("(vert|ind)ices$", "$1ex");
            AddSingularRule("(matr)ices$", "$1ix");
            AddSingularRule("(quiz)zes$", "$1");

            AddIrregularRule("person", "people");
            AddIrregularRule("man", "men");
            AddIrregularRule("child", "children");
            AddIrregularRule("sex", "sexes");
            AddIrregularRule("tax", "taxes");
            AddIrregularRule("move", "moves");

            AddUnknownCountRule("equipment");
            AddUnknownCountRule("information");
            AddUnknownCountRule("rice");
            AddUnknownCountRule("money");
            AddUnknownCountRule("species");
            AddUnknownCountRule("series");
            AddUnknownCountRule("fish");
            AddUnknownCountRule("sheep");
        }

        /// <summary>
        /// Adds the irregular rule.
        /// </summary>
        /// <param name="singular">The singular.</param>
        /// <param name="plural">The plural.</param>
        private static void AddIrregularRule(string singular, string plural)
        {
            AddPluralRule(String.Concat("(", singular[0], ")", singular.Substring(1), "$"),
                String.Concat("$1", plural.Substring(1)));
            AddSingularRule(String.Concat("(", plural[0], ")", plural.Substring(1), "$"),
                String.Concat("$1", singular.Substring(1)));
        }

        /// <summary>
        /// Adds the unknown count rule.
        /// </summary>
        /// <param name="word">The word.</param>
        private static void AddUnknownCountRule(string word)
        {
            _uncountables.Add(word.ToLower());
        }

        /// <summary>
        /// Adds the plural rule.
        /// </summary>
        /// <param name="rule">The rule.</param>
        /// <param name="replacement">The replacement.</param>
        private static void AddPluralRule(string rule, string replacement)
        {
            _plurals.Add(new InflectorRule(rule, replacement));
        }

        /// <summary>
        /// Adds the singular rule.
        /// </summary>
        /// <param name="rule">The rule.</param>
        /// <param name="replacement">The replacement.</param>
        private static void AddSingularRule(string rule, string replacement)
        {
            _singulars.Add(new InflectorRule(rule, replacement));
        }

        /// <summary>
        /// Makes the plural.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <returns></returns>
        public static string MakePlural(string word)
        {
            return ApplyRules(_plurals, word);
        }

        /// <summary>
        /// Makes the singular.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <returns></returns>
        public static string MakeSingular(string word)
        {
            return ApplyRules(_singulars, word);
        }

        /// <summary>
        /// Applies the rules.
        /// </summary>
        /// <param name="rules">The rules.</param>
        /// <param name="word">The word.</param>
        /// <returns></returns>
        private static string ApplyRules(IList<InflectorRule> rules, string word)
        {
            string result = word;
            if (!_uncountables.Contains(word.ToLower()))
            {
                for (int i = rules.Count - 1; i >= 0; i--)
                {
                    string currentPass = rules[i].Apply(word);
                    if (currentPass != null)
                    {
                        result = currentPass;
                        break;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Converts the string to title case.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <returns></returns>
        public static string ToTitleCase(string word)
        {
            return Regex.Replace(ToHumanCase(AddUnderscores(word)), @"\b([a-z])",
                match => match.Captures[0].Value.ToUpper());
        }

        /// <summary>
        /// Converts the string to human case.
        /// </summary>
        /// <param name="lowercaseAndUnderscoredWord">The lowercase and underscored word.</param>
        /// <returns></returns>
        public static string ToHumanCase(string lowercaseAndUnderscoredWord)
        {
            return MakeInitialCaps(Regex.Replace(lowercaseAndUnderscoredWord, @"_", " "));
        }

        /// <summary>
        /// Convert string to proper case
        /// </summary>
        /// <param name="sourceString">The source string.</param>
        /// <returns></returns>
        public static string ToProper(string sourceString)
        {
            string propertyName = ToPascalCase(sourceString);
            return propertyName;
        }

        /// <summary>
        /// Converts the string to pascal case.
        /// </summary>
        /// <param name="lowercaseAndUnderscoredWord">The lowercase and underscored word.</param>
        /// <returns></returns>
        public static string ToPascalCase(string lowercaseAndUnderscoredWord)
        {
            return ToPascalCase(lowercaseAndUnderscoredWord, true);
        }

        /// <summary>
        /// Converts text to pascal case...
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="removeUnderscores">if set to <c>true</c> [remove underscores].</param>
        /// <returns></returns>
        public static string ToPascalCase(string text, bool removeUnderscores)
        {
            if (String.IsNullOrEmpty(text))
                return text;

            text = text.Replace("_", " ");
            string joinString = removeUnderscores ? String.Empty : "_";
            string[] words = text.Split(' ');
            if (words.Length > 1)// || char.IsUpper(words[0][0]))
            {
                for (int i = 0; i < words.Length; i++)
                {
                    if (words[i].Length > 0)
                    {
                        string word = words[i];
                        string restOfWord = word.Substring(1);

                        restOfWord = restOfWord.ToLower(CultureInfo.CurrentUICulture);

                        char firstChar = char.ToUpper(word[0], CultureInfo.CurrentUICulture);
                        words[i] = String.Concat(firstChar, restOfWord);
                    }
                }
                return String.Join(joinString, words);
            }
            return String.Concat(words[0].Substring(0, 1).ToUpper(CultureInfo.CurrentUICulture), words[0].Substring(1));
        }

        /// <summary>
        /// Converts the string to camel case.
        /// </summary>
        /// <param name="lowercaseAndUnderscoredWord">The lowercase and underscored word.</param>
        /// <returns></returns>
        public static string ToCamelCase(string lowercaseAndUnderscoredWord)
        {
            return MakeInitialLowerCase(ToPascalCase(lowercaseAndUnderscoredWord));
        }

        /// <summary>
        /// Adds the underscores.
        /// </summary>
        /// <param name="pascalCasedWord">The pascal cased word.</param>
        /// <returns></returns>
        public static string AddUnderscores(string pascalCasedWord)
        {
            return
                Regex.Replace(
                    Regex.Replace(Regex.Replace(pascalCasedWord, @"([A-Z]+)([A-Z][a-z])", "$1_$2"), @"([a-z\d])([A-Z])",
                        "$1_$2"), @"[-\s]", "_").ToLower();
        }

        /// <summary>
        /// Converts the underscores to dashes.
        /// </summary>
        /// <param name="underscoredWord">The underscored word.</param>
        /// <returns></returns>
        public static string ConvertUnderscoresToDashes(string underscoredWord)
        {
            return underscoredWord.Replace('_', '-');
        }

        /// <summary>
        /// Makes the initial caps.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <returns></returns>
        public static string MakeInitialCaps(string word)
        {
            return String.Concat(word.Substring(0, 1).ToUpper(), word.Substring(1).ToLower());
        }

        /// <summary>
        /// Makes the initial lower case.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <returns></returns>
        public static string MakeInitialLowerCase(string word)
        {
            return String.Concat(word.Substring(0, 1).ToLower(), word.Substring(1));
        }

        #region Nested type: InflectorRule

        /// <summary>
        /// Summary for the InflectorRule class
        /// </summary>
        private class InflectorRule
        {
            /// <summary>
            /// 
            /// </summary>
            public readonly Regex regex;

            /// <summary>
            /// 
            /// </summary>
            public readonly string replacement;

            /// <summary>
            /// Initializes a new instance of the <see cref="InflectorRule"/> class.
            /// </summary>
            /// <param name="regexPattern">The regex pattern.</param>
            /// <param name="replacementText">The replacement text.</param>
            public InflectorRule(string regexPattern, string replacementText)
            {
                regex = new Regex(regexPattern, RegexOptions.IgnoreCase);
                replacement = replacementText;
            }

            /// <summary>
            /// Applies the specified word.
            /// </summary>
            /// <param name="word">The word.</param>
            /// <returns></returns>
            public string Apply(string word)
            {
                if (!regex.IsMatch(word))
                    return null;

                string replace = regex.Replace(word, replacement);
                if (word == word.ToUpper())
                    replace = replace.ToUpper();

                return replace;
            }
        }

        #endregion
    }
}