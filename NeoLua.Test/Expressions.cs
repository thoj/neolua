﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.IronLua;

namespace LuaDLR.Test
{
  [TestClass]
  public class Expressions : TestHelper
  {
    #region -- TestHelper --

    public struct TestOperator2
    {
      private int i;

      public TestOperator2(int i)
      {
        this.i = i;
      }

      public static implicit operator int(TestOperator2 a)
      {
        Console.WriteLine("  implicit int (TestOperator2)");
        return a.i;
      }

      public int Count { get { return i; } }
    }

    public class ComparableObject : IComparable
    {
      private int i;

      public ComparableObject(int i)
      {
        this.i = i;
      }

      public int CompareTo(object obj)
      {
        Console.WriteLine("CompareTo object");
        return ((IComparable)obj).CompareTo(obj);
      }
    }

    public class CompareTyped : IComparable<int>, IComparable<string>
    {
      private int i;

      public CompareTyped(int i)
      {
        this.i = i;
      }

      public int CompareTo(int other)
      {
        Console.WriteLine("CompareTo other");
        return i - other;
      }

      public int CompareTo(string other)
      {
        Console.WriteLine("CompareTo string");
        return i - int.Parse(other);
      }
    }

    public struct TestOperator
    {
      private int i;

      public TestOperator(int i)
      {
        this.i = i;
      }

      public override string ToString()
      {
        return i.ToString();
      }

      public int Length { get { return i; } }

      public static TestOperator operator -(TestOperator a)
      {
        Console.WriteLine("  operator- TestOperator");
        return new TestOperator(-a.i);
      }

      public static TestOperator operator ~(TestOperator a)
      {
        Console.WriteLine("  operator~ TestOperator");
        return new TestOperator(~a.i);
      }

      public static TestOperator operator +(TestOperator a, int b)
      {
        Console.WriteLine("  operator+ TestOperator,int");
        return new TestOperator(a.i + b);
      } // 

      public static TestOperator operator +(TestOperator a, TestOperator b)
      {
        Console.WriteLine("  operator+ TestOperator,TestOperator");
        return new TestOperator(a.i + b.i);
      } //

      public static TestOperator operator -(TestOperator a, int b)
      {
        Console.WriteLine("  operator- TestOperator,int");
        return new TestOperator(a.i - b);
      } //

      public static TestOperator operator *(TestOperator a, int b)
      {
        Console.WriteLine("  operator* TestOperator,int");
        return new TestOperator(a.i * b);
      } //

      public static TestOperator operator /(TestOperator a, int b)
      {
        Console.WriteLine("  operator/ TestOperator,int");
        return new TestOperator(a.i / b);
      } //

      public static TestOperator operator &(TestOperator a, int b)
      {
        Console.WriteLine("  operator& TestOperator,int");
        return new TestOperator(a.i & b);
      } //

      public static TestOperator operator |(TestOperator a, int b)
      {
        Console.WriteLine("  operator| TestOperator,int");
        return new TestOperator(a.i | b);
      } //

      public static TestOperator operator ^(TestOperator a, int b)
      {
        Console.WriteLine("  operator^ TestOperator,int");
        return new TestOperator(a.i ^  b);
      } //

      public static TestOperator operator >>(TestOperator a, int b)
      {
        Console.WriteLine("  operator>> TestOperator,int");
        return new TestOperator(a.i >> b);
      } //

      public static TestOperator operator <<(TestOperator a, int b)
      {
        Console.WriteLine("  operator<< TestOperator,int");
        return new TestOperator(a.i << b);
      } //

      public static bool operator >(TestOperator a, int b)
      {
        Console.WriteLine("  operator<< TestOperator,int");
        return a.i > b;
      } //

      public static bool operator >=(TestOperator a, int b)
      {
        Console.WriteLine("  operator>= TestOperator,int");
        return a.i >= b;
      } //

      public static bool operator <(TestOperator a, int b)
      {
        Console.WriteLine("  operator< TestOperator,int");
        return a.i < b;
      } //

      public static bool operator <=(TestOperator a, int b)
      {
        Console.WriteLine("  operator<= TestOperator,int");
        return a.i <= b;
      } //

      public static implicit operator int(TestOperator a)
      {
        Console.WriteLine("  implicit int (TestOperator)");
        return a.i;
      }

      public static explicit operator string(TestOperator a)
      {
        Console.WriteLine("  implicit string");
        return a.i.ToString();
      }

      public static implicit operator TestOperator(int a)
      {
        Console.WriteLine("  implicit TestOperator (int)");
        return new TestOperator(a);
      }
    }

