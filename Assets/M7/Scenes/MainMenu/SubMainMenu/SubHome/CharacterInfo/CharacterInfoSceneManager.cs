using UnityEngine;
using UnityEngine.AddressableAssets;
using M7;

public class CharacterInfoSceneManager : SceneManagerBase
{
	[SerializeField] AssetReference statsInfoScene;
	[SerializeField] AssetReference mintInfoScene;

	protected override void ExecuteButtonEvent(GameObject gameObject)
	{
		switch (gameObject.name)
		{
			case "Point_Button":
				LoadScene(statsInfoScene, UnityEngine.SceneManagement.LoadSceneMode.Additive);
				break;
			case "Mint_Button":
				LoadScene(mintInfoScene, UnityEngine.SceneManagement.LoadSceneMode.Additive);
				break;
			case "Close_Button":
				if(InventoryHeroes_HeroesSceneManager.Instance != null)
					InventoryHeroes_HeroesSceneManager.Instance.LevelUpTesting();
				UnloadAtSceneLayer(sceneLayer);
				break;
		}
	}
}
