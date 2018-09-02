using System;
using System.Collections.Generic;
using System.IO;
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

		public string Emit(ScopeExpression mainScope, string assemblyName)
		{
			var outputPath = Path.GetDirectoryName(assemblyName);
			var filePath = Start(mainScope, assemblyName, outputPath);
			return filePath;
		}

		private static void EmitOperation(ILGenerator ilg, string op)
		{
			switch (op)
			{
				case "+":
					ilg.Emit(OpCodes.Add);
					break;
				case "-":
					ilg.Emit(OpCodes.Sub);
					break;
				case "/":
					ilg.Emit(OpCodes.Div);
					break;
				case "*":
					ilg.Emit(OpCodes.Mul);
					break;
				case "==":
					ilg.Emit(OpCodes.Ceq);
					break;
				case ">":
					ilg.Emit(OpCodes.Cgt);
					break;
				case "<":
					ilg.Emit(OpCodes.Clt);
					break;
				case "%":
					ilg.Emit(OpCodes.Rem);
					break;
				case "!=":
					ilg.Emit(OpCodes.Ceq);
					ilg.Emit(OpCodes.Ldc_I4_0);
					ilg.Emit(OpCodes.Ceq);
					break;
				default:
					throw new NotImplementedException();
			}

		}

		private void DoEmit(ILGenerator ilg, ScopeExpression expr)
		{
			foreach (var ex in expr.Expressions)
			{
				DoEmit(ilg, ex);
			}
		}

		private void DoEmit(ILGenerator ilg, AbstractExpression expression)
		{

			if (expression is AssignmentExpression asgn)
			{
				EmitAssignment(ilg, asgn);
			}
			else if (expression is ReturnExpression ret)
			{
				DoEmit(ilg, ret.Expression);
				ilg.Emit(OpCodes.Ret);
			}
			else if (expression is Int32Expression num)
			{
				ilg.Emit(OpCodes.Ldc_I4, num.Value);
			}
			else if (expression is DoubleExpression dub)
			{
				ilg.Emit(OpCodes.Ldc_R8, dub.Value);
			}
			else if (expression is IdentifierExpression varexpr)
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
				DoEmit(ilg, log.LogicalCheck);
				ilg.Emit(OpCodes.Brfalse, jump);
				DoEmit(ilg, log.BodyExpression);
				ilg.MarkLabel(jump);
			}
			else if (log is ForLoopExpression fl)
			{
				var start = ilg.DefineLabel();
				var jump = ilg.DefineLabel();
				DoEmit(ilg, fl.Initialization);
				ilg.MarkLabel(start);
				DoEmit(ilg, fl.LogicalCheck);
				ilg.Emit(OpCodes.Brfalse, jump);
				DoEmit(ilg, fl.BodyExpression);
				DoEmit(ilg, fl.Incrementation);
				ilg.Emit(OpCodes.Br, start);
				ilg.MarkLabel(jump);
			}
			else
			{
				var start = ilg.DefineLabel();
				var jump = ilg.DefineLabel();
				ilg.MarkLabel(start);
				DoEmit(ilg, log.LogicalCheck);
				ilg.Emit(OpCodes.Brfalse, jump);
				DoEmit(ilg, log.BodyExpression);
				ilg.Emit(OpCodes.Br, start);
				ilg.MarkLabel(jump);
			}
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

		private string Start(ScopeExpression mainScope, string outputName, string outputPath)
		{
			var assemblyName = Path.GetFileNameWithoutExtension(outputName);
			var assemblyFile = assemblyName + ".exe";
			var an = new AssemblyName { Name = assemblyName };
			var ad = AppDomain.CurrentDomain;
			var ab = ad.DefineDynamicAssembly(an, AssemblyBuilderAccess.Save);

#if DEBUG
			var emitAsDebug = true;
#else
      bool emitAsDebug = false;
#endif
			var mb = ab.DefineDynamicModule(an.Name, assemblyFile, emitAsDebug);

			EmitTypes(mb, mainScope);

			ab.SetEntryPoint(Main, PEFileKinds.ConsoleApplication);
			var outputFile = Path.Combine(outputPath, assemblyFile);
			ab.Save(assemblyFile);
			File.Move(assemblyFile, outputFile);
			return outputFile;
		}

		private MethodBuilder EmitTypes(ModuleBuilder mb, ScopeExpression mainScope)
		{
			var list = new List<Type>();
			MethodBuilder main = null;
			foreach (var scope in mainScope.Expressions) // namespaces
			{
				ProcessNamespace(scope as NamespaceExpression, mb);
			}

			return main;
		}

		private MethodBuilder Main = null;

		private string ConcatenateNames(string a, string b)
		{
			if (string.IsNullOrEmpty(a))
			{
				return b;
			}

			return a + "." + b;
		}
		private void ProcessNamespace(NamespaceExpression scope, ModuleBuilder mb, string higherName = "")
		{
			var namespaceName = scope.NamespaceName;
			foreach (var innerScope in scope.Expressions) // classes or inner namespaces
			{
				if (innerScope is NamespaceExpression sc)
				{
					ProcessNamespace(sc, mb, namespaceName);
				}
				else if (innerScope is ClassExpression ce)
				{
					EmitType(ce, mb, higherName);
				}
				else
				{
					throw new ArgumentException();
				}
			}
		}
		private Dictionary<string, Type> KnownTypes = new Dictionary<string, Type>()
		{
			{"int",typeof(int) },
			{"bool", typeof(bool) },
			{"void",typeof(void) },
			{"string", typeof(string) },
			{"double", typeof(double) }
		};
		private void EmitType(ClassExpression classExpression, ModuleBuilder mb, string higherName)
		{
			var className = classExpression.ClassName;
			var tb = mb.DefineType(ConcatenateNames(higherName, className), TypeAttributes.Public | TypeAttributes.Class);

			foreach (var expr in classExpression.Expressions)
			{
				var method = expr as FunctionExpression;
				Type t = typeof(void);
				if (KnownTypes.ContainsKey(method.ReturnType))
				{
					t = KnownTypes[method.ReturnType];
				}



				List<Type> types = new List<Type>();
				if (method.Name != "Main")
				{
					foreach (var arg in method.Arguments)
					{
						types.Add(KnownTypes[arg.Item1]);
					}
				}
				else
				{ 
					types.Add(typeof(string[]));
				}

				var methodBuilder = tb.DefineMethod(method.Name, MethodAttributes.Public | MethodAttributes.Static, t, types.ToArray());
				for (int i = 0; i < method.Arguments.Count; ++i)
				{
					methodBuilder.DefineParameter(i + 1, ParameterAttributes.None, method.Arguments[i].Item2);
				}
				var ilg = methodBuilder.GetILGenerator();
				DoEmit(ilg, method);
				if (method.Name == "Main")
				{
					Main = methodBuilder;
				}
			}
			tb.CreateType();
		}
	}
}