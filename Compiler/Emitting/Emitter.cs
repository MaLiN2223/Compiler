using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

using Compiler.Language;
using Compiler.SyntaxTree;
using Compiler.SyntaxTree.DataTypeExpressions;

namespace Compiler.Emitting
{
  public class Emitter
  {
    private readonly Dictionary<string, int> _variablesDict = new Dictionary<string, int>();

    public void Emit(ScopeExpression mainScope)
    {
      Start(mainScope);
    }

    private static OpCode GetOperation(string op)
    {
      switch (op)
      {
        case "+":
          return OpCodes.Add;
        case "-":
          return OpCodes.Sub;
        case "/":
          return OpCodes.Div;
        case "*":
          return OpCodes.Mul;
        case "==":
          return OpCodes.Ceq;
        case ">":
          return OpCodes.Cgt;
        case "<":
          return OpCodes.Clt;
        case "%":
          return OpCodes.Rem;
      }

      throw new NotImplementedException();
    }

    private void DoEmit(ILGenerator ilg, ScopeExpression mainScope)
    {
      foreach (var q in mainScope.Expressions)
      {
        if (q is ScopeExpression expression)
        {
          DoEmit(ilg, expression);
        }
        else
        {
          DoEmit(ilg, q);
        }
      }
    }

    private void DoEmit(ILGenerator ilg, AbstractExpression expression)
    {
      if (expression is AssignmentExpression asgn)
      {
        EmitAssignment(ilg, asgn);
      }
      else if (expression is Int32Expression num)
      {
        ilg.Emit(OpCodes.Ldc_I4, num.Value);
      }
      else if (expression is DoubleExpression dub)
      {
        ilg.Emit(OpCodes.Ldc_R8, dub.Value);
      }
      else if (expression is TriboolExpression trb)
      {
        throw new NotSupportedException("Tribool is not supported yet");
      }
      else if (expression is VariableExpression varexpr)
      {
        var variable = _variablesDict[varexpr.Name];
        ilg.Emit(OpCodes.Ldloc, variable);
      }
      else if (expression is OperatorExpression op)
      {
        EmitOperator(ilg, op);
      }
      else if (expression is LogicalFunctionExpression log)
      {
        EmitLogical(ilg, log);
      }
      else if (expression is LogicExpression le)
      {
        ilg.Emit(le.Value ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0);
      }
    }

    private void EmitAssignment(ILGenerator ilg, AssignmentExpression asgn)
    {
      DoEmit(ilg, asgn.Right);
      if (asgn.Left is DeclarationExpression decl)
      {
        var q = ilg.DeclareLocal(LanguageModel.ValueTypeToTypeMap[decl.ValueType]);
#if DEBUG
        q.SetLocalSymInfo(decl.Name);
#endif
        _variablesDict.Add(decl.Name, q.LocalIndex);
      }

      ilg.Emit(OpCodes.Stloc, _variablesDict[asgn.Left.Name]);
    }

    private void EmitLogical(ILGenerator ilg, LogicalFunctionExpression log)
    {
      if (log.Value == "if")
      {
        var jump = ilg.DefineLabel();
        DoEmit(ilg, log.InnerExpression);
        ilg.Emit(OpCodes.Brfalse, jump);
        DoEmit(ilg, log.ScopeExpression);
        ilg.MarkLabel(jump);
      }
      else
      {
        var start = ilg.DefineLabel();
        var jump = ilg.DefineLabel();
        ilg.MarkLabel(start);
        DoEmit(ilg, log.InnerExpression);
        ilg.Emit(OpCodes.Brfalse, jump);
        DoEmit(ilg, log.ScopeExpression);
        ilg.Emit(OpCodes.Br, start);
        ilg.MarkLabel(jump);
      }
    }

    private void EmitOperation(ILGenerator ilg, string operation)
    {
      var op = GetOperation(operation);
      ilg.Emit(op);
    }

    private void EmitOperator(ILGenerator ilg, OperatorExpression expr)
    {
      if (expr is AssignmentExpression asgn)
      {
        EmitAssignment(ilg, asgn);
      }
      else
      {
        DoEmit(ilg, expr.Left);
        DoEmit(ilg, expr.Right);
        EmitOperation(ilg, expr.Operation);
      }
    }

    private void Start(ScopeExpression mainScope)
    {
      var assemblyFIle = "Test.exe";
      var an = new AssemblyName { Name = "Test" };
      var ad = AppDomain.CurrentDomain;
      var ab = ad.DefineDynamicAssembly(an, AssemblyBuilderAccess.Save);

#if DEBUG
      var emitAsDebug = true;
#else
      bool emitAsDebug = false;
#endif
      var mb = ab.DefineDynamicModule(an.Name, assemblyFIle, emitAsDebug);
      var tb = mb.DefineType("Program.Namespace", TypeAttributes.Public | TypeAttributes.Class);
      var fb = tb.DefineMethod(
        "Main",
        MethodAttributes.Public | MethodAttributes.Static,
        typeof(void),
        new[] { typeof(string[]) });
      var ilg = fb.GetILGenerator();

      DoEmit(ilg, mainScope);

      ilg.Emit(OpCodes.Ldloc, 0);
      ilg.Emit(OpCodes.Box, typeof(int));
      ilg.Emit(OpCodes.Call, typeof(Console).GetMethod("WriteLine", new[] { typeof(object) }));
      ilg.Emit(OpCodes.Ret);
      var t = tb.CreateType();
      ab.SetEntryPoint(fb, PEFileKinds.ConsoleApplication);
      ab.Save(assemblyFIle);
    }
  }
}