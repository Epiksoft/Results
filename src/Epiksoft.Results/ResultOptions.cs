using System.Text.Json;

namespace Epiksoft.Results
{
    public class ResultOptions
    {
        public JsonSerializerOptions JsonSerializerOptions { get; set; }
        public Func<Dictionary<string, object>> MetadataFactory { get; set; }
        public bool ReturnNotFoundWhenDataIsNull { get; set; } = true;

    }
}
