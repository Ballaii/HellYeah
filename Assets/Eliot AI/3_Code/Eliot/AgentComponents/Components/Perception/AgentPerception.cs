#pragma warning disable CS0414, CS0649, CS0612, CS1692
using System.Collections.Generic;
using Eliot.Environment;
using Eliot.Utility;
using UnityEngine;

namespace Eliot.AgentComponents
{
    /// <summary>
    /// Encapsulates Agent's abilities to understand what is going on around.
    /// </summary>
    public class AgentPerception : AgentComponent
    {
        /// <summary>
        /// Perception dimensions. 2D or 3D.
        /// </summary>
        public enum Dimensions
        {
            Two,
            Three
        }

        /// <summary>
        /// Possible ways of getting the perceived Units list. 
        /// </summary>
        public enum Modes
        {
            Distance,
            Raycasting,
            OverlapSphere
        }

        /// <summary>
        /// <para>Cache Agent's focus.</para>
        /// </summary>
        public Unit TargetUnit { get; set; }

        /// <summary>
        /// List of Entities that are spotted every Update.
        /// </summary>
        public List<Unit> SeenUnits
        {
            get { return _seenUnits; }
            set { _seenUnits = value; }
        }

        /// <summary>
        /// Last position in space where happened something that seemed suspicious to Agent.
        /// </summary>
        public Vector3 SuspiciousPosition
        {
            get { return _suspiciousPosition; }
            set { _suspiciousPosition = value; }
        }

        /// <summary>
        /// Degrees for which the y rotation of rays' direction is being offset.
        /// </summary>
        public float Offset
        {
            get { return _offset; }
            set { _offset = value; }
        }

        /// <summary>
        /// Field of Agent's view in degrees.
        /// </summary>
        public float FieldOfView
        {
            get { return _fieldOfView; }
            set { _fieldOfView = value; }
        }

        /// <summary>
        /// Number of rays that are being cast uniformly in Agent's field of view.
        /// </summary>
        public int Resolution
        {
            get { return _resolution; }
            set { _resolution = value; }
        }

        /// <summary>
        /// Maximum distance at which Agent can see anything with his rays.
        /// </summary>
        public float Range
        {
            get { return _range; }
            set { _range = value; }
        }

        /// <summary>
        /// Distance at which Agent will definitely spot Entities even if they are right behind him.
        /// </summary>
        public float SeeEverythingRange
        {
            get { return _seeEverythingRange; }
            set { _seeEverythingRange = value; }
        }

        /// <summary>
        /// Number of rays used to spot Entities in any direction in small _range.
        /// </summary>
        public int SeeEverythingResolution
        {
            get { return _seeEverythingResolution; }
            set { _seeEverythingResolution = value; }
        }

        /// <summary>
        /// Returns whether the user wants to debug the Perception configuration.
        /// </summary>
        public bool Visualize
        {
            get { return _visualize; }
            set { _visualize = value; }
        }

        /// <summary>
        /// Link to Perception's Memory component.
        /// </summary>
        public AgentMemory AgentMemory
        {
            get { return agentMemory; }
            set { agentMemory = value; }
        }

        /// <summary>
        /// 2D (XY) or 3D (XYZ).
        /// </summary>
        public Dimensions PerceptionDimensions;

        /// <summary>
        /// A way of getting the perceived Units list. 
        /// </summary>
        public Modes PerceptionMode;

        /// <summary>
        /// Should the agent see through walls?
        /// </summary>
        public bool IgnoreObstacles = true;

        public AgentPerception(EliotAgent agent) : base(agent)
        {
        }


        /// <summary>
        /// Where to cast the rays and calculate the distance from.
        /// </summary>
        public Transform Origin
        {
            get
            {
                if (!_origin)
                {
                    if(!_agent) _agent = GetComponent<EliotAgent>();
                    _origin = _agent.GetPerceptionOrigin();
                }
                    
                return _origin;
            }
            set { _origin = value; }
        }

        /// <summary>
        /// Whether there's a _range at which the agent can see even what's behind its back.
        /// </summary>
        public bool CanSeeEverythingAtRange
        {
            get { return _canSeeEverythingAtRange; }
            set { _canSeeEverythingAtRange = value; }
        }

        /// <summary>
        /// What algorithm to use for the "see everything" perception.
        /// </summary>
        public Modes SeeEverythingMode
        {
            get { return _seeEverythingMode; }
            set { _seeEverythingMode = value; }
        }

