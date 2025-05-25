using System;
using UnityEngine;

namespace Eliot.Environment
{
	/// <summary>
	/// Holds the customization information for each WayPointsGroup.
	/// </summary>
	[Serializable] public class WayPointsColors
	{
		/// <summary>
		/// Color of each way point object.
		/// </summary>
		public Color WaypointColor = Color.white;
		
		/// <summary>
		/// Color of the holder object of the way points in the group.
		/// </summary>
		public Color OriginColor = Color.white;
		
		/// <summary>
		/// Color of connections between way points.
		/// </summary>
		public Color LineColor = Color.white;
	}
}
