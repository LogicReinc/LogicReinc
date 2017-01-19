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
    public class MethodTests
    {
        const int itterations = 1000000;
        static Type t_TestClass = typeof(TestClass);
        public static MethodInfo m_Increment = typeof(TestClass).GetMethod("Increment");
        static Action<object, object[]> testAction;



        [ClassInitialize]
        public static void Init(TestContext context)
        {
            testAction = Method.BuildMethodAction(MethodTests.m_Increment);
        }



        [TestMethod]
        public void CallMethod()
        {
            MethodTests.TestClass c = new Expressions.MethodTests.TestClass();

            for (int i = 0; i < itterations; i++)
                testAction(c, new object[] { 1, 2, 3 });

        }
        [TestMethod]
        public void CallMethodReflection()
        {
            MethodTests.TestClass c = new Expressions.MethodTests.TestClass();

            for(int i = 0; i < itterations; i++)
                m_Increment.Invoke(c, new object[] { 1, 2, 3 });

        }


        public class TestClass
        {
            public int A { get; set; }

            public void Increment(int val, int val2, int val3)
            {
                A += val + val2 + val3;
            }
        }
    }
}
