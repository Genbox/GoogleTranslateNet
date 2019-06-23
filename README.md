# GoogleTranslate.Net - A full implementation of the Google Translate 2.0 API

### Features

* Based on RestSharp (http://restsharp.org) to deserialize the Google API JSON into objects
* Can translate text from one language to another
* Can automatically detect the source language
* Can detect the language of a text
* Can list the supported translation languages from the Google Translate API

### Tutorial

First you need to get a Google Translate API key from their website.
There is a Getting Started guide here: https://developers.google.com/translate/v2/getting_started

### Examples

```csharp
static void Main(string[] args)
{
    GoogleTranslate google = new GoogleTranslate("INSERT-YOUR-KEY-HERE");

    List<Translation> results = google.Translate(Language.English, Language.German, "Hello there.", "How are you?", "Multiple texts are allowed!");

    foreach (Translation translation in results)
    {
        Console.WriteLine(translation.TranslatedText);
    }

    Console.ReadLine();
}
```

For more examples, take a look at the GoogleTranslate.Examples and GoogleTranslate.Tests projects.