#pragma warning disable CS0414, CS0649, CS1692
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Eliot.BehaviourEditor;
using Eliot.BehaviourEngine;
using Eliot.Environment;
using Eliot.Utility;
using Eliot.Repository;
#if ASTAR_EXISTS
using Pathfinding;
using Pathfinding.RVO;
#endif

using UnityEngine;
using Action = System.Action;
using Unit = Eliot.Environment.Unit;

namespace Eliot.AgentComponents
{
	/// <summary>
	/// <para>Agent encapsulates all the components of the behaviour of AI and works
	/// as a driver for the Behaviour model defined by user.</para>
	/// </summary>
	[Serializable]
	[RequireComponent(typeof(Unit))]
	public partial class EliotAgent : MonoBehaviour
	{
		#region PROPERTIES
		/// <summary>
		/// The Behaviour model used by this Agent.
		/// </summary>
		public EliotBehaviour Behaviour
		{
			get { return _behaviour; }
			set { _behaviour = value; }
		}

		/// <summary>
		/// Behaviour driver that is built from Behaviour model diagram.
		/// </summary>
		public BehaviourCore BehaviourCore
		{
			get { return _behaviourCore; }
			set { _behaviourCore = value; }
		}

		/// <summary>
		/// The Unit component of this gameObject.
		/// </summary>
		public Unit Unit
		{
			get
			{
				if (!_unit)
					_unit = GetComponent<Unit>();
				return _unit;
			}
			set { _unit = value; }
		}

		/// <summary>
		/// Current target. Coincides with this gameObject's NavMeshAgent target.
		/// </summary>
		public Transform Target
		{
			get
			{
				if (Application.isPlaying)
				{
					if (!_target)
					{
						_target = GetDefaultTarget();
					}
				}

				return _target;
			}
			set { _target = value; }
		}

		/// <summary>
		/// Used to calculate at which distance is it possible to interact with agent.
		/// Especially noticable for large Agents.
		/// </summary>
		public float Radius
		{
			get
			{
				return _capsuleCollider ? _capsuleCollider.radius : 0.5f;
			}
		}

		/// <summary>
		/// How much time in seconds passes between updates.
		/// </summary>
		public float Ping
		{
			get { return _ping; }
			set { _ping = value; }
		}

		/// <summary>
		/// Current or last used skill.
		/// </summary>
		public Skill CurrentSkill
		{
			get { return _currentSkill; }
			set { _currentSkill = value; }
		}

		/// <summary>
		/// Set Agent inert when you want to use Agent's API, but dont want
		/// him to influence the behabiour of the game object.
		/// </summary>
		public bool AiEnabled
		{
			get { return _aiEnabled; }
			set { _aiEnabled = value; }
		}

		/// <summary>
		/// Set true if you want this Agent to apply changes to his attributes as
		/// defined by each waypoint he steps onto.
		/// </summary>
		public bool ApplyChangesOnWayPoints
		{
			get { return _applyChangesOnWayPoints; }
			set { _applyChangesOnWayPoints = value; }
		}

		/// <summary>
		/// Current Agent's _status. What _status actually influences totally depends on
		/// the user's definition of it in the behaviour model.
		/// </summary>
		public AgentStatus Status
		{
			get { return _status; }
			set { _status = value; }
		}

		/// <summary>
		/// Use to restrict walkable area, to set desired target places one by one
		/// or to pool agents in desired area.
		/// </summary>
		public WayPointsGroup WayPoints
		{
			get { return wayPoints; }
			set { wayPoints = value; }
		}
		
		/// <summary>
		/// The initial list of skill that the Agent is able to use.
		/// </summary>
		public List<Skill> Skills
		{
			get { return _skills; }
			set { _skills = value; }
		}

		/// <summary>
		/// If true then information about current skill is shown as
		/// Handles text near selected Agent.
		/// </summary>
		public bool DebugSkill
		{
			get { return _debugSkill; }
			set { _debugSkill = value; }
		}

		/// <summary>
		/// If true then information about Agent's WaypointsGroup is shown
		/// as Handles text near selected Agent.
		/// </summary>
		public bool DebugWaypoints
		{
			get { return _debugWaypoints; }
			set { _debugWaypoints = value; }
		}

