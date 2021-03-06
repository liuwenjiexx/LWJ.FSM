﻿using LWJ.FSM.Model;
using System;
using System.Collections.Generic;

namespace LWJ.FSM
{
    public class FSMExecutionContext
    {

        //private FSMExecutionContext parent;

        private FSMContext context; 

        private FSMachine machine;
        private TransitionalTarget state;
        public FSMEvent Event { get => machine.CurrentEvent; }
        private bool stateChanged;


        public FSMExecutionContext(FSMachine machine, TransitionalTarget state, FSMContext context)
        {
            this.machine = machine ?? throw new ArgumentNullException(nameof(machine));
            this.state = state ?? throw new ArgumentNullException(nameof(state));
            this.context = context ?? throw new ArgumentNullException(nameof(context));

        }


        public FSMContext Context => context;

        public TransitionalTarget State => state;

        public FSMachine StateMachine => machine;



        public bool EvalExpressionBool(object boolExpr)
        {
            object result = EvalExpression(boolExpr);
            if (result == null)
                return false;
            if (result is bool)
                return (bool)result;
            return false;
        }

        public virtual object EvalExpression(object expr)
        {
            
            return context.ExpressionProvider.EvalExpression(context, expr);
        }


        //public void ChangeChildState(string name)
        //{
        //    FSMState toState = state.GetChildState(name);
        //    FSMState fromState = state.Current;
        //    if (fromState != null)
        //    {
        //        if (fromState == toState)
        //            return;
        //        fromState.OnExit(this);
        //    }

        //    if (toState != null)
        //    {
        //        toState.OnEntry(this);
        //    }

        //    machine.OnTransition(fromState, toState);

        //}


        public void Reset()
        {
            //if (Context.paramerts != null)
            //{
            //    foreach (var item in Context.paramerts.Values)
            //    {
            //        if (item.ParamInfo.DefaultValueExpr != null)
            //            item.value = EvalExpression(item.ParamInfo.DefaultValueExpr);
            //        else
            //            item.value = item.ParamInfo.DefaultValue;
            //    }
            //}
            Context.Reset();
        }


        public void ExecuteAction(IEnumerable<Model.Action> actions)
        {
            if (actions == null)
                return;
            foreach (var action in actions)
                ExecuteAction(action);
        }

        public void ExecuteAction(Model.Action action)
        {
            if (action == null)
                return;
            action.Execute(this);
        }

        //public FSMExecutionContext GetExecutionContext(TransitionalTarget state)
        //{
        //    if(childs==null)
        //        throw new 
        //}

    }

}
