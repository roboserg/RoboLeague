using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RobotFreeAnim))]
public class RobotFreeAnimEditor : Editor
{
	// ** Clean Old Prefs ** To remove with Next update
	static string notificationSeenPref = "Notification.Seen";
	// -----------------------------------------------------

	static bool isInit = false;
	static GUIStyle style = new GUIStyle();
	static Color defaultBgColor;

	Rect rect = new Rect(0, 50, 200, 200);
	Texture2D testImage;
	
	private void Awake()
	{
		if (!isInit)
		{
			// ** Clean Old Prefs **
			if (EditorPrefs.HasKey(notificationSeenPref))
				EditorPrefs.DeleteKey(notificationSeenPref);
			// -------------------------------------------

			style.border = new RectOffset(10, 10, 10, 10);
			defaultBgColor = GUI.backgroundColor;
			testImage = Resources.Load<Texture2D>("Notifications/Banners/test");
			isInit = true;
		}
	}

	public override void OnInspectorGUI()
    {
		GUI.DrawTexture(rect, testImage);
		style.wordWrap = true;
		DrawText("*Upgrades*\n", 35, new Color(1f, 0.2373f, 0f, 1f));
		GUILayout.Space(10);
		DrawText("Get More! Do More! With expansion Packs.\n", 15, Color.white);
		GUILayout.Space(40);


		DrawButton("Animation Expansion 1", onButtonClick: () => 
		{
			Application.OpenURL("https://assetstore.unity.com/packages/3d/characters/robots/robot-sphere-basic-pack-147055");
		}, Color.red);
		GUILayout.Space(5);

		DrawButton("Texture Expansion (PBR) 1", onButtonClick: () =>
		{
			Application.OpenURL("https://assetstore.unity.com/packages/3d/characters/robots/robot-sphere-texture-pack-1-160553");
		}, Color.green);
		GUILayout.Space(20);

		//base.OnInspectorGUI();
	}

	void DrawText(string text, int font, Color color)
	{
		style.fontSize = font;
		style.normal.textColor = color;
		EditorGUILayout.LabelField(text, style);
	}

	void DrawButton(string name, Action onButtonClick, Color color)
	{
		GUI.backgroundColor = color;
		if (GUILayout.Button(name))
		{
			onButtonClick();
		}
		GUI.backgroundColor = defaultBgColor;
	}
}