        /// <summary>
        /// Filter the layers which the agent can perceive.
        /// </summary>
        public LayerMask PerceptionMask
        {
            get { return _perceptionMask; }
            set { _perceptionMask = value; }
        }

        #region CUSTOMIZATION

        [SerializeField] private float _offset = 90f;
        [SerializeField] private float _fieldOfView = 270f;
        [SerializeField] private int _resolution = 7;
        [SerializeField] private float _range = 10f;
        [SerializeField] private bool _canSeeEverythingAtRange = false;
        [SerializeField] private Modes _seeEverythingMode;

        [Tooltip("Distance at which Agent will definitelly spot Entities even if they are right behind him.")]
        [SerializeField]
        private float _seeEverythingRange = 2f;

        [Tooltip("Number of rays used to spot Entities in any direction in small _range.")] [SerializeField]
        private int _seeEverythingResolution = 7;

        [Space(3)] [Tooltip("Position out of which rays are cast.")] [SerializeField]
        private Transform _origin = null;

        [Space(5)] [Tooltip("Cache for Agents' representation of the world.")] [SerializeField]
        private AgentMemory agentMemory;

        [Space(10)] [Tooltip("Display the Perception configuration in editor?")] [SerializeField]
        private bool _visualize = true;

        [SerializeField] private LayerMask _perceptionMask = Physics.AllLayers;

        #endregion

        /// List of Entities that are spotted every Update.
        [SerializeField] private List<Unit> _seenUnits;

        /// Last position in space where happened something that seemed suspicious to Agent.
        private Vector3 _suspiciousPosition;

        /// Condition Interfaces of this instance of Agent's Perception component.
        private List<PerceptionConditionInterface> _conditionInterfaces = new List<PerceptionConditionInterface>();

        /// <summary>
        /// <para>Initialisation.</para>
        /// </summary>
        public override void Init(EliotAgent agent)
        {
            _seenUnits = new List<Unit>();
            _agent = agent;
            _origin = _agent.GetPerceptionOrigin();
            agentMemory = new AgentMemory();
            agentMemory.Init(agent);
            _suspiciousPosition = _agent.transform.position;
        }

        /// <summary>
        /// This function is called when the component is added to an Agent.
        /// </summary>
        public override void OnAddComponent()
        {
            _offset = 90f;
            _fieldOfView = 270f;
            _resolution = 15;
            _range = 10f;
            _canSeeEverythingAtRange = false;
            _seeEverythingRange = 2f;
            _seeEverythingResolution = 7;
            _visualize = true;
            _perceptionMask = Physics.AllLayers;
            PerceptionDimensions = Dimensions.Three;
            PerceptionMode = Modes.Raycasting;
            _seeEverythingMode = Modes.Distance;
        }

        /// <summary>
        /// Set all the settings to the initial values.
        /// </summary>
        public override void AgentReset()
        {
            _seenUnits = new List<Unit>();
            if(agentMemory != null)
                agentMemory.Units = new Queue<Unit>();
        }

        /// <summary>
        /// Get radius of an unit if it is an Agent or 0 otherwise.
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        private static float Radius(Unit unit)
        {
            var agent = unit.GetComponent<EliotAgent>();
            return agent ? agent.Radius : 0;
        }
        
        /// <summary>
        /// Get radius of current Agent's target.
        /// </summary>
        /// <returns></returns>
        public float TargetRadius()
        {
            return Radius(TargetUnit);
        }
        
        /// <summary>
        /// Let Agent know that something is going on at specific position.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="duration"></param>
        public void HearSomething(Vector3 position, float duration)
        {
            _suspiciousPosition = position;
            _agent.SetStatus(AgentStatus.HeardSomething, duration);
        }
        
        /// <summary>
        /// Build a ray starting at the origin and facing towards the target rotation.
        /// </summary>
        public static Vector3 InitRay(float r, float fi, float offset, Transform center)
        {
            var offsetRads = offset * Mathf.PI / 180;
            fi -= center.eulerAngles.y;
            var rads = fi * Mathf.PI / 180;
            rads = rads % (Mathf.PI * 2) + offsetRads % (Mathf.PI * 2);
            var x = center.position.x + r * Mathf.Cos(rads);
            var z = center.position.z + r * Mathf.Sin(rads);
            return new Vector3(x, center.position.y, z) - center.position;
        }
        
