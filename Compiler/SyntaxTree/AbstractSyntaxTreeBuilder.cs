using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using Compiler.Language;
using Compiler.Lexing;
using Compiler.SyntaxTree.DataTypeExpressions;

namespace Compiler.SyntaxTree
{
	public enum ScopeType
	{
		Namespace, Class, Function, Misc
	}
	public class AbstractSyntaxTreeBuilder
	{
		private readonly ILanguageModel languageModel;

		public AbstractSyntaxTreeBuilder(ILanguageModel languageModel)
		{
			this.languageModel = languageModel;
		}
		public ScopeExpression Build(IEnumerable<Token> tokens)
		{
			var postfixCreator = new PostfixCreator(languageModel);
			//var a = string.Join(",", tokens.Select(x => x.Value));
			var main = ConsumeScope(tokens.GetEnumerator(), ScopeType.Unknown);
			//var postfix = postfixCreator.Postfix(tokens);
			//var q = string.Join(",", postfix.Select(x => x.Value));
			//var mainBlock = ConsumeNewScope(postfix.GetEnumerator(), null);
			return main;
		}

		private AbstractExpression CreateNumberExpression(string value)
		{
			if (languageModel.IsSpecialDataKeyword(value))
			{
				// For now we only suppor true/false
				return new LogicExpression(value);
			}

			if (int.TryParse(value, out var q))
			{
				return new Int32Expression(q);
			}

			if (double.TryParse(value, out var z))
			{
				return new DoubleExpression(z);
			}

			throw new NotSupportedException();
		}

		public enum ScopeType
		{
			Namespace, Class, Function, Logical, Unknown, Other
		}

		private AbstractExpression ConsumeExpression(IEnumerable<Token> tokens)
		{
			return ConsumeExpression(tokens.GetEnumerator());
		}
		private AbstractExpression ConsumeExpression(IEnumerator<Token> tokens, string specialTerminator = null)
		{
			List<Token> tok = new List<Token>();
			var creator = new PostfixCreator(languageModel);

			if (specialTerminator != null)
			{
				while (tokens.Current.Value != specialTerminator)
				{
					tok.Add(tokens.Current);
					tokens.MoveNext();
				}
				tok.Add(new Token(";", TokenType.Terminator, tokens.Current.Line)); // HACKY WAY OF USING POSTFIXBELOW
			}
			else if (tokens.Current.Value == "(") // read until bracket
			{
				int bracketsCount = 1;
				tok.Add(tokens.Current);
				while (tokens.MoveNext() && bracketsCount > 0)
				{
					if (tokens.Current.Value == ")")
					{
						bracketsCount--;
					}
					else if (tokens.Current.Value == "(")
					{
						bracketsCount++;
					}
					tok.Add(tokens.Current);
				}

			}
			else // read until delimiter
			{
				while (tokens.Current.Type != TokenType.Terminator)
				{
					tok.Add(tokens.Current);
					tokens.MoveNext();
				}
				tok.Add(tokens.Current);
			}

			return GenerateExpressionFromPostfix(creator.Postfix(tok));
		}

		public AbstractExpression GenerateExpressionFromPostfix(List<Token> tokens)
		{
			var enumer = tokens.GetEnumerator();
			var workStack = new Stack<AbstractExpression>(tokens.Count);
			while (enumer.MoveNext())
			{
				var token = enumer.Current;
				var type = token.Type;
				if (type == TokenType.SpecialDataKeyword)
				{
					workStack.Push(CreateNumberExpression(token.Value));
				}
				else if (type == TokenType.Identifier)
				{
					workStack.Push(new IdentifierExpression(token.Value));
				}
				else if (type == TokenType.SimpleType)
				{
					if (workStack.Pop() is IdentifierExpression variable)
					{
						workStack.Push(new DeclarationExpression(token.Value, variable.Name));
					}
					else
					{
						throw new ArgumentException();
					}
				}
				else if (type == TokenType.Keyword)
				{
					workStack.Push(ConsumeKeyword(enumer));
				}
				else if (type == TokenType.Operator)
				{
					var a = workStack.Pop();
					var b = workStack.Pop();
					workStack.Push(OperatorExpression.Operator(token.Value, b, a));
				}
				else if (type == TokenType.Terminator)
				{
					// Expression finished
					break;
				}
				else
				{
					throw new ArgumentException("Unknown token");
				}
			}
			while (workStack.Count != 1)
			{
				throw new ArgumentException();
			}
			return workStack.Pop();
		}


