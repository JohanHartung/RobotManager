using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RobotManager.Classes
{
    public class ApiResponse<T>
    {
        [JsonPropertyName("value")]
        public List<T> Value { get; set; }

        [JsonPropertyName("formatters")]
        public List<object> Formatters { get; set; }

        [JsonPropertyName("contentTypes")]
        public List<object> ContentTypes { get; set; }

        [JsonPropertyName("declaredType")]
        public object DeclaredType { get; set; }

        [JsonPropertyName("statusCode")]
        public int StatusCode { get; set; }
    }
}
