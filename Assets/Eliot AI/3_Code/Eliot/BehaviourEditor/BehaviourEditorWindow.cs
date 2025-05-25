using Eliot.Utility;
#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using Eliot.AgentComponents;
using Eliot.BehaviourEngine;
using Eliot.Repository;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Eliot.BehaviourEditor
{
    /// <summary>
    /// Editor window for Behaviour models. Modified to use new GUI.
    /// 
    /// benjamin.erwin@subliminumindustries.com 26.11.19 (c) Subliminum Industries
    /// 
    /// Eliot Copyright boilerplate should be added
    /// </summary>
    [Serializable] public class BehaviourEditorWindow : EditorWindow
	{
		/// <summary>
		/// Instance of this window. Makes sure there is only one instance of it.
		/// </summary>
		public BehaviourEditorWindow Instance
		{
			get
			{
				if (_instance == null)
					_instance = EditorWindow.GetWindow<BehaviourEditorWindow>("Behaviour");
				return _instance;
			}
			
		}

		/// <summary>
		/// Access the current open window from a static context.
		/// </summary>
		public static BehaviourEditorWindow StaticInstance
		{
			get
			{
				if (_staticInstance == null)
				{
					_staticInstance = EditorWindow.GetWindow<BehaviourEditorWindow>("Behaviour");
				}
				return _staticInstance;
			}
			
		}

		/// <summary>
		/// Get the entry or create one.
		/// </summary>
		public EntryNode Entry
		{
			get
			{
				if (_entry == null)
				{
                    Rect rect = new Rect(2000, 1000, EliotNodes.Entry.textureWidth, EliotNodes.Entry.textureHeight);
					_entry = CreateInstance<EntryNode>();
					_entry.NodeName = "Entry";
					_entry.name = "Entry";
					_entry.Behaviour = Behaviour;
					_entry.Rect = rect;
					Nodes.Add(_entry);
				}

				return _entry;
			}
		}
		
		/// List of nodes currently present in the window.
		public List<Node> Nodes = new List<Node>();
		/// Behaviour model currently being updated with the Editor.
		public EliotBehaviour Behaviour;
		/// Returns the currently selected Agent. Used for visualizing the currently active elements.
		public EliotAgent currentAgent;
		/// Origin of the zoom coordinates.
		public Vector2 zoomOrigin = Vector2.zero;
		/// Keep the static reference to an open window.
		public static BehaviourEditorWindow _staticInstance;
		/// Callbacks executed on adding transitions.
		public Action<Node, Node, NodesTransition> onTransitionAdded;
		/// Current window scale.
		public float Scale = 1f;
		/// Instance of this window. Can be only single one.
		[SerializeField] private BehaviourEditorWindow _instance;
		/// Link to the Entry of the current Behaviour model.
		private EntryNode _entry;
		/// Position of the mouse cursor on the screen.
		private Vector2 _mousePos = Vector2.zero;
		/// Whether the user is currently making a new transition.
		private bool _makeTransitionMode;
		/// Origin transform of a new transition.
		private Rect _newTransitionStart;
		/// Link to the node that is the origin of a new transition.
		private Node _nodeThatStartedTransition;
		/// Color of a new transition.
		private Color _transitionColor;
		/// Wheather a new transition is a negative one.
		private bool _transitionIsNeg;
		/// Original color of last selected transition.
		private Color _lastTransSelectedColor;
		/// Link to the last selected transition.
		private NodesTransition _selectedTransition;
		/// Position of the scroll rect.
		private Vector2 _scrollPosition = Vector2.zero;
		/// Borders of the window.
		private float _minX, _maxX, _minY, _maxY;
		/// Space which all the nodes are in.
		private Rect _spaceRect;
		/// Rect that user can currently see.
		private Rect _viewRect;
		/// Wheather the user is currently dragging the corner of the selection box.
		private bool _dragging;
		/// Wheather the user is currently dragging the group of selected nodes.
		private bool _draggingGroup;
		/// Start position of selection box.
		private Vector2 _startPos;
		/// Rect of rhe selection box.
		private Rect _selectionRect;
		/// Context menu of this window.
		private GenericMenu _genericMenu;
		/// Backgtound texture of this window.
		public Texture2D _backgroundTexture;
		/// Current event that is updated every OnGUI call.
		private Event _event;
		///	Last time user used shortcuts for nodes creation.
		private int _lastInputUpdate;
		/// Distance between border and the closest node to the border.
		private const float Padding  = 1000f;
		/// Path to the current behaviour.
		[SerializeField] private string _pathToBehaviour;
		
		private float zoomMin = 0.1f;
		private float zoomMax = 2.0f;

		/// <summary>
		/// Initialize the window.
		/// </summary>
		[MenuItem("Tools/Eliot AI/Behaviour Editor")]
		private static void InitWindow() 
		{
			GetWindow<BehaviourEditorWindow>("Behaviour");
		}

		/// <summary>
		/// Display the new behaviour.
		/// </summary>
		/// <param name="behaviour"></param>
		public void SetBehaviour(EliotBehaviour behaviour)
		{
			Behaviour = behaviour;
			Instance._pathToBehaviour = AssetDatabase.GetAssetPath(Behaviour);
			Reverse(null);
			if (Behaviour.Nodes.Count == 0)
			{
				Clear(null);
				Save(null);
			}
			Frame();
		}
		
		/// <summary>
		/// Convert the screen coordinates to the zoomed coordinates.
		/// </summary>
		/// <param name="screenCoords"></param>
		/// <returns></returns>
		private Vector2 ConvertScreenCoordsToZoomCoords(Vector2 screenCoords)
		{
			return (screenCoords - _spaceRect.TopLeft()) / Scale + zoomOrigin;
		}
		
		/// <summary>
		/// This function is called when the object is loaded.
		/// </summary>
		private void OnEnable ()
		{
			hideFlags = HideFlags.HideAndDontSave;

            _backgroundTexture = EliotProjectSettings.GUIResources.BE_BackgroundTexture;
            _backgroundTexture.wrapMode = TextureWrapMode.Repeat;
            _backgroundTexture.filterMode = FilterMode.Trilinear;

            titleContent = new GUIContent(" Behaviour", EliotProjectSettings.GUIResources.EditorIcon);

			if (_pathToBehaviour != null) 
				Behaviour = AssetDatabase.LoadAssetAtPath<EliotBehaviour>(_pathToBehaviour);
			
			try{ Reverse(null); }catch (System.Exception){/**/}

            if (Nodes != null && Nodes.Count > 0){ Frame();}

			_transitionColor = EliotGUISkin.NeutralColor;
			
		}

		/// <summary>
		/// Make sure there is exactly one Entry in the Editor.
		/// </summary>
		private void RemoveExtraEntry()
		{
			var entries = from entry in Nodes where entry is EntryNode select entry;
			var qNodes = entries.ToList();
			if (qNodes.Count <= 1) return;
			foreach (var entry in qNodes) 
				if(entry.Transitions.Count == 0) 
					Nodes.Remove(entry);
		}
		
		/// <summary>
		/// Update the window when mouse is over another one.
		/// </summary>
		private void Update()
		{
			if (_event == null) return;
			if (_event.type == EventType.MouseUp)
			{
				_draggingGroup = false;
				ClearGrouped();
				_dragging = false;

				foreach (Node node in Nodes)
				{
					if (node == null || !node.Exist) continue;
					if (!_selectionRect.Contains(node.Rect.center)) continue;
					node.Grouped = true;
				}
				_genericMenu = null;
				_event.Use();
			}
		}
		
		/// <summary>
		/// Adjust the zoom with the mouse wheel.
		/// </summary>
		private void HandleZooming()
		{
			// Adjust the zoom with the mouse wheel. 
			if (Event.current.type == EventType.ScrollWheel)
			{
                Vector2 screenCoordsMousePos = Event.current.mousePosition;
                Vector2 delta = Event.current.delta;
				Vector2 zoomCoordsMousePos = ConvertScreenCoordsToZoomCoords(screenCoordsMousePos);
                float zoomDelta = -delta.y / 25.0f;
				float oldZoom = Scale;
				Scale += zoomDelta;
				Scale = Mathf.Clamp(Scale, zoomMin, zoomMax);
				zoomOrigin += (zoomCoordsMousePos - zoomOrigin) - (oldZoom / Scale) * (zoomCoordsMousePos - zoomOrigin);
 
				Event.current.Use();
			}
		}

		/// <summary>
		/// Draw GUI and run listeners.
		/// </summary>
		private void OnGUI()
		{
			_event = Event.current;

			HandleZooming();

			#region BindCoreComponents

			if (EditorApplication.isPlaying)
			{
				if (Selection.activeGameObject)
				{
					var agent = Selection.activeGameObject.GetComponent<EliotAgent>();
					if (agent != currentAgent || Entry.BindedCoreComponent == null)
					{
						Entry.UnbindCoreComponent();
						if (agent && Entry && agent.BehaviourCore != null &&
						    agent.BehaviourCore.FullPath == Behaviour.GetFullPath())
						{
							Entry.BindToCoreComponent(agent.BehaviourCore.Entry);
							agent.BehaviourCore.OnReset = () =>
							{
								Entry.UnbindCoreComponent();
							};
						}

						currentAgent = agent;
					}
				}
			}
			else
			{
				if (Entry && Entry.BindedCoreComponent != null)
					Entry.UnbindCoreComponent();
			}

			#endregion

			var width = (_maxX - _minX + Padding > position.width ? _maxX - _minX + Padding : position.width) * 5;
			var height = (_maxY - _minY + Padding > position.height ? _maxY - _minY + Padding : position.height) * 5;

			_spaceRect = new Rect(0, 0, width, height);
			_viewRect = new Rect(-zoomOrigin.x, -zoomOrigin.y, width, height);
			
			EditorWindowZoomArea.Begin(Scale, _spaceRect);
			
			_scrollPosition = GUI.BeginScrollView(_viewRect, _scrollPosition, _spaceRect, GUIStyle.none, GUIStyle.none);

			GUI.DrawTextureWithTexCoords(new Rect(0, 0, width, height), _backgroundTexture,new Rect(0, 0, _spaceRect.width / _backgroundTexture.width*2f,_spaceRect.height / _backgroundTexture.height*2f));

			UpdateShortcuts();

			InitEntry();
			RemoveExtraEntry(); //Get rid of this

			DragMultipleWindows(); //Improve this

			_mousePos = Event.current.mousePosition;
			if (!_makeTransitionMode)
			{
				EditorWindowZoomArea.Pause(Scale, _spaceRect);
				UpdateContextMenu();
				EditorWindowZoomArea.Resume();

				UpdateSelected();
				if (Behaviour)
					UpdateNodes();
				UpdateSelectedTransitions();
			}
			else
			{
				if (Event.current.type != EventType.Layout)
					UpdateNewTransition(_transitionColor, _transitionIsNeg);
				Repaint();
				if(Behaviour)
					UpdateNodes();
			}

			GUI.EndScrollView();
			EditorWindowZoomArea.End();
			
			if (!Behaviour)
			{
				var rectCol = Color.grey;
				rectCol.a = 0.5f;
                Rect rect = new Rect(0, 0, 500, 150);
				rect.center = new Vector2(this.position.width/2, this.position.height/2);
				Handles.DrawSolidRectangleWithOutline(rect, rectCol, rectCol);
                GUI.Label(rect, "Drag & Drop a Behaviour to begin editing", new GUIStyle(EliotGUISkin.LabelBoldCenteredBig) { alignment = TextAnchor.MiddleCenter,fontSize = 20}); 
            }
			
			DoDragAndDrop();
			PanScrollView();
			SelectionBox();
			ClampView();

            #region FileNameBox
            GUIStyle FileNameBox = new GUIStyle()
            {
                fontSize = 12,
                alignment = TextAnchor.MiddleRight,
                normal =
                {
	                
#if UNITY_PRO_LICENSE
					textColor = Color.white
#else
	                textColor = Color.black
#endif
                }
            };
			Rect fileNameBoxRect = new Rect(0, position.height-24, position.width+2, 24f);
            GUI.Box(fileNameBoxRect, "");

			var fileNameLabelRect = new Rect(0, position.height-24, position.width-5, 24f);
			string fileName = Behaviour!=null?(Behaviour.name):"";
			GUI.Label(fileNameLabelRect, fileName,FileNameBox);
			#endregion
		}

		/// <summary>
		/// Get user input and execute the associated action.
		/// </summary>
		private void UpdateShortcuts()
		{
			try
			{
				if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Delete)
				{
					Delete(null);
					Save(null);
					Event.current.Use();
				}

				if (Event.current.control && Event.current.keyCode == KeyCode.C &&
				    Event.current.type == EventType.KeyDown)
				{
					CopyBuffer.Copy(Selection.objects);
					Event.current.Use();
				}

				if (Event.current.control && Event.current.keyCode == KeyCode.V &&
				    Event.current.type == EventType.KeyDown)
				{
					CopyBuffer.Paste();
					Event.current.Use();
				}


				foreach (EliotKeyBinding keyBinding in EliotProjectSettings.KeyBindings)
				{
					var buttonsDown = true;
					foreach (var key in keyBinding.Keys)
					{
						if (!(Event.current.type == EventType.KeyDown && Event.current.keyCode == key))
						{
							buttonsDown = false;
						}
					}

					if (buttonsDown)
					{
						switch (keyBinding.keyBindingOptions)
						{
							case (EliotKeyBinding.KeyBindingOptions.CreateInvoker):
								AddNode("Action");
								Event.current.Use();
								break;
							case (EliotKeyBinding.KeyBindingOptions.CreateObserver):
								AddNode("Condition");
								Event.current.Use();
								break;
							case (EliotKeyBinding.KeyBindingOptions.CreateSkill):
								AddNode("Skill");
								Event.current.Use();
								break;
							case (EliotKeyBinding.KeyBindingOptions.CreateTime):
								AddNode("Time");
								Event.current.Use();
								break;
							case (EliotKeyBinding.KeyBindingOptions.CreateUtility):
								AddNode("Utility");
								Event.current.Use();
								break;
							case (EliotKeyBinding.KeyBindingOptions.Frame):
								Frame();
								Event.current.Use();
								break;
							case (EliotKeyBinding.KeyBindingOptions.StartDefaultTransition):
							{
								var node = HoverOnNode();
								if (node != null)
								{
									StartTransition(node.Rect, node, (node is ConditionNode ? EliotGUISkin.PositiveColor : node is LoopNode ? EliotGUISkin.LoopColor : EliotGUISkin.NeutralColor), false);
									Event.current.Use();
								}
							}
								break;
							case (EliotKeyBinding.KeyBindingOptions.StartAlternativeTransition):
							{
								var node = HoverOnNode();
								if (node != null)
								{
									StartTransition(node.Rect, node,
										(node is ConditionNode ? EliotGUISkin.NegativeColor : EliotGUISkin.NeutralColor), true);
									Event.current.Use();
								}
							}
								break;
						}
					}
				}
			}
			catch (System.Exception)
			{
				/*Its ok*/
			}
		}

		/// <summary>
		/// Update the dragging multiple nodes at once functionality.
		/// </summary>
		private void DragMultipleWindows()
		{
			if (!_draggingGroup && Selection.objects.Length > 0 && Event.current.button == 0 
			    && Event.current.type == EventType.MouseDown 
			    && Selection.objects.Contains(HoverOnNode(true)))
			{
				_draggingGroup = true;
			}

			if (Event.current.type == EventType.MouseUp)
			{
				_draggingGroup = false;
			}

			if (Event.current.type == EventType.MouseDrag && Event.current.button == 0 && _draggingGroup)
			{
				var delta = Event.current.delta;
				foreach (var node in Selection.objects)
				{
					if (!(node is Node)) continue;
					if (!(node as Node).Grouped) continue;
					var rect = ((Node) node).Rect;
					rect.position += delta;
					((Node) node).Rect = rect;
				}
			}

			if (Selection.objects.Length < 1)
			{
				ClearGrouped();
				_draggingGroup = false;
			}
			else if (Selection.objects.Length == 1)
			{
				if (!(Selection.objects[0] is Node)) return;
				GUI.FocusWindow(((Node) Selection.objects[0]).EditorId);
			}
			else if (Selection.objects.OfType<Node>().Any(node => !node.Grouped))
			{
				ClearGrouped();
				_draggingGroup = false;
			}
		}

		/// <summary>
		/// Update the pan view functionality.
		/// </summary>
		private void PanScrollView()
		{
			if (Event.current.type == EventType.MouseDrag && (Event.current.button == 2 || (Event.current.alt && Event.current.button == 0)))
			{
				var delta = Event.current.delta / Scale;
				_scrollPosition -= delta;
				zoomOrigin -= delta;
			}
		}

		/// <summary>
		/// Prevent the view position from going outside certain boundaries.
		/// </summary>
		private void ClampView()
		{
			var scrollX = Mathf.Clamp(_scrollPosition.x, 0, _maxX);
			var scrollY = Mathf.Clamp(_scrollPosition.y, 0, _maxY);
			_scrollPosition = new Vector2(scrollX, scrollY);

			var zoomX = Mathf.Clamp(zoomOrigin.x, 0, _maxX);
			var zoomY = Mathf.Clamp(zoomOrigin.y, 0, _maxY);
			zoomOrigin = new Vector2(zoomX, zoomY);
		}

		/// <summary>
		/// Clear the nodes group selection.
		/// </summary>
		private void ClearGrouped()
		{
			foreach (var node in Nodes)
			{
				if (node == null) continue;
				if (node.Grouped)
					node.Grouped = false;
			}
		}

		/// <summary>
		/// Update the load Behaviour by dropping it onto window functionality.
		/// </summary>
		private void DoDragAndDrop()
		{
			if (Event.current.type == EventType.DragUpdated || Event.current.type == EventType.DragPerform)
			{
				// Show a copy icon on the drag
				DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

				if (Event.current.type == EventType.DragPerform)
				{
					DragAndDrop.AcceptDrag();
					if (DragAndDrop.objectReferences.Length == 1 && DragAndDrop.objectReferences[0] is EliotBehaviour)
					{
						Behaviour = DragAndDrop.objectReferences[0] as EliotBehaviour;
						_pathToBehaviour = AssetDatabase.GetAssetPath(Behaviour);

						if (!string.IsNullOrEmpty(Behaviour.Json))
						{
							Behaviour.Json = BehaviourVersionManager.UpdateJson(Behaviour.Json);
							Behaviour.Serialize();
							Behaviour.Json = "";
						}
						Reverse(null);
						
					}
					Frame();
				}
				
				Event.current.Use(); 
			}
		}
		
		/// <summary>
		/// Update the selection box.
		/// </summary>
		private void SelectionBox()
		{
			_mousePos = Event.current.mousePosition;
			if (!Event.current.alt && Event.current.type == EventType.MouseDrag && Event.current.button == 0 && HoverOnNode(true) == null &&
			    HoverOnTransition() == null)
			{
				
				if (!_dragging)
				{
					_dragging = true;
					_startPos = _mousePos;
				}
			}

			if (Event.current.type == EventType.MouseUp)
			{
				_draggingGroup = false;
				ClearGrouped();
				_dragging = false;

				var selectedNodes = new List<Object>();
				foreach (var node in Nodes)
				{
					if (node == null) continue;
					if (!_selectionRect.Contains( node.Rect.center )) continue;
					selectedNodes.Add(node);
					node.Grouped = true;
				}

				if(!(Selection.activeObject is NodesTransition))
					Selection.objects = selectedNodes.ToArray();
			}

			if (_dragging)
			{
				Handles.color = Color.white;
				
				var x = _startPos.x > _mousePos.x ? _mousePos.x : _startPos.x;
				var y = _startPos.y > _mousePos.y ? _mousePos.y : _startPos.y;

				var w = Mathf.Abs(_mousePos.x - _startPos.x);
				var h = Mathf.Abs(_mousePos.y - _startPos.y);
				var v = ConvertScreenCoordsToZoomCoords( new Vector2(x , y) );
				_selectionRect = new Rect(v.x, v.y, w / Scale, h / Scale);
				Handles.DrawSolidRectangleWithOutline(new Rect(x, y , w, h), EliotGUISkin.SelectionBoxColor, EliotGUISkin.SelectionBoxOutlineColor);
			}

			Instance.Repaint();
		}

		/// <summary>
		/// Update each node rendering and functions.
		/// </summary>
		private void UpdateNodes()
		{
			if (Nodes.Count > 0) //Shouldn't Node Count always be over 0 if there is an Entry added to each one?
			{
				int id = 0;
				
				BeginWindows();

				_minX = _maxX = _minY = _maxY = 0;
                foreach (Node node in Nodes)
                {
                    if (node == null || !node.Exist) { continue; }

                    // Convert over to the new naming convention
                    {
	                    if (node.NodeName == "Invoker")
	                    {
		                    node.NodeName = "Action";
		                    node.NodeId = node.NodeName;
		                    node.name = node.NodeName;
	                    }

	                    if (node.NodeName == "Observer")
	                    {
		                    node.NodeName = "Condition";
		                    node.NodeId = node.NodeName;
		                    node.name = node.NodeName;
	                    }
                    }
                    
                    Rect normalRect = node.Rect;
                    var nodeSettings = EliotNodes.GetNode(node.NodeName);
                    node.Rect = new Rect(node.Rect.position, new Vector2(nodeSettings.textureWidth, nodeSettings.textureHeight));
                    normalRect.center = new Vector2(Mathf.Floor(normalRect.center.x - normalRect.center.x % 10), Mathf.Floor(normalRect.center.y - normalRect.center.y % 10));
					node.Rect = GUI.Window(id++, node.Grouped ? node.Rect : normalRect, node.NodeFunction, node.NodeName, node.Func.ToUpper() == "BREAK" && !node.Grouped ? EliotGUISkin.InvokerNode_Break: EliotGUISkin.GetNodeStyle(node.NodeName, node.Grouped ? 1 : 0));

					if (EditorApplication.isPlaying)
					{
						if (Entry.BindedCoreComponent != null)
						{
							HighlightNode(node);
						}
					}

					node.Update();

					var rX = node.Rect.x;
					if (rX < _minX) _minX = rX;
					else if (rX > _maxX)
						_maxX = rX;

					var rY = node.Rect.y;
					if (rY < _minY) _minY = rY;
					else if (rY > _maxY)
						_maxY = rY;
					
				}

				EndWindows();

				foreach (var node in Nodes)
				{
					if (node == null || !node.Exist) continue;
					node.LateUpdate();
				}
			}
		}

		/// <summary>
		/// Visualize the Node if it is active.
		/// </summary>
		/// <param name="node"></param>
		private void HighlightNode(Node node)
		{
			if (node.BindedCoreComponent == null) return;
			if (!(node.BindedCoreComponent.Active)) return;
			var c = GUI.color;
			var status = node.BindedCoreComponent.Status;
			
			switch (status)
			{
				case CoreComponentStatus.Normal: GUI.color = EliotGUISkin.NodeStatusNormal;
					break;
				case CoreComponentStatus.Warning: GUI.color = EliotGUISkin.NodeStatusWarning;
					break;
				case CoreComponentStatus.Error: GUI.color = EliotGUISkin.NodeStatusError;
					break;
			}

            Rect highlightRect = new Rect(node.Rect.x, node.Rect.y, EliotNodes.GetNode(node.NodeName).textureWidth, EliotNodes.GetNode(node.NodeName).textureHeight);
            GUI.DrawTexture(highlightRect, node.Func.ToUpper() == "BREAK" ? EliotProjectSettings.GUIResources.rectBreakGlow : EliotNodes.GetNode(node.NodeName).ShadowTexture);
			GUI.color = c;
		}

		/// <summary>
		/// Initialize a new transition.
		/// </summary>
		/// <param name="start"></param>
		/// <param name="node"></param>
		/// <param name="color"></param>
		/// <param name="isNegative"></param>
		public void StartTransition(Rect start, Node node, Color color, bool isNegative = false, Action<Node, Node, NodesTransition> onTransitionAddedAction = null)
		{
			_transitionColor = color;
			_transitionIsNeg = isNegative;
			_makeTransitionMode = true;
			_newTransitionStart = start;
			_nodeThatStartedTransition = node;
			onTransitionAdded = onTransitionAddedAction;
			
			ClearGrouped();
			_draggingGroup = false;
		}
		
		/// <summary>
		/// Update parameters of new transition while it is not a part of model yet.
		/// </summary>
		/// <param name="color"></param>
		/// <param name="isNegative"></param>
		private void UpdateNewTransition(Color? color = null, bool isNegative = false)
		{
			if (color == null) color = EliotGUISkin.NeutralColor;
			if (ClickedMouse())
			{
				var hover = HoverOnNode();
				if (hover == null){
					_makeTransitionMode = false;
					return;
				}
				if (_nodeThatStartedTransition.Equals(hover) || Entry.Equals(hover)) {
					_makeTransitionMode = false;
					return;
				}
				
				var transition = _nodeThatStartedTransition.AddTransition(_nodeThatStartedTransition, hover, color, isNegative);
				_makeTransitionMode = false;
				_transitionColor = EliotGUISkin.NeutralColor;
				_transitionIsNeg = false;
				
				ClearGrouped();
				_draggingGroup = false;

                if(onTransitionAdded != null)
				    onTransitionAdded(_nodeThatStartedTransition, hover, transition);
			}
			
			var startPos = new Vector3(_newTransitionStart.x + _newTransitionStart.width / 2, 
				_newTransitionStart.y + _newTransitionStart.height / 2, 0);
			var endPos = new Vector3(Event.current.mousePosition.x, Event.current.mousePosition.y, 0);
			
			Handles.color = color.Value;
			Handles.DrawAAPolyLine(4, startPos, endPos);
			DrawArrow(startPos, endPos);

			if (Event.current.type != EventType.Repaint)
				Event.current.Use();
		}

		/// <summary>
		/// Draw the arrow on a transition.
		/// </summary>
		/// <param name="start"></param>
		/// <param name="end"></param>
		private void DrawArrow(Vector3 start, Vector3 end)
		{
			var arrowHead = new Vector3[3];
			
			var forward = (end - start).normalized;
			var right = Vector3.Cross(Vector3.forward, forward).normalized;
			var size = HandleUtility.GetHandleSize(end);
			var width = size * 0.3f;
			var height = size * 0.5f;

			var len = (end - start).magnitude;
			var cen = start + (len*0.5f+height/2f) * forward;
			arrowHead[0] = cen;
			arrowHead[1] = cen - forward * height + right*width;
			arrowHead[2] = cen - forward * height - right*width;
			
			Handles.DrawAAConvexPolygon(arrowHead);
		}
		
		/// <summary>
		/// Update the editor's context menu.
		/// </summary>
		private void UpdateContextMenu()
		{
			if (ClickedMouseRight())
			{
				var hoverNode = HoverOnNode();
				if (hoverNode != null)
					hoverNode.DrawMenu();
				else if (HoverOnTransition() != null)
					HoverOnTransition().DrawMenu();
				else
					DrawContextMenu();
				
				Event.current.Use(); 
			}
		}

		/// <summary>
		/// Remove specific node from the editor.
		/// </summary>
		/// <param name="node"></param>
		public void RemoveNode(Node node)
		{
			try
			{
				if (Nodes.Contains(node))
				{
					Undo.RecordObject(Behaviour, "Node is about to be removed");
					Undo.RecordObject(node, "Node is about to be removed");
					Undo.RecordObjects(node.Transitions.ToArray(), "Node is about to be removed");
					
					
					foreach (var transition in node.Transitions)
						transition.Delete(null);
					Undo.DestroyObjectImmediate(node);
					Nodes.Remove(node);
				}
			}
			catch(System.Exception){/**/}
			Selection.activeObject = null;
		}

		/// <summary>
		/// Update the functionality of selection of a transition.
		/// </summary>
		private void UpdateSelectedTransitions()
		{
			if (!ClickedMouse()) return;
			if (Nodes == null || Nodes.Count <= 0) return;
			var hover = HoverOnTransition();
			if (hover != null)
			{
				if(Selection.activeObject != null)
					Undo.RecordObject(Selection.activeObject, "Selection.activeObject");
				_selectedTransition = hover;
				hover.IsSelected = true;
				Selection.activeObject = hover;
			}
			else if (Selection.objects.Length == 0)
			{
				if(Selection.activeObject != null)
					Undo.RecordObject(Selection.activeObject, "Selection.activeObject");
				Selection.activeObject = null;
			}
			_genericMenu = null;
		}
		
		/// <summary>
		/// Update functionality of selection of a node.
		/// </summary>
		private void UpdateSelected()
		{
			if (!ClickedMouseLeft() || _draggingGroup) return;
			Node hover = HoverOnNode();
			if (hover != null)
            {
				if(Selection.activeObject != null)
                {
                    Undo.RecordObject(Selection.activeObject, "Selection.activeObject");
                }
				Selection.activeObject = hover;
				if(_selectedTransition != null)
                {
                    _selectedTransition.IsSelected = false;
                }
			}
		}

		/// <summary>
		/// Whether any mouse button has been clicked.
		/// </summary>
		/// <returns></returns>
		private bool ClickedMouse()
		{
			return Event.current.type == EventType.MouseDown;
		}
		
		/// <summary>
		/// Whether the right mouse button has been clicked.
		/// </summary>
		/// <returns></returns>
		private bool ClickedMouseRight()
		{
			return Event.current.type == EventType.MouseDown && Event.current.button == 1;
		}
		
		/// <summary>
		/// Whether the left mouse button has been clicked.
		/// </summary>
		/// <returns></returns>
		private bool ClickedMouseLeft()
		{
			return Event.current.type == EventType.MouseDown && Event.current.button == 0;
		}

		/// <summary>
		/// Return the node which the mouse cursor is currently over or null if there is no such node.
		/// </summary>
		/// <returns></returns>
		private Node HoverOnNode(bool grouped = false)
		{
			if (Nodes == null || Nodes.Count <= 0) return null;
			foreach (Node node in Nodes)
				try
				{
					if (!grouped)
					{
						if (node.Rect.Contains( _mousePos ))
							return node;
					}
					else
					{
						var nodeRect = node.Rect;
						nodeRect.center -= (_scrollPosition  + zoomOrigin ) ;
						if (nodeRect.Contains( _mousePos/Scale ))
							return node;
					}
				}catch(System.Exception){return null;}
			return null;
		}
		
		/// <summary>
		/// Return the transition which the mouse cursor is currently over or null if there is no such transition.
		/// </summary>
		/// <returns></returns>
		private NodesTransition HoverOnTransition()
		{
			NodesTransition res = null;
			try
			{
				foreach (var node in Nodes)
				foreach (var trans in node.Transitions)
					if (trans.Contains( _mousePos ))
						res = trans;
					else
						trans.IsSelected = false;
			}
			catch(System.Exception) {res = null;}
			return res;
		}

		/// <summary>
		/// Draw the context menu.
		/// </summary>
		private void DrawContextMenu()
		{
			_genericMenu = new GenericMenu();
			_genericMenu.AddItem(new GUIContent("Add Node/Action"), false, AddNode, "Action");
			_genericMenu.AddItem(new GUIContent("Add Node/Condition"), false, AddNode, "Condition");
			_genericMenu.AddItem(new GUIContent("Add Node/Loop"), false, AddNode, "Loop");
			_genericMenu.AddSeparator("Add Node/");
			_genericMenu.AddItem(new GUIContent("Add Node/Skill"), false, AddNode, "Skill");
			_genericMenu.AddItem(new GUIContent("Add Node/Time"), false, AddNode, "Time");
			_genericMenu.AddSeparator("Add Node/");
			_genericMenu.AddItem(new GUIContent("Add Node/Utility"), false, AddNode, "Utility");
			_genericMenu.AddSeparator("");
			_genericMenu.AddItem(new GUIContent("Save"), false, Save, null);
			//_genericMenu.AddItem(new GUIContent("Reverse"), false, Reverse, null);
			_genericMenu.AddItem(new GUIContent("Select file"), false, SelectFile, null);
            _genericMenu.AddItem(new GUIContent("Settings"), false, DisplaySettings, null);
            _genericMenu.AddSeparator("");
			_genericMenu.AddItem(new GUIContent("Clear"), false, Clear, null);
			_genericMenu.ShowAsContext();
			
			Event.current.Use();
		}

		/// <summary>
		/// Set the current Behaviour as an active object of the Selection.
		/// </summary>
		/// <param name="obj"></param>
		private void SelectFile(object obj)
		{
			Selection.activeObject = Behaviour;
			_genericMenu = null;
		}
		
		/// <summary>
		/// Select the current settings scriptable object.
		/// </summary>
		/// <param name="obj"></param>
		private void DisplaySettings(object obj)
        {
            Selection.activeObject = Instance;
            _genericMenu = null;
        }

        /// <summary>
        /// Save the changes made in the editor to the Behaviour.
        /// </summary>
        /// <param name="obj"></param>
        public void Save(object obj)
		{
			try
			{
				if (Behaviour != null)
				{
					for (var i=Nodes.Count-1;i>=0; i--)
					{
						try
						{
							if (Nodes[i] == null)
								Nodes.RemoveAt(i);
						}
						catch (System.Exception)
						{
							Nodes.RemoveAt(i);
						}
					}
					
					Behaviour.Nodes = Nodes;
					Behaviour.InitTransitions(Nodes);
					Behaviour.Save();
					EditorUtility.SetDirty(Behaviour);

					CleanUp();
					AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(Behaviour));
				}

				_genericMenu = null;
			}
			catch (System.Exception e)
			{
				Debug.LogError(e);
			}
		}

        /// <summary>
        /// Remove the extra objects.
        /// </summary>
		private void CleanUp()
		{
			Undo.RecordObject(Behaviour, "Cleaning up");
			Object[] data = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(Behaviour));
			
			var transitions = new List<NodesTransition>();
			for (int i = Behaviour.Nodes.Count-1; i >= 0; i--)
			{
				if (Behaviour.Nodes[i] == null || !Behaviour.Nodes[i].Exist)
				{
					Behaviour.Nodes.RemoveAt(i);
				}
				else
				{
					for (int j = Behaviour.Nodes[i].Transitions.Count - 1; j >= 0; j--)
					{
						if (Behaviour.Nodes[i].Transitions[j] == null)
						{
							Behaviour.Nodes[i].Transitions.RemoveAt(j);
						}
						else
						{
							transitions.Add(Behaviour.Nodes[i].Transitions[j]);
						}
					}
				}
			}
			
			foreach (var entry in data)
			{
#if UNITY_2019
				if(!Behaviour.Nodes.Contains(entry) && !(entry is EliotBehaviour) && !transitions.Contains(entry))
#else
				if(!Behaviour.Nodes.Contains<Node>(entry as Node) && !(entry is EliotBehaviour) && !transitions.Contains<NodesTransition>(entry as NodesTransition))
#endif
				{
					Object.DestroyImmediate(entry, true);
				}
			}
		}

		/// <summary>
		/// Load the last saved version of the Behaviour.
		/// </summary>
		/// <param name="obj"></param>
		public void Reverse(object obj)
		{
			try
			{
				if (Behaviour == null)
				{
					return;
				}

				Behaviour.Load();
				Nodes = Behaviour.Nodes;
				foreach (var node in Behaviour.Nodes)
				{
					var entry = node as EntryNode;
					if (entry != null) _entry = entry;
					break;
				}

				_genericMenu = null;
			}
			catch (System.Exception e)
			{
				Debug.LogError(e);
			}
		}
		
		/// <summary>
		/// Remove selected nodes from the editor.
		/// </summary>
		/// <param name="obj"></param>
		private void Delete(object obj)
		{
			var selectedObjects = Selection.objects;
			if (selectedObjects.Length > 0)
			{
				foreach (var node in selectedObjects)
				{
					if(node is Node && !(node is EntryNode))
						(node as Node).Delete(null);
					if(node is NodesTransition)
						(node as NodesTransition).Delete(null);
				}
			}
			_genericMenu = null;
		}
		
		/// <summary>
		/// Add a new node to the Editor.
		/// </summary>
		/// <param name="obj"></param>
		private void AddNode(object obj)
		{
			string str = (string)obj;
			switch (str)
			{
				case "Action":
				{
                    Rect rect = new Rect(0, 0, EliotNodes.Action.textureWidth, EliotNodes.Action.textureHeight) {  position = ConvertScreenCoordsToZoomCoords(_mousePos)};
					Node node = CreateInstance<ActionNode>();
					EditorUtility.SetDirty(node);
					node.NodeName = "Action";
					node.name = "Action";
					node.Rect = rect;
					node.Behaviour = Behaviour;
					if(!Nodes.Contains(node)) Nodes.Add(node);
					Selection.activeObject = node;
					break;
				}
				case "Condition":
				{
					Rect rect = new Rect(0, 0, EliotNodes.Condition.textureWidth, EliotNodes.Condition.textureHeight) { position = ConvertScreenCoordsToZoomCoords(_mousePos)};
					Node node = CreateInstance<ConditionNode>();
					EditorUtility.SetDirty(node);
					node.NodeName = "Condition";
					node.name = "Condition";
					node.Rect = rect;
					node.Behaviour = Behaviour;
					if(!Nodes.Contains(node)) Nodes.Add(node);
					Selection.activeObject = node;
					break;
				}
				case "Loop":
				{
					Rect rect = new Rect(0, 0, EliotNodes.Loop.textureWidth, EliotNodes.Loop.textureHeight) { position = ConvertScreenCoordsToZoomCoords(_mousePos)};
                    Node node = CreateInstance<LoopNode>();
					EditorUtility.SetDirty(node);
					node.NodeName = "Loop";
					node.name = "Loop";
					node.Rect = rect;
					node.Behaviour = Behaviour;
					if(!Nodes.Contains(node)) Nodes.Add(node);
					Selection.activeObject = node;
					break;
				}
				
				case "Skill":
				{
					Rect rect = new Rect(0, 0, EliotNodes.Skill.textureWidth, EliotNodes.Skill.textureHeight) { position = ConvertScreenCoordsToZoomCoords(_mousePos)};
                    Node node = CreateInstance<SkillNode>();
					EditorUtility.SetDirty(node);
					node.NodeName = "Skill";
					node.name = "Skill";
					node.Rect = rect;
					node.Behaviour = Behaviour;
					if(!Nodes.Contains(node)) Nodes.Add(node);
					Selection.activeObject = node;
					break;
				}
				case "Time":
				{
					Rect rect = new Rect(0, 0, EliotNodes.Time.textureWidth, EliotNodes.Time.textureHeight){ position = ConvertScreenCoordsToZoomCoords(_mousePos)};
                    Node node = CreateInstance<TimeNode>();
					EditorUtility.SetDirty(node);
					node.NodeName = "Time";
					node.name = "Time";
					node.Rect = rect;
					node.Behaviour = Behaviour;
					if(!Nodes.Contains(node)) Nodes.Add(node);
					Selection.activeObject = node;
					break;
				}
				
				case "Utility":
				{
					Rect rect = new Rect(0, 0, EliotNodes.Utility.textureWidth, EliotNodes.Utility.textureHeight) { position = ConvertScreenCoordsToZoomCoords(_mousePos)};
                    Node node = CreateInstance<UtilityNode>();
					EditorUtility.SetDirty(node);
					node.NodeName = "Utility";
					node.name = "Utility";
					node.Rect = rect;
					node.Behaviour = Behaviour;
					if(!Nodes.Contains(node)) Nodes.Add(node);
					Selection.activeObject = node;
					break;
				}
			}
			_genericMenu = null;
		}

		/// <summary>
		/// Initialize Entry's parameters.
		/// </summary>
		private void InitEntry()
		{
			if (_entry != null || Nodes.Contains(_entry))
			{
				return;
			}
			Rect rect = new Rect(2000, 1000, EliotNodes.Entry.textureWidth, EliotNodes.Entry.textureHeight);
			_entry = CreateInstance<EntryNode>();
			_entry.NodeName = "Entry";
			_entry.name = "Entry";
			_entry.Behaviour = Behaviour;
			_entry.Rect = rect;
			Nodes.Add(_entry);
			
			zoomOrigin = Entry.Rect.center - new Vector2(Instance.position.width/2, Instance.position.height/2) / Scale;
		}
		
		/// <summary>
		/// Clear the editor and initialize the entry.
		/// </summary>
		/// <param name="obj"></param>
		public void Clear(object obj)
		{
			Nodes = new List<Node>();
			_entry = null;
			InitEntry();
		}

		/// <summary>
		/// Create a new empty Node of certain type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static Node TemplateNode(string type)
		{
			Node node = null;
			switch (type)
			{
				case "Entry":
					node = CreateInstance<EntryNode>();
					break;
				case "Action":
					node = CreateInstance<ActionNode>();
					break;
				case "Condition":
					node = CreateInstance<ConditionNode>();
					break;
				case "Loop":
					node = CreateInstance<LoopNode>();
					break;
			}
			return node;
		}

		/// <summary>
		/// Create a new empry transition.
		/// </summary>
		/// <returns></returns>
		public static NodesTransition TemplateTransition()
		{
			return CreateInstance<NodesTransition>();
		}

		/// <summary>
		/// Center the rect view on the nodes.
		/// </summary>
		public void Frame()
		{
			if (!Behaviour) return;
			if (Nodes.Count == 0) return;
			var centerX = 0f;
			var centerY = 0f;
			foreach (var node in Nodes)
			{
				if (!node) continue;
				centerX += node.Rect.center.x;
				centerY += node.Rect.center.y;
			}
			centerX /= Nodes.Count;
			centerY /= Nodes.Count;

			var centeredPosition = new Vector2(centerX, centerY);
			
			zoomOrigin = centeredPosition - new Vector2(Instance.position.width/2, Instance.position.height/2) / Scale;
		}
	}
}
#endif