    private enum IntEnum : int
    {
      Null= 0,Eins = 1, Zwei, Drei
    }
    private enum ShortEnum : short
    {
      Eins = 1, Zwei, Drei
    }

    public static int Return1()
    {
      return 1;
    }

    public static int Return2()
    {
      return 2;
    }

    public static LuaResult ReturnLua1()
    {
      return new LuaResult(1);
    }

    public static LuaResult ReturnLua2()
    {
      return new LuaResult(2);
    }

    public static LuaResult ReturnLua3()
    {
      return new LuaResult(3, 2, 1);
    }

    public static void ReturnVoid()
    {
      Console.WriteLine("ReturnVoid Called");
    }

    #endregion

    #region -- Conversion -------------------------------------------------------------

    [TestMethod]
    public void TestConvert01()
    {
      TestExpr("cast(bool, 1)", true);
      TestExpr("cast(bool, 0)", false);
      TestExpr("clr.LuaDLR.Test.Expressions.ReturnVoid()");
      TestExpr("(clr.LuaDLR.Test.Expressions.ReturnVoid())", NullResult);
      TestExpr("clr.LuaDLR.Test.Expressions.ReturnLua3()", 3, 2, 1);
      TestExpr("(clr.LuaDLR.Test.Expressions.ReturnLua3())", 3);
    }

    [TestMethod]
    public void TestConvert02()
    {
      using (Lua l = new Lua())
      {
        l.PrintExpressionTree = true;
        dynamic g = l.CreateEnvironment();
        var c = l.CompileChunk("return cast(bool, a)", "dummy", false, new KeyValuePair<string, Type>("a", typeof(object)));

        TestResult(g.dochunk(c, 1), true);
        TestResult(g.dochunk(c, ShortEnum.Eins), true);
        TestResult(g.dochunk(c, 0), false);
        TestResult(g.dochunk(c, null), false);
        TestResult(g.dochunk(c, new object()), true);
      }
    }

    [TestMethod]
    public void TestConvert03()
    {
      TestExpr("cast(string, 'a')", "a");
      TestExpr("cast(string, 1)", "1");
      TestExpr("cast(string, 0)", "0");
      TestExpr("cast(string, cast(short, 0))", "0");
    }

    [TestMethod]
    public void TestConvert04()
    {
      using (Lua l = new Lua())
      {
        l.PrintExpressionTree = true;
        dynamic g = l.CreateEnvironment();
        var c = l.CompileChunk("return cast(string, a)", "dummy", false, new KeyValuePair<string, Type>("a", typeof(object)));

        TestResult(g.dochunk(c, 'a'), "a");
        TestResult(g.dochunk(c, 1), "1");
        TestResult(g.dochunk(c, ShortEnum.Eins), "Eins");
        TestResult(g.dochunk(c, null), "");
        TestResult(g.dochunk(c, new object()), "System.Object");
      }
    }

    [TestMethod]
    public void TestConvert05()
    {
      TestExpr("cast(int, '1')", 1);
      TestExpr("cast(short, '0')", (short)0);
      TestExpr("cast(int, '1.2')", 1);
      TestExpr("cast(int, nil)", 0);
      TestExpr("cast(System.String, nil)", String.Empty);
      TestExpr("cast(System.Environment, nil)", NullResult);
      
    }

    [TestMethod]
    public void TestConvert06()
    {
      using (Lua l = new Lua())
      {
        l.PrintExpressionTree = true;
        dynamic g = l.CreateEnvironment();
        var c = l.CompileChunk("return cast(int, a)", "dummy", false, new KeyValuePair<string, Type>("a", typeof(object)));

        TestResult(g.dochunk(c, "1"), 1);
        TestResult(g.dochunk(c, ShortEnum.Eins), 1);
        TestResult(g.dochunk(c, '1'), 49);
        TestResult(g.dochunk(c, "1.2"), 1);
        TestResult(g.dochunk(c, null), 0);
      }
    }

    [TestMethod]
    public void TestConvert07()
    {
      TestExpr("cast(int, clr.LuaDLR.Test.Expressions.ReturnLua1())", 1);
      TestExpr("cast(short, clr.LuaDLR.Test.Expressions.ReturnLua2())", (short)2);
    }

    [TestMethod]
    public void TestConvert08()
    {
      using (Lua l = new Lua())
      {
        l.PrintExpressionTree = true;
        dynamic g = l.CreateEnvironment();
        var c = l.CompileChunk("return cast(int, a)", "dummy", false, new KeyValuePair<string, Type>("a", typeof(object)));

        TestResult(g.dochunk(c, new LuaResult(2)), 2);
        TestResult(g.dochunk(c, new LuaResult((short)2)), 2);
        TestResult(g.dochunk(c, new LuaResult(ShortEnum.Zwei)), 2);
      }
    }