        /// <summary>
        /// Build a ray starting at the origin and facing towards the target rotation. Works in 2D.
        /// </summary>
        public static Vector3 InitRay2D(float r, float fi, float offset, Transform center)
        {
            var offsetRads = offset * Mathf.PI / 180;
            fi += center.eulerAngles.z;
            var rads = fi * Mathf.PI / 180;
            rads = rads % (Mathf.PI * 2) + offsetRads % (Mathf.PI * 2);
            var x = center.position.x + r * Mathf.Cos(rads);
            var y = center.position.y + r * Mathf.Sin(rads);
            return new Vector2(x, y) - (Vector2)center.position;
        }

        /// <summary>
        /// Cast rays to understand what objects are around the Agent.
        /// </summary>
        /// <param name="seenUnits"></param>
        /// <param name="resolution"></param>
        /// <param name="fieldOfView"></param>
        /// <param name="range"></param>
        private void SeeIteration(ref List<Unit> seenUnits, int resolution, float fieldOfView, float range)
        {
            switch (PerceptionDimensions)
            {
                case Dimensions.Two:
                {
                    switch (PerceptionMode)
                    {
                        case Modes.Distance:
                        {
                            SeeIteration_Distance(ref seenUnits, range, Dimensions.Two);
                            break;
                        }

                        case Modes.Raycasting:
                        {
                            SeeIteration_2D_Rays(ref seenUnits, resolution, fieldOfView, range);
                            break;
                        }

                        case Modes.OverlapSphere:
                        {
                            SeeIteration_OverlapSphere(ref seenUnits, range, Dimensions.Two);
                            break;
                        }
                    }

                    break;
                }

                case Dimensions.Three:
                {
                    switch (PerceptionMode)
                    {
                        case Modes.Distance:
                        {
                            SeeIteration_Distance(ref seenUnits, range, Dimensions.Three);
                            break;
                        }

                        case Modes.Raycasting:
                        {
                            SeeIteration_3D_Rays(ref seenUnits, resolution, fieldOfView, range);
                            break;
                        }

                        case Modes.OverlapSphere:
                        {
                            SeeIteration_OverlapSphere(ref seenUnits, range, Dimensions.Three);
                            break;
                        }
                    }

                    break;
                }
            }
        }

        /// <summary>
        /// Update what the Agent is seeing.
        /// </summary>
        public override void AgentUpdate()
        {
            //Refresh the list of seen units
            _seenUnits = new List<Unit>();

            //Update in _range
            SeeIteration(ref _seenUnits, _resolution, _fieldOfView, _range);

            //Indisputably see
            if (_seeEverythingRange <= 0 || _seeEverythingResolution == 0) return;
            SeeIteration(ref _seenUnits, _seeEverythingResolution, 360, _seeEverythingRange);
            
            AgentMemory.Update();
        }

        /// <summary>
        /// Search for entities with specified characteristics in FOV or in memory. 
        /// </summary>
        public bool SeeUnit(UnitQuery query, bool setAsTarget = true, bool remember = true)
        {
            //	Check if we actually see it
            if (_seenUnits.Count > 0)
                foreach (var unit in _seenUnits)
                    if (unit && query(unit))
                    {
                        TargetUnit = unit;
                        if(setAsTarget) _agent.Target = TargetUnit.transform;
                        if (remember) AgentMemory.Memorise(unit);
                        return true;
                    }

            //	Otherwise check if we remember it
            if (!remember) return false;
            var unitEnemy = AgentMemory.RememberUnit(query);
            if (unitEnemy == null)
                return false;
            
            TargetUnit = unitEnemy;
            _agent.Target = TargetUnit != null ? TargetUnit.transform : _agent.GetDefaultTarget();
            return true;
        }
        
        /// <summary>
        /// Return all the entities that match the specified characteristics in FOV.
        /// </summary>
        public List<Unit> SeeUnits(UnitQuery query)
        {
            var result = new List<Unit>();
            // Check if we actually see it
            if (_seenUnits.Count > 0)
                foreach (var unit in _seenUnits)
                    if (unit && query(unit))
                    {
                        result.Add(unit);
                    }

            return result;
        }