		/// <summary>
		/// If true then information about current Agent's target is shown
		/// as Handles text near selected Agent.
		/// </summary>
		public bool DebugTarget
		{
			get { return _debugTarget; }
			set { _debugTarget = value; }
		}
		
		/// <summary>
		/// The transform that is used to cast rays from when using Skills.
		/// </summary>
		public Transform SkillOrigin
		{
			get
			{
				if (!_skillOrigin)
					_skillOrigin = GetComponent<EliotAgent>().GetSkillOrigin();
				return _skillOrigin;
			}
			set { _skillOrigin = value; }
		}

		/// <summary>
		/// Agent's default graphics. Agent's graphics is set to defualt when
		/// an Item from Inventory is unwield.
		/// </summary>
		private List<GameObject> DefaultGraphics { get; set; }

		/// <summary>
		/// Default perception origin position.
		/// </summary>
		private Vector3 DefaultPerceptionOriginPosition { get; set; }

		/// <summary>
		/// Default perception origin rotation.
		/// </summary>
		private Quaternion DefaultPerceptionOriginRotation { get; set; }

		/// <summary>
		/// Default skill origin position.
		/// </summary>
		private Vector3 DefaultSkillOriginPosition { get; set; }

		/// <summary>
		/// Default skill origin rotation.
		/// </summary>
		private Quaternion DefaultSkillOriginRotation { get; set; }

		/// <summary>
		/// Gets the list of the Agent Components that are currently added to the Agent.
		/// </summary>
		public List<AgentComponent> AgentComponents
		{
			get { return GetComponents<AgentComponent>().ToList(); }
		}
		
		#endregion

		#region FIELDS

		[Tooltip("Set Agent inert when you want to use Agent's API " +
		         "but dont want him to influence the behabiour of the game object.")]
		[SerializeField]
		private bool _aiEnabled = true;

		[Space] [Tooltip("The Behaviour model used by this Agent.")] [SerializeField]
		private EliotBehaviour _behaviour;

		[Tooltip("How much time in seconds passes between updates.")] [SerializeField]
		private float _ping;

		[Tooltip("Use to restrict walkable area, to set desired target " +
		         "places one by one or to pool agents in desired area.")]
		[SerializeField]
		private WayPointsGroup wayPoints;

		[Tooltip("Set true if you want this Agent to apply changes to his " +
		         "attributes as defined by each waypoint he steps onto.")]
		[SerializeField]
		private bool _applyChangesOnWayPoints = true;

		[Tooltip("Skills that the Agent can use.")] [SerializeField]
		private List<Skill> _skills = new List<Skill>();

		[Tooltip("Skills that are being applied to Agent. (Buffs / DoTs etc.)")] [SerializeField]
		private List<Skill> _activeEffects = new List<Skill>();

		[Space]
		[Tooltip("If true then information about current skill is shown as " +
		         "Handles text near selected Agent.")]
		[SerializeField]
		private bool _debugSkill = true;

		[Tooltip("If true then information about Agent's WaypointsGroup is " +
		         "shown as Handles text near selected Agent.")]
		[SerializeField]
		private bool _debugWaypoints = true;

		[Tooltip("If true then information about current Agent's target is " +
		         "shown as Handles text near selected Agent.")]
		[SerializeField]
		private bool _debugTarget = true;
		
		[Tooltip("The transform that is used to cast rays from when using Skills.")]
		[SerializeField] private Transform _skillOrigin = null;
		
		/// Current target. Coincides with this gameObject's NavMeshAgent target.
		private Transform _target;
		
		/// Link to the Unit component.
		private Unit _unit;

		/// Last WayPoint that's been an Agent's target except the default dummy one.
		public EliotWayPoint curWayPoint;
		
		/// Return whether the Agent is executing an EliotWayPoint action.
		public bool isDoingAction = false;

		/// Last time the agent's behaviour was updated.
		private float _lastPingUpdate;

		/// Skills that the Agent can use.
		private List<Skill> _usableSkills = new List<Skill>();

		/// Current or last used skill.
		private Skill _currentSkill;

		/// Current Agent's _status.
		private AgentStatus _status = AgentStatus.Normal;

		/// Behaviour driver that is built from Behaviour model diagram.
		private BehaviourCore _behaviourCore;
		
