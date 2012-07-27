using System;
using System.Collections.Generic;
using System.Configuration;
using GoogleTranslateNET;
using GoogleTranslateNET.Objects.Translation;

namespace GoogleTranslateNETClient
{
    public class Program
    {
        private static string _key = ConfigurationManager.AppSettings["Key"];

        static void Main(string[] args)
        {
            GoogleTranslate google = new GoogleTranslate(_key);

            List<Translation> results = google.Translate(Language.English, Language.German, "Hello there.", "How are you?", "Multiple texts are allowed!");

            foreach (Translation translation in results)
            {
                Console.WriteLine(translation.TranslatedText);
            }

            Console.ReadLine();
        }
    }
}
