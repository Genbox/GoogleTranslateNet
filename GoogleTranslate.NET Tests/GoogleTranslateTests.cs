using System;
using System.Collections.Generic;
using GoogleTranslateNET.Misc;
using GoogleTranslateNET.Objects.LanguageDetection;
using GoogleTranslateNET.Objects.SupportedLanguages;
using GoogleTranslateNET.Objects.Translation;
using Xunit;

namespace GoogleTranslateNET.Tests
{
    public class GoogleTranslateTests
    {
        private readonly string _key = "AIzaSyD2C0Njof-3mA_rNdjSD0k6nwD9_tJ6gfc";

        [Fact]
        public void GoogleTranslateConstructorTest()
        {
            GoogleTranslate google = new GoogleTranslate(_key);
            Assert.False(google.PrettyPrint.HasValue);
            Assert.False(google.LargeQuery.HasValue);
        }

        [Fact]
        public void TranslateTest()
        {
            GoogleTranslate google = new GoogleTranslate(_key);
            google.PrettyPrint = false;
            List<Translation> results = google.Translate(Language.Automatic, Language.German, "Hello there", "What are you doing?");

            Assert.Equal("Hallo", results[0].TranslatedText);
            Assert.Equal(Language.English.GetStringValue(), results[0].DetectedSourceLanguage);

            Assert.Equal("Was machen Sie?", results[1].TranslatedText);
            Assert.Equal(Language.English.GetStringValue(), results[1].DetectedSourceLanguage);
        }

        [Fact]
        public void SupportedLanaugesTest()
        {
            GoogleTranslate google = new GoogleTranslate(_key);
            google.PrettyPrint = false;
            List<TranslationLanaguage> results = google.GetSupportedLanguages();

            //We know that english and german is in the list. So they should be there
            Assert.Contains(results, lang => lang.Language == Language.English.GetStringValue());
            Assert.Contains(results, lang => lang.Language == Language.German.GetStringValue());
        }

        [Fact]
        public void DetectLanguagesTest()
        {
            GoogleTranslate google = new GoogleTranslate(_key);
            google.PrettyPrint = false;
            List<LanguageDetection> results = google.DetectLanguage("Hallo gibt es");

            //There has to be at least 1 result
            Assert.True(results.Count >= 1);

            //It should detect german
            Assert.Equal(Language.German.GetStringValue(), results[0].Language);

            //It has to have a confidence of more than 0
            Assert.True(results[0].Confidence > 0.0f);

            //Too small of a text to be reliable
            Assert.False(results[0].IsReliable);
        }

        [Fact]
        public void TranslateLargeText()
        {
            GoogleTranslate google = new GoogleTranslate(_key);
            google.PrettyPrint = false;

            //2037 characters long text
            const string text = "Wikipedia was launched in January 2001 by Jimmy Wales and Larry Sanger.[13] Sanger coined the name Wikipedia,[14] which is a portmanteau of wiki (a type of collaborative website, from the Hawaiian word wiki, meaning quick)[15] and encyclopedia. Wikipedia's departure from the expert-driven style of encyclopedia building and the presence of a large body of unacademic content have received extensive attention in print media. In its 2006 Person of the Year article, Time magazine recognized the rapid growth of online collaboration and interaction by millions of people around the world. It cited Wikipedia as an example, in addition to YouTube, MySpace, and Facebook.[16] Wikipedia has also been praised as a news source because of how quickly articles about recent events appear.[17][18] Students have been assigned to write Wikipedia articles as an exercise in clearly and succinctly explaining difficult concepts to an uninitiated audience.[19] Although the policies of Wikipedia strongly espouse verifiability and a neutral point of view, criticisms leveled at Wikipedia include allegations about quality of writing,[20] inaccurate or inconsistent information, and explicit content. Various experts (including Wales and Jonathan Zittrain) have expressed concern over possible (intentional or unintentional) biases.[21][22][23][24] These allegations are addressed by various Wikipedia policies. Other disparagers of Wikipedia simply point out vulnerabilities inherent to any wiki that may be edited by anyone. These critics observe that much weight is given to topics that more editors are likely to know about, like popular culture,[25] and that the site is vulnerable to vandalism,[26][27] though some studies indicate that vandalism is quickly deleted. Critics point out that some articles contain unverified or inconsistent information,[28] though a 2005 investigation in Nature showed that the science articles they compared came close to the level of accuracy of Encyclopædia Britannica and had a similar rate of serious errors";

            //We have not set LargeQuery to true
            Assert.Throws<ArgumentException>(() => google.Translate(Language.English, Language.German, text));

            //We set the LargeQuery to false, so it should still fail
            google.LargeQuery = false;

            Assert.Throws<ArgumentException>(() => google.Translate(Language.English, Language.German, text));

            //We set the LargeQuery to true.
            google.LargeQuery = true;

            //It should still fail when the message is more than 5000 characters.
            Assert.Throws<ArgumentException>(() => google.Translate(Language.English, Language.German, text + text + text + text));

            //Finally we try with the message and it should work
            List<Translation> largeResults = google.Translate(Language.English, Language.German, text);
            Assert.False(string.IsNullOrEmpty(largeResults[0].TranslatedText));
        }
    }
}