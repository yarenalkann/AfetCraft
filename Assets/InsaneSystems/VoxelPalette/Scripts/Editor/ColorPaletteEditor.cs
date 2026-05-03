using UnityEngine;
using UnityEditor;

namespace InsaneSystems.VoxelPalette
{
	[CustomEditor(typeof(ColorPalette))]
	public sealed class ColorPaletteEditor : Editor
	{
		bool showPalette = true;

		Vector2 paletteScrollPosition;

		string postfix = "New";

		public override void OnInspectorGUI()
		{
			var colorPalette = target as ColorPalette;
			var affectedMaterialsProperty = serializedObject.FindProperty("affectedMaterials");

			GUILayout.Label("Primary settings", EditorStyles.boldLabel);
			
			DrawSourceTextureField(colorPalette);

			if (colorPalette && colorPalette.SourcePaletteTexture != null)
			{
				DrawColorPalette(colorPalette);

				if (GUILayout.Button("Reset palette to the source texture"))
					colorPalette.ResetToSource();
				
				GUILayout.Space(15);
				GUILayout.Label("View changes at scene", EditorStyles.boldLabel);
				
				if (affectedMaterialsProperty.arraySize == 0)
					EditorGUILayout.HelpBox("If you want see your changes in realtime on your models, add materials, which uses the source color palette texture, in list below.", MessageType.Info);

				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField(affectedMaterialsProperty, true);
				if (EditorGUI.EndChangeCheck())
				{
					serializedObject.ApplyModifiedProperties();

					colorPalette.DoMaterialUpdate();
				}
				
				if (affectedMaterialsProperty.arraySize > 0)
					if (GUILayout.Button(new GUIContent("Refresh materials",  "Press this button, if palette is not shown on your model.")))
						colorPalette.DoMaterialUpdate();
				
				GUILayout.Space(15);
				
				GUILayout.Label("Save changes", EditorStyles.boldLabel);
				
				EditorGUILayout.HelpBox("Don't forget to save your changes to texture.", MessageType.Info);

				if (GUILayout.Button("Save as override for source texture"))
					colorPalette.SaveToSource();

				GUILayout.Space(10);
				
				GUILayout.BeginHorizontal();
				GUILayout.Label("Filename postfix");
				postfix = GUILayout.TextField(postfix, 24);
				GUILayout.EndHorizontal();
				
				if (GUILayout.Button("Save as new texture file"))
					colorPalette.SaveToNewFile(postfix);
			}

			serializedObject.ApplyModifiedProperties();
			serializedObject.Update();
		}

		void DrawSourceTextureField(ColorPalette targetColorPalette)
		{
			var sourceTexProperty = serializedObject.FindProperty("sourcePaletteTexture");

			if (targetColorPalette.SourcePaletteTexture == null)
				EditorGUILayout.HelpBox("Please, add a color palette texture, which you want to edit, in field below.", MessageType.Info);

			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(sourceTexProperty);
			if (EditorGUI.EndChangeCheck())
			{
				serializedObject.ApplyModifiedProperties();

				targetColorPalette.OnSourceTextureChanged();
			}
		}

		void DrawColorPalette(ColorPalette targetColorPalette)
		{
			var paletteColorsProperty = serializedObject.FindProperty("paletteColors");
			
			showPalette = EditorGUILayout.Foldout(showPalette, "Show Palette", true);

			if (!showPalette)
				return;

			var isOpened = false;
			var rowElement = 0;
			var maxRowElements = 12;

			paletteScrollPosition = EditorGUILayout.BeginScrollView(paletteScrollPosition);
			
			EditorGUI.BeginChangeCheck();

			for (int i = 0; i < paletteColorsProperty.arraySize; i++)
			{
				var colorProperty = paletteColorsProperty.GetArrayElementAtIndex(i);
				var isEndOfPalette = i == paletteColorsProperty.arraySize - 1;

				if (!isOpened)
				{
					EditorGUILayout.BeginHorizontal();
					isOpened = true;
				}

				colorProperty.colorValue = EditorGUILayout.ColorField(GUIContent.none, colorProperty.colorValue, false, false, false, GUILayout.Width(24));

				rowElement++;

				if (rowElement == maxRowElements || isEndOfPalette)
				{
					EditorGUILayout.EndHorizontal();
					isOpened = false;
					
					rowElement = 0;
				}
			}

			if (isOpened)
				EditorGUILayout.EndHorizontal();

			EditorGUILayout.EndScrollView();

			if (EditorGUI.EndChangeCheck())
			{
				serializedObject.ApplyModifiedProperties();
				targetColorPalette.ApplyPalette();
			}
		}
	}
}