#pragma warning disable CS0414, CS0649, CS1692
using System;
using System.Collections;
#if ASTAR_EXISTS
using Pathfinding;
#endif
using UnityEngine;
using UnityEngine.AI;

namespace Eliot.AgentComponents
{
	/// <summary>
	/// Encapsulates the control over Agent's Animation or Animator
	/// components depending on the current state of the Agent.
	/// </summary>
	[Serializable]
	public class AgentAnimation : AgentComponent
	{
		/// <summary>
		/// Animation engine.
		/// </summary>
		public AnimationMode AnimationMode
		{
			get { return _animationMode; }
			set { _animationMode = value; }
		}

		/// <summary>
		/// Reference to the Legacy Animation component.
		/// </summary>
		public UnityEngine.Animation LegacyAnimation
		{
			get { return _animation; }
			set { _animation = value; }
		}

		/// <summary>
		/// Reference to the Animator component.
		/// </summary>
		public Animator Animator
		{
			get { return _animator; }
			set { _animator = value; }
		}

		/// <summary>
		/// Container for the Animation clips associated with certain actions.
		/// </summary>
		public LegacyAnimationClips Clips
		{
			get { return _clips; }
			set { _clips = value; }
		}

		/// <summary>
		/// List of the parameters names which the component uses to interact with the Animator.
		/// </summary>
		public AnimatorParameters Parameters
		{
			get { return _parameters; }
			set { _parameters = value; }
		}

		/// <summary>
		/// Whether to apply Animator rootMotion or not.
		/// </summary>
		public bool ApplyRootMotion
		{
			get { return _applyRootMotion; }
			set { _applyRootMotion = value; }
		}
		
		/// <summary>
		/// Whether to use rootMotion animations for rotations.
		/// </summary>
		public bool RootMotionRotation
		{
			get { return _rootMotionRotation; }
			set { _rootMotionRotation = value; }
		}

		/// Animation engine.
		[SerializeField] private AnimationMode _animationMode;
		
		#region LEGACY PROPERTIES
		[Tooltip("Reference to the Legacy Animation component. Leave empty if " +
		         "you are not using legacy animations on Agent's graphics.")]
		[SerializeField] private UnityEngine.Animation _animation;
		[SerializeField] private LegacyAnimationClips _clips = new LegacyAnimationClips();
		#endregion
		
		#region ANIMATOR PROPERTIES
		[Tooltip("Reference to the Animator component. Leave empty if " +
		         "you are not using Animator on Agent's graphics.")]
		[SerializeField] private Animator _animator;
		[Tooltip("Use this if you want this Agent to utilize Animator's root" +
		         " motion for moving rather than just one of motion engines.")]
		[SerializeField] private bool _applyRootMotion;
		[SerializeField] private bool _rootMotionRotation;
		[Tooltip("If you use rootmotion, agent's graphics will be detached from " +
		         "the agent itself in the hierarchy. Set this toggle to true if" +
		         " you want to change graphics' name to be able to see more clearly" +
		         " which Agent does it belong to.")]
		[SerializeField] private bool _changeGraphicsName;
		[SerializeField] private AnimatorParameters _parameters = new AnimatorParameters();
		#endregion
		
		#region ANIMATOR ROOTMOTION DATA

		private Vector3 _groundNormal;
		private float _groundCheckDistance = 1f;
		private float _turnAmount;
		private float _forwardAmount;
		private float _stationaryTurnSpeed = 1f;
		private float _movingTurnSpeed = 10f;
		
		#endregion

		#region Optional Components

		private AgentDeathHandler _agentDeathHandler;

		#endregion
		
		public AgentAnimation(EliotAgent agent) : base(agent)
		{
		}

		/// <summary>
		/// This function gets called when the Agent is selected in the Editor.
		/// </summary>
		/// <param name="agent"></param>
		public override void AgentOnEnable(EliotAgent agent)
		{
			Init(agent);
		}

		/// <summary>
		/// Initialize the component
		/// </summary>
		/// <param name="agent"></param>
		public override void Init(EliotAgent agent)
		{
			_agent = agent;

			_agentDeathHandler = _agent.GetAgentComponent<AgentDeathHandler>();
			
			if (!_animation)
			{
				_animation = Agent.transform.GetComponentInChildren<Animation>();
			}
			
			if (!_animator)
			{
				_animator = Agent.transform.GetComponentInChildren<Animator>();
			}
			
			if (_animator)
			{
				_animationMode = AnimationMode.Mecanim;
				_applyRootMotion = _animator.applyRootMotion;
			}
			
			// Detach the graphics if Agent uses rootmotion.
			if (Application.isPlaying && AnimationMode == AnimationMode.Mecanim && _applyRootMotion)
			{
				_animator.gameObject.transform.parent = _agent.transform.parent;
				if (_changeGraphicsName)
					_animator.gameObject.name = "__graphics__" + "[" + _agent.gameObject.name + "]";
			}
		}

