using System;
using System.Collections.Generic;
using Eliot.BehaviourEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Eliot.BehaviourEditor
{
	/// <summary>
	/// Represents the List of Transition Components of a Behaviour model.
	/// </summary>
	public class NodesTransition : ScriptableObject
	{
		/// <summary>
		/// The start position of the transition.
		/// </summary>
		public Vector3 StartPos { get; set; }

		/// <summary>
		/// The end position of the transition.
		/// </summary>
		public Vector3 EndPos { get; set; }

		/// <summary>
		/// The node from which the transition is started.
		/// </summary>
		public Node Start;

		/// <summary>
		/// The node to which the transition is pointing.
		/// </summary>
		public Node End;

		/// <summary>
		/// Color of the transition graphics.
		/// </summary>
		public Color Color;

		/// <summary>
		/// Whether the transition is in the negative group or not.
		/// </summary>
		public bool IsNegative
		{
			get { return _isNegative; }
			set { _isNegative = value; }
		}

		/// <summary>
		/// Whether the transition is currently selected.
		/// </summary>
		public bool IsSelected { private get; set; }

		/// Whether the transition is in the negative group or not.
		[HideInInspector] [SerializeField] private bool _isNegative;

		/// <summary>
		/// List of the actual transitions.
		/// </summary>
		public List<NodesTransitionData> TransitionsData = new List<NodesTransitionData>();

#if UNITY_EDITOR
		/// <summary>
		/// Reference to the editor window.
		/// </summary>
		protected BehaviourEditorWindow _window;

		/// <summary>
		/// Reference to the editor window.
		/// </summary>
		public BehaviourEditorWindow Window
		{
			get
			{
				if (_window == null)
				{
					_window = EditorWindow.GetWindow<BehaviourEditorWindow>("Behaviour");
				}

				return _window;
			}
			set { _window = value; }
		}
#endif

		/// <summary>
		/// Initialize the transition.
		/// </summary>
		/// <param name="start"></param>
		/// <param name="end"></param>
		/// <param name="color"></param>
		public NodesTransition(Node start, Node end, Color? color = null)
		{
			Start = start;
			End = end;
#if UNITY_EDITOR
			Color = color ?? EliotGUISkin.NeutralColor;
#endif
			name = "Transition";
			TransitionsData = new List<NodesTransitionData>();
			TransitionsData.Add(new NodesTransitionData(this));

		}
#if UNITY_EDITOR
		/// <summary>
		/// This function is called when object is loaded.
		/// </summary>
		public void OnEnable()
		{
			hideFlags = HideFlags.HideInHierarchy;
			if (Start && !Start.Transitions.Contains(this))
			{
				Start.Transitions.Add(this);
			}

			name = "Transition";

			if (TransitionsData == null || TransitionsData.Count == 0)
			{
				TransitionsData = new List<NodesTransitionData>();
				TransitionsData.Add(new NodesTransitionData(this));
			}

			foreach (var t in TransitionsData)
				if (t.MethodData == null)
				{
					t.MethodData = new MethodData();
					t.MinProbability = 100f;
					t.MaxProbability = 100f;
				}
		}

		/// <summary>
		/// Render the graphics.
		/// </summary>
		public void Draw()
		{
			if (!End || !End.Exist)
			{
				Delete(null);
				return;
			}

			var highlight = false;

			if (TransitionsData != null && TransitionsData.Count > 0)
				foreach (var t in TransitionsData)
					if (t.BindedCoreTransition != null && t.BindedCoreTransition.active)
					{
						highlight = true;
						break;
					}

			if (highlight)
				Highlight();

			StartPos = new Vector3(Start.Rect.x + Start.Rect.width / 2,
				Start.Rect.y + Start.Rect.height / 2, 0);
			EndPos = new Vector3(End.Rect.x + End.Rect.width / 2, End.Rect.y + End.Rect.height / 2, 0);

			var direction = EndPos - StartPos;
			var offset = Vector3.Cross(Vector3.forward, direction).normalized * 5;
			StartPos += offset;
			EndPos += offset;

			var _maxProbability = 0f;
			if (TransitionsData != null && TransitionsData.Count > 0)
				foreach (var t in TransitionsData)
				{
					if (t.MaxProbability > _maxProbability)
					{
						_maxProbability = t.MaxProbability;
					}
				}

			bool useCondition = false;
			bool captureControl = false;
			foreach (var t in TransitionsData)
			{
				if (t.UseCondition) useCondition = true;
				if (t.CaptureControl || t.CaptureControlOnTrue || t.CaptureControlOnFalse)
				{
					captureControl = true;
				}
			}


			var transparency = Mathf.Clamp((_maxProbability * 0.01f) + 0.2f, 0.5f, 2f);
			Color = new Color(Color.r, Color.g, Color.b, transparency);
			Handles.color = IsSelected ? EliotGUISkin.SelectedColor : Color;
			if (useCondition)
			{
				Vector3 start = StartPos;

				float scale = Vector3.Distance(StartPos, EndPos);
				float size = 3f;
				float curOffset = size;
				var curPos = Vector3.Lerp(StartPos, EndPos, curOffset / scale);
				while (curPos != EndPos || Vector3.Distance(curPos, EndPos) > 0.01f)
				{
					Handles.DrawAAPolyLine(4, start, curPos);
					curOffset += size * 2;
					start = Vector3.Lerp(StartPos, EndPos, curOffset / scale);
					curOffset += size * 2;
					curPos = Vector3.Lerp(StartPos, EndPos, curOffset / scale);
				}
			}
			else
			{
				Handles.DrawAAPolyLine(4, StartPos, EndPos);
			}

			DrawArrow(StartPos);

			if (captureControl)
			{
				DisplayCaptureControl();
			}

			if (!Start || !Start.Exist)
			{
				Delete(null);
			}
		}

		/// <summary>
		/// Visualize the fact that the transition is active.
		/// </summary>
		public void Highlight()
		{
			Handles.color = new Color(0f, 0.7f, 1f, 1f);
			Handles.DrawAAPolyLine(12, StartPos, EndPos);
		}

		/// <summary>
		/// Draw the context menu of the transition.
		/// </summary>
		public void DrawMenu()
		{
			var menu = new GenericMenu();
			menu.AddItem(new GUIContent("Delete"), false, Delete, null);
			menu.ShowAsContext();
		}

		/// <summary>
		/// Check whether specific point on the screen is inside the boundaries
		/// of the transition's graphics.
		/// </summary>
		/// <param name="point"></param>
		/// <returns></returns>
		public bool Contains(Vector2 point)
		{
			return Math.Abs(Vector2.Distance(StartPos, point) + Vector2.Distance(EndPos, point) -
			                Vector2.Distance(StartPos, EndPos)) < 0.2f;
		}

		/// <summary>
		/// Render the arrow on top of the transition.
		/// </summary>
		private void DrawArrow(Vector2 startPos)
		{
			var arrowHead = new Vector3[3];

			var forward = (Vector3) (End.Rect.center - Start.Rect.center).normalized;
			var right = Vector3.Cross(Vector3.forward, forward).normalized;
			var size = HandleUtility.GetHandleSize(End.Rect.center);
			var width = size * 0.3f;
			var height = size * 0.5f;

			var len = (End.Rect.center - Start.Rect.center).magnitude;
			Vector3 cen = startPos + (len * 0.5f + height / 2f) * (Vector2) forward;
			arrowHead[0] = cen;
			arrowHead[1] = cen - forward * height + right * width;
			arrowHead[2] = cen - forward * height - right * width;

			Handles.DrawAAConvexPolygon(arrowHead);
		}

		/// <summary>
		/// Visualize the fact that the transition might return the control to the start node.
		/// </summary>
		/// <param name="color"></param>
		private void DisplayCaptureControl(Color? color = null)
		{
			var arrowHead = new Vector3[3];

			var forward = (Vector3) (EndPos - StartPos).normalized;
			var right = Vector3.Cross(Vector3.forward, forward).normalized;
			var size = HandleUtility.GetHandleSize(End.Rect.center) * 0.8f;
			var width = size * 0.3f;
			var height = size * 0.5f;

			var len = (EndPos - StartPos).magnitude;
			Vector3 cen = (Vector2) StartPos + (len * 0.5f + height / 2f) * (Vector2) forward;
			arrowHead[0] = cen - forward * 25f;
			arrowHead[1] = cen + forward * height + right * width - forward * 25f;
			arrowHead[2] = cen + forward * height - right * width - forward * 25f;

			var c = Handles.color;
			if (color != null) Handles.color = color.Value;
			Handles.DrawAAConvexPolygon(arrowHead);
			Handles.color = c;
		}

		/// <summary>
		/// Remove this transition from the editor.
		/// </summary>
		/// <param name="obj"></param>
		public void Delete(object obj)
		{
			if (Start)
			{
				Start.RemoveTransition(this);
			}

			if (End)
			{
				Undo.RecordObject(End, "Transition is about to be removed");
				End.TransitionsIn.Remove(this);
			}

			Undo.DestroyObjectImmediate(this);
		}
#endif

		/// <summary>
		/// Build the behaviour core element(s).
		/// </summary>
		/// <param name="startComponent"></param>
		/// <param name="gameObject"></param>
		/// <param name="core"></param>
		/// <returns></returns>
		public List<BehaviourEngine.Transition> BuildBehaviourTransition(CoreComponent startComponent,
			GameObject gameObject, BehaviourCore core)
		{
			List<BehaviourEngine.Transition> result = new List<BehaviourEngine.Transition>();
			foreach (var t in TransitionsData)
			{
				result.Add(t.BuildBehaviourTransition(startComponent, gameObject, core));
			}

			return result;
		}

#if UNITY_EDITOR
		/// <summary>
		/// Remove the transition's association with a runtime behaviour core component for the debugging purposes. 
		/// </summary>
		public virtual void UnbindCoreTransition()
		{
			foreach (var t in TransitionsData)
				t.BindedCoreTransition = null;
			End.UnbindCoreComponent();
		}
#endif
	}
}