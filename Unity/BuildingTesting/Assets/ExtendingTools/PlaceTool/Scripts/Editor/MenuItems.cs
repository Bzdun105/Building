using UnityEngine;
using UnityEditor;

namespace ExtendingTools.ProPlacer
{
	public class MenuItems
	{
		[MenuItem("Tools/ProPlacer")]
		static void ShowWindow()
		{
			ProPlacerWindow.ShowProPlacer();
		}
	}
}