		/// <summary>
		/// Reset the Agent Component to the initial state.
		/// </summary>
		public override void AgentReset()
		{
			Animate(AnimationState.Idling);
		}

		/// <summary>
		/// This method gets called every Agent's update. Once every frame by default.
		/// </summary>
		public void Update()
		{
			if (_agentDeathHandler && _agentDeathHandler.IsDead) return;

			if (_animation && AnimationMode == AnimationMode.Legacy && !_animation.isPlaying)
				Animate(AnimationState.Idling);

			if (_animator && AnimationMode == AnimationMode.Mecanim && _applyRootMotion)
			{
				var remainingDistance = Vector3.Distance(_agent.transform.position, _agent.Target.position);
				var stoppingDistance = 0f;
				var desiredVelocity = Vector3.zero;
#if ASTAR_EXISTS
				var agentMotion = _agent.GetAgentComponent<AgentMotion>();
				if (agentMotion)
				{
					desiredVelocity = agentMotion.Type == MotionEngine.NavMesh
						? _agent.GetComponent<NavMeshAgent>().desiredVelocity
						: agentMotion.Type == MotionEngine.Astar
							? _agent.GetComponent<IAstarAI>().desiredVelocity
							: Vector3.zero;
					stoppingDistance = agentMotion.Type == MotionEngine.NavMesh
						? _agent.GetComponent<NavMeshAgent>().stoppingDistance
						: agentMotion.Type == MotionEngine.Astar
							? _agent.GetComponent<AIPath>().endReachedDistance
							: 0f;
				}
#else
				desiredVelocity = _agent.GetAgentComponent<AgentMotion>().Type == MotionEngine.NavMesh
					? _agent.GetComponent<NavMeshAgent>().desiredVelocity
					: Vector3.zero;
				stoppingDistance = _agent.GetAgentComponent<AgentMotion>().Type == MotionEngine.NavMesh
					? _agent.GetComponent<NavMeshAgent>().stoppingDistance
					: 0f;
#endif
				if (remainingDistance > stoppingDistance
				    && _agent.GetComponent<NavMeshAgent>().updatePosition
				    && _agent.GetComponent<NavMeshAgent>().updateRotation
				    && _agent.GetAgentComponent<AgentMotion>().PathIsValid())
				{
					var playerInput = _agent.GetAgentComponent<AgentPlayerInput>();
					if ( (playerInput && !playerInput.motionAccelerationActiveted) || _agent.GetAgentComponent<AgentMotion>().State == MotionState.Walking)
					{
						desiredVelocity = desiredVelocity.normalized * 0.5f;
					}
					
					RootmotionMove(desiredVelocity);
				}
				else
					RootmotionMove(Vector3.zero);

				_agent.transform.position = new Vector3(_animator.transform.position.x, _agent.transform.position.y,
					_animator.transform.position.z);
				_agent.transform.rotation = _animator.transform.rotation;

				_agent.transform.position = _agent.GetComponent<NavMeshAgent>().nextPosition;
				_animator.transform.position = _agent.GetComponent<NavMeshAgent>().nextPosition;

				if (_animator.transform.position.y != _agent.transform.position.y)
					_animator.transform.position +=
						new Vector3(0, _agent.transform.position.y - _animator.transform.position.y, 0);
			}
		}

		/// <summary>
		/// Execute legacy animation or trigger Animator depending on the Agent's Animation Mode.
		/// </summary>
		/// <param name="clip">Legacy Animation Clip.</param>
		/// <param name="fadeLength">Period of time over which animation clip will fade.</param>
		/// <param name="animatorTrigger">Sets the value of the given Animator parameter.</param>
		/// <param name="force">Legacy Animation Clip.</param>
		private void AnimateByMode(AnimationClip clip, float fadeLength, string animatorTrigger, bool force = false)
		{
			if (_agentDeathHandler && _agentDeathHandler.IsDead) return;
			
			if (_animationMode == AnimationMode.Legacy)
				Animate(clip, fadeLength, force);
			else if(!string.IsNullOrEmpty(animatorTrigger))
				Animate(animatorTrigger);
		}

		/// <summary>
		/// Execute legacy animation.
		/// </summary>
		/// <param name="clip">Legacy Animation Clip.</param>
		/// <param name="fadeLength">Period of time over which animation clip will fade.</param>
		/// <param name="force">Legacy Animation Clip.</param>
		public void Animate(AnimationClip clip, float fadeLength = 0.3F, bool force = false)
		{
			if (_agentDeathHandler && _agentDeathHandler.IsDead) return;
			
			if (_animation && clip)
			{
				if (force) _animation.Stop();
				_animation.CrossFade(clip.name, fadeLength);
			}
		}

