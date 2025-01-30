using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Canvases.AnimUtilities{
	public class SendMessage : StateMachineBehaviour
	{
		[System.Serializable]
		enum Method
		{
			OnEnter,
			OnExit
		}
		
		[SerializeField] Method When;
		[SerializeField] string Message;
		[SerializeField] bool Direct;
		
		// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
		override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (When != Method.OnEnter) return;
			if (Direct) animator.SendMessage(Message);
			else animator.SendMessageUpwards(Message);
		}

		// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
		//override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		//{
		//    
		//}

		// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
		override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (When != Method.OnExit) return;
			if (Direct) animator.SendMessage(Message);
			else animator.SendMessageUpwards(Message);
		}

		// OnStateMove is called right after Animator.OnAnimatorMove()
		//override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		//{
		//    // Implement code that processes and affects root motion
		//}

		// OnStateIK is called right after Animator.OnAnimatorIK()
		//override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		//{
		//    // Implement code that sets up animation IK (inverse kinematics)
		//}
	}
}