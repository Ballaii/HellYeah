using System.Collections;
using System.Collections.Generic;
using Eliot.Environment;
using Eliot.Utility;
using UnityEngine;

namespace Eliot.AgentComponents
{
	/// <summary>
	/// Partial Skill class that holds functions that the Skill can perform.
	/// </summary>
    public partial class Skill
    {
        /// <summary>
		/// Initialize all the components.
		/// </summary>
		/// <param name="agent"></param>
		/// <param name="source"></param>
		/// <returns></returns>
		public Skill Init(EliotAgent agent, GameObject source)
		{
			_agent = agent;
			_sourceObject = source;
			_agentAnimation = _agent.GetAgentComponent<AgentAnimation>(); 
			_agentAgentPerception = _agent.GetAgentComponent<AgentPerception>(); 
			_agentResources = _agent.GetAgentComponent<AgentResources>(); 
			_agentAgentMotion = _agent.GetAgentComponent<AgentMotion>(); 
			state = SkillState.Idling; 
			return this; 
		}

		/// <summary>
		/// Trigger the execution of the skill.
		/// </summary>
		public object Invoke()
		{
			if (_skillAvailable)
			{
				var resources = _agent.GetAgentComponent<AgentResources>();
				if (resources)
				{
					if (!resources.CanHandle(resourcesCost)) return null;
					resources.Action(resourcesCost);
				}
				
				_agent.StartSkillCoroutine(INTERNAL_LoadEnum());
				_agent.CurrentSkill = this;
			}

			return null;
		}

		/// <summary>
		/// Prevent the skill from having its effects.
		/// </summary>
		public void Interrupt()
		{
			if (_interruptible)
			{
				_interrupted = true;
			}
		}

		public void ForceStop()
		{
			_agent.ForceStopSkill();
		}

		/// <summary>
		/// Let all Agents in specified radius hear suspitious noises from certain location.
		/// </summary>
		/// <param name="range"></param>
		/// <param name="transform"></param>
		/// <param name="duration"></param>
		public static void MakeNoise(float range, Transform transform, float duration)
		{
			Collider[] hitColliders = Physics.OverlapSphere(transform.position, range);
			foreach (var hitCollider in hitColliders)
			{
				var agent = hitCollider.GetComponent<EliotAgent>();
				if (!agent) continue;
				var perception = agent.GetAgentComponent<Eliot.AgentComponents.AgentPerception>();
				if (perception)
					perception.HearSomething(transform.position, duration);
			}
		}

		#region INTERNAL

		/// <summary>
		/// Animate Agent's graphics by legacy Animation Clip.
		/// </summary>
		/// <param name="clip"></param>
		/// <param name="fadeLength"></param>
		private void Animate(AnimationClip clip, float fadeLength = 0.3F)
		{
			if (!_agentAnimation) return;
			_agentAnimation.Animate(clip, fadeLength, true);
		}

		/// <summary>
		/// Set Agent's graphics Animator trigger.
		/// </summary>
		/// <param name="message"></param>
		private void Animate(string message)
		{
			if (!_agentAnimation) return;
			_agentAnimation.Animate(message);
		}

		/// <summary>
		/// Trigger Agent's graphics default animation by state.
		/// </summary>
		/// <param name="state"></param>
		/// <param name="fadeLength"></param>
		private void Animate(AnimationState state, float fadeLength = 0.3F)
		{
			if (!_agentAnimation) return;
			_agentAnimation.Animate(state, fadeLength, true);
		}

		/// <summary>
		/// Play animation of loading the skill.
		/// </summary>
		private void AnimateLoading()
		{
			if (!_agentAnimation || state != SkillState.Loading) return;
			if(_agentAnimation.AnimationMode == AnimationMode.Legacy) Animate(_loadingAnimation);
			else if (_agentAnimation.AnimationMode == AnimationMode.Mecanim)
			{
				if(!string.IsNullOrEmpty(_loadingMessage))
					_agentAnimation.Animate(_loadingMessage, _loadTime);
			}
			else Animate(AnimationState.LoadingSkill);
		}
		
