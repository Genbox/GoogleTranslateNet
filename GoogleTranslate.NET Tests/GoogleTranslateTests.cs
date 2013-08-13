using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using GoogleTranslateNET;
using GoogleTranslateNET.Misc;
using GoogleTranslateNET.Objects.LanguageDetection;
using GoogleTranslateNET.Objects.SupportedLanguages;
using GoogleTranslateNET.Objects.Translation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GoogleTranslateNETTests
{
    [TestClass]
    public class GoogleTranslateTests
    {
        private string _key = ConfigurationManager.AppSettings["Key"];

        [TestMethod]
        public void GoogleTranslateConstructorTest()
        {
            GoogleTranslate google = new GoogleTranslate(_key);
            Assert.IsFalse(google.PrettyPrint.HasValue);
            Assert.IsFalse(google.LargeQuery.HasValue);
        }

        [TestMethod]
        public void TranslateTest()
        {
            GoogleTranslate google = new GoogleTranslate(_key);
            google.PrettyPrint = false;
            List<Translation> results = google.Translate(Language.Automatic, Language.German, "Hello there", "What are you doing?");

            Assert.AreEqual("Hallo es", results[0].TranslatedText);
            Assert.AreEqual(Language.English.GetStringValue(), results[0].DetectedSourceLanguage);

            Assert.AreEqual("Was machst du?", results[1].TranslatedText);
            Assert.AreEqual(Language.English.GetStringValue(), results[1].DetectedSourceLanguage);
        }

        [TestMethod]
        public void SupportedLanaugesTest()
        {
            GoogleTranslate google = new GoogleTranslate(_key);
            google.PrettyPrint = false;
            List<TranslationLanaguage> results = google.GetSupportedLanguages();

            //We know that english and german is in the list. So they should be there
            Assert.IsTrue(results.Any(lang => lang.Language == Language.English.GetStringValue()));
            Assert.IsTrue(results.Any(lang => lang.Language == Language.German.GetStringValue()));
        }

        [TestMethod]
        public void DetectLanguagesTest()
        {
            GoogleTranslate google = new GoogleTranslate(_key);
            google.PrettyPrint = false;
            List<LanguageDetection> results = google.DetectLanguage("Hallo gibt es");

            //There has to be at least 1 result
            Assert.IsTrue(results.Count >= 1);

            //It should detect german
            Assert.AreEqual(Language.German.GetStringValue(), results[0].Language);

            //It has to have a confidence of more than 0
            Assert.IsTrue(results[0].Confidence > 0.0f);

            //Too small of a text to be reliable
            Assert.AreEqual(false, results[0].IsReliable);
        }

        [TestMethod]
        public void TranslateLargeText()
        {
            GoogleTranslate google = new GoogleTranslate(_key);
            google.PrettyPrint = false;

            //2037 characters long text
            const string text = "Wikipedia was launched in January 2001 by Jimmy Wales and Larry Sanger.[13] Sanger coined the name Wikipedia,[14] which is a portmanteau of wiki (a type of collaborative website, from the Hawaiian word wiki, meaning quick)[15] and encyclopedia. Wikipedia's departure from the expert-driven style of encyclopedia building and the presence of a large body of unacademic content have received extensive attention in print media. In its 2006 Person of the Year article, Time magazine recognized the rapid growth of online collaboration and interaction by millions of people around the world. It cited Wikipedia as an example, in addition to YouTube, MySpace, and Facebook.[16] Wikipedia has also been praised as a news source because of how quickly articles about recent events appear.[17][18] Students have been assigned to write Wikipedia articles as an exercise in clearly and succinctly explaining difficult concepts to an uninitiated audience.[19] Although the policies of Wikipedia strongly espouse verifiability and a neutral point of view, criticisms leveled at Wikipedia include allegations about quality of writing,[20] inaccurate or inconsistent information, and explicit content. Various experts (including Wales and Jonathan Zittrain) have expressed concern over possible (intentional or unintentional) biases.[21][22][23][24] These allegations are addressed by various Wikipedia policies. Other disparagers of Wikipedia simply point out vulnerabilities inherent to any wiki that may be edited by anyone. These critics observe that much weight is given to topics that more editors are likely to know about, like popular culture,[25] and that the site is vulnerable to vandalism,[26][27] though some studies indicate that vandalism is quickly deleted. Critics point out that some articles contain unverified or inconsistent information,[28] though a 2005 investigation in Nature showed that the science articles they compared came close to the level of accuracy of Encyclopædia Britannica and had a similar rate of serious errors";

            //We have not set LargeQuery to true, it should fail as the message is 
            try
            {
                List<Translation> results = google.Translate(Language.English, Language.German, text);
                Assert.Fail();
            }
            catch (ArgumentException ex)
            {
            }

            //We set the LargeQuery to false, so it should still fail
            google.LargeQuery = false;

            try
            {
                List<Translation> results = google.Translate(Language.English, Language.German, text);
                Assert.Fail();
            }
            catch (ArgumentException ex)
            {
            }

            //We set the LargeQuery to true.
            google.LargeQuery = true;

            //It should still fail when the mssage is more than 5000 characters.
            try
            {
                List<Translation> results = google.Translate(Language.English, Language.German, text + text + text + text);
                Assert.Fail();
            }
            catch (ArgumentException ex)
            {
            }

            //Finally we try with the message and it should work
            List<Translation> largeResults = google.Translate(Language.English, Language.German, text);
            Assert.IsFalse(string.IsNullOrEmpty(largeResults[0].TranslatedText));
        }
    }
}