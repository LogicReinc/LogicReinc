using LogicReinc.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LogicReinc.Tests.Expressions
{
    [TestClass]
    public class PropertyTests
    {
        const string _testValue = "Abc";
        const int _testItterations = 1000000;


        static TestClass _testObj;
        static Func<object, object> _propGetter;
        static Action<object, object> _propSetter;

        //Reflection for speed comparison
        static PropertyInfo _propInfo = typeof(TestClass).GetProperty("SomeName");

        [ClassInitialize]
        public static void Init(TestContext context)
        {
            _testObj = new TestClass()
            {
                SomeName = _testValue
            };
            _propGetter = Property.BuildPropertyGetter("SomeName", typeof(TestClass));
            _propSetter = Property.BuildPropertySetter("SomeName", typeof(TestClass));
        }

        #region PropetyGetter
        [TestMethod]
        public void BuildPropertyGetter()
        {
            var getter = Property.BuildPropertyGetter("SomeName", typeof(TestClass));
        }

        [TestMethod]    //1,000,000* = 15-20ms
        public void GetProperty()
        {
            for (int i = 0; i < _testItterations; i++)
            {
                object val = _propGetter(_testObj);
            }
        }

        [TestMethod]    //1,000,000* = 4-7ms
        public void GetPropertyDirect()
        {
            for (int i = 0; i < _testItterations; i++)
            {
                object val = _testObj.SomeName;
            }
        }

        [TestMethod]    //1,000,000* = 200ms
        public void GetPropertyReflection()
        {
            for (int i = 0; i < _testItterations; i++)
            {
                object val = _propInfo.GetValue(_testObj);
            }
        }
        #endregion


        #region PropertySetter
        [TestMethod]
        public void BuildPropertySetter()
        {
            var setter = Property.BuildPropertySetter("SomeName", typeof(TestClass));
        }

        [TestMethod]    //1,000,000* = 17-20ms
        public void SetProperty()
        {
            for (int i = 0; i < _testItterations; i++)
            {
                _propSetter(_testObj, _testValue);
            }
        }

        [TestMethod]    //1,000,000* = 5-7ms
        public void SetPropertyDirect()
        {
            for (int i = 0; i < _testItterations; i++)
            {
                _testObj.SomeName = _testValue;
            }
        }

        [TestMethod]    //1,000,000* = 300ms
        public void SetPropertyReflection()
        {
            for (int i = 0; i < _testItterations; i++)
            {
                _propInfo.SetValue(_testObj, _testValue);
            }
        }
        #endregion

        public class TestClass
        {
            public string SomeName { get; set; }
        }
    }
}