    #endregion

    #region -- Arithmetic -------------------------------------------------------------

    [TestMethod]
    public void TestOperator01()
    {
      int a = 1 + 2;
      Assert.IsTrue(a == 3);
      Console.WriteLine("Test 1 (int a = new TestOperator(2) + 1):");
      a = new TestOperator(2) + 1;
      Assert.IsTrue(a == 3);

      Console.WriteLine("Test 2 (TestOperator b = 1 + 2):");
      TestOperator b = 1 + 2;
      Assert.IsTrue(a == 3);

      Console.WriteLine("Test 3 (TestOperator c = 2 + new TestOperator(1)):");
      TestOperator c = 2 + new TestOperator(1);
      Assert.IsTrue(c == 3);

      Console.WriteLine("Test 4 (int c = 2 + new TestOperator(1)):");
      int d = 2 + new TestOperator(1);
      Assert.IsTrue(d == 3);

      Console.WriteLine("Test 5 (int c = new TestOperator(1) + 2):");
      int e = new TestOperator(1) + 2;
      Assert.IsTrue(e == 3);

      Console.WriteLine("Test 6 (int c = new TestOperator(1) + 2):");
      int f = (byte)1 + new TestOperator(2);
      Assert.IsTrue(f == 3);

      Console.WriteLine("Methods:");
      Type t = typeof(TestOperator);
      foreach (MethodInfo mi in t.GetMethods(BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.DeclaredOnly))
        Console.WriteLine(mi.Name);
    } // proc TestOperator01

    [TestMethod]
    public void TestArithmetic01() { TestExpr("1 + 2", 3); }

    [TestMethod]
    public void TestArithmetic02() { TestExpr("1 + 2.0", 3.0); }

    [TestMethod]
    public void TestArithmetic03() { TestExpr("1 + '2'", 3); }

    [TestMethod]
    public void TestArithmetic04() { TestExpr("1 + '2.0'", 3.0); }

    [TestMethod]
    public void TestArithmetic05()
    {
      using (Lua l = new Lua())
      {
        l.PrintExpressionTree = true;
        var g = l.CreateEnvironment();
        var c = l.CompileChunk("return 1 + a;", "test.lua", false, new KeyValuePair<string, Type>("a", typeof(string)));

        TestResult(g.DoChunk(c, "2"), 3);
        TestResult(g.DoChunk(c, "2.0"), 3.0);
      }
    } 

    [TestMethod]
    public void TestArithmetic06()
    {
      using (Lua l = new Lua())
      {
        l.PrintExpressionTree = true;
        var g = l.CreateEnvironment();
        var c = l.CompileChunk("return 1 + a;", "test.lua", false, new KeyValuePair<string, Type>("a", typeof(object)));

        TestResult(g.DoChunk(c, 2), 3);
        TestResult(g.DoChunk(c, 2.0), 3.0);
        TestResult(g.DoChunk(c, "2"), 3);
        TestResult(g.DoChunk(c, "2.0"), 3.0);
        TestResult(g.DoChunk(c, 2.0f), 3.0f);
      }
    }

    [TestMethod]
    public void TestArithmetic07()
    {
      try
      {
        TestExpr("1 + nil", null);
        Assert.Fail();
      }
      catch (TargetInvocationException e)
      {
        Assert.IsTrue(e.InnerException is LuaRuntimeException && ((LuaRuntimeException)e.InnerException).Message.IndexOf("Object") >= 0);
      }
    }

    [TestMethod]
    public void TestArithmetic08()
    {
      try
      {
        TestExpr("1 + clr.LuaDLR.Test.Expressions.ReturnVoid()", NullResult);
        Assert.Fail();
      }
      catch (TargetInvocationException e)
      {
        Assert.IsTrue(e.InnerException is LuaRuntimeException && ((LuaRuntimeException)e.InnerException).Message.IndexOf("Object") >= 0);
      }
    }

