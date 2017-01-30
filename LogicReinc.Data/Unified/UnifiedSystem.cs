using LogicReinc.Collection;
using LogicReinc.Collections;
using LogicReinc.Data.Unified.Attributes;
using LogicReinc.Expressions;
using LogicReinc.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LogicReinc.Data.Unified
{
    public static class UnifiedSystem
    {
        public static bool UseOmniBase { get; set; } = true;
        public static bool AllowReferences { get; set; } = true;
        public static bool AllowIndexes { get; set; } = true;
        public static bool CreateReferenceIndexes { get; set; } = true;

        internal static Dictionary<Type, IList> _databaseGetters = new Dictionary<Type, IList>();
        public static List<UnifiedIMReference> References { get; set; } = new List<UnifiedIMReference>();
        public static Dictionary<Type, List<UnifiedIMReference>> HostReferences { get; set; } = new Dictionary<Type, List<UnifiedIMReference>>();
        public static Dictionary<Type, List<UnifiedIMReference>> TargetReferences { get; set; } = new Dictionary<Type, List<UnifiedIMReference>>();
        public static Dictionary<Type, List<UnifiedIMReference>> TypeReferences { get; set; } = new Dictionary<Type, List<UnifiedIMReference>>();

        internal static Dictionary<Type, List<string>> IndexProperties { get; set; } = new Dictionary<Type, List<string>>();
        internal static Dictionary<Type, ObjectIndex<IUnifiedIMObject>> Indexes { get; set; } = new Dictionary<Type, ObjectIndex<IUnifiedIMObject>>();


        internal static Dictionary<string, IUnifiedIMObject> OmniBase { get; } = new Dictionary<string, IUnifiedIMObject>();
        

        //Public

        public static bool HasIndex(Type type, string property)
        {
            if (!Indexes.ContainsKey(type))
                return false;
            return Indexes[type].HasTypeProperty(property);
        }

        public static void AddIndex(Type type, string property)
        {
            if (!IndexProperties.ContainsKey(type))
                IndexProperties.Add(type, new List<string>());
            IndexProperties[type].Add(property);

            if (!Indexes.ContainsKey(type))
                Indexes.Add(type, new ObjectIndex<IUnifiedIMObject>());
        }
        public static void RemoveIndex(Type type, string property)
        {
            if(IndexProperties.ContainsKey(type))
            {
                IndexProperties[type].Remove(property);
                if (IndexProperties[type].Count == 0)
                    IndexProperties.Remove(type);
            }
        }


        //Internals
        internal static void FixIndex(IUnifiedIMObject obj, Type t, string property, object oldValue)
        {
            if (Indexes.ContainsKey(t))
            {
                ObjectIndex<IUnifiedIMObject> pIndexes = Indexes[t];
                object cVal = Property.Get(obj, property) ?? null;
                pIndexes.RemoveIndex(property, oldValue, obj);
                pIndexes.AddIndex(property, cVal, obj);
            }
        }

        internal static void DeleteIndexes(IUnifiedIMObject obj)
        {
            if (Indexes.ContainsKey(obj.DataType))
            {
                ObjectIndex<IUnifiedIMObject> pIndexes = Indexes[obj.DataType];
                foreach (string prop in pIndexes.Properties)
                {
                    object cVal = Property.Get(obj, prop) ?? null;
                    pIndexes.RemoveIndex(prop, cVal, obj);
                }
            }
        }

        internal static void HandleObjectChange<T>(IUnifiedIMObject obj)
        {
            string key = obj.ObjectID;
            Type t = typeof(T);


            if (UnifiedSystem.AllowReferences || UnifiedSystem.AllowIndexes)
            {
                //Loop over Changes
                foreach(KeyValuePair<string, UIMPropertyState> changeState in obj.PropertyStates)
                {
                    object cVal = Property.Get(obj, changeState.Key) ?? null;
                    changeState.Value.HasChangedAndUpdate(cVal, (oldVal, newVal) =>
                    {
                        //Index Updating
                        if (AllowIndexes)
                            FixIndex(obj, t, changeState.Key, changeState.Value.LastState);

                        //Reference Updating
                        if (AllowReferences)
                        {
                            if (HostReferences.ContainsKey(t))
                            {
                                UnifiedIMReference hRefs = HostReferences[t].FirstOrDefault(x => x.HostProperty == changeState.Key);
                                if (hRefs != null)
                                {
                                    DeleteReference(obj, hRefs);
                                    ResolveReference(obj, hRefs, t);
                                }
                            }
                            if (TargetReferences.ContainsKey(t))
                            {
                                List<UnifiedIMReference> tRefs = TargetReferences[t].Where(x => x.TargetProperty == changeState.Key).ToList();
                                foreach (UnifiedIMReference reff in tRefs)
                                {
                                    DeleteReference(obj, reff);
                                    ResolveReference(obj, reff);
                                }
                            }
                        }
                    });
                }
                
            }
        }

        internal static void HandleObjectCreation<T>(IUnifiedIMObject obj)
        {
            if (UnifiedSystem.AllowReferences)
                UnifiedSystem.ResolveReferences(obj, typeof(T));
            if (UnifiedSystem.UseOmniBase)
                lock (UnifiedSystem.OmniBase)
                    UnifiedSystem.OmniBase.Add(obj.ObjectID, obj);

            List<string> indexesToAdd = new List<string>();
            if (UnifiedSystem.AllowReferences)
            {
                
                List<UnifiedIMReference> refs = new List<UnifiedIMReference>();
                if (TypeReferences.ContainsKey(typeof(T)))
                    refs = TypeReferences[typeof(T)];

                /*
                ObjectIndex<IUnifiedIMObject> oi = null;
                if (CreateReferenceIndexes)
                    if (Indexes.ContainsKey(typeof(T)))
                        oi = Indexes[typeof(T)];*/

                foreach (UnifiedIMReference reff in refs)
                {
                    if (reff.HostType == typeof(T))
                    {
                        //object value = reff.GetHostProperty(obj);
                        //if(!obj.PropertyStates.ContainsKey(reff.HostProperty))
                        //obj.PropertyStates.Add(reff.HostProperty, new UIMPropertyState(value));
                        if (CreateReferenceIndexes && reff.HostProperty != "ObjectID")
                            // oi.AddIndex(reff.HostProperty, value, obj);
                            indexesToAdd.Add(reff.HostProperty);
                    }

                    else if (reff.TargetType == typeof(T))
                    {
                        //object value = reff.GetTargetProperty(obj);
                        //if (!obj.PropertyStates.ContainsKey(reff.TargetProperty))
                        //    obj.PropertyStates.Add(reff.TargetProperty, new UIMPropertyState(value));
                        if (CreateReferenceIndexes && reff.TargetProperty != "ObjectID")
                            //oi.AddIndex(reff.TargetProperty, value, obj);
                            indexesToAdd.Add(reff.TargetProperty);
                    }
                }
            }

            if (AllowIndexes)
            {
                if (IndexProperties.ContainsKey(typeof(T)))
                    indexesToAdd.AddRange(IndexProperties[typeof(T)]);

                ObjectIndex<IUnifiedIMObject> oi = null;
                if (Indexes.ContainsKey(typeof(T)))
                    oi = Indexes[typeof(T)];

                if(oi != null)
                    foreach (string idx in indexesToAdd.Distinct())
                    {
                        object value = Property.Get(obj, idx);
                        if (!obj.PropertyStates.ContainsKey(idx))
                            obj.PropertyStates.Add(idx, new UIMPropertyState(value));
                        oi.AddIndex(idx, value, obj);
                    }
            }
        }

        internal static void HandleObjectDeletion<T>(IUnifiedIMObject obj)
        {
            if (UnifiedSystem.AllowReferences)
                UnifiedSystem.DeleteReferences(obj);
            if (UnifiedSystem.UseOmniBase)
                lock (UnifiedSystem.OmniBase)
                    UnifiedSystem.OmniBase.Remove(obj.ObjectID);
            if (UnifiedSystem.AllowIndexes)
                UnifiedSystem.DeleteIndexes(obj);
        }

        internal static void RegisterType(Type type)
        {
            if (AllowReferences)
            {
                PropertyInfo[] props = type.GetPropertiesCached();
                foreach (PropertyInfo prop in props)
                {
                    UnifiedIMReference reff = prop.GetCustomAttribute<UnifiedIMReference>();
                    UnifiedIMIndex indx = prop.GetCustomAttribute<UnifiedIMIndex>();
                    bool alreadyIndexed = false;

                    if (reff != null)
                    {
                        if (reff.HostPropertyType == null)
                            reff.HostPropertyType = props.FirstOrDefault(x => x.Name == reff.HostProperty)?.PropertyType;
                        if (reff.HostType == null)
                            reff.HostType = prop.DeclaringType;
                        reff.SetProperty = prop.Name;
                        reff.SetPropertyType = prop.PropertyType;
                        if (reff.SetPropertyType == null)
                            throw new ArgumentException("Set Property does not exist");
                        References.Add(reff);
                        if (!HostReferences.ContainsKey(type))
                            HostReferences.Add(type, new List<UnifiedIMReference>());
                        HostReferences[type].Add(reff);
                        if (!TargetReferences.ContainsKey(reff.TargetType))
                            TargetReferences.Add(reff.TargetType, new List<UnifiedIMReference>());
                        TargetReferences[reff.TargetType].Add(reff);
                        if (!TypeReferences.ContainsKey(reff.HostType))
                            TypeReferences.Add(reff.HostType, new List<UnifiedIMReference>());
                        if (!TypeReferences.ContainsKey(reff.TargetType))
                            TypeReferences.Add(reff.TargetType, new List<UnifiedIMReference>());
                        TypeReferences[reff.TargetType].Add(reff);
                        TypeReferences[reff.HostType].Add(reff);
                        if (AllowIndexes && CreateReferenceIndexes)
                        {
                            if (reff.TargetProperty != "ObjectID" && !HasIndex(reff.TargetType, reff.TargetProperty))
                                AddIndex(reff.TargetType, reff.TargetProperty);
                            if (reff.HostProperty != "ObjectID" && !HasIndex(reff.HostType, reff.HostProperty))
                                AddIndex(reff.HostType, reff.HostProperty);

                            if (reff.TargetType == type || reff.HostType == type)
                                alreadyIndexed = true;
                        }
                    }

                    if(!alreadyIndexed && prop.Name != "ObjectID" && indx != null)
                    {
                        AddIndex(type, prop.Name);
                    }
                }
            }

            if (!_databaseGetters.ContainsKey(type))
                _databaseGetters.Add(type, ((IUnifiedIMObject)Activator.CreateInstance(type)).DatabaseBase);
            
        }

        internal static void DeleteReference(IUnifiedIMObject obj, UnifiedIMReference rf)
        {
            lock (obj.RefTo)
            {
                if (rf.TargetType == obj.DataType)
                {
                    foreach (KeyValuePair<UnifiedIMReference, IUnifiedIMObject> o in obj.RefTo)
                    {
                        if (o.Value.DataType == rf.HostType)
                        {
                            object sProperty = rf.GetReferenceProperty(o.Value);
                            if (typeof(IList).IsAssignableFrom(rf.SetPropertyType))
                                ((IList)sProperty).Remove(obj);
                            else if (sProperty == obj)
                                rf.SetReferenceProperty(o.Value, null);
                        }
                    }
                    obj.RefTo.RemoveAll(x => x.Key == rf);
                }
                else if(rf.HostType == obj.DataType)
                {
                    object sProperty = rf.GetReferenceProperty(obj);
                    if (typeof(IList).IsAssignableFrom(rf.SetPropertyType))
                    {
                        foreach (object o in ((IList)sProperty))
                            ((IUnifiedIMObject)o).RefTo.RemoveAll(x=>x.Key == rf && x.Value == obj);
                        ((IList)sProperty).Clear();
                    }
                    else if (sProperty == obj)
                    {
                        rf.SetReferenceProperty(obj, null);
                        ((IUnifiedIMObject)sProperty).RefTo.RemoveAll(x => x.Key == rf && x.Value == obj);
                    }
                }
            }
        }

        internal static void DeleteReferences(IUnifiedIMObject obj)
        {
            Type t = obj.GetType();
            
            foreach(UnifiedIMReference rf in References)
                DeleteReference(obj, rf);
        }

        internal static void UpdateReference(UnifiedIMReference reference, IUnifiedIMObject host, IUnifiedIMObject target)
        {
            object hVal = reference.GetHostProperty(host);
            object tVal = reference.GetTargetProperty(target);

            if(Matches(reference, hVal, tVal))
            {
                if(typeof(IList).IsAssignableFrom(reference.SetPropertyType))
                {
                    IList list = (IList)reference.GetReferenceProperty(host);
                    if (list == null)
                        throw new ArgumentException("Reference Lists should be initiated with an instance. And never be null");
                    //if (!list.Contains(target))
                        list.Add(target);
                }
                else
                {
                    reference.SetReferenceProperty(host, target);
                }
                target.RefTo.Add(new KeyValuePair<UnifiedIMReference, IUnifiedIMObject>(reference, host));
            }
            else
            {
                if (typeof(IList).IsAssignableFrom(reference.SetPropertyType))
                {
                    IList list = (IList)reference.GetReferenceProperty(host);
                    if (list == null)
                        throw new ArgumentException("Reference Lists should be initiated with an instance. And never be null");
                    //if (list.Contains(target))
                        list.Remove(target);
                }
                else
                {
                    object curVal = reference.GetReferenceProperty(host);
                    if(curVal == target)
                        reference.SetReferenceProperty(host, null);
                }
                target.RefTo.RemoveAll(x=>x.Key == reference && x.Value == host);
            }
        }

        internal static bool Matches(UnifiedIMReference reference, object hostVal, object targetVal)
        {
            if (typeof(IList).IsAssignableFrom(reference.TargetPropertyType))
            {
                IList targetL = (IList)targetVal;
                if (typeof(IList).IsAssignableFrom(reference.HostPropertyType))
                {
                    IList hostL = (IList)hostVal;
                    throw new NotImplementedException("Cannot use IList -> IList comparison yet");
                }
                else
                    return targetL.Contains(hostVal);
            }
            else
            {
                if (typeof(IList).IsAssignableFrom(reference.HostPropertyType))
                {
                    IList hostL = (IList)hostVal;
                    return hostL.Contains(targetVal);
                }
                else
                    return hostVal.Equals(targetVal);
            }
        }

        internal static void ResolveReferences(IUnifiedIMObject obj, Type type = null)
        {
            if (type == null)
                type = obj.GetType();
            if(TypeReferences.ContainsKey(type))
            foreach (UnifiedIMReference reference in TypeReferences[type])            
                ResolveReference(obj, reference, type);
        }

        internal static void ResolveReference(IUnifiedIMObject obj, UnifiedIMReference reference, Type type = null)
        {
            if (type == null)
                type = obj.GetType();

            if (reference.TargetType == type)
            {

                if (UseOmniBase && reference.HostProperty == "ObjectID")
                {
                    string od = (string)reference.GetTargetProperty(obj);
                    if (od != null)
                    {
                        bool hasKey = false;
                        IUnifiedIMObject omniObj = null;
                        lock (UnifiedSystem.OmniBase)
                        {
                            hasKey = UnifiedSystem.OmniBase.ContainsKey(od);
                            if(hasKey)
                                omniObj = OmniBase[od];
                        }
                        if(hasKey)
                        UpdateReference(reference, omniObj, obj);
                    }
                }
                else
                {
                    if (AllowIndexes && Indexes.ContainsKey(reference.HostType) && Indexes[reference.HostType].HasTypeProperty(reference.HostProperty))
                    {
                        List<IUnifiedIMObject> objs = Indexes[reference.HostType].GetIndex(reference.HostProperty, reference.GetTargetProperty(obj));
                        foreach (object o in objs)
                            UpdateReference(reference, (IUnifiedIMObject)o, obj);
                    }
                    else
                    if (_databaseGetters.ContainsKey(reference.HostType))
                        foreach (object o in _databaseGetters[reference.HostType])
                            UpdateReference(reference, (IUnifiedIMObject)o, obj);
                }

            }
            else if (reference.HostType == type)
            {
                if (UseOmniBase && reference.TargetProperty == "ObjectID")
                {
                    string od = (string)reference.GetHostProperty(obj);
                    if (od != null)
                    {
                        bool hasKey = false;
                        IUnifiedIMObject omniObj = null;
                        lock (UnifiedSystem.OmniBase)
                        {
                            hasKey = UnifiedSystem.OmniBase.ContainsKey(od);
                            if (hasKey)
                                omniObj = OmniBase[od];
                        }
                        if (hasKey)
                            UpdateReference(reference, obj, omniObj);
                    }
                }
                else
                {
                    if(AllowIndexes && Indexes.ContainsKey(reference.TargetType) && Indexes[reference.TargetType].HasTypeProperty(reference.TargetProperty))
                    {
                        List<IUnifiedIMObject> objs = Indexes[reference.TargetType].GetIndex(reference.TargetProperty, reference.GetHostProperty(obj));
                        foreach (object o in objs)
                            UpdateReference(reference, obj, (IUnifiedIMObject)o);
                    }
                    else if (_databaseGetters.ContainsKey(reference.TargetType))
                        foreach (object o in _databaseGetters[reference.TargetType])
                            UpdateReference(reference, obj, (IUnifiedIMObject)o);
                }
            }
        }
    }
}