		/// Store the used attributes to call them faster the second time and on.
		private Dictionary<string, Attribute> attributesCache = new Dictionary<string, Attribute>();
		
		/// Link to the Capsule Collider.
		private CapsuleCollider _capsuleCollider;
		
		/// Make sure Agent's Motion never misses a target.
		private Transform _defaultTarget;
		
		/// List of all the interfaces API of which is utilized by the Agent.
		protected List<EliotInterface> _interfaces = new List<EliotInterface>();
		
		/// Store the used Agent Components to call them faster the second time and on.
		protected Dictionary<Type, AgentComponent> _agentComponents = new Dictionary<Type, AgentComponent>();

		private IEnumerator _currentSkillCoroutine;
		#endregion

		public void StartSkillCoroutine(IEnumerator iEnumerator)
		{
			_currentSkillCoroutine = iEnumerator;
			StartCoroutine(iEnumerator);
		}
		
		public void ForceStopSkill()
		{
			if(_currentSkillCoroutine != null)
				StopCoroutine(_currentSkillCoroutine);
		}

		/// <summary>
		/// Removes all the agent components before being removed.
		/// </summary>
		private void OnDestroy()
		{
			var agentComponents = gameObject.GetComponents<AgentComponent>();
			for (int i = agentComponents.Length - 1; i >= 0; i--)
			{
				if (agentComponents[i])
						Destroy(agentComponents[i]);
			}
		}

		/// <summary>
		/// Returns the requested Agent component. Uses cache so works faster compared to GetComponent.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public void SetAgentComponent<T>(T clipboard) where T : AgentComponent
		{
			var type = typeof(T);
			if(_agentComponents.ContainsKey(type))
			{
				_agentComponents[type] = clipboard;
			}

			for (int i = 0; i < AgentComponents.Count; i++)
			{
				if (AgentComponents[i].GetType().Name == typeof(T).Name)
				{
					AgentComponents[i] = clipboard;
					break;
				}
			}
		}

		#region MONOBEHAVIOUR_BASED_GETSET
		/// <summary>
		/// Returns the requested Agent component. Uses cache so works faster compared to GetComponent.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public T GetAgentComponent<T>() where T : AgentComponent
		{
#if !UNITY_EDITOR
			var type = typeof(T);
			if(_agentComponents.ContainsKey(type))
			{
				return _agentComponents[type] as T;
			}
#endif
			var component = GetComponent<T>();
			if (component)
			{
				if(!_agentComponents.ContainsKey(typeof(T)))
					_agentComponents.Add(typeof(T), component);
				return component;
			}
			return null;
		}
		
		/// <summary>
		/// Adds an Agent Component and immediately stores it in the cache.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public T AddAgentComponent<T>() where T : AgentComponent
		{
			var comp = gameObject.GetComponent<T>();
			if (!comp)
			{
				comp = gameObject.AddComponent<T>();
#if !UNITY_EDITOR
				_agentComponents.Add(typeof(T), comp);
#endif
			}
			comp.OnAddComponent();
			comp.Init(gameObject.GetComponent<EliotAgent>());
			return comp;
		}

		/// <summary>
		/// Remove an Agent Component of a given type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		public void RemoveComponent<T>() where T : AgentComponent
		{
			if(_agentComponents.ContainsKey(typeof(T)))
				_agentComponents.Remove(typeof(T));
			for (int i = AgentComponents.Count - 1; i >= 0; i--)
			{
				if (AgentComponents[i].GetType().Name == typeof(T).Name)
				{
					DestroyImmediate(AgentComponents[i]);
				}
			}
		}
		
		/// <summary>
		/// Remove an Agent Component of a given type.
		/// </summary>
		public void RemoveComponent(AgentComponent component)
		{
			var type = component.GetType();
			if(_agentComponents.ContainsKey(type))
				_agentComponents.Remove(type);
			for (int i = AgentComponents.Count - 1; i >= 0; i--)
			{
				if (AgentComponents[i].GetType().Name == type.Name)
				{
					DestroyImmediate(AgentComponents[i]);
				}
			}
		}
		
		#endregion

