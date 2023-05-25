using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections;
using System.Reflection;

namespace Dax.Vpax.Json
{
    internal class ShouldSerializeContractResolver : DefaultContractResolver
    {
        internal static readonly ShouldSerializeContractResolver Instance = new ShouldSerializeContractResolver();

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);

            //if (property.PropertyType == typeof(Metadata.DaxNote)) {
            //    property.ShouldSerialize = (instance) =>
            //    {
            //        var daxNode = (Metadata.DaxNote)instance.GetType().GetProperty(property.PropertyName).GetValue(instance);
            //        return daxNode.Note?.Length > 0;
            //    };
            //}

            // Don't serialize empty JSON arrays
            if (typeof(IList).IsAssignableFrom(property.PropertyType)) {
                property.ShouldSerialize = (instance) =>
                {
                    var list = (IList)instance.GetType().GetProperty(property.PropertyName).GetValue(instance);
                    return list.Count > 0;
                };
            }

            if (property.DeclaringType == typeof(Metadata.Column)) {
                switch (property.PropertyName) {
                    case nameof(Metadata.Column.SortByColumnName):
                        property.ShouldSerialize = (instance) => ((Metadata.Column)instance).SortByColumnName?.Name != null;
                        break;
                }
            }

            return property;
        }
    }
}
