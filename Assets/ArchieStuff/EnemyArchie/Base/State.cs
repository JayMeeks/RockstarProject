using System;
using UnityEngine;
using UnityEngine.AI;

namespace ArchieStuff.EnemyArchie
{
   
    public interface IState
    {
        public  void Enter();
        public  void Tick();
        public  void Exit();
    }
    public abstract class State : IState
    {
        protected EnemyStateController controller;
        protected NavMeshAgent agent;
        
        protected State(EnemyStateController controller, NavMeshAgent agent) 
        { 
            this.controller = controller; 
            this.agent = agent; 
        } 
        
        public abstract void Enter();
        public abstract void Tick();
        public abstract void Exit();
    }
}