        /// <summary>
        /// Search for entities with specified characteristics in FOV or in memory
        /// and select the one by user defined criteria.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="criterion"></param>
        /// <param name="chooseMax"></param>
        /// <param name="setAsTarget"></param>
        /// <param name="remember"></param>
        /// <returns></returns>
        public bool SelectUnitByCriteria(UnitQuery query, UnitCriterion criterion, bool chooseMax,
            bool setAsTarget = true, bool remember = true)
        {
            var units = new List<Unit>();
            //	Check if we actually see it
            if (_seenUnits.Count > 0)
            {
                foreach (var unit in _seenUnits)
                    if (unit && query(unit))
                        units.Add(unit);
            }
            else
            {
                if (remember)
                {
                    var rememberedUnits = AgentMemory.RememberUnits(query);
                    foreach(var unit in rememberedUnits) units.Add(unit);
                }
            }

            if (units.Count == 0) return false;
            if (units.Count == 1)
            {
                TargetUnit = units[0];
                if(setAsTarget && TargetUnit) _agent.Target = TargetUnit.transform;
                if (remember) AgentMemory.Memorise(units[0]);
                return true;
            }
            
            // Now select the one Unit that fits the criteria the best.
            var index = 0;
            var crit = criterion(units[0]);
            for (var i = 1; i < units.Count; i++)
            {
                if (chooseMax)
                {
                    if (!(criterion(units[i]) > crit)) continue;
                    index = i;
                    crit = criterion(units[i]);
                }
                else
                {
                    if (!(criterion(units[i]) < crit)) continue;
                    index = i;
                    crit = criterion(units[i]);
                }
            }
            
            TargetUnit = units[index];
            if(setAsTarget && TargetUnit) _agent.Target = TargetUnit.transform;
            if (remember) AgentMemory.Memorise(units[index]);
            return true;
        }

        /// <summary>
        /// Return true if there are any obstacles between the Agent and its target.
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="target"></param>
        /// <param name="targetYOffset"></param>
        /// <returns></returns>
        public static bool ObstaclesBetweenMeAndTarget(Transform origin, Transform target, Dimensions dimensions = Dimensions.Three, float targetYOffset = 0.5f, int layerMask = Physics.AllLayers)
        {
            var helper = new GameObject("__ObstaclesBetweenMeAndTarget__HELPER__");
            helper.transform.position = origin.transform.position;
            var newDir = Vector3.RotateTowards(origin.transform.forward, 
                target.position + new Vector3(0, targetYOffset, 0) - origin.transform.position, 100, 0);
            helper.transform.rotation = Quaternion.LookRotation(newDir);
            
            if (dimensions == Dimensions.Three)
            {
                RaycastHit hit;
                Physics.Raycast(helper.transform.position, helper.transform.forward, out hit, Mathf.Infinity, layerMask);
                GameObject.DestroyImmediate(helper);
                return hit.transform != target;
            }
            else 
            {
#if UNITY_2019
                ContactFilter2D contactFilter = new ContactFilter2D();
                List<RaycastHit2D> results = new List<RaycastHit2D>();
                Physics2D.Raycast((Vector2)helper.transform.position, (Vector2)helper.transform.forward, contactFilter, results, layerMask);
                GameObject.DestroyImmediate(helper);
                bool containsTarget = false;
                foreach (var hit in results)
                {
                    if (hit.transform == target)
                    {
                        containsTarget = true;
                        break;
                    }
                }
                return !containsTarget;
#else
                var hit = Physics2D.Raycast((Vector2)helper.transform.position, (Vector2)helper.transform.forward, layerMask);
                GameObject.DestroyImmediate(helper);
                return hit.transform != target;
#endif
            }
        }

        #region Utility

        /// <summary>
        /// Cast rays to understand what objects are around the Agent.
        /// </summary>
        /// <param name="seenUnits"></param>
        /// <param name="resolution"></param>
        /// <param name="fieldOfView"></param>
        /// <param name="range"></param>
        private void SeeIteration_3D_Rays(ref List<Unit> seenUnits, int resolution, float fieldOfView, float range)
        {
            var n = resolution;
            var delta = fieldOfView / (n-1);
            var origin = Origin ?? _agent.transform;
            var vectors = new List<Vector3>();
			
            if (resolution % 2 != 0)
            {
                var rot = delta;
				
                vectors = new List<Vector3> {InitRay(range, 0, _offset, origin)};
                for (var i = 0; i < n / 2; i++)
                {
                    vectors.Add(InitRay(range, -rot, _offset, origin));
                    vectors.Add(InitRay(range, rot, _offset, origin));
                    rot += delta;
                }
            }
            else
            {
                for (var i = 0; i < resolution; i++)
                {
                    if (i % 2 == 0) continue;
                    vectors.Add(InitRay(range, -i * delta / 2, _offset, origin));
                    vectors.Add(InitRay(range, i * delta / 2, _offset, origin));
                }
            }

            //Add all QUnits in the FOV
            foreach (var vec in vectors)
            {
                RaycastHit hit;
                if (!Physics.Raycast(Origin.position, vec, out hit, range, _perceptionMask)) continue;
                var unit = hit.transform.gameObject.GetComponent<Unit>();
                if (unit && !seenUnits.Contains(unit)) seenUnits.Add(unit);
            }
        }