		/// <summary>
		/// Play animation of executing the skill.
		/// </summary>
		private void AnimateInvoking()
		{
			if (!_agentAnimation) return;
			if(_agentAnimation.AnimationMode == AnimationMode.Legacy) Animate(_executingAnimation);
			else if (_agentAnimation.AnimationMode == AnimationMode.Mecanim)
			{
				if (!string.IsNullOrEmpty(_executingMessage))
				{
					_agentAnimation.Animate(_executingMessage, _invocationDuration + _invocationPing + _effectDuration + _effectPing);
				}
			}
			else Animate(AnimationState.UsingSkill);
		}
		
		#endregion
		
		#region LISTENERS
		
		/// <summary>
		/// Create all the necessary special effects upon starting to load the skill.
		/// </summary>
		/// <param name="source"></param>
		private void OnApplyEffectOnInit(GameObject source)
		{
			if (_onApplyFx)
			{
				var fx = Instantiate(_onApplyFx) as GameObject;
				var shootOriginAttribute = _agent["ShootOrigin"];
				var shootOrigin = shootOriginAttribute != null ? shootOriginAttribute.objectValue as Transform : _agent.transform;
				fx.transform.position = shootOrigin ? 
					shootOrigin.position : _agent.GetAgentComponent<Eliot.AgentComponents.AgentPerception>().Origin ? 
						_agent.GetAgentComponent<Eliot.AgentComponents.AgentPerception>().Origin.position : _sourceObject.transform.position;
				if (_makeChildOfTarget) fx.transform.parent = _sourceObject.transform;
			}
			if (_makeNoise) MakeNoise(_range, _sourceObject.transform, _noiseDuration);
			if(_onApplyMessageToCaster.Length > 0)
				source.SendMessage(_onApplyMessageToCaster, SendMessageOptions.DontRequireReceiver);
		}
		
		/// <summary>
		/// Create all the necessary special effects upon applying the skill's effects.
		/// </summary>
		/// <param name="target"></param>
		private void OnApplyEffect(GameObject target)
		{
			if (_onApplyFxOnTarget)
			{
				var fx = Instantiate(_onApplyFxOnTarget) as GameObject;
				fx.transform.position = target.transform.position;
				if (_makeChildOfTarget) fx.transform.parent = target.transform;
			}
			if(_agent.Target && _onApplyMessageToTarget.Length > 0)
				_agent.Target.SendMessage(_onApplyMessageToTarget, SendMessageOptions.DontRequireReceiver);
		}
		#endregion
		
		#region INTERNAL ENUMS
		
		/// <summary>
		/// Start loading the skill.
		/// </summary>
		/// <returns></returns>
		private IEnumerator INTERNAL_LoadEnum()
		{
			_agent.GetAudioSource().PlayRandomClip(_loadingSkillSounds);
			
			state = SkillState.Loading;
			var motion = _agent.GetAgentComponent<Eliot.AgentComponents.AgentMotion>();
			if(_freezeMotion && motion) motion.Lock();
			AnimateLoading();
			_skillAvailable = false;
			yield return new WaitForSeconds(_loadTime + UnityEngine.Random.Range(-LoadTimeError, LoadTimeError));
			if (!_interrupted) _agent.StartSkillCoroutine(INTERNAL_InvokeEnum());
			else {_interrupted = false; _agent.StartSkillCoroutine(INTERNAL_CoolDownEnum()); }
		}
		
		/// <summary>
		/// Start applying skill's effects to the target or targets.
		/// </summary>
		/// <returns></returns>
		private IEnumerator INTERNAL_InvokeEnum()
		{
			_agent.GetAudioSource().PlayRandomClip(_executingSkillSounds);
			
			state = SkillState.Invoking;
			AnimateInvoking();
			var beginTime = Time.time;
			if(_invocationPing >= _invocationDuration) _agent.StartCoroutine(INTERNAL_ApplyEffectEnum());
			else while (Time.time <= beginTime + _invocationDuration)
			{
				_agent.StartCoroutine(INTERNAL_ApplyEffectEnum());
				/*if (_loadingAnimation && _executingAnimation && !_loadingAnimation.Equals(_executingAnimation)) 
					AnimateInvoking();*/
				yield return new WaitForSeconds(_invocationPing);
			}
			_agent.StartSkillCoroutine(INTERNAL_CoolDownEnum());
		}
		
