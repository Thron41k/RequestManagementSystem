using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RequestManagement.Common.Models.Extensions
{
    public static class ObjectExtensions
    {
        public static T Clone<T>(this T obj)
        {
            if (obj is null)
                return default!;

            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
                IncludeFields = true,
                PropertyNameCaseInsensitive = true,
                WriteIndented = false
            };

            var json = JsonSerializer.Serialize(obj, options);
            return JsonSerializer.Deserialize<T>(json, options)!;
        }
    }
}