        /// <summary>
        /// Use Physics.OverlapSphere to detect Units in a certain radius.
        /// </summary>
        /// <param name="seenUnits"></param>
        /// <param name="range"></param>
        /// <param name="dimensions"></param>
        private void SeeIteration_OverlapSphere(ref List<Unit> seenUnits, float range, Dimensions dimensions)
        {
            Collider[] hitColliders = Physics.OverlapSphere(Origin.position, range, _perceptionMask, QueryTriggerInteraction.UseGlobal);
            foreach (var hitCollider in hitColliders)
            {
                var unit = hitCollider.GetComponent<Unit>();
                if (!unit) continue;
                if (unit == GetComponent<Unit>()) continue;
                if (IgnoreObstacles)
                {
                    seenUnits.Add(unit);
                }
                else
                {
                    if (!ObstaclesBetweenMeAndTarget(Origin, unit.transform, dimensions, 0.5f, _perceptionMask))
                    {
                        seenUnits.Add(unit);
                    }
                }
            }
        }
        
        /// <summary>
        /// Use Vector3.Distance to detect Units in a certain radius.
        /// </summary>
        /// <param name="seenUnits"></param>
        /// <param name="range"></param>
        /// <param name="dimensions"></param>
        private void SeeIteration_Distance(ref List<Unit> seenUnits, float range, Dimensions dimensions)
        {
            var units = GameObject.FindObjectsOfType<Unit>();
            foreach (var unit in units)
            {
                var distance = dimensions == Dimensions.Three ? 
                    Vector3.Distance(_origin.transform.position, unit.transform.position) :
                    Vector2.Distance(_origin.transform.position, unit.transform.position);
                if (distance <= range)
                {
                    if (IgnoreObstacles)
                    {
                        seenUnits.Add(unit);
                    }
                    else
                    {
                        if (!ObstaclesBetweenMeAndTarget(_origin, unit.transform, dimensions, 0.5f, _perceptionMask))
                        {
                            seenUnits.Add(unit);
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// Cast 2D rays to detect Units.
        /// </summary>
        /// <param name="seenUnits"></param>
        /// <param name="resolution"></param>
        /// <param name="fieldOfView"></param>
        /// <param name="range"></param>
        private void SeeIteration_2D_Rays(ref List<Unit> seenUnits, int resolution, float fieldOfView, float range)
        {
            var n = resolution;
            var delta = fieldOfView / (n-1);
            var origin = Origin ?? _agent.transform;
            var vectors = new List<Vector2>();
			
            if (resolution % 2 != 0)
            {
                var rot = delta;
				
                vectors = new List<Vector2> {InitRay2D(range, 0, _offset, origin)};
                for (var i = 0; i < n / 2; i++)
                {
                    vectors.Add(InitRay2D(range, -rot, _offset, origin));
                    vectors.Add(InitRay2D(range, rot, _offset, origin));
                    rot += delta;
                }
            }
            else
            {
                for (var i = 0; i < resolution; i++)
                {
                    if (i % 2 == 0) continue;
                    vectors.Add(InitRay2D(range, -i * delta / 2, _offset, origin));
                    vectors.Add(InitRay2D(range, i * delta / 2, _offset, origin));
                }
            }

            //Add all QUnits in the FOV
            foreach (var vec in vectors)
            {
                RaycastHit hit;
                if (!Physics.Raycast(Origin.position, vec, out hit, range, _perceptionMask)) continue;
                var unit = hit.transform.gameObject.GetComponent<Unit>();
                if (unit && !seenUnits.Contains(unit)) seenUnits.Add(unit);
            }
        }

        #endregion
        
    }
}