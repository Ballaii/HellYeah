using System;
using System.Collections.Generic;
using Eliot.Utility;
using UnityEditor;
using UnityEngine;
using EditorGUI = UnityEditor.EditorGUI;
using Object = UnityEngine.Object;

namespace Eliot.AgentComponents.Editor
{
	/// <summary>
	/// Editor script for the Agent Perception component.
	/// </summary>
	public class AgentPerceptionEditor : AgentComponentEditor
    {
	    /// <summary>
	    /// Link to the target component.
	    /// </summary>
	    private AgentPerception _agentPerception;

	    #region Scene

	    /// <summary>
        /// Enables the Editor to handle an event in the scene view.
        /// </summary>
        public override void DrawSceneGUI(AgentComponent agentComponent)
	    {
		    if (!agentComponent) return;
		    if (_agentPerception == null)
		    {
			    try
			    {
				    _agentPerception = agentComponent as AgentPerception;
			    }
			    catch (Exception)
			    {
				    return;
			    }
		    }
	        if (_agentPerception == null) return;
	        if (!_agentPerception.Visualize) return;
            _agent = _agentPerception.GetComponent<EliotAgent>();
            if (_agent == null) return;

            switch (_agentPerception.PerceptionDimensions)
            {
	            case AgentPerception.Dimensions.Two:
	            {
		            switch (_agentPerception.PerceptionMode)
		            {
			            case AgentPerception.Modes.Distance:case AgentPerception.Modes.OverlapSphere:
			            {
				            OnSceneGUI_Perception_Distance_2D();
				            break;
			            }
			            case AgentPerception.Modes.Raycasting:
			            {
				            OnSceneGUI_Perception_2D();
				            break;
			            }
		            }
		            break;
	            }
	            case AgentPerception.Dimensions.Three:
	            {
		            switch (_agentPerception.PerceptionMode)
		            {
			            case AgentPerception.Modes.Distance:case AgentPerception.Modes.OverlapSphere:
			            {
				            OnSceneGUI_Perception_Distance();
				            break;
			            }
			            case AgentPerception.Modes.Raycasting:
			            {
				            OnSceneGUI_Perception();
				            break;
			            }
		            }
		            break;
	            }
            }

            OnSceneGUI_DisplayAttributes();
            
            DrawOriginHandles();
        }
	    
	    /// <summary>
	    /// Draw the handles for the Perception ray caster position.
	    /// </summary>
	    private void DrawOriginHandles()
	    {
		    var c = Handles.color;
		    Handles.color = Color.white;
		    Handles.ArrowHandleCap(0, _agentPerception.Origin.position, _agentPerception.Origin.rotation, 0.25f, EventType.Repaint);
		    Handles.color = c;
		    
		    if (!_adjustOrigin) return;
		    EditorGUI.BeginChangeCheck();
		    Vector3 newPosition = Handles.PositionHandle(_agentPerception.Origin.position,
			                                             _agentPerception.Origin.rotation);
		    
		    Quaternion newRotation = Handles.RotationHandle(_agentPerception.Origin.rotation,
			                                             _agentPerception.Origin.position );
		    if (EditorGUI.EndChangeCheck())
		    {
			    Undo.RecordObject(_agentPerception.Origin, "Change Origin Position");
			    _agentPerception.Origin.position = newPosition;
			    _agentPerception.Origin.rotation = newRotation;
		    }

	    }

	    /// <summary>
	    /// Visualize the attributes if those are added to the agent.
	    /// </summary>
	    private void OnSceneGUI_DisplayAttributes()
	    {
		    var aimFov = _agent["AimFieldOfView"];
		    var closeRange = _agent["CloseRange"];
		    if(aimFov != null && closeRange != null)
				DrawDoubleArc(Color.red, aimFov.floatValue, _agentPerception.Origin, closeRange.floatValue, 0, _agentPerception.PerceptionDimensions);
		    var backFov = _agent["BackFieldOfView"];
		    if (backFov != null)
			    DrawDoubleArc(Color.yellow, backFov.floatValue, _agentPerception.Origin, _agentPerception.SeeEverythingRange + 0.1f, 180, _agentPerception.PerceptionDimensions);
	    }
        
