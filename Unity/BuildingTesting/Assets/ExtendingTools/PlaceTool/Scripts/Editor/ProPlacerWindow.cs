using UnityEngine;
using UnityEditor;

namespace ExtendingTools.ProPlacer
{
	public partial class ProPlacerWindow : EditorWindow
	{
		public enum InstantiateMode
		{
			Copy		= 0,
			Reference	= 1
		}

		private static readonly string[] INSTANTIATE_MODE_STRINGS =
		{
			"Copy",
			"Reference"
		};

		private enum PlacingMode
		{
			AtSelectedTransforms = 0
		}

		private static ProPlacerWindow _instance;

		private const string TITLE = "ProPlacer";
		private const int WIDTH  = 160;
		private const int HEIGHT = 500;

		private int			_instance_to_place_picker_id;
		private GameObject	_instance_to_place;
		private Editor		_instance_to_place_editor;

		private InstantiateMode	_current_instatiate_mode;
		private PlacingMode		_current_placing_mode;
		
		public static void ShowProPlacer()
		{
			_instance = CreateInstance<ProPlacerWindow>();
			_instance.titleContent = new GUIContent(TITLE);
			_instance.minSize = new Vector2(WIDTH, HEIGHT);
			_instance.maxSize = new Vector2(WIDTH, 2 * HEIGHT);
			_instance.ShowUtility();
		}

		private void OnSelectionChange()
		{
			Repaint();
		}

		private void OnGUI()
		{
			HandleGUIEvents();

			bool buffered_gui_enabled = GUI.enabled;
			GUI.enabled = true;

			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();   
			GUILayout.BeginVertical(Styles.MainContainer);
			{
				GUI.enabled = true;
				GUILayout.Label("Instance to place", Styles.HeaderLabel);

				GUILayout.BeginVertical(Styles.BigSectionBox);
				{
					OnPlaceInstanceGUI();
				}
				GUILayout.EndVertical();

				GUI.enabled = true;
				GUILayout.Label("Instantiate mode", Styles.HeaderLabel);

				GUILayout.BeginVertical(Styles.BigSectionBox);
				{
					OnInstantiateModeGUI();
				}
				GUILayout.EndVertical();

				GUI.enabled = true;
				GUILayout.Label("Placing mode", Styles.HeaderLabel);

				GUILayout.BeginVertical(Styles.BigSectionBox);
				{
					OnPlacingModeGUI();
				}
				GUILayout.EndVertical();
			}
			GUILayout.EndVertical();	
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();

			GUI.enabled = buffered_gui_enabled;
		}

		private void OnPlaceInstanceGUI()
		{
			GUILayout.BeginVertical(Styles.PlaceInstanceInfoBox);
			GUILayout.FlexibleSpace();
			{
				OnPlaceInstancePreviewGUI();

				string instance_name = (_instance_to_place) ? _instance_to_place.name : "None";

				EditorGUILayout.SelectableLabel(instance_name, Styles.MiddleCenterLabel);
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndVertical();
				
			if (GUILayout.Button("Select"))
			{
				ShowGameObjectPicker();
			}
		}

		private void OnPlaceInstancePreviewGUI()
		{
			if (!_instance_to_place) return;

			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			{
				if (AssetPreview.GetAssetPreview(_instance_to_place))
				{
					_instance_to_place_editor.OnPreviewGUI(GUILayoutUtility.GetRect(100, 100), Styles.PreviewBox);
				}
				else
				{
					GUILayout.Label(AssetPreview.GetMiniThumbnail(_instance_to_place), Styles.MiddleCenterLabel);
				}
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}

		private void OnInstantiateModeGUI()
		{
			bool object_to_place_is_prefab =
				_instance_to_place &&
				(PrefabUtility.GetPrefabType(_instance_to_place) == PrefabType.Prefab ||
				 PrefabUtility.GetPrefabType(_instance_to_place) == PrefabType.ModelPrefab);

			_current_instatiate_mode = object_to_place_is_prefab ? _current_instatiate_mode : InstantiateMode.Copy;

			GUI.enabled = object_to_place_is_prefab;

			_current_instatiate_mode = (InstantiateMode)GUILayout.SelectionGrid((int)_current_instatiate_mode, INSTANTIATE_MODE_STRINGS, 1, EditorStyles.radioButton);
		}

		private void OnPlacingModeGUI()
		{
			_current_placing_mode = (PlacingMode)EditorGUILayout.EnumPopup(_current_placing_mode);
			GUILayout.Space(5);

			switch (_current_placing_mode)
			{
				case PlacingMode.AtSelectedTransforms:
				{
					OnPlacerAtSelectedTransformsGUI();
					break;
				}
				default:
				{
					Debug.LogError("Default reached!");
					break;
				}
			}
		}

		private void OnPlacerAtSelectedTransformsGUI()
		{
			int selected_transforms_count = Selection.transforms.Length;

			GUILayout.Label("Transforms count: " + selected_transforms_count.ToString());
			GUILayout.Space(5);

			GUILayout.Label("Place as:");
			PlacerAtSelectedTransforms.PlaceAs =  
				(PlacerAtSelectedTransforms.PlaceAsMode)EditorGUILayout.EnumPopup(PlacerAtSelectedTransforms.PlaceAs);
			GUILayout.Space(5);

			GUILayout.Label("Naming rule:");
			PlacerAtSelectedTransforms.NamingRule = 
				(PlacerAtSelectedTransforms.NamingMode)EditorGUILayout.EnumPopup(PlacerAtSelectedTransforms.NamingRule);
			GUILayout.Space(5);

			GUI.enabled = (selected_transforms_count != 0) && (_instance_to_place != null);
			if (GUILayout.Button("Place", Styles.PlaceButton))
			{
				PlacerAtSelectedTransforms.InstatiateAsReference	= (_current_instatiate_mode == InstantiateMode.Reference);
				PlacerAtSelectedTransforms.InstanceToPlace			= _instance_to_place;
				PlacerAtSelectedTransforms.Place();
			}
		} 

		private void ShowGameObjectPicker()
		{
			_instance_to_place_picker_id = EditorGUIUtility.GetControlID(FocusType.Passive);
			EditorGUIUtility.ShowObjectPicker<GameObject>(_instance_to_place, true, "", _instance_to_place_picker_id);
		}

		private void HandleGUIEvents()
		{
			if (Event.current.commandName != "ObjectSelectorUpdated")							return;
			if (EditorGUIUtility.GetObjectPickerControlID() != _instance_to_place_picker_id)	return;

			_instance_to_place = (GameObject)EditorGUIUtility.GetObjectPickerObject();
			_instance_to_place_editor = Editor.CreateEditor(_instance_to_place);

			Repaint();
		}
	}
}