		/// <summary>
		/// Make all the Agent Components as they were at initialization.
		/// </summary>
		public void AgentReset()
		{
			foreach (var component in _agentComponents.Values)
				component.AgentReset();
		}

		/// <summary>
		/// Enable Agent to use new skill.
		/// </summary>
		/// <param name="skill"></param>
		public void AddSkill(Skill skill)
		{
			if (!_skills.Contains(skill))
				_skills.Add(skill.Clone().Init(GetComponent<EliotAgent>(), gameObject));
			if (!_usableSkills.Contains(skill))
				_usableSkills.Add(skill.Clone().Init(GetComponent<EliotAgent>(), gameObject));
		}

		/// <summary>
		/// Prevent Agent from using certain skill.
		/// </summary>
		/// <param name="skillToRemove"></param>
		public void RemoveSkill(Skill skillToRemove)
		{
			foreach (var skill in _skills)
				if (skill.Matches(skillToRemove))
					_skills.Remove(skill);
			foreach (var skill in _usableSkills)
				if (skill.Matches(skillToRemove))
					_usableSkills.Remove(skill);
		}

		/// <summary>
		/// Add effect (DoT/Buff etc.) to Agent.
		/// </summary>
		/// <param name="effect"></param>
		/// <param name="caster"></param>
		public void AddEffect(Skill effect, EliotAgent caster)
		{
			var contains = false;
			foreach (var skl in _activeEffects)
				if (skl.name == effect.name)
				{
					contains = true;
					break;
				}

			if (contains) return;

			var clone = effect.Clone().Init(caster != null ? caster : this, gameObject);
			_activeEffects.Add(clone);
			StartCoroutine(clone.ApplyEffectEnum(GetComponent<EliotAgent>(), _activeEffects));
		}

		/// <summary>
		/// Initialize the usable skills from the scriptable templates.
		/// </summary>
		public void InitSkills()
		{
			var agent = GetComponent<EliotAgent>();
			_usableSkills = new List<Skill>();
			foreach (var skill in _skills)
				if(skill)
					_usableSkills.Add(skill.Clone().Init(agent, gameObject));
		}
		
		/// <summary>
		/// Get the Transform component of the default target object.
		/// </summary>
		/// <returns></returns>
		public Transform GetDefaultTarget()
		{
			if (!_defaultTarget)
			{
				var newGo = new GameObject("__target__[" + name + "]__");
				newGo.transform.position = transform.position;
				var waypoint = newGo.AddComponent<EliotWayPoint>();
				waypoint.automatic = true;
				var unit = newGo.GetComponent<Unit>();
				unit.Type = Environment.UnitType.Agent;
				unit.Team = Unit.Team;
				_defaultTarget = waypoint.transform;
			}
			return _defaultTarget;
		}

		/// <summary>
		/// Move the default target object to specific position and get its Transform component.
		/// </summary>
		/// <param name="pos"></param>
		/// <returns></returns>
		public Transform GetDefaultTarget(Vector3 pos)
		{
			if (!_defaultTarget)
			{
				var newGo = new GameObject("__target__[" + name + "]__");
				newGo.transform.position = pos;
				var waypoint = newGo.AddComponent<EliotWayPoint>();
				waypoint.automatic = true;
				var unit = newGo.AddComponent<Unit>();
				unit.Type = Environment.UnitType.Agent;
				_defaultTarget = waypoint.transform;
				
			}

			_defaultTarget.position = pos;
			return _defaultTarget;
		}

		/// <summary>
		/// Initialize Agent on scene load.
		/// </summary>
		private void Start()
		{
			Unit = GetComponent<Unit>();
			var agent = GetComponent<EliotAgent>();

			InitSkills();
				
			_capsuleCollider = GetComponent<CapsuleCollider>();
			_agentComponents = new Dictionary<Type, AgentComponent>();
			foreach (var component in gameObject.GetComponents<AgentComponent>())
				_agentComponents.Add(component.GetType(), component);
			foreach (var component in _agentComponents.Values)
				component.Init(agent);

			DefaultGraphics = new List<GameObject>();
			var container = this.GetGraphicsContainer();
			if (container.childCount > 0)
				for (var i = container.childCount - 1; i >= 0; i--)
					DefaultGraphics.Add(container.GetChild(i).gameObject);

			DefaultPerceptionOriginPosition = this.GetPerceptionOrigin().localPosition;
			DefaultPerceptionOriginRotation = this.GetPerceptionOrigin().localRotation;

			DefaultSkillOriginPosition = this.GetSkillOrigin().localPosition;
			DefaultSkillOriginRotation = this.GetSkillOrigin().localRotation;

			if (!_behaviour) return;
			_behaviourCore = CoresPool.GetCore(_behaviour, gameObject);
		}

