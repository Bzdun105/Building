using UnityEngine;
using UnityEditor;

using System.Collections.Generic;

namespace ExtendingTools.PlaceTool
{
	public class GameObjectEditorWindow : EditorWindow
	{

		GameObject gameObject;
		Editor gameObjectEditor;

		[MenuItem("Window/GameObject Editor")]
		static void ShowWindow()
		{
			GetWindow<GameObjectEditorWindow>("GameObject Editor");
		}

		void OnGUI()
		{
			gameObject = (GameObject)EditorGUILayout.ObjectField(gameObject, typeof(GameObject), true);

			if (gameObject != null)
			{
				if (gameObjectEditor == null)
					gameObjectEditor = Editor.CreateEditor(gameObject);

				GUIStyle style = new GUIStyle(EditorStyles.label);

				style.normal.background = (Texture2D)Resources.Load("transparent");

				gameObjectEditor.OnInteractivePreviewGUI(GUILayoutUtility.GetRect(500, 500), GUI.skin.box);
			}
		}
	}

	public class MainWindow : EditorWindow
	{
		/////////////////////////////////////////////////////////////////////////
		// Static part
		/////////////////////////////////////////////////////////////////////////

		private static MainWindow _instance;

		[MenuItem("Tools/ProPlacer")]
		private static void ShowWindow()
		{
			_instance = ScriptableObject.CreateInstance<MainWindow>();
			_instance.titleContent = new GUIContent(TITLE_NAME);
			_instance.minSize = new Vector2(WIDTH, HEIGHT);
			_instance.maxSize = new Vector2(WIDTH, 2 * HEIGHT);
			_instance.ShowUtility();
		}

		/////////////////////////////////////////////////////////////////////////
		// Member part
		/////////////////////////////////////////////////////////////////////////


		private const int WIDTH = 160;
		private const int HEIGHT = 500;
		private const string TITLE_NAME = "Instantiator";

		private Editor _game_object_editor;

		private Dictionary<string, GUIStyle> _styles;

		private PlacingModeGUI[] _placing_modes = new PlacingModeGUI []
		{
			new SelectedTransformsMode(),
			new GridMode(),
			new MeshMode(),
			new SplineMode()
		};

		private string[] _placing_modes_names;

		private int _current_placing_mode;

		private int _object_picker_id;

		private GameObject _selected_instance;

		private void OnEnable()
		{
			_placing_modes_names = new string[_placing_modes.Length];

			for (int mode_index = 0; mode_index < _placing_modes.Length; ++mode_index)
			{
				_placing_modes_names[mode_index] = _placing_modes[mode_index].GetName();
			}
		}

		private void ReloadStyles()
		{
			_styles = new Dictionary<string, GUIStyle>()
			{
				{
					"HeaderLabel",
					new GUIStyle("label")
					{
						alignment = TextAnchor.MiddleCenter,
						fontStyle = FontStyle.Bold
					}
				},
				{
					"BigPartBox",
					new GUIStyle("box")
					{
						padding = new RectOffset(5, 5, 5, 5)
					}
				},
				{
					"Default",
					new GUIStyle()
				},
				{
					"InstancePreviewLabel",
					new GUIStyle("label")
					{
						alignment		= TextAnchor.MiddleCenter,
						imagePosition	= ImagePosition.ImageAbove,
						fixedWidth		= 130,
						fixedHeight		= 140
					}
				},
				{
					"MiddleCenterLabel",
					new GUIStyle(GUI.skin.label)
					{
						alignment = TextAnchor.MiddleCenter
					}
				}
			};
		}

		private void OnGUI()
		{
			HandleGUIEvents();

			ReloadStyles();

			EditorGUILayout.BeginHorizontal(_styles["Default"]);
			{
				GUILayout.FlexibleSpace();

				GUILayout.BeginVertical();
				{
					DrawInstanceGUI();
					//DrawPlacingModeGUI();
				}
				GUILayout.EndVertical();

				GUILayout.FlexibleSpace();
			}
			EditorGUILayout.EndHorizontal();
		}

		private void DrawInstanceGUI()
		{
			GUILayout.Label("Instance", _styles["HeaderLabel"]);

			GUILayout.BeginVertical(_styles["BigPartBox"]);
			{
				DrawInstanceInfoGUI();

				if (GUILayout.Button("Select"))
				{
					ShowGameObjectPicker();
				}
			}
			GUILayout.EndVertical();
		}

		private void DrawInstanceInfoGUI()
		{
			GUILayout.BeginVertical("box", GUILayout.MinWidth(150), GUILayout.MinHeight(180));
			{
				GUILayout.FlexibleSpace();

				DrawPreviewGUI();

				string instance_name = (_selected_instance) ? _selected_instance.name : "None";

				EditorGUILayout.SelectableLabel(instance_name, _styles["MiddleCenterLabel"]);

				GUILayout.FlexibleSpace();
			}
			GUILayout.EndVertical();
		}

