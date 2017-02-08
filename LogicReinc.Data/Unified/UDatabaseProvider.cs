using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicReinc.Data.Unified
{
    public interface UnifiedDatabaseProvider
    {
        string DatabaseName { get; }
        bool GenerateID { get; }

        bool LoadCollection<C>() where C : UnifiedIMObject<C>;

        //Select
        T GetSingleObject<T>(string id) where T : UnifiedIMObject<T>;
        List<T> GetAllObjects<T>() where T : UnifiedIMObject<T>;

        //Manipulation
        bool UpdateObject<T>(T obj) where T : UnifiedIMObject<T>;
        bool UpdateProperties<T>(string id, T obj, string[] properties) where T : UnifiedIMObject<T>;
        bool DeleteObject<T>(string id) where T : UnifiedIMObject<T>;
        bool InsertObject<T>(T obj) where T : UnifiedIMObject<T>;

    }
}
