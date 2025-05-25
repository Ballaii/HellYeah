#pragma warning disable CS0649, CS1692
using System;
using System.Collections.Generic;
using Eliot.Utility;
using UnityEngine;

namespace Eliot.AgentComponents
{
    /// <summary>
    /// Encapsulates Agent's abilities to move around in space.
    /// </summary>
    [Serializable]
    public class AgentMotion : AgentComponent
    {
        #region PROPERTIES
        /// <summary>
        /// Radius is used to calculate the distance for interaction.
        /// </summary>
        public float Radius
        {
            get
            {
                return _agent.Radius;
            }
        }

        /// <summary>
        /// Destination for Agent's movement.
        /// </summary>
        public Vector3 Target
        {
            get { return _motionEngine.GetTarget(); }
            set { _motionEngine.SetTarget(value); }
        }

        /// <summary>
        /// Availability of Agent's movement.
        /// </summary>
        public bool Locked
        {
            get { return _motionEngine == null || _motionEngine.Locked(); }
            set
            {
                if (_motionEngine == null) return;
                if (value) _motionEngine.Lock();
                else _motionEngine.Unlock();
            }
        }

        /// <summary>
        /// Defines how hard is it to push Agent around.
        /// </summary>
        public float Weight
        {
            get { return _weight; }
            set { _weight = value; }
        }

        /// <summary>
        /// Transform component of Agent's target.
        /// </summary>
        public Transform TargetTransform
        {
            get { return _agent.Target;}
            set { _agent.Target = value; }
        }

        /// <summary>
        /// Current state of Agent's Motion Component.
        /// </summary>
        public MotionState State
        {
            get { return _state; }
            set { _state = value; }
        }

        /// <summary>
        /// Agent's Motion Type.
        /// </summary>
        public MotionEngine Type
        {
            get { return _type; }
            set { _type = value; }
        }

        /// <summary>
        /// Returns whether the user wants to debug the Motion configuration.
        /// </summary>
        public bool Debug
        {
            get { return _debug; }
            set { _debug = value; }
        }

        /// <summary>
        /// The speed with which the Agent walks.
        /// </summary>
        public float WalkSpeed
        {
            get { return _walkSpeed; }
            set { _walkSpeed = value; }
        }

        /// <summary>
        /// Whether the agent can run.
        /// </summary>
        public bool CanRun
        {
            get { return _canRun; }
            set { _canRun = value; }
        }

        /// <summary>
        /// The speed with which the Agent runs.
        /// </summary>
        public float RunSpeed
        {
            get { return _runSpeed; }
            set { _runSpeed = value; }
        }

        /// <summary>
        /// Units of energy per second which it takes Agent to walk.
        /// </summary>
        public List<ResourceAction> WalkCost
        {
            get { return _walkCost; }
            set { _walkCost = value; }
        }

        /// <summary>
        /// Units of energy per second which it takes Agent to run.
        /// </summary>
        public List<ResourceAction> RunCost
        {
            get { return _runCost; }
            set { _runCost = value; }
        }

        /// <summary>
        /// Speed with which Agent will turn towards target.
        /// </summary>
        public float RotationSpeed
        {
            get { return _rotationSpeed; }
            set { _rotationSpeed = value; }
        }
        
        /// <summary>
        /// Whether the agent can dodge.
        /// </summary>
        public bool CanDodge
        {
            get { return _canDodge; }
            set { _canDodge = value; }
        }

        /// <summary>
        /// Defines the speed with which the Agent is moving while dodging.
        /// </summary>
        public float DodgeSpeed
        {
            get { return _dodgeSpeed; }
            set { _dodgeSpeed = value; }
        }

        /// <summary>
        /// Time in seconds that passes between calling Dodge and actually dodging.
        /// </summary>
        public float DodgeDelay
        {
            get { return _dodgeDelay; }
            set { _dodgeDelay = value; }
        }

        /// <summary>
        /// Amount of time that needs to pass after dodge to be able to do it again.
        /// </summary>
        public float DodgeCoolDown
        {
            get { return _dodgeCoolDown; }
            set { _dodgeCoolDown = value; }
        }

        /// <summary>
        /// The amount of time for which the Agent is performoing the dodge.
        /// </summary>
        public float DodgeDuration
        {
            get { return _dodgeDuration; }
            set { _dodgeDuration = value; }
        }

        /// <summary>
        /// Duration of standing between location change while patroling.
        /// </summary>
        public float PatrolTime
        {
            get { return _patrolTime; }
            set { _patrolTime = value; }
        }