		/// <summary>
		/// Assumes that scope is being opened via ( or {
		/// </summary> 
		private ScopeExpression ConsumeScope(IEnumerator<Token> instruction, ScopeType scopeType)
		{
			var scope = new ScopeExpression();
			var enumer = instruction;
			while (enumer.MoveNext())
			{
				var token = enumer.Current;
				var type = token.Type;
				if (type == TokenType.Keyword)
				{
					scope.Expressions.Add(ConsumeKeyword(instruction));
				}
				else if (type == TokenType.Operator)
				{
					throw new NotSupportedException();
				}
				else if (type == TokenType.SimpleType)
				{
					if (scopeType == ScopeType.Class) // static / class variable or return type of method
					{
						var returnType = enumer.Current.Value;
						enumer.MoveNext(); // skip type
						if (enumer.Current.Type == TokenType.Identifier)
						{
							var identifier = enumer.Current.Value;
							enumer.MoveNext(); // skip identifier
							if (enumer.Current.Value == "(" || enumer.Current.Value == "()") // function
							{
								List<Tuple<string, string>> arguments = new List<Tuple<string, string>>();
								if (enumer.Current.Value != "()")
								{
									enumer.MoveNext();// skip (
									arguments = ConsumeFunctionArguments(enumer);
									enumer.MoveNext(); // skip )
								}
								else
								{
									enumer.MoveNext(); // skip ()
								}

								var function = new FunctionExpression
								{
									Arguments = arguments,
									Name = identifier,
									ReturnType = returnType
								};
								if (enumer.Current.Value == "{")
								{
									var innerScope = ConsumeScope(enumer, ScopeType.Function);
									function.Expressions = innerScope.Expressions;
									scope.Expressions.Add(function);
								}
								else
								{
									throw new CompilationSyntaxErrorException(token);
								}
							}
							else // static / class variable
							{
								throw new NotImplementedException();
							}
						}
						else
						{
							throw new SyntaxErrorException();
						}
					}
					else if (scopeType == ScopeType.Function) // declaration
					{
						var declaration = ConsumeExpression(enumer);
						scope.Expressions.Add(declaration);
					}
					else // ??
					{
						throw new NotImplementedException();
					}

				}
				else if (type == TokenType.Identifier)
				{
					var identifier = ConsumeExpression(enumer);
					scope.Expressions.Add(identifier);
				}
				else if (type == TokenType.Punctuation)
				{
					throw new NotImplementedException();
				}
				else if (type == TokenType.Terminator)
				{
					//skip
				}
				else if (type == TokenType.SpecialDataKeyword)
				{
					throw new NotImplementedException();
				}
				else if (type == TokenType.Scope)
				{
					//Finished
					break;
				}
				else
				{
					throw new ArgumentOutOfRangeException();
				}
			}

			return scope;

		}

		private List<Tuple<string, string>> ConsumeFunctionArguments(IEnumerator<Token> enumer)
		{
			var outputList = new List<Tuple<string, string>>();
			if (enumer.Current.Value == "(")
			{
				enumer.MoveNext();
			}
			while (enumer.Current.Value != ")")
			{
				if (enumer.Current.Value == ",")
				{
					enumer.MoveNext(); // skipping comma
				}
				var type = enumer.Current.Value;
				if (!enumer.MoveNext())
				{
					throw new SyntaxErrorException();
				}

				var name = enumer.Current.Value;
				outputList.Add(new Tuple<string, string>(type, name));
				enumer.MoveNext();

			}

			return outputList;
		}


		//private ScopeExpression ConsumeNewScope(IEnumerator<Token> instruction, ScopeType? scopeType)
		//{
		//	var stack = new Stack<AbstractExpression>();
		//	var scope = new ScopeExpression();
		//	var instructionsStack = new Stack<AbstractExpression>();
		//	var enumer = instruction;
		//	while (enumer.MoveNext())
		//	{	
		//		var token = enumer.Current;
		//		var type = token.Type;
		//		if (type == TokenType.DataType)
		//		{
		//			stack.Push(CreateNumberExpression(token.Value));
		//		}
		//		else if (type == TokenType.Identifier)
		//		{
		//			stack.Push(new IdentifierExpression(token.Value));
		//		}
		//		else if (type == TokenType.Type)
		//		{
		//			if (stack.Pop() is IdentifierExpression variable)
		//			{
		//				stack.Push(new DeclarationExpression(token.Value, variable.Name));
		//			}
		//			else
		//			{
		//				throw new ArgumentException();
		//			}
		//		}
		//		else if (type == TokenType.Terminator)
		//		{
		//			while (stack.Count > 0 && !(stack.Peek() is ScopeExpression))
		//			{
		//				instructionsStack.Push(stack.Pop());
		//			}
		//		}
		//		else if (type == TokenType.Keyword)
		//		{
		//			instructionsStack.Push(ConsumeKeyword(token.Value, stack, instructionsStack, enumer));
		//		}
		//		else if (type == TokenType.Scope)
		//		{
		//			if (token.Value == "{")
		//			{
		//				var innerScope = ConsumeNewScope(enumer, null);
		//				instructionsStack.Push(innerScope);
		//			}
		//			else if (token.Value == "}")
		//			{
		//				while (stack.Count > 0)
		//				{
		//					throw new ArgumentException();
		//				}