		private void DrawPreviewGUI()
		{
			if (!_selected_instance) return;

			GUILayout.BeginHorizontal();
			{
				GUILayout.FlexibleSpace();

				if (AssetPreview.GetAssetPreview(_selected_instance))
				{
					GUIStyle preview_style = new GUIStyle(GUI.skin.box)
					{
						border	= new RectOffset(0, 0, 0, 0),
						overflow = new RectOffset(20, 20, 20, 20)
					};

					_game_object_editor.OnPreviewGUI(GUILayoutUtility.GetRect(100, 100), preview_style);
				}
				else
				{
					GUILayout.Label(AssetPreview.GetMiniThumbnail(_selected_instance), _styles["MiddleCenterLabel"]);
				}

				GUILayout.FlexibleSpace();
			}
			GUILayout.EndHorizontal();
		}

		private void HandleGUIEvents()
		{
			if (Event.current.commandName != "ObjectSelectorUpdated")
				return;

			if (EditorGUIUtility.GetObjectPickerControlID() != _object_picker_id)
				return;

			_selected_instance = (GameObject)EditorGUIUtility.GetObjectPickerObject();
			_game_object_editor = Editor.CreateEditor(_selected_instance);

			Repaint();
		}

		private void OnSelectionChange()
		{
			Repaint();
		}

		private void ShowGameObjectPicker()
		{
			_object_picker_id = EditorGUIUtility.GetControlID(FocusType.Passive);
			EditorGUIUtility.ShowObjectPicker<GameObject>(_selected_instance, true, "", _object_picker_id);
		}

		private void DrawPlacingModeGUI()
		{
			using (var horisontal_scope = new GUILayout.HorizontalScope())
			{
				GUILayout.FlexibleSpace();
				GUILayout.Label("Placing mode", EditorStyles.boldLabel);
				GUILayout.FlexibleSpace();
			}

			GUIStyle popup_style = EditorStyles.popup;
			popup_style.alignment = TextAnchor.MiddleCenter;

			_current_placing_mode = EditorGUILayout.Popup(_current_placing_mode, _placing_modes_names, popup_style);

			using (var horisontal_scope = new GUILayout.HorizontalScope())
			{
				GUILayout.FlexibleSpace();
				GUILayout.Label("Parameters", EditorStyles.boldLabel);
				GUILayout.FlexibleSpace();
			}

			using (var vertical_scope = new GUILayout.VerticalScope("box"))
			{
				//_placing_modes[_current_placing_mode].OnGUI(_instance_object, _name_to_style);
			}
		}
	}

	public abstract class PlacingModeGUI
	{
		public virtual void OnGUI(GameObject _placing_instance, Dictionary<string, GUIStyle> name_to_style_)
		{
			GUILayout.Label("in develop");
		}

		public abstract string GetName();
	};

	public class SelectedTransformsMode : PlacingModeGUI
	{
		public static string[] _sub_mode_names =
		{
				"Replacement",
				"Child",
				"Sibling"
		};

		private bool _replace_transform;

		private int _current_sub_mode;

		public override void OnGUI(GameObject _instance, Dictionary<string, GUIStyle> name_to_style_)
		{
			int space_width = 5;
			bool saved_gui_state = GUI.enabled;

			int selected_objects_count = Selection.transforms.Length;
			GUILayout.Label("Transforms: " + selected_objects_count.ToString());
			GUILayout.Space(space_width);

			//_replace_transform = GUILayout.Toggle(_replace_transform, "Substitude");
			//GUILayout.Space(space_width);

			GUILayout.Label("Place as:");
			//GUI.enabled = (!_replace_transform);

			using (var vertical_scope = new GUILayout.VerticalScope("box"))
			{
				_current_sub_mode = GUILayout.SelectionGrid(_current_sub_mode, _sub_mode_names, 1, EditorStyles.radioButton);
			}

			GUILayout.Space(space_width);
			GUI.enabled = (selected_objects_count != 0) && (_instance != null);
			if (GUILayout.Button("Place"))
			{
				ReplaceSelection();
			}
			else
			{
				// Do nothing.
			}

			GUI.enabled = saved_gui_state;
		}

		private void ReplaceSelection()
		{
			Debug.Log("ReplaceSelection()");
			Selection.objects = new UnityEngine.Object[0];
		}

		public override string GetName()
		{
			return "At selected Transforms";
		}
	}

	public class GridMode : PlacingModeGUI
	{
		public override string GetName()
		{
			return "Grid";
		}
	}

	public class MeshMode : PlacingModeGUI
	{
		public override string GetName()
		{
			return "Along Mesh";
		}
	}

	public class SplineMode : PlacingModeGUI
	{
		public override string GetName()
		{
			return "Along Spline";
		}
	}
}