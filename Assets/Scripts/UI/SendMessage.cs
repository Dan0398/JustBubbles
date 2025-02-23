using UnityEngine;

namespace Canvases.AnimUtilities
{
	public class SendMessage : StateMachineBehaviour
	{
		[System.Serializable]
		private enum Method
		{
			OnEnter,
			OnExit
		}
        
		[SerializeField] private Method _when;
		[SerializeField] private string _message;
		[SerializeField] private bool _direct;
		
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (_when != Method.OnEnter) return;
			if (_direct) animator.SendMessage(_message);
			else animator.SendMessageUpwards(_message);
		}
        
		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (_when != Method.OnExit) return;
			if (_direct) animator.SendMessage(_message);
			else animator.SendMessageUpwards(_message);
		}
	}
}