        /// <summary>
        /// Draw GUI for Perception.
        /// </summary>
        private void OnSceneGUI_Perception()
        {
	        if (!_agentPerception.Visualize) return;
			
            Handles.color = new Color(1f,1f,1f,0.3f);
            if (_agentPerception.Resolution == 0) return;
            var rotation = _agentPerception.Offset;
            var fieldOfView = _agentPerception.FieldOfView;
            var resolution = _agentPerception.Resolution;
            var range = _agentPerception.Range;

            var n = resolution;
            var delta = fieldOfView / (n-1);
            var origin = _agentPerception.Origin==null ? _agent.transform : _agentPerception.Origin;
            var vectors = new List<Vector3>();
			
            if (resolution % 2 != 0)
            {
                var rot = delta;
				
                vectors = new List<Vector3> {InitRay(range, 0, rotation, origin)};
                for (var i = 0; i < n / 2; i++)
                {
                    vectors.Add(InitRay(range, -rot, rotation, origin));
                    vectors.Add(InitRay(range, rot, rotation, origin));
                    rot += delta;
                }
            }
            else
            {
                for (var i = 0; i < resolution; i++)
                {
                    if (i % 2 == 0) continue;
                    vectors.Add(InitRay(range, -i * delta / 2, rotation, origin));
                    vectors.Add(InitRay(range, i * delta / 2, rotation, origin));
                }
            }
			
            foreach (var vec in vectors)
                Handles.DrawDottedLine(origin.position, vec, 2f);

            Handles.color = Color.white;

            var arcT = new GameObject("__arc_t__");
            arcT.transform.position = origin.position;
            arcT.transform.rotation = new Quaternion(0f, origin.rotation.y, 0f, origin.rotation.w);
            arcT.transform.Rotate(arcT.transform.up, 90-fieldOfView/2 + 90-rotation);
            var arcRot = -arcT.transform.right;
            UnityEngine.Object.DestroyImmediate(arcT);
			
            Handles.DrawWireArc(origin.position, Vector3.up, arcRot, fieldOfView, range);

            if (_agentPerception.CanSeeEverythingAtRange)
            {
	            Handles.DrawWireArc(origin.position, origin.up, -origin.right, 360,
		            _agentPerception.SeeEverythingRange);
	            Handles.DrawWireArc(origin.position, origin.up, -origin.right, 360,
		            _agentPerception.SeeEverythingRange - 0.05f);
            }

            DrawDoubleSphere(Color.white, _agentPerception.Origin, 0.1f);
        }
        
        /// <summary>
        /// Visualize 2-dimensional perception.
        /// </summary>
        private void OnSceneGUI_Perception_2D()
        {
	        if (!_agentPerception.Visualize) return;
			
            Handles.color = new Color(1f,1f,1f,0.3f);
            if (_agentPerception.Resolution == 0) return;
            var rotation = _agentPerception.Offset;
            var fieldOfView = _agentPerception.FieldOfView;
            var resolution = _agentPerception.Resolution;
            var range = _agentPerception.Range;

            var n = resolution;
            var delta = fieldOfView / (n-1);
            var origin = _agentPerception.Origin==null ? _agent.transform : _agentPerception.Origin;
            var vectors = new List<Vector3>();
			
            if (resolution % 2 != 0)
            {
                var rot = delta;
				
                vectors = new List<Vector3> {AgentPerception.InitRay2D(range, 0, rotation, origin)};
                for (var i = 0; i < n / 2; i++)
                {
                    vectors.Add(AgentPerception.InitRay2D(range, -rot, rotation, origin));
                    vectors.Add(AgentPerception.InitRay2D(range, rot, rotation, origin));
                    rot += delta;
                }
            }
            else
            {
                for (var i = 0; i < resolution; i++)
                {
                    if (i % 2 == 0) continue;
                    vectors.Add(AgentPerception.InitRay2D(range, -i * delta / 2, rotation, origin));
                    vectors.Add(AgentPerception.InitRay2D(range, i * delta / 2, rotation, origin));
                }
            }
			
            foreach (var vec in vectors)
                Handles.DrawDottedLine(origin.position, origin.position + vec, 2f);

            Handles.color = Color.white;

            var arcT = new GameObject("__arc_t__");
            arcT.transform.position = origin.position;
            arcT.transform.rotation = origin.rotation;
            arcT.transform.Rotate(arcT.transform.up, 90-fieldOfView/2 + 90-rotation);
            var arcRot = arcT.transform.right;
            var arcFrom = new Vector3(arcRot.x, arcRot.z, arcRot.y);
            var arcNormal = arcT.transform.forward;
            UnityEngine.Object.DestroyImmediate(arcT);
			
            Handles.DrawWireArc(origin.position, origin.forward, arcFrom, -fieldOfView, -range);
			
            Handles.DrawWireArc(origin.position, origin.forward, -origin.right, 360, _agentPerception.SeeEverythingRange);
            Handles.DrawWireArc(origin.position, origin.forward, -origin.right, 360, _agentPerception.SeeEverythingRange-0.05f);

            DrawDoubleSphere(Color.white, _agentPerception.Origin, 0.1f);
        }
        
