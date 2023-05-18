using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace MvcWebPage
{
    //PlainJsonStringConverter
    public class PlainJson : JsonConverter
    {

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return reader.Value;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            try
            {


                /*                    using (var sw = new StringWriter())
                                    {
                                         // Copy relevant settings
                                         using (var nWriter = new JsonTextWriter(sw) 
                                                                   { 
                                                                       QuoteName = false,
                                                                   })
                                         {
    
                                             nWriter.WriteStartObject();
    
                                             Type tModelType = value.GetType();
    
                                             PropertyInfo[] arrayPropertyInfos = tModelType.GetProperties();
    
    
                                             foreach (PropertyInfo property in arrayPropertyInfos)
                                             {
                                                  var sr = property.GetValue(value, null);
    
                                                  if (sr != null)
                                                  {
                                                       nWriter.WritePropertyName(property.Name, false);
                                                       nWriter.WriteRawValue(sr.ToString());
                                                  }
    
                                             }
    
                                             nWriter.WriteEndObject();
                                         }
    
    
                                         writer.WriteRawValue(sw.ToString());
                                    }*/

                if (value is bool)
                {
                    writer.WriteRawValue(Convert.ToString(value).ToLower());
                }
                else
                {
                    writer.WriteRawValue(Convert.ToString(value));
                }



            }
            catch (Exception e)
            {

            }
        }
    }
}