    [TestMethod]
    public void TestArithmetic09()
    {
      using (Lua l = new Lua())
      {
        l.PrintExpressionTree = true;
        var g = l.CreateEnvironment();
        var c1 = l.CompileChunk("return a + b;", "test.lua", false, new KeyValuePair<string, Type>("a", typeof(int)), new KeyValuePair<string, Type>("b", typeof(TestOperator)));
        var c2 = l.CompileChunk("return a + b;", "test.lua", false, new KeyValuePair<string, Type>("a", typeof(object)), new KeyValuePair<string, Type>("b", typeof(object)));

        TestResult(g.DoChunk(c1, 1, new TestOperator(2)), new TestOperator(3));

        TestResult(g.DoChunk(c2, 1, new TestOperator(2)), new TestOperator(3));
        TestResult(g.DoChunk(c2, new TestOperator(2), 1), new TestOperator(3));
        TestResult(g.DoChunk(c2, new TestOperator(2), new TestOperator(1)), new TestOperator(3));
        TestResult(g.DoChunk(c2, new TestOperator(2), (short)1), new TestOperator(3));
        TestResult(g.DoChunk(c2, new TestOperator2(2), 1L), 3L);
        TestResult(g.DoChunk(c2, 2, new TestOperator2(1)), 3);
      }
    }

    [TestMethod]
    public void TestArithmetic10()
    {
      using (Lua l = new Lua())
      {
        l.PrintExpressionTree = true;
        var g = l.CreateEnvironment();
        var c = l.CompileChunk("return a + b;", "test.lua", false, new KeyValuePair<string, Type>("a", typeof(object)), new KeyValuePair<string, Type>("b", typeof(object)));

        TestResult(g.DoChunk(c, ShortEnum.Eins, ShortEnum.Zwei), ShortEnum.Drei);
        TestResult(g.DoChunk(c, IntEnum.Eins, IntEnum.Zwei), IntEnum.Drei);
        TestResult(g.DoChunk(c, (short)1, IntEnum.Zwei), 3);
        TestResult(g.DoChunk(c, ShortEnum.Eins, 2), 3);
      }
    }

    [TestMethod]
    public void TestArithmetic11()
    {
      using (Lua l = new Lua())
      {
        l.PrintExpressionTree = true;
        var g = l.CreateEnvironment();
        var c1 = l.CompileChunk("return a + b;", "test.lua", false, new KeyValuePair<string, Type>("a", typeof(Nullable<int>)), new KeyValuePair<string, Type>("b", typeof(Nullable<int>)));
        var c2 = l.CompileChunk("return a + b;", "test.lua", false, new KeyValuePair<string, Type>("a", typeof(object)), new KeyValuePair<string, Type>("b", typeof(object)));

        TestResult(g.DoChunk(c1, 1, 2), 3);
        TestResult(g.DoChunk(c1, null, 2), NullResult);

        TestResult(g.DoChunk(c2, new Nullable<short>(1), new Nullable<short>(2)), (short)3);
        TestResult(g.DoChunk(c2, new Nullable<int>(1), new Nullable<int>(2)), 3);
        TestResult(g.DoChunk(c2, new Nullable<int>(1), new Nullable<short>(2)), 3);
        TestResult(g.DoChunk(c2, new Nullable<short>(1), (short)2), (short)3);
        TestResult(g.DoChunk(c2, new Nullable<int>(1), 2), 3);
        TestResult(g.DoChunk(c2, new Nullable<int>(1), (short)2), 3);
      }
    }

    [TestMethod]
    public void TestArithmetic12()
    {
      using (Lua l = new Lua())
      {
        l.PrintExpressionTree = true;
        var g = l.CreateEnvironment();
        TestResult(g.DoChunk("return clr.LuaDLR.Test.Expressions.Return1() + clr.LuaDLR.Test.Expressions.Return2();", "test.lua"), 3);
        Console.WriteLine();
        TestResult(g.DoChunk("return clr.LuaDLR.Test.Expressions.ReturnLua1() + clr.LuaDLR.Test.Expressions.ReturnLua2();", "test.lua"), 3);
        Console.WriteLine();
        TestResult(g.DoChunk("return clr.LuaDLR.Test.Expressions.Return1() + clr.LuaDLR.Test.Expressions.ReturnLua2();", "test.lua"), 3);
        Console.WriteLine();
        var c2 = l.CompileChunk("return a() + b();", "test.lua", false, new KeyValuePair<string, Type>("a", typeof(object)), new KeyValuePair<string, Type>("b", typeof(object)));
        TestResult(g.DoChunk(c2, new Func<int>(Return1), new Func<int>(Return2)), 3);
        TestResult(g.DoChunk(c2, new Func<LuaResult>(ReturnLua1), new Func<LuaResult>(ReturnLua2)), 3);
        TestResult(g.DoChunk(c2, new Func<int>(Return1), new Func<LuaResult>(ReturnLua2)), 3);
      }
    }

    [TestMethod]
    public void TestArithmetic13() { TestExpr("2 ^ 3", 8.0); }

