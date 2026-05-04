using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace InsaneSystems.VoxelPalette
{
	[CreateAssetMenu(fileName = "ColorPalette", menuName = "Insane Systems/Voxel Palette/Color Palette")]
	public sealed class ColorPalette : ScriptableObject
	{
		[SerializeField] Texture2D sourcePaletteTexture;
		[SerializeField] public List<Color> paletteColors = new List<Color>();
		[SerializeField] public Material[] affectedMaterials = Array.Empty<Material>();

		public Texture2D SourcePaletteTexture
		{
			get => sourcePaletteTexture;
			set => sourcePaletteTexture = value;
		}

		Texture2D currentPaletteTexture;

		void Awake() => ApplyPalette();
		void OnEnable() => ApplyPalette();

		public void OnSourceTextureChanged()
		{
			if (sourcePaletteTexture != null)
				GeneratePaletteFromSourceTexture();
			else
				paletteColors = new List<Color>();
		}

		public void GeneratePaletteFromSourceTexture()
		{
			paletteColors = new List<Color>(sourcePaletteTexture.GetPixels());
			ApplyPalette();
		}

		public void ApplyPalette()
		{
			if (paletteColors.Count == 0)
				return;

			currentPaletteTexture = new Texture2D(paletteColors.Count, 1)
			{
				filterMode = FilterMode.Point
			};

			currentPaletteTexture.SetPixels(0, 0, paletteColors.Count, 1, paletteColors.ToArray());
			currentPaletteTexture.Apply();

			AssignPaletteTextureToMaterials(currentPaletteTexture);

			#if UNITY_EDITOR
			UnityEditor.EditorUtility.SetDirty(this);
			#endif
		}

		public void DoMaterialUpdate()
		{
			if (currentPaletteTexture)
				AssignPaletteTextureToMaterials(currentPaletteTexture);
		}

		public void AssignPaletteTextureToMaterials(Texture paletteTexture)
		{
			for (int i = 0; i < affectedMaterials.Length; i++)
				if (affectedMaterials[i] != null)
				{
					affectedMaterials[i].mainTexture = paletteTexture;
			
#if UNITY_EDITOR
					UnityEditor.EditorUtility.SetDirty(affectedMaterials[i]);
#endif
				}
		}

		public void ResetToSource()
		{
			GeneratePaletteFromSourceTexture();
		}

		public void SaveToSource()
		{
			if (!sourcePaletteTexture || !currentPaletteTexture)
				return;

			try
			{
				sourcePaletteTexture.SetPixels(currentPaletteTexture.GetPixels());
				sourcePaletteTexture.Apply();

				AssignPaletteTextureToMaterials(sourcePaletteTexture);

				Debug.Log("Color palette texture was saved to the source texture.");
			}
			catch (System.Exception ex)
			{
				Debug.LogError($"Save error: {ex.Message}");
			}
		}

		public void SaveToNewFile(string extraName = "New")
		{
			if (!sourcePaletteTexture || !currentPaletteTexture)
				return;

			var newName = $"{sourcePaletteTexture.name}{extraName}.png";

			try
			{
				var pathToResources = Application.dataPath + "/Resources/";
				if (!Directory.Exists(pathToResources))
					Directory.CreateDirectory(pathToResources);

				var textureBytes = currentPaletteTexture.EncodeToPNG();
				var path = pathToResources + newName;

				File.WriteAllBytes(path, textureBytes);
				Debug.Log($"Color palette texture was saved to '{path}'.");

				#if UNITY_EDITOR
				UnityEditor.AssetDatabase.Refresh();
				var textureFromCreatedFile = (Texture2D)UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/Resources/" + newName, typeof(Texture2D));
				
				AssignPaletteTextureToMaterials(textureFromCreatedFile);
				#endif
			}
			catch (Exception ex)
			{
				Debug.LogError($"Save error: {ex.Message}");
			}
		}
	}
}