        /// <summary>
        /// Visualize the perception for the Distance mode.
        /// </summary>
        private void OnSceneGUI_Perception_Distance()
        {
	        var origin = _agentPerception.Origin==null ? _agent.transform : _agentPerception.Origin;
	        var range = _agentPerception.Range;
	        
	        Handles.DrawWireDisc(origin.position, origin.up, range);
	        Handles.DrawWireDisc(origin.position, origin.forward, range);
	        Handles.DrawWireDisc(origin.position, origin.right, range);

	        if (_agentPerception.CanSeeEverythingAtRange)
	        {
		        Handles.DrawWireDisc(origin.position, origin.up, _agentPerception.SeeEverythingRange);
		        Handles.DrawWireDisc(origin.position, origin.up, _agentPerception.SeeEverythingRange - 0.05f);
	        }

	        DrawDoubleSphere(Color.white, _agentPerception.Origin, 0.1f);
        }
        
        /// <summary>
        /// Visualize 2-dimensional Distance mode.
        /// </summary>
        private void OnSceneGUI_Perception_Distance_2D()
        {
	        if (!_agentPerception) return;
	        var origin = _agentPerception.Origin==null ? _agent.transform : _agentPerception.Origin;
	        var range = _agentPerception.Range;
	        
	        Handles.DrawWireArc(origin.position, origin.forward, -origin.right, 360, range);
			
	        Handles.DrawWireArc(origin.position, origin.forward, -origin.right, 360, _agentPerception.SeeEverythingRange);
	        Handles.DrawWireArc(origin.position, origin.forward, -origin.right, 360, _agentPerception.SeeEverythingRange-0.05f);
	        
	        DrawDoubleSphere(Color.white, _agentPerception.Origin, 0.1f);
        }
        #endregion
        
        #region UTILITY

		/// <summary>
		/// Return a vector that points to a specific position.
		/// </summary>
		/// <param name="r"></param>
		/// <param name="fi"></param>
		/// <param name="offset"></param>
		/// <param name="center"></param>
		/// <returns></returns>
		private static Vector3 InitRay(float r, float fi, float offset, Transform center)
		{
			var offsetRads = offset * Mathf.PI / 180;
			fi -= center.eulerAngles.y;
			var rads = fi * Mathf.PI / 180;
			rads = rads%(Mathf.PI*2) + offsetRads%(Mathf.PI*2);
			var x = center.position.x + r * Mathf.Cos(rads);
			var z = center.position.z + r * Mathf.Sin(rads);
			return new Vector3(x, center.position.y, z);
		}

