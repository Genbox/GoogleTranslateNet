# GoogleTranslate.Net - A full implementation of the Google Translate 2.0 API

### Features

* Based on RestSharp (http://restsharp.org) to deserialize the Google API JSON into objects
* Can translate text from one language to another
* Can automatically detect the source language
* Can detect the language of a text
* Can list the supported translation languages from the Google Translate API

### Tutorial

First you need to get a Google Translate API key from their website.
Alternatively, there is a Getting Started guide here: https://developers.google.com/translate/v2/getting_started

1. Go to https://code.google.com/apis/console/?api=translate&promo=tr and login with your Google account
2. Click 'Services' and toggle 'Translate API' to 'ON'.
3. It is a paid API, but the first 1 million characters are free. In the Google Console, click "Billing".
4. Click the 'Google Checkout' button and enter your payment details. (This is needed in order to get the 1M free characters)
5. Return to the Google Console and click 'API Access'
6. Generate a Browser or Server key depending on where you plan to use the API.

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

For more examples, take a look at the GoogleTranslate.NET Client and GoogleTranslate.NET Tests projects.