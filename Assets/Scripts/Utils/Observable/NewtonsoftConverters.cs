using System;
using Newtonsoft.Json;

namespace Utils.Observables.NewtonsoftConverter
{
    public class ObsFloatConverter : JsonConverter<ObsFloat>
    {
        [UnityEngine.Scripting.Preserve]
        public ObsFloatConverter() { }
        
        const bool UseRound = true;
        public override ObsFloat ReadJson(JsonReader reader, Type objectType, ObsFloat existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return new ObsFloat(serializer.Deserialize<float>(reader));
        }

        public override void WriteJson(JsonWriter writer, ObsFloat value, JsonSerializer serializer)
        {
#pragma warning disable CS0162
            if (UseRound)
            {
                writer.WriteValue(Math.Round((value as ObsFloat).Value,2));
            }
            else 
            {
                writer.WriteValue((value as ObsFloat).Value);
            }
        }
#pragma warning restore CS0162
    }
    
    public class ObsIntConverter : JsonConverter<ObsInt>
    {
        [UnityEngine.Scripting.Preserve]
        public ObsIntConverter() { }
        
        public override ObsInt ReadJson(JsonReader reader, Type objectType, ObsInt existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return new ObsInt(serializer.Deserialize<int>(reader));
        }

        public override void WriteJson(JsonWriter writer, ObsInt value, JsonSerializer serializer)
        {
            writer.WriteValue((value as ObsInt).Value);
        }
    }
    
    public class ObsStringConverter : JsonConverter<ObsString>
    {
        [UnityEngine.Scripting.Preserve]
        public ObsStringConverter() { }
        
        public override ObsString ReadJson(JsonReader reader, Type objectType, ObsString existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return new ObsString(serializer.Deserialize<string>(reader));
        }

        public override void WriteJson(JsonWriter writer, ObsString value, JsonSerializer serializer)
        {
            writer.WriteValue((value as ObsString).Value);
        }
    }
    
    public class ObsBoolConverter : JsonConverter<ObsBool>
    {
        [UnityEngine.Scripting.Preserve]
        public ObsBoolConverter() { }
        
        public override ObsBool ReadJson(JsonReader reader, Type objectType, ObsBool existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return new ObsBool(serializer.Deserialize<int>(reader) == 1);
        }

        public override void WriteJson(JsonWriter writer, ObsBool value, JsonSerializer serializer)
        {
            writer.WriteValue((value as ObsBool).Value? 1 : 0);
        }
    }
}