		/// <summary>
		/// Update components logic and Behaviour states every frame.
		/// </summary>
		public void Update()
		{
			if (_behaviourCore == null && _behaviour)
			{
				Debug.Log("Failed to initialize Behavior Core.", gameObject);
			}

			if (Time.time >= _lastPingUpdate + _ping)
			{
				try
				{
					if(_aiEnabled)
						_behaviourCore.Update();
				}
				catch(Exception){}

				foreach (var component in _agentComponents.Values)
					component.AgentUpdate();

				if (Target)
				{
					var targetWayPoint = Target.GetComponent<EliotWayPoint>();
					if (targetWayPoint && !targetWayPoint.automatic)
					{
						curWayPoint = targetWayPoint;
					}

					var actionDistance = curWayPoint ? curWayPoint.ActionDistance : 0.5f;

					if (curWayPoint && Vector3.Distance(transform.position, curWayPoint.transform.position) <= actionDistance)
					{
						StartWayPointAction(curWayPoint);
					}
				}

				_lastPingUpdate = Time.time;
			}
		}

		/// <summary>
		/// Update components that work with physics.
		/// </summary>
		private void FixedUpdate()
		{
			if (Time.time < _lastPingUpdate + _ping) return;
			foreach (var component in _agentComponents.Values)
				component.AgentFixedUpdate();
		}

		/// <summary>
		/// Get the Invoke method of a skill if Agent has such.
		/// Return nothing if no such skill was found.
		/// </summary>
		/// <param name="skillName"></param>
		/// <param name="execute"></param>
		/// <returns></returns>
		public Action Skill(string skillName, bool execute)
		{
			if (execute)
			{
				foreach (var skill in _usableSkills)
					if (skill.name == skillName)
					{
						_currentSkill = skill;
						return () =>
						{
							if (Target && _skillOrigin && skill._adjustOriginRotation)
							{
								Vector3 targetPoint;
								var targetCollider = Target.GetComponent<Collider>();
								if (targetCollider)
								{
									targetPoint = targetCollider.ClosestPoint(_skillOrigin.position);
								}
								else
								{
									targetPoint = Target.position;
								}
								_skillOrigin.LookAt(targetPoint);
							}
							skill.Invoke();
						};
					}
			}
			else
			{
				return () =>
				{
					foreach (var skill in _usableSkills)
						if (skill.name == skillName)
						{
							_currentSkill = skill;
						}
				};
			}

			return null;
		}

		/// <summary>
		/// Forces the Agent to use a skill with a given name if it is present in the skills list.
		/// </summary>
		/// <param name="skillName"></param>
		public void ExecuteSkill(string skillName)
		{
			Skill(skillName, true)();
		}
		
		/// <summary>
		/// Forces the Agent to use a given skill if it is present in the skills list.
		/// </summary>
		/// <param name="skill"></param>
		public void ExecuteSkill(Skill skill)
		{
			if (!skill) return;
			if (!_skills.Contains(skill))
			{
				_skills.Add(skill);
				InitSkills();
			}
			Skill(skill.name, true)();
		}

		/// <summary>
		/// Interrupt the execution of a skill.
		/// </summary>
		public void Interrupt()
		{
			if (_currentSkill)
				_currentSkill.Interrupt();
		}

		/// <summary>
		/// Assign new Behaviour model to Agent.
		/// </summary>
		/// <param name="behaviour"></param>
		public void SetBehaviour(EliotBehaviour behaviour)
		{
			try{
				_behaviour = behaviour;
				_behaviourCore = CoresPool.GetCore(_behaviour, gameObject);
			}catch(Exception){/**/}
		}