		/// <summary>
		/// Wait untill skill is recharged to be able to use it again.
		/// </summary>
		/// <returns></returns>
		private IEnumerator INTERNAL_CoolDownEnum()
		{
			state = SkillState.CoolDown;
			if(!_freezeMotionOnCooldown && _freezeMotion && _agentAgentMotion) _agentAgentMotion.Unlock();
			yield return new WaitForSeconds(_coolDown);
			if(_freezeMotionOnCooldown && _freezeMotion && _agentAgentMotion) _agentAgentMotion.Unlock();
			_skillAvailable = true;
			state = SkillState.Idling;

			if (_agentAnimation && _agentAnimation.Animator)
			{
				if (_agentAnimation.AnimationMode == AnimationMode.Mecanim)
				{
					if (_loadingMessage.Length > 0)
						_agentAnimation.Animator.SetBool(_loadingMessage, false);
					if (_executingMessage.Length > 0)
						_agentAnimation.Animator.SetBool(_executingMessage, false);
				}
			}
		}

		/// <summary>
		/// Implementation of finding the target.
		/// </summary>
		/// <returns></returns>
		private IEnumerator INTERNAL_ApplyEffectEnum()
		{
			// Create the loading special effects.
			OnApplyEffectOnInit(_sourceObject);
			
			// Cast the ray to find the target
			if (_initSkillBy == InitSkillBy.Ray)
			{
				//Try find target agent
				var targetDir = _agent.Target
					? _agent.Target.position - _agent.transform.position
					: _agentAgentPerception ? AgentPerception.InitRay(_range, 0, 90, _agentAgentPerception.Origin) : _agent.transform.forward;
				var agentAimFov = _agent["AimFieldOfView"];
				var fov = (_fieldOfView > 0) ? _fieldOfView :
					(agentAimFov != null) ? _agent["AimFieldOfView"].floatValue : _fieldOfView;
				if (Vector3.Angle(targetDir, _agentAgentPerception ? _agentAgentPerception.Origin.forward : _agent.transform.forward) >= fov) yield break;
				RaycastHit hit;
				var shootOrigin = _agent.SkillOrigin;
				if (!Physics.Raycast(shootOrigin!=null ? shootOrigin.position : _agent.transform.position, targetDir, out hit, _range)) yield break;
				var agent = hit.transform.gameObject.GetComponent<EliotAgent>();
				
				var unit = hit.transform.gameObject.GetComponent<Unit>();
				if (unit)
				{
					var targetIsFriend = _agent.Unit.IsFriend(unit);
					if (targetIsFriend && !_canAffectFriends) yield break;
					if (!targetIsFriend && !_canAffectEnemies) yield break;
				}

				//Apply effects to him
				if (agent) agent.AddEffect(this, _agent);
				else INTERNAL_ApplyEffectNonAgent(hit.transform.gameObject); 
			}
			
			// Create a Projectile and pass the skill to it.
			else if (_initSkillBy == InitSkillBy.Projectile)
			{
				var shootOrigin = _agent.SkillOrigin;
				if (!_projectilePrefab)
				{
					Debug.Log("Projectile reference is missing. (Skill: " + name + ")");
				}
				else
				{
					var pjtl =
						Instantiate(_projectilePrefab, shootOrigin.position, shootOrigin.rotation) as EliotProjectile;
					pjtl.Init(_agent, _agent.Target, this, _minPower, _maxPower, _canAffectEnemies, _canAffectFriends);
				}
			}
			
			// Apply the skill to oneself.
			else if (_initSkillBy == InitSkillBy.Self)
			{
				_agent.AddEffect(this, _agent);
			}
			
			// Apply effects to a target directly by a reference to the GameObject.
			else if (_initSkillBy == InitSkillBy.Direct)
			{
				if (!_agent.Target) yield return null;
				var agent = _agent.Target.GetComponent<EliotAgent>();
				if (agent)
				{
					var unit = _agent.Target.GetComponent<Unit>();
					if (unit)
					{
						var targetIsFriend = _agent.Unit.IsFriend(unit);
						if (targetIsFriend && !_canAffectFriends) yield break;
						if (!targetIsFriend && !_canAffectEnemies) yield break;
					}
					
					agent.AddEffect(this, _agent);
				}
				else INTERNAL_ApplyEffectNonAgent(_agent.Target.gameObject); 
			}
			
			// Find all appropriate targets in cpecified radius and apply effects to them.
			else if (_initSkillBy == InitSkillBy.Radius)
			{
				var agents = FindObjectsOfType<EliotAgent>();
				foreach (var agent in agents)
				{
					var targetIsFriend = _agent.Unit.IsFriend(agent.Unit);
					if (targetIsFriend && !_canAffectFriends) continue;
					if (!targetIsFriend && !_canAffectEnemies) continue;
					
					if(Vector3.Distance(agent.transform.position, _agent.transform.position) <= _range
					   && agent != _agent)
						agent.AddEffect(this, _agent);
				}
			}

			// Create special effects on target after applying the effects.
			if (_agent.Target)
			{
				var agent = _agent.Target.GetComponent<EliotAgent>();
				if (agent)
				{
					var agentMotion = agent.GetAgentComponent<AgentMotion>();
					if (agentMotion && _lookAtTarget)
					{
						agentMotion.Engine.LookAtTarget(agent.GetDefaultTarget().position);
					}
					var targetIsFriend = _agent.Unit.IsFriend(agent.Unit);
					if (targetIsFriend && !_canAffectFriends) yield break;
					if (!targetIsFriend && !_canAffectEnemies) yield break;
				}

				//OnApplyEffect(_agent.Target.gameObject);
			}
		}
		
