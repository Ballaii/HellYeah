using System;
using System.Collections.Generic;
using Eliot.Environment;
using Eliot.Utility;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Eliot.AgentComponents
{
    /// <summary>
    /// Agent Component that gathers the Player Input and is capable of turning that into Agent actions.
    /// </summary>
    [Serializable]
    public class AgentPlayerInput : AgentComponent
    {
        public const string VerticalAxis = "Vertical";
        public const string HorizontalAxis = "Horizontal";
        public const string VerticalAxisMouse = "Mouse Y";
        public const string HorizontalAxisMouse = "Mouse X";

        /// <summary>
        /// All the possible Input ways to control the Agent motion.
        /// </summary>
        public enum MotionInputType
        {
            None,
            WASD,
            CursorProjection
        }
        
        /// <summary>
        /// Offset coordinates of the motion input.
        /// </summary>
        public enum MotionInputSpace
        {
            World,
            Self
        }
        
        /// <summary>
        /// All the possible Input ways to control the Agent rotation.
        /// </summary>
        public enum RotationType
        {
            None,
            CursorProjection,
            MouseAxis
        }

        /// <summary>
        /// Enumerates possible ways of input of a Key.
        /// </summary>
        public enum KeyEvent
        {
            KeyPress,
            KeyUp,
            KeyDown
        }

        /// <summary>
        /// Encapsulates an association between Player Input and a Skill.
        /// </summary>
        [Serializable]
        public class KeySkillBinding
        {
            /// <summary>
            /// Input Key Code.
            /// </summary>
            public KeyCode key;
            
            /// <summary>
            /// Input Key Event.
            /// </summary>
            public KeyEvent keyEvent = KeyEvent.KeyPress;
            
            /// <summary>
            /// Skill to execute.
            /// </summary>
            public Skill skill;

            /// <summary>
            /// Gather information about the player input.
            /// </summary>
            /// <param name="keySkillBindingToExecute"></param>
            /// <returns></returns>
            public bool Check(ref KeySkillBinding keySkillBindingToExecute)
            {
                switch (keyEvent)
                {
                    case KeyEvent.KeyPress:
                    {
                        if (Input.GetKey(key))
                        {
                            keySkillBindingToExecute = this;
                            return true;
                        }
                        break;
                    }

                    case KeyEvent.KeyDown:
                    {
                        if (Input.GetKeyDown(key))
                        {
                            keySkillBindingToExecute = this;
                            return true;
                        }
                        break;
                    }

                    case KeyEvent.KeyUp:
                    {
                        if (Input.GetKeyUp(key))
                        {
                            keySkillBindingToExecute = this;
                            return true;
                        }
                        break;
                    }
                }

                return false;
            }
            
            /// <summary>
            /// Make Agent execute the bind Skill.
            /// </summary>
            /// <param name="agent"></param>
            public void Execute(EliotAgent agent)
            {
                agent.ExecuteSkill(skill);
            }
        }
        
        /// <summary>
        /// Encapsulates an association between Player Input and a UnityEvent.
        /// </summary>
        [Serializable]
        public class KeyActionBinding
        {
            /// <summary>
            /// Input Key Code.
            /// </summary>
            public KeyCode key;
            
            /// <summary>
            /// Input Key Event.
            /// </summary>
            public KeyEvent keyEvent = KeyEvent.KeyPress;
            
            /// <summary>
            /// Unity Event to invoke.
            /// </summary>
            public UnityEvent action;

            /// <summary>
            /// Gather information about the player input.
            /// </summary>
            /// <param name="keyActionBindingToExecute"></param>
            /// <returns></returns>
            public bool Check(ref KeyActionBinding keyActionBindingToExecute)
            {
                switch (keyEvent)
                {
                    case KeyEvent.KeyPress:
                    {
                        if (Input.GetKey(key))
                        {
                            keyActionBindingToExecute = this;
                            return true;
                        }
                        break;
                    }

                    case KeyEvent.KeyDown:
                    {
                        if (Input.GetKeyDown(key))
                        {
                            keyActionBindingToExecute = this;
                            return true;
                        }
                        break;
                    }

                    case KeyEvent.KeyUp:
                    {
                        if (Input.GetKeyUp(key))
                        {
                            keyActionBindingToExecute = this;
                            return true;
                        }
                        break;
                    }
                }

                return false;
            }

            /// <summary>
            /// Invoke the Unity Event.
            /// </summary>
            public void Execute()
            {
                action.Invoke();
            }
        }

        /// <summary>
        /// Whether the Player Input has been detected during the current frame.
        /// </summary>
        public bool inputDetected = false;
        
        /// <summary>
        /// Whether to override the Behaviour driven activity with the Player Input actions.
        /// </summary>
        public bool forceExecution = true;

        /// <summary>
        /// Player Camera. Used for some of the input gathering.
        /// </summary>
        public Camera agentCamera;
        
        /// <summary>
        /// The Input type that is going to be translated into motion.
        /// </summary>
        public MotionInputType motionInputType = MotionInputType.WASD;
        
        /// <summary>
        /// Key that transitions walking into running.
        /// </summary>
        public KeyCode motionAccelerationKey = KeyCode.LeftShift;
        
        /// <summary>
        /// Whether motion Acceleration Key is being pressed.
        /// </summary>
        public bool motionAccelerationActiveted = false;
        
        /// <summary>
        /// The space relative to which the motion is going to be performed. 
        /// </summary>
        public MotionInputSpace motionInputSpace = MotionInputSpace.World;
        
        /// <summary>
        /// The key that sets the target position on a surface.
        /// </summary>
        public KeyCode cursorProjectionMotionKey = KeyCode.Mouse1;

        /// <summary>
        /// Object to instantiate to indicate the Agent's target set by the player.
        /// </summary>
        public GameObject targetIndicatorPrefab;

        /// <summary>
        /// If true, the default target indicator prefab will be loaded automatically.
        /// </summary>
        public bool useDefaultTargetIndicator = true;
        
        /// <summary>
        /// If false, the default target indicator prefab will not be spawned when other Agents are targeted.
        /// </summary>
        public bool spawnIndicatorOnAgents = true;
        
        #region Input Axis

        public float horizontal = 0f;
        public float vertical = 0f;
        
        public float mouseHorizontal = 0f;
        public float mouseVertical = 0f;

        public Vector3 projectedPosition;
        public Unit projectedUnit;

        #endregion
        
        #region Rotation

        /// <summary>
        /// The Input type that is going to be translated into rotation.
        /// </summary>
        public RotationType rotationType = RotationType.CursorProjection;

        /// <summary>
        /// Axis around which to rotate.
        /// </summary>
        public Transform rotationAxis;
        
        /// <summary>
        /// Target towards which the Agent is going to rotate.
        /// </summary>
        public Transform rotationTarget;

        #endregion

        #region KeySkillnBindings

        /// <summary>
        /// List of the Key-Skill bindings.
        /// </summary>
        public List<KeySkillBinding> keySkillBindings = new List<KeySkillBinding>();
        
        /// <summary>
        /// The last requested Key-Skill binding.
        /// </summary>
        public KeySkillBinding keySkillBindingToExecute;

        #endregion
        
        #region KeyActionBindings

        /// <summary>
        /// List of the Key-Action bindings.
        /// </summary>
        public List<KeyActionBinding> keyActionBindings = new List<KeyActionBinding>();
        
        /// <summary>
        /// The last requested Key-Action binding.
        /// </summary>
        public KeyActionBinding keyActionBindingToExecute;

        #endregion

        #region OptionalComponents

        private AgentPerception _agentPerception;
        private AgentMotion _agentMotion;

        #endregion


        public AgentPlayerInput(EliotAgent agent) : base(agent)
        {
        }

        /// <summary>
        /// Initialize the component.
        /// </summary>
        /// <param name="agent"></param>
        public override void Init(EliotAgent agent)
        {
            base.Init(agent);
            Agent = agent;
            var agentTransform = Agent.transform;

            if (!Application.isPlaying) return;
            
            float height = 1f;
            if (agent.GetComponent<CapsuleCollider>()) height = agent.GetComponent<CapsuleCollider>().height;
            else if(agent.GetComponent<CharacterController>()) height = agent.GetComponent<CharacterController>().height;
            
            rotationAxis = agent.FindTransformByName("_rotationAxis");
            rotationAxis.position = agentTransform.position + height * 0.5f * Vector3.up;
            rotationAxis.rotation = agentTransform.rotation;
            rotationAxis.parent = agentTransform;

            var rotationAxisEulerAngles = rotationAxis.eulerAngles;

            mouseHorizontal = rotationAxisEulerAngles.y;
            mouseVertical = rotationAxisEulerAngles.x;

            projectedPosition = transform.position;
            
            rotationTarget = agent.FindTransformByName("_rotationTarget");
            rotationTarget.rotation = rotationAxis.rotation;
            rotationTarget.position = rotationAxis.position + agent.transform.forward;
            rotationTarget.parent = rotationAxis;

            _agentPerception = agent.GetAgentComponent<AgentPerception>();
            _agentMotion = agent.GetAgentComponent<AgentMotion>();

            if (useDefaultTargetIndicator)
            {
                var guidsToTargetIndicatorPrefab = AssetDatabase.FindAssets("EliotTargetIndicatorPrefab");
                if (guidsToTargetIndicatorPrefab.Length > 0)
                {
                    var pathToTargetIndicatorPrefab = AssetDatabase.GUIDToAssetPath(guidsToTargetIndicatorPrefab[0]);
                    targetIndicatorPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(pathToTargetIndicatorPrefab);
                }
                else
                {
                    Debug.LogWarning("Failed to find EliotTargetIndicatorPrefab");
                }
            }
        }

        public override void AgentOnEnable(EliotAgent agent)
        {
            base.AgentOnEnable(agent);
            Init(agent);
        }

        /// <summary>
        /// Collect the Player Input information before executing it.
        /// </summary>
        public void CollectInputData()
        {
            inputDetected = false;
            
            var horizontalAxisMouse = Input.GetAxis(HorizontalAxisMouse);
            var verticalAxisMouse = Input.GetAxis(VerticalAxisMouse);
            
            horizontal = Input.GetAxis(HorizontalAxis);
            vertical = Input.GetAxis(VerticalAxis);

            if (horizontalAxisMouse != 0 || verticalAxisMouse != 0 || horizontal != 0 || vertical != 0)
                inputDetected = true;
            
            mouseHorizontal += horizontalAxisMouse;
            mouseVertical += verticalAxisMouse;

            if (motionInputType == MotionInputType.CursorProjection)
            {
                float distance;
                var collider = Agent.Target.GetComponent<Collider>();
                var waypoint = Agent.Target.GetComponent<EliotWayPoint>();
                if (collider)
                {
                    distance = Vector3.Distance(transform.position, collider.ClosestPoint(transform.position));
                }
                else
                {
                    distance = Vector3.Distance(transform.position, Agent.Target.transform.position);
                }

                if (distance > (waypoint ? waypoint.ActionDistance : 0.5f))
                {
                    inputDetected = true;
                }

                if (Input.GetKeyDown(cursorProjectionMotionKey))
                {
                    projectedPosition = GetProjectedPosition();

                    if (targetIndicatorPrefab)
                    {
                        var target = _agent.Target;
                        var agent = target.GetComponent<EliotAgent>();
                        if (!(agent && !spawnIndicatorOnAgents))
                        {
                            GameObject clickVisualObject = Instantiate(targetIndicatorPrefab) as GameObject;
                            clickVisualObject.transform.position = projectedPosition;
                        }
                    }
                    
                    inputDetected = true;
                }
            }

            if (Input.GetKey(motionAccelerationKey))
            {
                motionAccelerationActiveted = true;
            }
            else motionAccelerationActiveted = false;
            
            foreach (var keySkillBinding in keySkillBindings)
            {
                if (keySkillBinding.Check(ref keySkillBindingToExecute))
                    inputDetected = true;
            }
            
            foreach (var keyActionBinding in keyActionBindings)
            {
                if (keyActionBinding.Check(ref keyActionBindingToExecute))
                    inputDetected = true;
            }
        }

        /// <summary>
        /// Translate the Player Input into actions.
        /// </summary>
        public void ExecuteInputCommands()
        {
            if (_agentMotion)
            {
                #region Rotation

                if (rotationType == RotationType.CursorProjection)
                {
                    _agentMotion.Engine.LookAtTarget(GetProjectedPosition());
                }

                if (rotationType == RotationType.MouseAxis)
                {
                    rotationAxis.eulerAngles = new Vector3(mouseVertical, mouseHorizontal, 0.0f);
                    if (rotationTarget)
                        _agentMotion.Engine.LookAtTarget(GetTargetRotationFromMouseOffset());
                }

                #endregion

                #region Motion

                if (motionInputType == MotionInputType.WASD)
                {
                    _agentMotion.Engine.Move(GetTargetPositionFromKeyboard(), motionAccelerationActiveted);
                }

                if (motionInputType == MotionInputType.CursorProjection)
                {
                    if (projectedUnit)
                    {
                        if (_agentPerception)
                        {
                            _agentPerception.SeenUnits = new List<Unit>();
                            _agentPerception.SeenUnits.Add(projectedUnit);
                            _agentPerception.AgentMemory.Memorise(projectedUnit);
                        }

                        _agent.Target = projectedUnit.transform;
                    }
                    else
                    {
                        Agent.Target = Agent.GetDefaultTarget(projectedPosition);
                    }

                    if (motionAccelerationActiveted)
                    {
                        _agentMotion.Engine.RunToTarget();
                    }
                    else _agentMotion.Engine.WalkToTarget();
                }

                #endregion
            }

            #region Skills and Actions
            
            if(keySkillBindingToExecute != null)
                keySkillBindingToExecute.Execute(_agent);
            keySkillBindingToExecute = null;
            if(keyActionBindingToExecute != null)
                keyActionBindingToExecute.Execute();
            keyActionBindingToExecute = null;

            #endregion
        }

        /// <summary>
        /// Called by Agent on Update.
        /// </summary>
        public override void AgentUpdate()
        {
            CollectInputData();
            if (inputDetected && forceExecution)
            {
                ExecuteInputCommands();
            }
        }

        #region Get Target Position

        /// <summary>
        /// Translate input into motion target.
        /// </summary>
        /// <returns></returns>
        private Vector3 GetTargetPositionFromKeyboard()
        {
            if (motionInputSpace == AgentPlayerInput.MotionInputSpace.World)
            {
                return new Vector3(horizontal, 0, vertical);
            }

            if (motionInputSpace == AgentPlayerInput.MotionInputSpace.Self)
            {
                return Agent.transform.TransformVector(new Vector3(horizontal, 0, vertical));
            }
            return Vector3.zero;
        }
        
        /// <summary>
        /// Get a projected position from mouse cursor onto a surface.
        /// </summary>
        /// <returns></returns>
        public Vector3 GetProjectedPosition()
        {
            RaycastHit hit;
            Ray ray = agentCamera.ScreenPointToRay(Input.mousePosition);
            
            if (Physics.Raycast(ray.origin, ray.direction, out hit))
            {
                var targetAgent = hit.collider.GetComponent<EliotAgent>();
                if (targetAgent)
                {
                    projectedUnit = targetAgent.Unit;
                    // Return the position of the target
                    return targetAgent.transform.position;
                }
                else
                {
                    projectedUnit = null;
                    Plane plane = new Plane(Vector3.up, 0.0f);
                    float distanceToPlane;
                    if (plane.Raycast(ray, out distanceToPlane))
                    {
                        Vector3 pointOut = ray.GetPoint(distanceToPlane);
                        return pointOut;
                    }
                }
            }
            
            return _agent.transform.position;
        }

        #endregion
        
        
        #region Get Target Rotation

        /// <summary>
        /// Translate input into rotation target.
        /// </summary>
        /// <returns></returns>
        private Vector3 GetTargetRotationFromMouseOffset()
        {
            
            return rotationTarget.position;
        }

        #endregion

        /// <summary>
        /// This function is called when the component is added to an Agent.
        /// </summary>
        public override void OnAddComponent()
        {
            forceExecution = true;

            motionInputType = MotionInputType.WASD;
            motionAccelerationKey = KeyCode.LeftShift;

            motionInputSpace = MotionInputSpace.World;
            cursorProjectionMotionKey = KeyCode.Mouse1;
            
            rotationType = RotationType.MouseAxis;
        }
    }
}