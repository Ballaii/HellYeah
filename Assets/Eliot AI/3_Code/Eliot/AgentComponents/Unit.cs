using System.Collections.Generic;
using Eliot.AgentComponents;
using UnityEngine;

namespace Eliot.Environment
{
	/// <summary>
	/// The buffer of object's characteristics that Agent can easily understand.
	/// Add Unit component to all the objects that you want Agent to interact well with.
	/// </summary>
	public class Unit : MonoBehaviour
	{
		/// <summary>
		/// Enumerations of possible ways to visualize the Unit in the Editor.
		/// </summary>
		public enum UnitVisualizationType
		{
			None,
			FromCollider,
			Box,
			Sphere
		}
		
		/// <summary>
		/// User-defined type of the unit. Depends highly on the context of the game.
		/// </summary>
		public UnitType Type
		{
			get { return _type;}
			set { _type = value;} 
		}
		
		/// <summary>
		/// Team to which the unit belongs.
		/// </summary>
		public string Team
		{
			get { return _team;}
			set
			{
				_team = value;
				var agent = GetComponent<EliotAgent>();
				if (agent && Application.isPlaying)
				{
					agent.GetDefaultTarget().GetComponent<Unit>().Team = _team;
				}
			} 
		}

#if PIXELCRUSHERS_LOVEHATE
		/// <summary>
		/// Whether to use the Pixelcrushers LoveHate system to check if another Unit is a friend.
		/// </summary>
		public bool UseLoveHate
		{
			get { return _useLoveHate; }
		}
#endif
		
		/// User-defined type of the unit. Depends highly on the context of the game.
		[SerializeField] private UnitType _type;
		/// Team to which the unit belongs.
		[SerializeField] private string _team;
		/// Names of teams that are considered friendly to this unit.
		[SerializeField] private List<string> _friendlyTeams = new List<string>();
#if PIXELCRUSHERS_LOVEHATE
		/// Whether to use the Pixelcrushers LoveHate system to check if another Unit is a friend.
		[SerializeField] private bool _useLoveHate = true;
#endif
		/// <summary>
		/// List of the attributes groups attached to the Unit.
		/// </summary>
		[SerializeField] public List<AttributesGroup> attributesGroups = new List<AttributesGroup>();
		
		/// Store the used attributes to call them faster the second time and on.
		private Dictionary<string, Attribute> attributesCache = new Dictionary<string, Attribute>();

		#region Visualize

		/// <summary>
		/// The way the Unit is going to be visualized in the Editor.
		/// </summary>
		public UnitVisualizationType visualizationType;
		
		/// <summary>
		/// The color which the Unit is going to be visualized with.
		/// </summary>
		public Color gizmosColor = Color.white;

		/// <summary>
		/// For visualizationType = Box. Local scale of the box.
		/// </summary>
		public Vector3 visualizationBoxSize = Vector3.one;
		
		/// <summary>
		/// For visualizationType = Sphere. Radius of the sphere.
		/// </summary>
		public float visualizationSphereRadius = 1f;

		#endregion
		

		/// <summary>
		/// Check whether the given unit belongs to the same team or to one of the friendly teams.
		/// </summary>
		/// <param name="unit"></param>
		/// <returns></returns>
		public bool IsFriend(Unit unit)
		{
#if PIXELCRUSHERS_LOVEHATE
			if (!_useLoveHate)
				return _team == unit._team || _friendlyTeams.Contains(unit._team);
			else
			{
				var myFactionMember = GetComponent<PixelCrushers.LoveHate.FactionMember>();
				if (!myFactionMember)
					return _team == unit._team || _friendlyTeams.Contains(unit._team);
				var otherFactionMember = unit.GetComponent<PixelCrushers.LoveHate.FactionMember>();
				if (!otherFactionMember)
					return _team == unit._team || _friendlyTeams.Contains(unit._team);
				return myFactionMember.GetAffinity(otherFactionMember) >= 0;
			}
#else
			return _team == unit._team || _friendlyTeams.Contains(unit._team);
#endif
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

				foreach (var attributesGroup in attributesGroups)
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
		/// Visualize the Unit in the Editor.
		/// </summary>
		private void OnDrawGizmos()
		{
			if (visualizationType == UnitVisualizationType.FromCollider)
			{
				var boxCollider = GetComponent<BoxCollider>();
				if (boxCollider)
				{
					var initColor = Gizmos.color;
					Gizmos.color = gizmosColor;
					var position = transform.position;
					Gizmos.DrawWireCube(position + boxCollider.center, boxCollider.size);
					Gizmos.color = new Color(gizmosColor.r, gizmosColor.g, gizmosColor.b, gizmosColor.a * 0.3f);
					Gizmos.DrawCube(position + boxCollider.center, boxCollider.size);
					Gizmos.color = initColor;
				}
				else
				{
					var sphereCollider = GetComponent<SphereCollider>();
					if (sphereCollider)
					{
						var initColor = Gizmos.color;
						Gizmos.color = gizmosColor;
						var position = transform.position;
						Gizmos.DrawWireSphere(position + sphereCollider.center, sphereCollider.radius);
						Gizmos.color = new Color(gizmosColor.r, gizmosColor.g, gizmosColor.b, gizmosColor.a * 0.3f);
						Gizmos.DrawSphere(position + sphereCollider.center, sphereCollider.radius);
						Gizmos.color = initColor;
					}
				}
			}
			else if (visualizationType == UnitVisualizationType.Box)
			{
				var initColor = Gizmos.color;
				Gizmos.color = gizmosColor;
				var position = transform.position;
				Gizmos.DrawWireCube(position, visualizationBoxSize);
				Gizmos.color = new Color(gizmosColor.r, gizmosColor.g, gizmosColor.b, gizmosColor.a * 0.3f);
				Gizmos.DrawCube(position, visualizationBoxSize);
				Gizmos.color = initColor;
			}
			else if (visualizationType == UnitVisualizationType.Sphere)
			{
				var initColor = Gizmos.color;
				Gizmos.color = gizmosColor;
				var position = transform.position;
				Gizmos.DrawWireSphere(position, visualizationSphereRadius);
				Gizmos.color = new Color(gizmosColor.r, gizmosColor.g, gizmosColor.b, gizmosColor.a * 0.3f);
				Gizmos.DrawSphere(position, visualizationSphereRadius);
				Gizmos.color = initColor;
			}
		}
	}
}