        /// <summary>
        /// Clip that is used as a sound of Agent's step while it is walking.
        /// </summary>
        public List<AudioClip> WalkingStepSounds
        {
            get { return _walkingStepSounds; }
            set { _walkingStepSounds = value; }
        }

        /// <summary>
        /// Amount of time that passes between playing the WalkingStepSound.
        /// </summary>
        public float WalkingStepPing
        {
            get { return _walkingStepPing; }
            set { _walkingStepPing = value; }
        }

        /// <summary>
        /// Clip that is used as a sound of Agent's step while it is running.
        /// </summary>
        public List<AudioClip> RunningStepSounds
        {
            get { return _runningStepSounds; }
            set { _runningStepSounds = value; }
        }

        /// <summary>
        /// Amount of time that passes between playing the WalkingStepSound.
        /// </summary>
        public float RunningStepPing
        {
            get { return _runningStepPing; }
            set { _runningStepPing = value; }
        }

        /// <summary>
        /// Clip that is used as a sound of Agent's dodge.
        /// </summary>
        public List<AudioClip> DodgeSounds
        {
            get { return _dodgeSounds; }
            set { _dodgeSounds = value; }
        }

        /// <summary>
        /// Moving part of the cannon.
        /// </summary>
        public Transform Head
        {
            get { return _head; }
            set { _head = value; }
        }

        /// <summary>
        /// Speed of rotation from side to side while idling.
        /// </summary>
        public float IdleRotationSpeed
        {
            get { return _idleRotationSpeed; }
            set { _idleRotationSpeed = value; }
        }

        /// <summary>
        /// Set true if cannon's idle rotation angles should be restricted.
        /// </summary>
        public bool ClampIdleRotation
        {
            get { return _clampIdleRotation; }
            set { _clampIdleRotation = value; }
        }

        /// <summary>
        /// Starting angle of cannon's idle rotation.
        /// </summary>
        public float ClampedIdleRotStart
        {
            get { return _clampedIdleRotStart; }
            set { _clampedIdleRotStart = value; }
        }

        /// <summary>
        /// Ending angle of cannon's idle rotation.
        /// </summary>
        public float ClampedIdleRotEnd
        {
            get { return _clampedIdleRotEnd; }
            set { _clampedIdleRotEnd = value; }
        }

        /// <summary>
        /// The engine that the Agent uses for pathfinding.
        /// </summary>
        public IMotionEngine Engine
        {
            get { return _motionEngine; }
            set { _motionEngine = value; }
        }

        #endregion

        #region CUSTOMIZATION
        /// Agent's Motion Type.
        [SerializeField] private MotionEngine _type;

        [Tooltip("Defines how hard it is to push Agent around.")]
        [SerializeField] private float _weight = 1f;

        [SerializeField] private float _walkSpeed = 5f;
        [SerializeField] private bool _canRun = false;
        [SerializeField] private float _runSpeed = 10f;
        [Tooltip("Units of energy per second which it takes Agent to move.")]
        [SerializeField] private List<ResourceAction> _walkCost = new List<ResourceAction>();
        [Tooltip("Units of energy per second which it takes Agent to run.")]
        [SerializeField] private List<ResourceAction> _runCost = new List<ResourceAction>();
        [Tooltip("Speed with which Agent will turn towards target.")]
        [SerializeField] private float _rotationSpeed = 10f;
        
        [SerializeField] private bool _canDodge = false;
        [SerializeField] private float _dodgeSpeed = 10f;
        [SerializeField] private float _dodgeDelay = 0.0f;
        [SerializeField] private float _dodgeCoolDown = 2f;
        [SerializeField] private float _dodgeDuration = .1f;
        
        [Tooltip("Duration of standing between location change while patroling.")]
        [SerializeField] private float _patrolTime = 5f;
        [SerializeField] private List<AudioClip> _walkingStepSounds;
        [SerializeField] private float _walkingStepPing = 0.5f;
        [SerializeField] private List<AudioClip> _runningStepSounds;
        [SerializeField] private float _runningStepPing = 0.2f;
        [SerializeField] private List<AudioClip> _dodgeSounds;