    [TestMethod]
    public void TestArithmetic14()
    {
      TestExpr("2 - 3", -1);
      TestExpr("2 * 3", 6);
      TestExpr("15 / 3", 5.0);
      TestExpr("15 // 3", 5);
      TestExpr("5 / 2", 2.5);
      TestExpr("5 // 2", 2);
      TestExpr("5.2 // 2", 2);
      TestExpr("5 % 2", 1);
      TestExpr("5.2 % 2", 1.2);
      TestExpr("2 ^ 0.5", 1.414);

      TestExpr("3 & 2", 2);
      TestExpr("2 | 1", 3);
      TestExpr("3 ~ 2", 1);
      TestExpr("1 << 8", 256);
      TestExpr("256 >> 8", 1);
      TestExpr("3.2 ~ 2", 1);

      TestExpr("clr.LuaDLR.Test.Expressions.TestOperator(2) + 3", new TestOperator(5));
      TestExpr("clr.LuaDLR.Test.Expressions.TestOperator(2) - 3", new TestOperator(-1));
      TestExpr("clr.LuaDLR.Test.Expressions.TestOperator(2) * 3", new TestOperator(6));
      TestExpr("clr.LuaDLR.Test.Expressions.TestOperator(6) / 3", new TestOperator(2));
      TestExpr("clr.LuaDLR.Test.Expressions.TestOperator(6) // 3", new TestOperator(2));
      TestExpr("clr.LuaDLR.Test.Expressions.TestOperator(3) & 2", new TestOperator(2));
      TestExpr("clr.LuaDLR.Test.Expressions.TestOperator(2) | 1", new TestOperator(3));
      TestExpr("clr.LuaDLR.Test.Expressions.TestOperator(3) ~ 2", new TestOperator(1));
      TestExpr("clr.LuaDLR.Test.Expressions.TestOperator(1) << 8", new TestOperator(256));
      TestExpr("clr.LuaDLR.Test.Expressions.TestOperator(256) >> 8", new TestOperator(1));
    }

    [TestMethod]
    public void TestArithmetic15()
    {
      using (Lua l = new Lua())
      {
        l.PrintExpressionTree = true;

        dynamic g = l.CreateEnvironment();
        TestResult(g.dochunk("return a // b", "dummy", "a", 15, "b", 3), 5);
        TestResult(g.dochunk("return a // b", "dummy", "a", 5.2, "b", 2), 2);

        TestResult(g.dochunk("return a & b", "dummy", "a", 3.0, "b", 2), 2);
        TestResult(g.dochunk("return a | b", "dummy", "a", 2.0, "b", 3), 3);
        TestResult(g.dochunk("return a ~ b", "dummy", "a", 3.0, "b", 2), 1);
        TestResult(g.dochunk("return a << b", "dummy", "a", 1.0, "b", 8), 256);
        TestResult(g.dochunk("return a >> b", "dummy", "a", 256.0, "b", 8), 1);

        LuaChunk c1 = l.CompileChunk("return a ~ b", "dummy", false, new KeyValuePair<string, Type>("a", typeof(object)), new KeyValuePair<string, Type>("b", typeof(int)));
        TestResult(g.dochunk(c1, 3.2, 2), 1);
        TestResult(g.dochunk(c1, "3.2", 2), 1);
        TestResult(g.dochunk(c1, ShortEnum.Drei, 2), 1);
        TestResult(g.dochunk(c1, new Nullable<short>(3), 2), 1);
        LuaChunk c2 = l.CompileChunk("return a() ~ b", "dummy", false, new KeyValuePair<string, Type>("a", typeof(object)), new KeyValuePair<string, Type>("b", typeof(int)));
        TestResult(g.dochunk(c2, new Func<LuaResult>(() => new LuaResult(3.2)), 2), 1);
        TestResult(g.dochunk(c2, new Func<LuaResult>(() => new LuaResult(new TestOperator(3))), 2), new TestOperator(1));
        TestResult(g.dochunk(c2, new Func<LuaResult>(() => new LuaResult(new Nullable<float>(3.2f))), 2), 1);
      }
    }

    [TestMethod]
    public void TestArithmetic16()
    {
      TestExpr("1 + 2 * 3", 7);
      TestExpr("1 + 2.2 * 3", 7.6);
      TestExpr("1 + (3 & 2) * 3", 7);
      TestExpr("1 + (2 | 1) * 3", 10);
      TestExpr("1 + 2 * 3 & 3", 3);
    }

    [TestMethod]
    public void TestArithmetic17()
    {
      TestExpr("-2", -2);
      TestExpr("-2.1", -2.1);
      TestExpr("-'2.1'", -2.1);
      TestExpr("~2", ~2);
      TestExpr("~2.1", ~2);
      TestExpr("~'2'", ~2);
      TestExpr("~'2.1'", ~2);
    }