		//				scope.Expressions = instructionsStack.Reverse().ToList();
		//				return scope;
		//			}
		//		}
		//		else if (type == TokenType.Operator)
		//		{
		//			var a = stack.Pop();
		//			var b = stack.Pop();
		//			stack.Push(OperatorExpression.Operator(token.Value, b, a));
		//		}
		//	}
		//	while (stack.Count > 0)
		//	{
		//		throw new ArgumentException();
		//	}
		//	scope.Expressions = instructionsStack.Reverse().ToList();
		//	return scope;
		//}

		private AbstractExpression ConsumeKeyword(IEnumerator<Token> enumerator)
		{
			var value = enumerator.Current.Value;
			if (value == "namespace" || value == "class")
			{
				enumerator.MoveNext();
				var namespaceIdentifier = enumerator.Current;
				if (namespaceIdentifier.Type == TokenType.Identifier)
				{
					enumerator.MoveNext();// move over identifier
					if (enumerator.Current.Value != "{")
					{
						throw new SyntaxErrorException("{ expected");
					}
					if (value == "namespace")
					{
						var inner = ConsumeScope(enumerator, ScopeType.Namespace);
						return new NamespaceExpression(namespaceIdentifier.Value, inner);
					}
					if (value == "class")
					{
						var inner = ConsumeScope(enumerator, ScopeType.Class);
						return new ClassExpression(namespaceIdentifier.Value, inner);
					}
				}
				else
				{
					throw new SyntaxErrorException("no keyword identifier found");
				}
			}
			else if (value == "if")
			{
				enumerator.MoveNext(); // skip  if
				enumerator.MoveNext(); // skip (
				var logic = ConsumeExpression(enumerator, ")");
				enumerator.MoveNext(); // )
				var inner = ConsumeScope(enumerator, ScopeType.Other);
				return new IfExpression(value, logic, inner);
			}
			else if (value == "while")
			{
				enumerator.MoveNext(); // skip  while
				enumerator.MoveNext(); // skip (
				var logic = ConsumeExpression(enumerator, ")");
				enumerator.MoveNext(); // )
				var scope = ConsumeScope(enumerator, ScopeType.Function);
				return new WhileLoopExpression(logic, scope);
			}
			else if (value == "for") // (a;b;c)
			{
				enumerator.MoveNext();// skip for
				enumerator.MoveNext(); // skip (
															 //	var data = enumerator.ReadUntilValue(";").ToList();
				var initialization = ConsumeExpression(enumerator);
				enumerator.MoveNext(); // ;
				var logicalCheck = ConsumeExpression(enumerator);
				enumerator.MoveNext();// ;
				var incrementation = ConsumeExpression(enumerator, ")");
				enumerator.MoveNext(); // )
				var scope = ConsumeScope(enumerator, ScopeType.Function);
				return new ForLoopExpression(logicalCheck, initialization, incrementation, scope);
			}
			else if (value == "return")
			{
				enumerator.MoveNext(); // skip return
				var data = ConsumeExpression(enumerator);
				return new ReturnExpression(data);
			}

			throw new Exception();

		}

		//private KeywordExpression ConsumeKeyword(
		//	string value,
		//	Stack<AbstractExpression> stack,
		//	Stack<AbstractExpression> scopeStack,
		//	IEnumerator<Token> enumer)
		//{
		//	KeywordExpression expr = null;
		//	enumer.MoveNext(); // skip current keyword
		//	var innerScope = ConsumeNewScope(enumer, null);
		//	if (value == "namespace")
		//	{
		//		var namespaceIdentifier = stack.Pop();
		//		if (namespaceIdentifier is IdentifierExpression nms)
		//		{
		//			expr = new NamespaceExpression(value, nms.Name);
		//		}
		//		else
		//		{
		//			throw new ArgumentException();
		//		}
		//	}
		//	else if (value == "class")
		//	{
		//		var clasIdentifier = stack.Pop();
		//		if (clasIdentifier is IdentifierExpression cls)
		//		{
		//			expr = new ClassExpression(value, cls.Name);
		//		}
		//		else
		//		{
		//			throw new ArgumentException();
		//		}
		//	}
		//	else if (value == "if")
		//	{
		//		var logicalCondition = stack.Pop();
		//		expr = new IfExpression(value, logicalCondition);
		//	}
		//	else if (value == "else")
		//	{
		//		expr = new ElseExpression(value);
		//	}
		//	else if (value == "while")
		//	{
		//		var logicalCondition = stack.Pop();

		//		expr = new WhileLoop(value, logicalCondition);
		//	}
		//	else if (value == "for")
		//	{
		//		var incr = stack.Pop();
		//		var condition = scopeStack.Pop();
		//		var assignment = scopeStack.Pop();
		//		expr = new NamespaceExpression.ForLoop(value, condition, assignment, incr);
		//	}
		//	else
		//	{
		//		expr = new KeywordExpression(value);
		//	}
		//	expr.ScopeExpression = innerScope;
		//	return expr;
		//}
	}
}