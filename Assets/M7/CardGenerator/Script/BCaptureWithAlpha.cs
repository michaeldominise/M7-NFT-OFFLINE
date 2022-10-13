using UnityEngine;
using System.IO;
using System;

[RequireComponent (typeof(Camera))]
public class BCaptureWithAlpha : MonoBehaviour
{
	public int UpScale = 0;
	public bool AlphaBackground = true;

	public Texture2D CaptureScreen ()
	{
		var camera = GetComponent<Camera> ();
		int w = 1151 * UpScale;
		int h = 1151 * UpScale;
		var rt = new RenderTexture (w, h, 32);
		camera.targetTexture = rt;
		var screenShot = new Texture2D (w, h, TextureFormat.ARGB32, false);
		var clearFlags = camera.clearFlags;
		if (AlphaBackground) {
			camera.clearFlags = CameraClearFlags.SolidColor;
			camera.backgroundColor = new Color (0, 0, 0, 0);
		}
		camera.Render ();
		RenderTexture.active = rt;
		screenShot.ReadPixels (new Rect (0, 0, w, h), 0, 0);
		screenShot.Apply ();
		camera.targetTexture = null;
		RenderTexture.active = null;
		DestroyImmediate (rt);
		camera.clearFlags = clearFlags;
		return screenShot;
	}

	//public void SaveCaptureScreen (string id, string rarityName, string jsonNft, string jsonInGame)
	//{
	//	var path = Environment.GetFolderPath (Environment.SpecialFolder.Desktop) + "/Export/" + rarityName + "/" + id + "/";
	//	if (!Directory.Exists(path))
	//	{
	//		Directory.CreateDirectory(path);
	//	}

	//	var filename = id;
	//	File.WriteAllBytes (Path.Combine (path, "image_" + filename + ".png"), CaptureScreen().EncodeToPNG ());
	//	File.WriteAllText(path + "/hero_nft_" + filename + ".json", jsonNft);
	//	File.WriteAllText(path + "/hero_ingame_" + filename + ".json", jsonInGame);
	//}
	
	public void SaveCaptureRandom (string id, string jsonNft, string jsonInGame)
	{
		var path = Environment.GetFolderPath (Environment.SpecialFolder.Desktop) + "/Export/" + id + "/";
		if (!Directory.Exists(path))
		{
			Directory.CreateDirectory(path);
		}

		var filename = id;
		File.WriteAllBytes (Path.Combine (path, "image_" + filename + ".png"), CaptureScreen().EncodeToPNG ());
		File.WriteAllText(path + "/hero_nft_" + filename + ".json", jsonNft);
		File.WriteAllText(path + "/hero_ingame_" + filename + ".json", jsonInGame);
	}
	
	private static BCaptureWithAlpha m_BCaptureWithAlpha;
	public static BCaptureWithAlpha Instance
	{
		get
		{
			if (m_BCaptureWithAlpha == null) { m_BCaptureWithAlpha = FindObjectOfType<BCaptureWithAlpha>(); }
			return m_BCaptureWithAlpha;
		}
	}
}