    [TestMethod]
    public void TestArithmetic18()
    {
      using (Lua l = new Lua())
      {
        l.PrintExpressionTree = true;

        dynamic g = l.CreateEnvironment();
        TestResult(g.dochunk("return -a", "dummy", "a", 2), -2);
        TestResult(g.dochunk("return -a", "dummy", "a", 2.1), -2.1);
        TestResult(g.dochunk("return -a", "dummy", "a", new TestOperator(2)), new TestOperator(-2));
        TestResult(g.dochunk("return -a", "dummy", "a", new TestOperator2(2)), -2);
        TestResult(g.dochunk("return -a", "dummy", "a", ShortEnum.Zwei), (ShortEnum)(-2));

        TestResult(g.dochunk("return ~a", "dummy", "a", 2), ~2);
        TestResult(g.dochunk("return ~a", "dummy", "a", 2.1), ~2);
        TestResult(g.dochunk("return ~a", "dummy", "a", new TestOperator(2)), new TestOperator(~2));
        TestResult(g.dochunk("return ~a", "dummy", "a", new TestOperator2(2)), ~2);
        TestResult(g.dochunk("return ~a", "dummy", "a", ShortEnum.Zwei), ~2);
      }
    }

    [TestMethod]
    public void TestArithmetic19()
    {
      TestExpr("1 + -2.2 * 3", -5.6);
      TestExpr("-2 * '2'", -4);
      TestExpr("2 * -'2'", -4);
      TestExpr("-2 * '2.0'", -4.0);
      TestExpr("1 + (1 + 2) * 3", 10);
    }

    [TestMethod]
    public void TestArithmetic20()
    {
      try
      {
        TestExpr("-clr.LuaDLR.Test.Expressions.ReturnVoid()", null);
        Assert.Fail();
      }
      catch (TargetInvocationException e)
      {
        Assert.IsTrue(e.InnerException is LuaRuntimeException && ((LuaRuntimeException)e.InnerException).Message.IndexOf("nil") >= 0);
      }
    }

    [TestMethod]
    public void TestArithmetic21()
    {
      TestExpr("true", true);
      TestExpr("false", false);
      TestExpr("nil", NullResult);
    }

    #endregion

    #region -- Const ------------------------------------------------------------------

    [TestMethod]
    public void TestConst01()
    {
      using (Lua l = new Lua())
      {
        l.PrintExpressionTree = true;
        var g = l.CreateEnvironment();

        TestResult(g.DoChunk("const a = 20; return a;", "dummy"), 20);
        TestResult(g.DoChunk("const a = cast(ushort, 20); return a;", "dummy"), (ushort)20);
        TestResult(g.DoChunk("const a = cast(int, '20'); return a;", "dummy"), 20);
      }
    }

    #endregion

    #region -- Logic ------------------------------------------------------------------

    [TestMethod]
    public void TestLogic01() { TestExpr("10 or 20", 10); }

    [TestMethod]
    public void TestLogic02() { TestExpr("10 or false", 10); }

    [TestMethod]
    public void TestLogic03() { TestExpr("nil or 'a'", "a"); }

    [TestMethod]
    public void TestLogic04() { TestExpr("false or nil", NullResult); }

    [TestMethod]
    public void TestLogic05() { TestExpr("nil and 10", NullResult); }

    [TestMethod]
    public void TestLogic06() { TestExpr("false and false", false); }

    [TestMethod]
    public void TestLogic07() { TestExpr("false and nil", false); }

    [TestMethod]
    public void TestLogic08() { TestExpr("10 and 20", 20); }

    [TestMethod]
    public void TestLogic09()
    {
      TestExpr("clr.LuaDLR.Test.Expressions.TestOperator(10) and 20", 20);
      TestExpr("clr.LuaDLR.Test.Expressions.TestOperator(10) or 20", new LuaResult(new TestOperator(10)));
      TestExpr("clr.LuaDLR.Test.Expressions.ReturnLua1() or 20", new LuaResult(1));
    }

    [TestMethod]
    public void TestLogic10() 
    {
      using (Lua l = new Lua())
      {
        l.PrintExpressionTree = true;
        var g = l.CreateEnvironment();
        var c = l.CompileChunk("if a then return true else return false end", "dummy", false, new KeyValuePair<string, Type>("a", typeof(object)));

        TestResult(g.DoChunk(c, 1), true);
        TestResult(g.DoChunk(c, 0), false);
        TestResult(g.DoChunk(c, ShortEnum.Drei), true);
        TestResult(g.DoChunk(c, IntEnum.Null), false);
        TestResult(g.DoChunk(c, new object[] { null }), false);
      }
    }