        [Tooltip("Moving part of the cannon.")]
        [SerializeField] private Transform _head;
        [Tooltip("Speed of rotation from side to side while idling.")]
        [SerializeField] private float _idleRotationSpeed;
        [Tooltip("Set true if cannon's idle rotation angles should be restricted.")]
        [SerializeField] private bool _clampIdleRotation;
        [Tooltip("Starting angle of cannon's idle rotation.")]
        [SerializeField] private float _clampedIdleRotStart;
        [Tooltip("Ending angle of cannon's idle rotation.")]
        [SerializeField] private float _clampedIdleRotEnd;

        [Space(10)]
        [Tooltip("Display the Motion configuration in editor?")]
        [SerializeField] private bool _debug = true;
        #endregion

        #region FIELDS
        /// The engine used by this Agent for path-finding.
        private IMotionEngine _motionEngine;
        /// Current state of Agent's Motion Component.
        private MotionState _state;
        /// Animation component of the Agent.
        private AgentAnimation _agentAnimation;
        /// Make sure Agent's Motion never misses a target.
        private Transform _defaultTarget;
        /// Last time Agent's Motion component made a sound.
        private float _lastSoundTime;
        /// Stores the position at which the agent has been initialized the first time.
        public Vector3 initialPosition;
        
        #endregion
        
        public AgentMotion(EliotAgent agent) : base(agent)
        {
        }

        /// <summary>
        /// This function is called when the component is added to an Agent.
        /// </summary>
        public override void OnAddComponent()
        {
            _weight = 1.0f;
            _walkSpeed = 5f;
            _canRun = false;
            _runSpeed = 10f;
            _rotationSpeed = 10f;
            _canDodge = false;
            _dodgeSpeed = 10f;
            _dodgeDelay = 0.0f;
            _dodgeCoolDown = 2f;
            _dodgeDuration = .1f;
            _patrolTime = 5f;
            _walkingStepPing = 0.5f;
            _runningStepPing = 0.2f;
        }

        /// <summary>
        /// Invoke animation on Agent's graphics.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="fadeLength"></param>
        public void Animate(AnimationState state, float fadeLength = 0.3F)
        {
            if (_agentAnimation != null)
                _agentAnimation.Animate(state, fadeLength);
        }

        /// <summary>
        /// Play Audio Clip via Agent's AudioSource component.
        /// </summary>
        /// <param name="audio"></param>
        /// <param name="ping"></param>
        public void PlayAudio(List<AudioClip> audio, float ping)
        {
            if (!_agent.GetAudioSource().isPlaying && Time.time >= _lastSoundTime + ping)
            {
                _agent.GetAudioSource().PlayRandomClip(audio);
                _lastSoundTime = Time.time;
            }
        }

        /// <summary>
        /// Initialize Motion component of the Agent.
        /// </summary>
        /// <param name="agent"></param>
        public override void Init(EliotAgent agent)
        {
            _agent = agent;
            initialPosition = transform.position;
            _agentAnimation = agent.GetAgentComponent<AgentAnimation>();
            switch (_type)
            {
                case MotionEngine.Turret:
                    _motionEngine = new TurretMotionEngine();
                    break;
                case MotionEngine.NavMesh:
                    _motionEngine = new NavMeshMotionEngine();
                    break;
#if ASTAR_EXISTS
		        case MotionEngine.Astar:
			        _motionEngine = new AstarMotionEngine();
			        break;
#endif
            }
            if(_motionEngine != null)
                _motionEngine.Init(agent);
        }

        /// <summary>
        /// Reset the component to the initial state.
        /// </summary>
        public override void AgentReset()
        {
            if(_motionEngine != null)
                _motionEngine.Idle();
        }

        /// <summary>
        /// Lock the Agent's Motion component.
        /// </summary>
        public void Lock()
        {
            if(_motionEngine != null)
                _motionEngine.Lock();
        }

        /// <summary>
        /// Unlock the Agent's Motion component.
        /// </summary>
        public void Unlock()
        {
            if(_motionEngine != null)
                _motionEngine.Unlock();
        }

        #region EXTERNAL INFLUENCE
        /// <summary>
        /// Push Agent using NavMeshAgent's Warp method.
        /// </summary>
        /// <param name="pusherPos"></param>
        /// <param name="pushPower"></param>
        public void Push(Vector3 pusherPos, float pushPower)
        {
            if(_motionEngine != null)
                _motionEngine.Push(pusherPos, pushPower);
        }
        #endregion

        /// <summary>
        /// Check if the path to the current target is valid.
        /// </summary>
        /// <returns></returns>
        public bool PathIsValid()
        {
            return _motionEngine.PathIsValid();
        }
    }
}