		/// <summary>
		/// Use Handles to draw two arcs in a row with slightly different radiuses.
		/// </summary>
		/// <param name="color"></param>
		/// <param name="fov"></param>
		/// <param name="origin"></param>
		/// <param name="range"></param>
		/// <param name="offset"></param>
		private void DrawDoubleArc(Color color, float fov, Transform origin, float range, float offset = 0, AgentPerception.Dimensions dimensions = AgentPerception.Dimensions.Three)
		{
			var startColor = Handles.color;
			var originPos = origin.position;
			
			var arcT = new GameObject("__arc_t__");
			arcT.transform.position = originPos;
			if (dimensions == AgentPerception.Dimensions.Three)
			{
				arcT.transform.rotation = new Quaternion(0f, origin.rotation.y, 0, origin.rotation.w);
				arcT.transform.Rotate(arcT.transform.up,
					180 - fov / 2 - _agentPerception.Offset + offset);
			}
			else
			{
				arcT.transform.rotation = new Quaternion(0f, 0f, _agent.transform.rotation.z, 1f);
				arcT.transform.Rotate(Vector3.forward,
					-90 - fov / 2 - _agentPerception.Offset + offset);
			}

			var arcRot = -arcT.transform.right;
			Object.DestroyImmediate(arcT);
			
			Handles.color = color;
			var normal = dimensions == AgentPerception.Dimensions.Three ? Vector3.up : Vector3.forward;
			Handles.DrawWireArc(originPos, normal, arcRot, fov, range);
			Handles.DrawWireArc(originPos, normal, arcRot, fov, range-0.05f);

			var startAim = AgentPerception.InitRay(range, fov/2, _agentPerception.Offset + offset, origin);
			var endAim = AgentPerception.InitRay(range, -fov/2, _agentPerception.Offset + offset, origin);
			
			Handles.DrawDottedLine(originPos, originPos+startAim, 1f);
			Handles.DrawDottedLine(originPos, originPos+endAim, 1f);

			Handles.color = startColor;
		}

		/// <summary>
		/// Use handles to draw a wired sphere.
		/// </summary>
		/// <param name="color"></param>
		/// <param name="origin"></param>
		/// <param name="radius"></param>
		private static void DrawSphere(Color color, Transform origin, float radius)
		{
			var startColor = Handles.color;
			Handles.color = color;
			try{
			Handles.DrawWireArc(origin.position, origin.up, -origin.right, 360, radius);
			Handles.DrawWireArc(origin.position, origin.forward, -origin.right, 360, radius);
			Handles.DrawWireArc(origin.position, origin.right, -origin.forward, 360, radius);
			}
			catch(System.Exception){}
			Handles.color = startColor;
		}

		/// <summary>
		/// Draw two wired spheres with slightly different radiuses.
		/// </summary>
		/// <param name="color"></param>
		/// <param name="origin"></param>
		/// <param name="radius"></param>
		private static void DrawDoubleSphere(Color color, Transform origin, float radius)
		{
			DrawSphere(color, origin, radius);
			DrawSphere(color, origin, radius+0.005f);
		}

		#endregion
		
		#region Inspector
		
		/// <summary>
		/// Variables used in the Editor for GUI drawing.
		/// </summary>
		#region InspectorVariables

		private SerializedProperty _perceptionMode;
		private SerializedProperty _perceptionDimensions;
		private SerializedProperty _perceptionMask;
		private SerializedProperty _ignoreObstacles;
		
		private SerializedProperty _offset;
		private SerializedProperty _fieldOfView;
		private SerializedProperty _resolution;
		private SerializedProperty _range;
		
		private SerializedProperty _canSeeEverythingAtRange;
		private SerializedProperty _seeEverythingMode;
		private SerializedProperty _seeEverythingRange;
		private SerializedProperty _seeEverythingResolution;
		
		private SerializedProperty _agentMemory;
		
		private SerializedProperty _visualize;
		private SerializedProperty _seenUnits;
		
		private bool _adjustOrigin = false;

		#endregion

		public AgentPerceptionEditor(AgentPerception agentPerception) : base(agentPerception)
		{
			_agentPerception = agentPerception;
		}

