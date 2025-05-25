#pragma warning disable CS0414, CS0649, CS0612, CS1692
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Eliot.AgentComponents
{
	/// <summary>
	/// Skill encapsulates all the ways Agents can affect each other.
	/// Each skill is saved as a separate file.
	/// </summary>
	[Serializable][CreateAssetMenu(fileName = "New Skill", menuName = "Eliot AI/Skill")]
	public partial class Skill : ScriptableObject
	{
		[Header("Basic")]
		[Tooltip("Can the loading of this skill be interrupted by other skills?")]
		public bool _interruptible = true;
		[Tooltip("Can multiple identical skills affect the same Agent at the same time?")]
		public bool _canStack;
		public bool _canAffectEnemies = true;
		public bool _canAffectFriends = true;
		public bool _adjustOriginRotation = true;
		public InitSkillBy _initSkillBy;
		[Tooltip("Leave empty if Skill is not initialized by a projectile.")]
		public EliotProjectile _projectilePrefab;

		[Header("Area")]
		[Tooltip("The distance to the target(s) at which the skill will have an effect.")]
		public float _range;
		[Tooltip("The angle (starting at the agent's forward direction) at which the skill will have an effect on the target(s).")]
		public float _fieldOfView;

		[Header("Timing")]
		public float _loadTime;
		public float _invocationDuration;
		public float _invocationPing;
		public float _effectDuration;
		public float _effectPing;
		public float _coolDown;
		
		public int _minPower;
		public int _maxPower;
		public float _pushPower;
		[Tooltip("Should this skill lock the target Agent?")]
		public bool _interruptTarget;
		[Tooltip("Should Agent lock itself when casting the skill?")]
		public bool _freezeMotion;
		[Tooltip("Should Agent lock itself when casting the skill?")]
		public bool _freezeMotionOnCooldown;
		[Tooltip("Should Agent rotate towards the target upon using the skill?")]
		public bool _lookAtTarget;
		[Tooltip("These actions will be applied to the caster upon the Skill invocation.")]
		public List<ResourceAction> resourcesCost = new List<ResourceAction>();
		
		public List<ResourceAction> resourcesActions = new List<ResourceAction>();
		[Space] 
		[Tooltip("Should the Skill change target's _status?")]
		public bool _setStatus;
		public AgentStatus _status;
		public float _statusDuration;
		[Space] 
		[Tooltip("Lets target know about casting position.")]
		public bool _setPositionAsTarget;
		[Space] 
		public bool _makeNoise;
		public float _noiseDuration;
		[Tooltip("Side skills that are cast upon target as a consequence of casting the main one.")]
		public List<Skill> _additionalEffects = new List<Skill>();

		[Header("Legacy Animations")]
		public AnimationClip _loadingAnimation;
		public AnimationClip _executingAnimation;
		[Header("Animator")]
		public string _loadingMessage;
		public string _executingMessage;

		[Header("Misc")] 
		[Tooltip("Special effects Instantiated at the casting position.")]
		public GameObject _onApplyFx;
		[Tooltip("Special effects Instantiated at target position.")]
		public GameObject _onApplyFxOnTarget;
		public bool _makeChildOfTarget;
		[Space]
		[Tooltip("Invoke any method on target upon applying the skill.")]
		public string _onApplyMessageToTarget;
		[Tooltip("Pass a random value between min power and max power as" +
		         " a parameter of the above mentioned method.")]
		public bool _passPowerToTarget;
		public bool _tryPassDamageToNonAgent;
		[Tooltip("Invoke any method on the caster Agent upon casting the skill.")]
		public string _onApplyMessageToCaster;
		
		[Header("Audio")] 
		public List<AudioClip> _loadingSkillSounds;
		public List<AudioClip> _executingSkillSounds;
		
		/// A link to actual controller. Caster of the skill.
		private EliotAgent _agent;
		/// Agent's Animation component.
		private AgentAnimation _agentAnimation;
		private AgentPerception _agentAgentPerception;
		private AgentResources _agentResources;
		private AgentMotion _agentAgentMotion;
		
		/// Current state of the skill.
		public SkillState state;

		/// Whether the skill is available for use.
		private bool _skillAvailable = true;
		/// Whether the skill is interrupted.
		private bool _interrupted;
		/// Caster Agent's gameObject.
		private GameObject _sourceObject;
		/// Makes sure Agents with the same _loadTime can both attack each other.
		private const float LoadTimeError = 0.075f;
		
		/// <summary>
		/// Make a non-file copy of the Skill.
		/// </summary>
		/// <returns></returns>
		public Skill Clone()
		{
			var result = CreateInstance<Skill>();
			result._canStack = _canStack;
			result._canAffectEnemies = _canAffectEnemies;
			result._canAffectFriends = _canAffectFriends;
			result._adjustOriginRotation = _adjustOriginRotation;
			result._initSkillBy = _initSkillBy;
			result._projectilePrefab = _projectilePrefab;
			result.name = name;
			result._interruptible = _interruptible;
			result._interrupted = _interrupted;
			result._range = _range;
			result._fieldOfView = _fieldOfView;
			result._loadTime = _loadTime;
			result._invocationDuration = _invocationDuration;
			result._invocationPing = _invocationPing;
			result._effectDuration = _effectDuration;
			result._effectPing = _effectPing;
			result._coolDown = _coolDown;
			result._minPower = _minPower;
			result._maxPower = _maxPower;
			result.resourcesCost = resourcesCost;
			result._pushPower = _pushPower;
			result._interruptTarget = _interruptTarget;
			result._freezeMotion = _freezeMotion;
			result._freezeMotionOnCooldown = _freezeMotionOnCooldown;
			result._lookAtTarget = _lookAtTarget;
			result._setStatus = _setStatus;
			result._status = _status;
			result._statusDuration = _statusDuration;
			result._setPositionAsTarget = _setPositionAsTarget;
			result._makeNoise = _makeNoise;
			result._noiseDuration = _noiseDuration;
			result.resourcesActions = resourcesActions;
			
			result._loadingAnimation = _loadingAnimation;
			result._executingAnimation = _executingAnimation;
			result._loadingMessage = _loadingMessage;
			result._executingMessage = _executingMessage;
			result._skillAvailable = _skillAvailable;
			result.state = state;
			result._onApplyFx = _onApplyFx;
			result._onApplyFxOnTarget = _onApplyFxOnTarget;
			result._makeChildOfTarget = _makeChildOfTarget;
			result._onApplyMessageToTarget = _onApplyMessageToTarget;
			result._passPowerToTarget = _passPowerToTarget;
			result._tryPassDamageToNonAgent = _tryPassDamageToNonAgent;
			result._onApplyMessageToCaster = _onApplyMessageToCaster;
			result._additionalEffects = _additionalEffects;
			result._loadingSkillSounds = _loadingSkillSounds;
			result._executingSkillSounds = _executingSkillSounds;
			
#if UNITY_EDITOR
			result.editorAdvancedMode = editorAdvancedMode;
#endif

			return result;
		}
		
		/// <summary>
		/// Does this skill match the other one?
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public bool Matches(Skill other)
		{
			return
				_canStack == other._canStack
				&& other._canAffectEnemies == _canAffectEnemies
				&& other._canAffectFriends == _canAffectFriends
				&& other._adjustOriginRotation == _adjustOriginRotation
				&& other._initSkillBy == _initSkillBy
				&& other._projectilePrefab == _projectilePrefab
				&& other.name == name
				&& other._interruptible == _interruptible
				&& other._interrupted == _interrupted
				&& other._range == _range
				&& other._fieldOfView == _fieldOfView
				&& other._loadTime == _loadTime
				&& other._invocationDuration == _invocationDuration
				&& other._invocationPing == _invocationPing
				&& other._effectDuration == _effectDuration
				&& other._effectPing == _effectPing
				&& other._coolDown == _coolDown
				&& other._minPower == _minPower
				&& other._maxPower == _maxPower
				&& other.resourcesCost == resourcesCost
				&& other._pushPower == _pushPower
				&& other._interruptTarget == _interruptTarget
				&& other._freezeMotion == _freezeMotion
				&& other._freezeMotionOnCooldown == _freezeMotionOnCooldown
				&& other._lookAtTarget == _lookAtTarget
				&& other._setStatus == _setStatus
				&& other._status == _status
				&& other._statusDuration == _statusDuration
				&& other._setPositionAsTarget == _setPositionAsTarget
				&& other._makeNoise == _makeNoise
				&& other._noiseDuration == _noiseDuration
				&& other.resourcesActions == resourcesActions

				&& other._loadingAnimation == _loadingAnimation
				&& other._executingAnimation == _executingAnimation
				&& other._loadingMessage == _loadingMessage
				&& other._executingMessage == _executingMessage
				&& other._skillAvailable == _skillAvailable
				&& other.state == state
				&& other._onApplyFx == _onApplyFx
				&& other._onApplyFxOnTarget == _onApplyFxOnTarget
				&& other._makeChildOfTarget == _makeChildOfTarget
				&& other._onApplyMessageToTarget == _onApplyMessageToTarget
				&& other._passPowerToTarget == _passPowerToTarget
				&& other._tryPassDamageToNonAgent == _tryPassDamageToNonAgent
				&& other._onApplyMessageToCaster == _onApplyMessageToCaster
				&& other._additionalEffects == _additionalEffects
				&& other._loadingSkillSounds == _loadingSkillSounds
				&& other._executingSkillSounds == _executingSkillSounds;
		}
	}
}