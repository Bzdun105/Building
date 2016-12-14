using UnityEngine;
using UnityEditor;

namespace ExtendingTools.ProPlacer
{
	public partial class ProPlacerWindow
	{
		private static class Styles
		{
			public static GUIStyle MainContainer = new GUIStyle()
			{
				fixedWidth = 150,
				margin = new RectOffset(0, 0, 0, 0),
				padding = new RectOffset(0, 0, 0, 0)
			};

			public static GUIStyle HeaderLabel = new GUIStyle(GUI.skin.label)
			{
				fontStyle = FontStyle.Bold,
				alignment = TextAnchor.MiddleCenter
			};

			public static GUIStyle BigSectionBox = new GUIStyle(GUI.skin.box)
			{
				margin = new RectOffset(0, 0, 0, 0),
				padding = new RectOffset(5, 5, 5, 5)
			};

			public static GUIStyle MiddleCenterLabel = new GUIStyle(GUI.skin.label)
			{
				alignment = TextAnchor.MiddleCenter
			};

			public static GUIStyle PreviewBox = new GUIStyle(GUI.skin.box)
			{
				border		= new RectOffset(0, 0, 0, 0),
				overflow	= new RectOffset(20, 20, 20, 20)
			};

			public static GUIStyle PlaceInstanceInfoBox = new GUIStyle(GUI.skin.box)
			{
				fixedHeight = 160
			};

			public static GUIStyle PlaceButton = new GUIStyle(GUI.skin.button)
			{
				alignment = TextAnchor.MiddleCenter,
				fixedHeight = 3 * GUI.skin.button.lineHeight
			};
		}
	}
}
