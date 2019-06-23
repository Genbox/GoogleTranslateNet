using System.Collections.Generic;

namespace GoogleTranslateNet.Objects.Error
{
    public class Error
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public List<ErrorData> Errors { get; set; }
    }
}