    [TestMethod]
    public void TestLogic11()
    {
      using (Lua l = new Lua())
      {
        l.PrintExpressionTree = true;
        var g = l.CreateEnvironment();
        var c = l.CompileChunk("return a or b", "dummy", false, new KeyValuePair<string, Type>("a", typeof(object)), new KeyValuePair<string, Type>("b", typeof(object)));

        TestResult(g.DoChunk(c, 10, 20), 10);
        TestResult(g.DoChunk(c, (short)10, (short)20), (short)10);
        TestResult(g.DoChunk(c, 10, (short)20), 10);
        TestResult(g.DoChunk(c, 10, false), 10);
        TestResult(g.DoChunk(c, null, "a"), "a");
        TestResult(g.DoChunk(c, false, null), NullResult);
        TestResult(g.DoChunk(c, new TestOperator(10), 20), new TestOperator(10));
      }
    }

    [TestMethod]
    public void TestLogic12()
    {
      using (Lua l = new Lua())
      {
        l.PrintExpressionTree = true;
        var g = l.CreateEnvironment();
        var c = l.CompileChunk("return a and b", "dummy", false, new KeyValuePair<string, Type>("a", typeof(object)), new KeyValuePair<string, Type>("b", typeof(object)));

        TestResult(g.DoChunk(c, null, 10), NullResult);
        TestResult(g.DoChunk(c, false, false), false);
        TestResult(g.DoChunk(c, false, null), false);
        TestResult(g.DoChunk(c, 10, 20), 20);
        TestResult(g.DoChunk(c, new TestOperator(10), 20), 20);
      }
    }

    [TestMethod]
    public void TestLogic13()
    {
      TestExpr("not true", false);
      TestExpr("not false", true);
      TestExpr("not 1", false);
      TestExpr("not 0", true);
      TestExpr("not nil", true);
    }

    [TestMethod]
    public void TestLogic14()
    {
      using (Lua l = new Lua())
      {
        l.PrintExpressionTree = true;
        var g = l.CreateEnvironment();
        var c = l.CompileChunk("return not a", "dummy", false, new KeyValuePair<string, Type>("a", typeof(object)));

        TestResult(g.DoChunk(c, 1), false);
        TestResult(g.DoChunk(c, 0), true);
        TestResult(g.DoChunk(c, ShortEnum.Drei), false);
        TestResult(g.DoChunk(c, IntEnum.Null), true);
        TestResult(g.DoChunk(c, new LuaResult(0)), true);
        TestResult(g.DoChunk(c, new object[] { null }), true);
      }
    }

    #endregion

    #region -- Compare ----------------------------------------------------------------

    [TestMethod]
    public void TestCompare01() { TestExpr("1 < 2", true); }

    [TestMethod]
    public void TestCompare02() { TestExpr("1 > 2", false); }

    [TestMethod]
    public void TestCompare03()
    {
      using (Lua l = new Lua())
      {
        l.PrintExpressionTree = true;
        dynamic g = l.CreateEnvironment();
        var c = l.CompileChunk("return a < b", "dummy", false,
          new KeyValuePair<string, Type>("a", typeof(object)),
          new KeyValuePair<string, Type>("b", typeof(object))
        );

        TestResult(g.dochunk(c, 1, 2), true);
        TestResult(g.dochunk(c, 2, 1), false);
        TestResult(g.dochunk(c, 2, 2), false);
        TestResult(g.dochunk(c, new TestOperator(1), 2), true);
        TestResult(g.dochunk(c, 1, new TestOperator(2)), true);
      }
    }

    [TestMethod]
    public void TestCompare04() { TestExpr("1 <= 2", true); }

    [TestMethod]
    public void TestCompare05() { TestExpr("1 >= 2", false); }

    [TestMethod]
    public void TestCompare06() { TestExpr("1 == 2", false); }

    [TestMethod]
    public void TestCompare07() { TestExpr("1 ~= 2", true); }

    [TestMethod]
    public void TestCompare08()
    {
      using (Lua l = new Lua())
      {
        l.PrintExpressionTree = true;
        dynamic g = l.CreateEnvironment();
        var c = l.CompileChunk("return a == b", "dummy", false,
          new KeyValuePair<string, Type>("a", typeof(object)),
          new KeyValuePair<string, Type>("b", typeof(object))
        );

        TestResult(g.dochunk(c, 1, 2), false);
        TestResult(g.dochunk(c, 2, 1), false);
        TestResult(g.dochunk(c, 2, 2), true);
        TestResult(g.dochunk(c, 2, (short)2), true);
        TestResult(g.dochunk(c, new TestOperator(1), 2), false);
        TestResult(g.dochunk(c, 2, new TestOperator(2)), true);
        object a = new object();
        TestResult(g.dochunk(c, a, a), true);
        TestResult(g.dochunk(c, "a", "a"), true);
      }
    }

