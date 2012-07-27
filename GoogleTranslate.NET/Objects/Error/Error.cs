using System.Collections.Generic;

namespace GoogleTranslateNET.Objects.Error
{
    public class Error
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public List<ErrorData> Errors { get; set; }
    }
}