﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Runtime;

namespace Irony.Compiler.AST {
  public class UnExprNode : AstNode {
    public AstNode Arg;
    public string Op;
    CallDispatcher _dispatcher;


    public UnExprNode(NodeArgs args, string op, AstNode arg) : base(args) {
      ChildNodes.Clear();
      Op = op;
      if (!Op.EndsWith("U"))
        Op += "U"; //Unary operations are marked as "+U", "-U", "!U"
      Arg = arg;
      ChildNodes.Add(arg);
      //Flags |= AstNodeFlags.TypeBasedDispatch;
    }
    public UnExprNode(NodeArgs args) : this(args, args.ChildNodes[0].GetContent(), args.ChildNodes[1]) {  }

    public override void OnCodeAnalysis(CodeAnalysisArgs args) {
      switch (args.Phase) {
        case CodeAnalysisPhase.Binding:
          if (Op == "+U") 
            Evaluate = EvaluatePlus;
          else {
            _dispatcher = args.Context.Runtime.GetDispatcher(Op);
            Evaluate = EvaluateOther;
          }
          break;
      }//switch
      base.OnCodeAnalysis(args);
    }

    #region Evaluation methods
    private void EvaluatePlus(EvaluationContext context) {
      Arg.Evaluate(context);
    }
    private void EvaluateOther(EvaluationContext context) {
      Arg.Evaluate(context);
      context.Arg1 = context.CurrentResult;
      _dispatcher.Evaluate(context);
    }
    #endregion

    public override string ToString() {
      return Op + " (unary)";
    }
  }//class
}//namespace