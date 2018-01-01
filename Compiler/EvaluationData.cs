using System.Collections.Generic;

namespace Compiler
{
  public enum Associativity
  {
    Left,

    Right
  }

  public struct OperatorData
  {
    public OperatorData(int priority, Associativity associativity)
    {
      Priority = priority;
      Associativity = associativity;
    }

    public int Priority { get; set; }

    public Associativity Associativity { get; set; }
  }

  internal static class EvaluationData
  {
    public static readonly Dictionary<string, OperatorData> OperatorData = new Dictionary<string, OperatorData>
                                                                             {
                                                                               {
                                                                                 // { "~", new OperatorData(3,Associativity.Right) }, // TODO: unary minus?
                                                                                 "+",
                                                                                 new
                                                                                   OperatorData(
                                                                                     2,
                                                                                     Associativity
                                                                                       .Left)
                                                                               },
                                                                               {
                                                                                 "-",
                                                                                 new
                                                                                   OperatorData(
                                                                                     2,
                                                                                     Associativity
                                                                                       .Left)
                                                                               },
                                                                               {
                                                                                 "*",
                                                                                 new
                                                                                   OperatorData(
                                                                                     3,
                                                                                     Associativity
                                                                                       .Left)
                                                                               },
                                                                               {
                                                                                 "/",
                                                                                 new
                                                                                   OperatorData(
                                                                                     3,
                                                                                     Associativity
                                                                                       .Left)
                                                                               },
                                                                               {
                                                                                 "&",
                                                                                 new
                                                                                   OperatorData(
                                                                                     5,
                                                                                     Associativity
                                                                                       .Right)
                                                                               },
                                                                               {
                                                                                 "%",
                                                                                 new
                                                                                   OperatorData(
                                                                                     5,
                                                                                     Associativity
                                                                                       .Right)
                                                                               },
                                                                               {
                                                                                 "=",
                                                                                 new
                                                                                   OperatorData(
                                                                                     1,
                                                                                     Associativity
                                                                                       .Left)
                                                                               },
                                                                               {
                                                                                 "==",
                                                                                 new
                                                                                   OperatorData(
                                                                                     0,
                                                                                     Associativity
                                                                                       .Left)
                                                                               },
                                                                               {
                                                                                 "!=",
                                                                                 new
                                                                                   OperatorData(
                                                                                     0,
                                                                                     Associativity
                                                                                       .Left)
                                                                               },
                                                                               {
                                                                                 "<",
                                                                                 new
                                                                                   OperatorData(
                                                                                     0,
                                                                                     Associativity
                                                                                       .Left)
                                                                               },
                                                                               {
                                                                                 "<=",
                                                                                 new
                                                                                   OperatorData(
                                                                                     0,
                                                                                     Associativity
                                                                                       .Left)
                                                                               },
                                                                               {
                                                                                 ">=",
                                                                                 new
                                                                                   OperatorData(
                                                                                     0,
                                                                                     Associativity
                                                                                       .Left)
                                                                               },
                                                                               {
                                                                                 ">",
                                                                                 new
                                                                                   OperatorData(
                                                                                     0,
                                                                                     Associativity
                                                                                       .Left)
                                                                               }
                                                                             };
  }
}