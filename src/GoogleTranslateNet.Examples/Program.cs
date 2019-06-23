using System;
using System.Collections.Generic;
using GoogleTranslateNet.Objects.Translation;

namespace GoogleTranslateNet.Examples
{
    public class Program
    {
        private const string _key = "YOUR API KEY HERE";

        private static void Main(string[] args)
        {
            GoogleTranslate google = new GoogleTranslate(_key);

            //Notice that we set the source language to Language.Automatic. This means Google Translate automatically detect the source language before translating.
            List<Translation> results = google.Translate(Language.Automatic, Language.German, "Hello there.", "How are you?", "Multiple texts are allowed!");

            foreach (Translation translation in results)
            {
                Console.WriteLine("Detected language: " + translation.DetectedSourceLanguage + " translation: " + translation.TranslatedText);
            }
        }
    }
}
