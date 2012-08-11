using System;
using System.Collections.Generic;
using System.Text;
using GoogleTranslateNET.Misc;
using GoogleTranslateNET.Objects.Error;
using GoogleTranslateNET.Objects.LanguageDetection;
using GoogleTranslateNET.Objects.SupportedLanguages;
using GoogleTranslateNET.Objects.Translation;
using RestSharp;
using RestSharp.Deserializers;
using System.Linq;

namespace GoogleTranslateNET
{
    public class GoogleTranslate
    {
        private string _key;
        private static RestClient _client = new RestClient("https://www.googleapis.com/language/translate/v2");

        public GoogleTranslate(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("Key is required.", "key");

            _key = key;
        }

        /// <summary>
        /// When true, the output from google is in human readable format.
        /// Default: Not set, Google defaults to true
        /// </summary>
        public bool? PrettyPrint { get; set; }

        /// <summary>
        /// When true, queries will be sent using HTTP POST instead of GET.
        /// GET queries have a limit of 2000 characters, POST queries have a limit of 5000 characters.
        /// Default: Not set, Google defaults to false
        /// </summary>
        public bool? LargeQuery { get; set; }

        /// <summary>
        /// Translates a text from one language to another.
        /// You can input multiple texts and get them translated all at once.
        /// Warning: Setting source and destination languages to the same language will result in an error.
        /// </summary>
        /// <param name="sourceLanguage">The language to translate from. Set it to Language.Automatic to let Google Translate determine the language.</param>
        /// <param name="destinationLanaguage">The language to translate to.</param>
        /// <param name="text">The text to translate. You may input more than one text.</param>
        /// <returns>The translated text.</returns>
        public List<Translation> Translate(Language sourceLanguage, Language destinationLanaguage, params string[] text)
        {
            //https://www.googleapis.com/language/translate/v2?key=key&q=hello%20world&source=en&target=de
            RestRequest request = CreateRequest(string.Empty);

            CheckRequest(text);

            //Required
            foreach (string q in text)
            {
                request.AddParameter("q", q);
            }
            request.AddParameter("target", destinationLanaguage.GetStringValue());

            //Optional
            if (sourceLanguage != Language.Automatic)
                request.AddParameter("source", sourceLanguage.GetStringValue());

            //Output
            TranslateResult results = GetResponse<TranslateResult>(request);
            return results.Data.Translations;
        }

        /// <summary>
        /// Gives you a list of supported languages.
        /// </summary>
        /// <param name="targetLanguage">When defined, gives you a list of languages that translates into the target language.</param>
        /// <returns>A list of supported languages</returns>
        public List<TranslationLanaguage> GetSupportedLanguages(Language targetLanguage = Language.Unknown)
        {
            //https://www.googleapis.com/language/translate/v2/languages?key=key&target=zh-TW
            RestRequest request = CreateRequest("languages");

            //Optional
            if (targetLanguage != Language.Unknown)
                request.AddParameter("target", targetLanguage.GetStringValue());

            //Output
            SupportedLanguageResult results = GetResponse<SupportedLanguageResult>(request);
            return results.Data.Languages;
        }

        /// <summary>
        /// Detects the languages that might be used in the text.
        /// You can send more than one text in a single request to detect multiple texts.
        /// </summary>
        /// <param name="text">The text to use when detecting languages.</param>
        /// <returns>A list of languages that might be used in the text.</returns>
        public List<LanguageDetection> DetectLanguage(params string[] text)
        {
            //https://www.googleapis.com/language/translate/v2/detect?key=key&q=google+translate+is+fast
            RestRequest request = CreateRequest("detect");

            CheckRequest(text);

            //Required
            foreach (string q in text)
            {
                request.AddParameter("q", q);
            }

            //Output
            LanguageDetectionResult results = GetResponse<LanguageDetectionResult>(request);

            //Flatten the results from Google Translate API
            List<LanguageDetection> detections = new List<LanguageDetection>();
            foreach (List<LanguageDetection> languageDetections in results.Data.Detections)
            {
                detections.AddRange(languageDetections);
            }

            return detections;
        }

        private void CheckRequest(IEnumerable<string> requestContent)
        {
            //Compute the total size of the content
            int sum = requestContent.Sum(item => item.Length);

            if (((LargeQuery.HasValue && !LargeQuery.Value) || !LargeQuery.HasValue) && sum >= 2000)
            {
                throw new ArgumentException("Your text content is larger than 2000 characters. Set LargeQuery to 'true' to enable support up to 5000 characters.");
            }

            if (sum > 5000)
                throw new ArgumentException("Your text content is larger than 5000 characters. Google Translate only allow up to 5000 characters");
        }

        private RestRequest CreateRequest(string function)
        {
            RestRequest request;

            if (LargeQuery.HasValue && LargeQuery.Value)
            {
                request = new RestRequest(function, Method.POST);

                //To use POST, you must use the X-HTTP-Method-Override header to tell the Translate API to treat the request as a GET (use X-HTTP-Method-Override: GET).
                request.AddHeader("X-HTTP-Method-Override", "GET");
            }
            else
            {
                request = new RestRequest(function, Method.GET);
            }

            request.AddParameter("key", _key);

            if (PrettyPrint.HasValue)
                request.AddParameter("prettyprint", PrettyPrint.ToString().ToLower());

            return request;
        }

        private T GetResponse<T>(RestRequest request)
        {
            RestResponse response = (RestResponse)_client.Execute(request);
            JsonDeserializer deserializer = new JsonDeserializer();
            T results = deserializer.Deserialize<T>(response);

            //Try to deserialize it as an error - it is a hack since I'm using generics here.
            ErrorResponse errorResponse = deserializer.Deserialize<ErrorResponse>(response);

            if (errorResponse.Error != null)
                throw new Exception(GetErrorText(errorResponse.Error));

            return results;
        }

        private string GetErrorText(Error error)
        {
            if (error != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(error.Message);

                if (error.Errors.Count >= 1)
                {
                    ErrorData errorData = error.Errors.First();
                    sb.Append("Reason: " + errorData.Reason);
                }

                return sb.ToString();
            }

            return "There was an error. Unable to determine the cause.";
        }
    }
}