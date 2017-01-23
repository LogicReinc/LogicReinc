using LogicReinc.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LogicReinc.Data.Unified.Attributes
{
    public class UnifiedIMReference : Attribute
    {
        public bool Cache { get; set; }
        public string HostProperty { get; set; }
        public string TargetProperty { get; set; }
        public string SetProperty { get; set; }

        public Type HostPropertyType { get; set; }
        public Type TargetPropertyType { get; set; }
        public Type SetPropertyType { get; set; }

        public Type TargetType { get; set; }
        public Type HostType { get; set; }
        

        public UnifiedIMReference(string hostProp, Type targetType, string targetProp, bool cache = false)
        {
            Cache = cache;
            HostProperty = hostProp;
            TargetProperty = targetProp;
            TargetType = targetType;
            TargetPropertyType = targetType.GetProperty(targetProp)?.PropertyType;
            if (TargetPropertyType == null)
                throw new ArgumentException("Target Property does not exist");
        }
        public UnifiedIMReference(Type hostType, string hostProp, Type targetType, string targetProp, bool cache = false)
        {
            Cache = cache;
            HostProperty = hostProp;
            TargetProperty = targetProp;
            TargetType = targetType;
            HostType = hostType;

            HostPropertyType = hostType.GetProperty(hostProp)?.PropertyType;
            if(HostPropertyType == null)
                throw new ArgumentException("Host Property does not exist");
            TargetPropertyType = targetType.GetProperty(targetProp)?.PropertyType;
            if (TargetPropertyType == null)
                throw new ArgumentException("Target Property does not exist");
           
        }
        public object GetTargetProperty(object obj)
        {
            //Uses expression caching underneath
            return Property.Get(obj, TargetProperty);
        }

        public object GetHostProperty(object obj)
        {
            //Uses expression caching underneath
            return Property.Get(obj, HostProperty);
        }

        public object GetReferenceProperty(object obj)
        {
            //Uses expression caching underneath
            return Property.Get(obj, SetProperty);
        }
        public void SetReferenceProperty(object obj, object val)
        {
            Property.Set(obj, SetProperty, val);
        }
    }

    public enum ReferenceType
    {
        ToN,
        ToOne
    }
}