		/// <summary>
		/// Executed on selecting the object.
		/// </summary>
		public override void OnEnable()
		{
			_perceptionMode = serializedObject.FindProperty("PerceptionMode");
			_perceptionDimensions = serializedObject.FindProperty("PerceptionDimensions");
			_perceptionMask = serializedObject.FindProperty("_perceptionMask");
			_ignoreObstacles = serializedObject.FindProperty("IgnoreObstacles");
			
			_offset = serializedObject.FindProperty("_offset");
			_fieldOfView = serializedObject.FindProperty("_fieldOfView");
			_resolution = serializedObject.FindProperty("_resolution");
			_range = serializedObject.FindProperty("_range");
			
			_canSeeEverythingAtRange = serializedObject.FindProperty("_canSeeEverythingAtRange");
			_seeEverythingMode = serializedObject.FindProperty("_seeEverythingMode");
			_seeEverythingRange = serializedObject.FindProperty("_seeEverythingRange");
			_seeEverythingResolution = serializedObject.FindProperty("_seeEverythingResolution");
			
			_agentMemory = serializedObject.FindProperty("agentMemory");
			
			_visualize = serializedObject.FindProperty("_visualize");
			_seenUnits = serializedObject.FindProperty("_seenUnits");
			
		}
		
		/// <summary>
		/// Draw the Inspector GUI.
		/// </summary>
		public override void DrawInspector(AgentComponent agentPerception)
		{
			base.DrawInspector(agentPerception);
			
			agentPerception.displayEditor = EliotEditorUtility.DrawAgentComponentHeader<AgentPerception>("Eliot Agent Component: " + "Perception", agentPerception.displayEditor, agentPerception,
				EliotGUISkin.AgentComponentGreen);
			if (!agentPerception.displayEditor) return;
			
			EditorGUI.BeginChangeCheck();

			EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
			_adjustOrigin = GUILayout.Toggle(_adjustOrigin, "Adjust Origin", EditorStyles.toolbarButton);
			EditorGUILayout.EndVertical();
			if (_adjustOrigin) Tools.current = Tool.None;

			EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
			EditorGUILayout.PropertyField(_perceptionDimensions);
			EditorGUILayout.PropertyField(_perceptionMask);
			EditorGUILayout.PropertyField(_perceptionMode);
			EditorGUILayout.EndVertical();
			
			EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
			if (_agentPerception.PerceptionMode == AgentPerception.Modes.Distance ||
			    _agentPerception.PerceptionMode == AgentPerception.Modes.OverlapSphere)
			{
				EditorGUILayout.PropertyField(_ignoreObstacles);
			}
			else if (_agentPerception.PerceptionMode == AgentPerception.Modes.Raycasting)
			{
				EditorGUILayout.PropertyField(_offset);
				EditorGUILayout.PropertyField(_fieldOfView);
				EditorGUILayout.PropertyField(_resolution);
			}
			EditorGUILayout.PropertyField(_range);
			EditorGUILayout.EndVertical();
			
			EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
			EditorGUILayout.PropertyField(_canSeeEverythingAtRange);
			if (_agentPerception.CanSeeEverythingAtRange)
			{
				EditorGUILayout.PropertyField(_seeEverythingMode);
				EditorGUILayout.PropertyField(_seeEverythingRange);
				if (_agentPerception.SeeEverythingMode == AgentPerception.Modes.Raycasting)
				{
					EditorGUILayout.PropertyField(_seeEverythingResolution);
				}
			}
			EditorGUILayout.EndVertical();
			
			EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
			EditorGUI.indentLevel++;
			EditorGUILayout.PropertyField(_agentMemory, true);
			EditorGUI.indentLevel--;
			EditorGUILayout.EndVertical();
			
			EditorGUILayout.BeginVertical(EliotGUISkin.GrayBackground);
			EditorGUILayout.PropertyField(_visualize);
			if (Application.isPlaying)
			{
				GUI.enabled = false;
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField(_seenUnits, true);
				EditorGUI.indentLevel--;
				GUI.enabled = true;
			}

			EditorGUILayout.EndVertical();

			if (EditorGUI.EndChangeCheck())
			{
				Undo.RegisterCompleteObjectUndo(agentPerception, "Perception change");
				serializedObject.ApplyModifiedProperties();
			}
		}
		#endregion
    }
}