#pragma warning disable CS0414, CS0649, CS1692
using System;
using System.Collections;
using System.Collections.Generic;
using Eliot.Environment;
using Eliot.Utility;
using UnityEngine;
using UnityEngine.Events;

namespace Eliot.AgentComponents
{
	/// <summary>
	/// <para>Control death of the Agent.</para>
	/// <para>Hold information about possible loot, ragdoll, animations etc.</para>
	/// </summary>
	[Serializable]
	public class AgentDeathHandler : AgentComponent
	{
		/// <summary>
		/// The way of processing the object upon death.
		/// </summary>
		public enum DeathType
		{
			Destroy,
			Deactivate
		}

		/// <summary>
		/// Return whether the Agent is dead.
		/// </summary>
		public bool IsDead
		{
			get { return _isDead; }
			set { _isDead = value; }
		}

		/// The way of processing the object upon death.
		public DeathType deathType;
		
		[Tooltip("Death animation will be initiated immediately. Ragdoll instantiation is delayed.")]
		public float deathDelay = 1.5f;

		/// <summary>
		/// Whether to spawn a rag doll upon death.
		/// </summary>
		public bool spawnRagdoll;

		/// Reference to an object which will be instantiated on Die().
		[SerializeField] private GameObject _ragdollPref;

		/// If true, Agent will drop all items from inventory upon death.
		[SerializeField] private bool _dropItems = true;

		/// <summary>
		/// Callback that's invoked on death.
		/// </summary>
		public UnityEvent onDeath;

		/// <summary>
		/// Callback that's invoked on respawn.
		/// </summary>
		public UnityEvent onRespawn;

		/// Sound that will be played upon Agent's death.
		[Space] [SerializeField] private List<AudioClip> _onDeathSounds;

		/// <summary>
		/// Reference to the object that handles agents respawn.
		/// </summary>
		public AgentsPoolHandler agentsPoolHandler;

		/// <summary>
		/// Stores the position at which the agent has been initialized the first time.
		/// </summary>
		private Vector3 _initialPosition;

		/// <summary>
		/// Stores the rotation at which the agent has been initialized the first time.
		/// </summary>
		private Quaternion _initialRotation;
		
		/// <summary>
		/// Return whether the Agent is dead.
		/// </summary>
		private bool _isDead = false;

		/// <summary>
		/// Whether the agent should be placed in a new random position or respawn at the same place every time.
		/// </summary>
		public bool newPositionOnRespawn = false;

		#region OptionalComponents

		/// <summary>
		/// Link to agent's perception component.
		/// </summary>
		private AgentPerception _agentPerception;

		/// <summary>
		/// Link to agent's animation component.
		/// </summary>
		private AgentAnimation _animation;

		/// <summary>
		/// Link to agent's inventory component.
		/// </summary>
		private AgentInventory _agentInventory;
		
		/// <summary>
		/// Link to agent's motion component.
		/// </summary>
		private AgentMotion _agentMotion;

		#endregion

		public AgentDeathHandler(EliotAgent agent) : base(agent)
		{
		}

		/// <summary>
		/// <para>Initialisation.</para>
		/// </summary>
		public override void Init(EliotAgent agent)
		{
			_agent = agent;
			if (!_ragdollPref)
				_ragdollPref = null;
			var wayPoints = _agent.WayPoints;
			if (wayPoints)
			{
				agentsPoolHandler = wayPoints.agentsPoolHandler;
			}

			_initialPosition = transform.position;
			_initialRotation = transform.rotation;

			_agentPerception = _agent.GetAgentComponent<AgentPerception>();
			_animation = _agent.GetAgentComponent<AgentAnimation>();
			_agentInventory = _agent.GetAgentComponent<AgentInventory>();
			_agentMotion = _agent.GetAgentComponent<AgentMotion>();
		}

		/// <summary>
		/// Reset the component to the initial state.
		/// </summary>
		public override void AgentReset()
		{
			_isDead = false;
			if(onRespawn != null)
				onRespawn.Invoke();
			transform.position = _initialPosition;
			transform.rotation = _initialRotation;
		}

		/// <summary>
		/// Cases to account for:
		/// 1. play an animation of death and then destroy the object
		/// 2. play an animation of death and then deactivate the object
		/// 3. destroy the object and then spawn a ragdoll
		/// 4. deactivate the object and then spawn a ragdoll
		/// </summary>
		public void Die()
		{
			if (_isDead) return;
			if(_agentMotion) _agentMotion.Lock();
			_agent.ForceStopSkill();
			
			StartCoroutine(DieEnum());
			
		}

		/// <summary>
		/// Animate dying and execute the rest of the associated actions after a delay.
		/// </summary>
		/// <returns></returns>
		private IEnumerator DieEnum()
		{
			AnimateDeath();
			_isDead = true;
			yield return new WaitForSeconds(deathDelay);
			
			switch (this.deathType)
			{
				case DeathType.Destroy:
				{
					DieDestroy();
					break;
				}

				case DeathType.Deactivate:
				{
					DieDeactivate();
					break;
				}
			}
			
			Agent.enabled = false;
		}

		/// <summary>
		/// Make Agent play animation of dying.
		/// </summary>
		private void AnimateDeath()
		{
			if (!_animation) return;
			_animation.Animate(AnimationState.Dying, force: true);
		}
		

		/// <summary>
		/// Perform the actions upon death that should be executed regardless of the mode.
		/// </summary>
		private void DieCommon()
		{
			if (spawnRagdoll && _ragdollPref)
			{
				var ragdoll = GameObject.Instantiate(_ragdollPref, _agent.transform.position, _agent.transform.rotation) as GameObject;
				if (_onDeathSounds.Count > 0)
				{
					var audioSource = ragdoll.GetComponent<AudioSource>()
						? ragdoll.GetComponent<AudioSource>()
						: ragdoll.AddComponent<AudioSource>();
					audioSource.PlayRandomClip(_onDeathSounds);
				}
			}

			if (_dropItems && _agentInventory)
			{
				_agentInventory.DropAllItems();
			}
			_isDead = true;

			if (onDeath != null)
				onDeath.Invoke();
		}

		/// <summary>
		/// <para>Spawn ragdoll, loot, etc. Log the event.</para>
		/// </summary>
		private void DieDestroy()
		{
			DieCommon();
			
			if (_agentPerception && _agent.GetDefaultTarget())
				GameObject.Destroy(_agent.GetDefaultTarget().gameObject);
			if (_animation && _animation.ApplyRootMotion)
				GameObject.Destroy(_animation.Animator.gameObject);
			
			GameObject.Destroy(_agent.gameObject);
		}

		/// <summary>
		/// Reuse the same object upon death.
		/// </summary>
		public void DieDeactivate()
		{
			DieCommon();
			if (agentsPoolHandler != null)
			{
				agentsPoolHandler.AddAgentToPool(GetComponent<EliotAgent>());
			}
		}

		/// <summary>
		/// This function is called when the component is added to an Agent.
		/// </summary>
		public override void OnAddComponent()
		{ 
			deathDelay = 1.5f;
		}
	}
}