		/// <summary>
		/// Apply changes to Agent component according to Waypoint's settings.
		/// </summary>
		/// <param name="wayPoint"></param>
		public void StartWayPointAction(EliotWayPoint wayPoint)
		{
			// But do it only if both WayPoint and Agent agree to do the change.
			if (isDoingAction) return;
			isDoingAction = true;

			wayPoint.StartAction(GetComponent<EliotAgent>());
		}

		/// <summary>
		/// Replace the Agent's graphics, keeping proper references to components if needed.
		/// </summary>
		/// <param name="newGraphics"></param>
		public void ReplaceGraphics(GameObject newGraphics)
		{
			// Clear the old graphics.
			var container = GetComponent<EliotAgent>().GetGraphicsContainer();
			if (DefaultGraphics.Count > 0)
				foreach (var graphics in DefaultGraphics)
					graphics.SetActive(false);

			// Instantiate a new one
			var inst = Instantiate(newGraphics, container.position, container.rotation) as GameObject;
			inst.transform.parent = container;
			// Look for an AgentGraphics component on a new graphics gameObject
			var agentGraphics = inst.GetComponent<AgentGraphics>();
			if (!agentGraphics) return; // If there is none, job is finished
			
			// Recover the reference to Animation or Animator component.
			var agentAnimation = GetAgentComponent<AgentAnimation>();
			if (agentAnimation)
			{
				if (agentAnimation.AnimationMode == AnimationMode.Legacy)
					agentAnimation.LegacyAnimation = agentGraphics.Animation;
				else agentAnimation.Animator = agentGraphics.Animator;
			}

			// Set a new transform of the spell caster origin
			if (agentGraphics.NewShooterPosition)
			{
				SkillOrigin.localPosition = agentGraphics.NewShooterPosition.localPosition;
				SkillOrigin.localRotation = agentGraphics.NewShooterPosition.localRotation;
			}

			// Set a new transform of the perception origin
			var agentPerception = GetAgentComponent<AgentPerception>();
			if (agentPerception && agentGraphics.NewPerceptionOriginPosition)
			{
				agentPerception.Origin.localPosition = agentGraphics.NewPerceptionOriginPosition.localPosition;
				agentPerception.Origin.localRotation = agentGraphics.NewPerceptionOriginPosition.localRotation;
			}

			// Invoke any custom method for more complex changes in Agent's graphics
			if (agentGraphics.SendMessageOnChange)
			{
				if (!string.IsNullOrEmpty(agentGraphics.MethodParams))
					gameObject.SendMessage(agentGraphics.MethodName, agentGraphics.MethodParams,
						SendMessageOptions.DontRequireReceiver);
				else
					gameObject.SendMessage(agentGraphics.MethodName,
						SendMessageOptions.DontRequireReceiver);
			}
		}

		/// <summary>
		/// Set all settings that are related to graphics
		/// and internal objects positions to default.
		/// </summary>
		public void ResetGraphics()
		{
			var container = this.GetGraphicsContainer();
			if (container.childCount > 0)
				for (var i = container.childCount - 1; i >= 0; i--)
					if (!DefaultGraphics.Contains(container.GetChild(i).gameObject))
						Destroy(container.GetChild(i).gameObject);

			foreach (var graphics in DefaultGraphics)
				graphics.SetActive(true);

			var perception = GetAgentComponent<AgentPerception>();
			if (perception)
			{
				perception.Origin.localPosition = DefaultPerceptionOriginPosition;
				perception.Origin.localRotation = DefaultPerceptionOriginRotation;
			}

			SkillOrigin.localPosition = DefaultSkillOriginPosition;
			SkillOrigin.localRotation = DefaultSkillOriginRotation;
		}

		/// <summary>
		/// Set a new _status to the Agent for a specific duration.
		/// </summary>
		/// <param name="stat"></param>
		/// <param name="statusDuration"></param>
		public void SetStatus(AgentStatus stat, float statusDuration)
		{
			_status = stat;
			StartCoroutine(ResetStatusIn(statusDuration));
		}

		/// <summary>
		/// Reset _status back to normal.
		/// </summary>
		/// <param name="time"></param>
		/// <returns></returns>
		private IEnumerator ResetStatusIn(float time)
		{
			yield return new WaitForSeconds(time);
			_status = AgentStatus.Normal;
		}