		/// <summary>
		/// Execute animation by state using default animations.
		/// </summary>
		/// <param name="state">State which dictates what Animation clip
		/// or what Animator message exactly to use.</param>
		/// <param name="fadeLength">Period of time over which animation clip will fade.</param>
		/// <param name="force">Legacy Animation Clip.</param>
		public void Animate(AnimationState state, float fadeLength = 0.3F, bool force = false)
		{
			if (_agentDeathHandler && _agentDeathHandler.IsDead)
			{
				AnimateByMode(_clips.Death, fadeLength, _parameters.DeathBool, true);
				return;
			}
			
			switch (state)
			{
				case AnimationState.Idling:
					AnimateByMode(_clips.Idle, fadeLength, null, force);
					break;
				case AnimationState.Walking:
					AnimateByMode(_clips.Walk, fadeLength, null, force);
					break;
				case AnimationState.Running:
					AnimateByMode(_clips.Run, fadeLength, null, force);
					break;
				case AnimationState.Dodging:
					AnimateByMode(_clips.Dodge, fadeLength, _parameters.DodgeBool, force);
					break;
				case AnimationState.TakingDamage:
					AnimateByMode(_clips.TakeDamage, fadeLength, _parameters.GetHitBool, force);
					break;
				case AnimationState.LoadingSkill:
					AnimateByMode(_clips.LoadSkill, fadeLength, _parameters.LoadSkillBool, force);
					break;
				case AnimationState.UsingSkill:
					AnimateByMode(_clips.UseSkill, fadeLength, _parameters.ExecuteSkillBool, force);
					break;
				case AnimationState.Dying:
					AnimateByMode(_clips.Death, fadeLength, _parameters.DeathBool, force);
					break;
			}
		}

		#region ANIMATOR

		/// <summary>
		/// Trigger Animator by message.
		/// </summary>
		/// <param name="animatorBool">Sets the value of the given Animator parameter.</param>
		/// <param name="duration"></param>
		public void Animate(string animatorBool, float duration = 0.1f)
		{
			if (_agentDeathHandler && _agentDeathHandler.IsDead) return;

			if (_animator)
			{
				if (duration < 0.1f) duration = 0.1f;
				_animator.SetBool(animatorBool, true);
				_agent.StartCoroutine(ResetBoolEnum(animatorBool, duration));
			}
		}

		/// <summary>
		/// Make sure Agent's animation does not stuck.
		/// </summary>
		/// <param name="animatorBool"></param>
		/// <param name="duration"></param>
		/// <returns></returns>
		private IEnumerator ResetBoolEnum(string animatorBool, float duration)
		{
			yield return new WaitForSeconds(duration);
			_animator.SetBool(animatorBool, false);
		}

		/// <summary>
		/// Animate Agent's motion, taking into consideration current Agent's Motion State.
		/// </summary>
		/// <param name="state"></param>
		public void SetSpeed(MotionState state)
		{
			if (_applyRootMotion) return;
			var animatorSpeed = 0f;
			switch (state)
			{
				case MotionState.Walking: case MotionState.WalkingAway:
					animatorSpeed = 0.4f;
					break;
				case MotionState.Running: case MotionState.RunningAway:
					animatorSpeed = 1f;
					break;
			}
			if(_animator)
				_animator.SetFloat(_parameters.Vertical, animatorSpeed);
		}
		
		/// <summary>
		/// Update the Animator parameters based upon Agent's input.
		/// This method is based on Unity Standard Assets' Third person character.
		/// </summary>
		/// <param name="move"></param>
		private void RootmotionMove(Vector3 move)
		{
			// convert the world relative moveInput vector into a local-relative
			// turn amount and forward amount required to head in the desired
			// direction.
			if (move.magnitude > 1f) move.Normalize();
			move = _agent.transform.InverseTransformDirection(move);
			CheckGroundStatus();
			move = Vector3.ProjectOnPlane(move, _groundNormal);
			_turnAmount = Mathf.Atan2(move.x, move.z);
			_forwardAmount = move.z;

			var turnSpeed = Mathf.Lerp(_stationaryTurnSpeed, _movingTurnSpeed, _forwardAmount);
			_agent.transform.Rotate(0, _turnAmount * turnSpeed * Time.deltaTime, 0);

			_animator.SetFloat(_parameters.Vertical, _forwardAmount, 0.1f, Time.deltaTime);
			_animator.SetFloat(_parameters.Horizontal, _turnAmount, 0.1f, Time.deltaTime);
		}
		
		/// <summary>
		/// Update the Animator parameters based upon Agent's input.
		/// This method is based on Unity Standard Assets' Third person character.
		/// </summary>
		private void CheckGroundStatus()
		{
			RaycastHit hitInfo;
#if UNITY_EDITOR
			// helper to visualise the ground check ray in the scene view
			Debug.DrawLine(_agent.transform.position + (Vector3.up * 0.1f),
				_agent.transform.position + (Vector3.up * 0.1f) + (Vector3.down * _groundCheckDistance));
#endif
			// 0.1f is a small offset to start the ray from inside the character
			Ray ray = new Ray(_agent.transform.position + (Vector3.up * 0.5f), Vector3.down);
			if (Physics.Raycast(ray, out hitInfo, _groundCheckDistance))
			{
				_groundNormal = hitInfo.normal;
			}
			else
			{
				_groundNormal = Vector3.up;
			}
		}
		

		#endregion
	}
}