    [TestMethod]
    public void TestCompare09()
    {
      using (Lua l = new Lua())
      {
        l.PrintExpressionTree = true;
        dynamic g = l.CreateEnvironment();
        var c = l.CompileChunk("return a ~= b", "dummy", false,
          new KeyValuePair<string, Type>("a", typeof(object)),
          new KeyValuePair<string, Type>("b", typeof(object))
        );

        TestResult(g.dochunk(c, 1, 2), true);
        TestResult(g.dochunk(c, 2, 1), true);
        TestResult(g.dochunk(c, 2, 2), false);
        TestResult(g.dochunk(c, 2, (short)2), false);
        TestResult(g.dochunk(c, new TestOperator(1), 2), true); // int -> testoperator, but no equal
        TestResult(g.dochunk(c, 2, new TestOperator(2)), false);
        object a = new object();
        TestResult(g.dochunk(c, "a", "a"), false);
      }
    }

    #endregion

    #region -- Concat -----------------------------------------------------------------

    [TestMethod]
    public void TestConcat01() { TestExpr("'a' .. 'b' .. 'c'", "abc"); }

    [TestMethod]
    public void TestConcat02() { TestExpr("'a' .. 1 .. 'c'", "a1c"); }

    [TestMethod]
    public void TestConcat03() { TestExpr("'a' .. clr.LuaDLR.Test.Expressions.TestOperator(1) .. 'c'", "a1c"); }

    [TestMethod]
    public void TestConcat04() { TestExpr("'a' .. clr.LuaDLR.Test.Expressions.ReturnVoid() .. 'c'", "ac"); }

    [TestMethod]
    public void TestConcat05() { TestExpr("'a' .. nil .. 'c'", "ac"); }

    #endregion

    #region -- ArrayLength ------------------------------------------------------------

    [TestMethod]
    public void TestArrayLength01()
    {
      TestExpr("#'abc'", 3);
    }

    [TestMethod]
    public void TestArrayLength02()
    {
      using (Lua l = new Lua())
      {
        l.PrintExpressionTree = true;
        dynamic g = l.CreateEnvironment();
        var c = l.CompileChunk("return #a;", "dummy", false, new KeyValuePair<string, Type>("a", typeof(object)));

        TestResult(g.dochunk(c, "abc"), 3);
        TestResult(g.dochunk(c, new TestOperator(3)), 3);
        TestResult(g.dochunk(c, new TestOperator2(3)), 3);
      }
    }
    #endregion
  } // class Expressions

  [TestClass]
  public class ExpressionsTables : TestHelper
  {
    [TestMethod]
    public void TestExpr01() { Assert.IsTrue(TestExpressionTable(false, "{}")); }

    [TestMethod]
    public void TestExpr02()
    {
      Assert.IsTrue(TestExpressionTable(false, "{1, 2, 3; 4}",
        TV(1, 1),
        TV(2, 2),
        TV(3, 3),
        TV(4, 4)));
    } // proc TestExpr

    [TestMethod]
    public void TestExpr03()
    {
      Assert.IsTrue(TestExpressionTable(false,"{a = 1, b = 2, c = 3;  d = 4}",
        TV("a", 1),
        TV("b", 2),
        TV("c", 3),
        TV("d", 4)));
    } // proc TestExpr

    [TestMethod]
    public void TestExpr04()
    {
      Assert.IsTrue(TestExpressionTable(false, "{['a'] = 1}",
        TV("a", 1)));
    } // proc TestExpr

    [TestMethod]
    public void TestExpr06()
    {
      Assert.IsTrue(TestExpressionTable(true, "function f(a) return a; end;" + Environment.NewLine +
        "local g = 32; local x = 24;" + Environment.NewLine+
        "a = { [f('z')] = g; 'x', 'y'; x = 1, f(x), [30] = 23; 45 }" + Environment.NewLine +
        "return a;",
        TV("z", 32),
        TV(1, "x"),
        TV(2, "y"),
        TV("x", 1),
        TV(3, 24),
        TV(30, 23),
        TV(4, 45)));
    } // proc TestExpr06

    //[TestMethod]
    //public void TestExpr05()
    //{
    //  Assert.IsTrue(TestExpressionTable("{a = {}}"));
    //} // proc TestExpr
  } // class ExpressionsTables
}
