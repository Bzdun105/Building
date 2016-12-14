using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEditor;

namespace ExtendingTools.ProPlacer
{
	public static class PlacerAtSelectedTransforms
	{
		public enum PlaceAsMode
		{
			Replacement = 0,
			Child		= 1,
			Sibling		= 2
		}

		public enum NamingMode
		{
			TargetName				= 0,
			InstanceName			= 1,
			InstanceNameAndIndex	= 2
		}

		public static PlaceAsMode		PlaceAs					{ get; set; }
		public static bool				InstatiateAsReference	{ get; set; }
		public static NamingMode		NamingRule				{ get; set; }

		public static GameObject		InstanceToPlace			{ get; set; }

		public static void Place()
		{
			if (!InstanceToPlace)					return;
			if (Selection.transforms.Length == 0)	return;

			Transform[] selected_transforms = Selection.transforms;

			Undo.SetCurrentGroupName("PlaceAtSelectedTransforms");

			bool instaniate_as_reference =
					InstatiateAsReference &&
					(PrefabUtility.GetPrefabType(InstanceToPlace) == PrefabType.Prefab ||
					 PrefabUtility.GetPrefabType(InstanceToPlace) == PrefabType.ModelPrefab);

			for (int transform_index = 0; transform_index < selected_transforms.Length; ++transform_index)
			{
				Transform target_transform = selected_transforms[transform_index];

				GameObject placed_object =
					instaniate_as_reference ?
						PrefabUtility.InstantiatePrefab(InstanceToPlace) as GameObject :
						UnityEngine.Object.Instantiate(InstanceToPlace)  as GameObject;

				switch (NamingRule)
				{
					case NamingMode.TargetName:
						placed_object.name = target_transform.gameObject.name;
						break;
					case NamingMode.InstanceName:
						placed_object.name = InstanceToPlace.name;
						break;
					case NamingMode.InstanceNameAndIndex:
						placed_object.name = InstanceToPlace.name + transform_index.ToString();
						break;
					default:
						Debug.LogError("Default reached");
						break;
				}

				switch (PlaceAs)
				{
					case PlaceAsMode.Replacement:
						placed_object.transform.parent = target_transform.parent;
						placed_object.transform.localPosition = target_transform.localPosition;
						placed_object.transform.localRotation = target_transform.localRotation;
						Undo.DestroyObjectImmediate(target_transform.gameObject);
						break;
					case PlaceAsMode.Child:
						placed_object.transform.parent = target_transform;
						placed_object.transform.localPosition = Vector3.zero;
						placed_object.transform.localRotation = Quaternion.identity;
						break;
					case PlaceAsMode.Sibling:
						placed_object.transform.parent = target_transform.parent;
						placed_object.transform.localPosition = target_transform.localPosition;
						placed_object.transform.localRotation = target_transform.localRotation;
						break;
					default:
						Debug.LogError("Default reached");
						break;
				}

				Undo.RegisterCreatedObjectUndo(placed_object, "Place at transform");
			}
		}
	}
}