		/// <summary>
		/// Damage the Agent after, for example, raycast check.
		/// </summary>
		/// <param name="power"></param>
		public void Damage(int power)
		{
			var agentResources = GetAgentComponent<AgentResources>();
			if (!agentResources) return;
			agentResources.Action(new ResourceAction(AgentResources.DefaultHealthResourceName, ResourceAffectionWay.Reduce, power));
		}

		/// <summary>
		/// Damage the Agent after, for example, raycast check.
		/// </summary>
		/// <param name="power"></param>
		public void Damage(float power)
		{
			var agentResources = GetAgentComponent<AgentResources>();
			if (!agentResources) return;
			agentResources.Action(new ResourceAction(AgentResources.DefaultHealthResourceName, ResourceAffectionWay.Reduce, Mathf.RoundToInt(power)));
		}
		
		/// <summary>
		/// Access an Agent's attribute by name. Return null if no attribute found.
		/// </summary>
		/// <param name="attributeName"></param>
		public Attribute this[string attributeName]
		{
			get
			{
				if (attributesCache.ContainsKey(attributeName))
				{
					return attributesCache[attributeName];
				}

				foreach (var attributesGroup in Unit.attributesGroups)
				{
					var attribute = attributesGroup[attributeName];
					if (attribute != null)
					{
						attributesCache.Add(attributeName, attribute);
						return attribute;
					}
				}

				return null;
			}
		}
		
		/// <summary>
		/// Return the int value of an attribute or a default value on failure to fetch it.
		/// </summary>
		/// <param name="attributeName"></param>
		/// <param name="defaultValue"></param>
		public int this[string attributeName, int defaultValue]
		{
			get
			{
				var result = defaultValue;
				var attribute = this[attributeName];
				if (attribute != null) result = attribute.intValue;
				return result;
			}
		}
		
		/// <summary>
		/// Return the float value of an attribute or a default value on failure to fetch it.
		/// </summary>
		/// <param name="attributeName"></param>
		/// <param name="defaultValue"></param>
		public float this[string attributeName, float defaultValue]
		{
			get
			{
				var result = defaultValue;
				var attribute = this[attributeName];
				if (attribute != null) result = attribute.floatValue;
				return result;
			}
		}
		
		/// <summary>
		/// Return the boolean value of an attribute or a default value on failure to fetch it.
		/// </summary>
		/// <param name="attributeName"></param>
		/// <param name="defaultValue"></param>
		public bool this[string attributeName, bool defaultValue]
		{
			get
			{
				var result = defaultValue;
				var attribute = this[attributeName];
				if (attribute != null) result = attribute.boolValue;
				return result;
			}
		}
		
		/// <summary>
		/// Return the string value of an attribute or a default value on failure to fetch it.
		/// </summary>
		/// <param name="attributeName"></param>
		/// <param name="defaultValue"></param>
		public string this[string attributeName, string defaultValue]
		{
			get
			{
				var result = defaultValue;
				var attribute = this[attributeName];
				if (attribute != null) result = attribute.stringValue;
				return result;
			}
		}
		
		/// <summary>
		/// Return the UnityEngine.Object value of an attribute or a default value on failure to fetch it.
		/// </summary>
		/// <param name="attributeName"></param>
		/// <param name="defaultValue"></param>
		public UnityEngine.Object this[string attributeName, UnityEngine.Object defaultValue]
		{
			get
			{
				var result = defaultValue;
				var attribute = this[attributeName];
				if (attribute != null) result = attribute.objectValue;
				return result;
			}
		}

		/// <summary>
		/// Add an interface for Agent to be able to use its methods at runtime.
		/// </summary>
		/// <param name="className"></param>
		/// <returns></returns>
		public virtual EliotInterface AddInterface(string className)
		{
			Type type = null;
			try
			{
				type = Assembly.GetAssembly(typeof(EliotAgent)).GetType(className);
			}
			catch(Exception e){Debug.Log(e);}
			
			var ctor = type.GetConstructor(new[] { typeof(EliotAgent) });
			var eliotInterface = ctor.Invoke(
				new object[]
				{
					GetComponent<EliotAgent>()
				}) as EliotInterface;
			_interfaces.Add(eliotInterface);
			return eliotInterface;
		}
	}
}