		/// <summary>
		/// Apply skill's effects over specific duration.
		/// </summary>
		/// <param name="agent"></param>
		/// <param name="effectsList"></param>
		/// <returns></returns>
		public IEnumerator ApplyEffectEnum(EliotAgent agent, List<Skill> effectsList = null)
		{
			var beginTime = Time.time;
			if(agent && _effectPing >= _effectDuration) INTERNAL_ApplyEffect(agent);
			else if(agent) while (Time.time <= beginTime + _effectDuration)
			{
				INTERNAL_ApplyEffect(agent);
				yield return new WaitForSeconds(_effectPing);
			}

			if (effectsList != null) effectsList.Remove(this);
		}
		
		/// <summary>
		/// Apply effects to the target changing its parameters.
		/// </summary>
		/// <param name="targetAgent"></param>
		private void INTERNAL_ApplyEffect(EliotAgent targetAgent)
		{
			if (!targetAgent || !_agent) return;
			
			OnApplyEffect(targetAgent.gameObject);
			var hitPower = UnityEngine.Random.Range(_minPower, _maxPower + 1);
			
			if(_interruptTarget) targetAgent.Interrupt();
			var agentResources = targetAgent.GetAgentComponent<AgentResources>();
			if (agentResources)
			{
				agentResources.Action(resourcesActions);
			}

			if (_pushPower > 0)
			{
				var targetMotion = targetAgent.GetAgentComponent<Eliot.AgentComponents.AgentMotion>();
				if(targetMotion)
					targetMotion.Push(_agent.transform.position, hitPower*_pushPower);
			}

			if (_setStatus) targetAgent.SetStatus(_status, _statusDuration);

			if (_setPositionAsTarget)
			{
				var targetPerception = targetAgent.GetAgentComponent<Eliot.AgentComponents.AgentPerception>();
				var tmp = targetPerception ? targetPerception.AgentMemory.GetDefaultTarget(_agent.transform.position) : targetAgent.Unit;
				targetAgent.Target = tmp.transform;
			}

			if (_additionalEffects.Count <= 0) return;
			foreach (var effect in _additionalEffects)
				if(effect) targetAgent.AddEffect(effect, _agent);
		}
		
		/// <summary>
		/// If the target is not an Agent, invoke specified methods on it and create special effects.
		/// </summary>
		/// <param name="target"></param>
		private void INTERNAL_ApplyEffectNonAgent(GameObject target)
		{
			if (!target || !_agent) return;
			
			if (_onApplyFxOnTarget)
			{
				var fx = Instantiate(_onApplyFxOnTarget);
				fx.transform.position = target.transform.position;
				if (_makeChildOfTarget) fx.transform.parent = target.transform;
			}

			if (_passPowerToTarget)
			{
				var hitPower = UnityEngine.Random.Range(_minPower, _maxPower + 1); 
				target.SendMessage(_onApplyMessageToTarget, hitPower, SendMessageOptions.DontRequireReceiver);
			}
			else if (_onApplyMessageToTarget.Length > 0)
			{
				target.SendMessage(_onApplyMessageToTarget, SendMessageOptions.DontRequireReceiver);
			}

			if (_tryPassDamageToNonAgent)
			{
				IntegrationDamageHandler.PassDamage(target, UnityEngine.Random.Range(_minPower, _maxPower + 1), _agent.gameObject, _pushPower);
			} 
		}
		#endregion
    }
}