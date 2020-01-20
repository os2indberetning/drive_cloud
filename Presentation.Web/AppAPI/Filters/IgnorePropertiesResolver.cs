using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Presentation.Web.AppAPI.Filters
{
    public class IgnorePropertiesResolver : DefaultContractResolver
    {
        private IEnumerable<string> _propsToIgnore;
        public IgnorePropertiesResolver(IEnumerable<string> propNamesToIgnore)
        {
            _propsToIgnore = propNamesToIgnore;
        }
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);
            property.ShouldSerialize = (x) => { return !_propsToIgnore.Contains(property.PropertyName); };
            return property;
        }
    }
}
