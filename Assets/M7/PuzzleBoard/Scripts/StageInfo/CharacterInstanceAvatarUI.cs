using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M7.GameRuntime;
using UnityEngine.UI;

public class CharacterInstanceAvatarUI : MonoBehaviour
{
    public CharacterInstance_Avatar characterInstance_Avatar;

    [SerializeField] Image avatarIcon;

    public void Init()
    {
        avatarIcon.sprite = characterInstance_Avatar.IconAsset;
    }
}
