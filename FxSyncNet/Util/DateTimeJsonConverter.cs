using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FxSyncNet.Util
{
    public class DateTimeJsonConverter : JsonConverter
    {

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(DateTime);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            long seconds = (long)reader.Value;

            if (seconds.ToString().Length == 16) // Microseconds
                seconds /= 1000000;

            DateTime dateTime = new DateTime(1970, 1, 1);
            return dateTime.AddSeconds(seconds);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            DateTime dateTime = (DateTime)value;
            double microseconds = (dateTime - new DateTime(1970, 1, 1)).TotalMilliseconds * 1000.0;
            writer.WriteValue(microseconds);
        }
    }


}
