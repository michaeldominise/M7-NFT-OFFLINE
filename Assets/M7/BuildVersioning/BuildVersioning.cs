using M7.CDN;
using M7.GameRuntime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BuildVersioning : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI version;
    [SerializeField] string versionFormat = "v{0}b{1}p{2}";
    TextAsset bundleVersionText => GameManager.Instance.BundleVersionText;

    void Start() => version.text = string.Format(versionFormat, Application.version, bundleVersionText.text, M7AddressableProfile.CDNCurrentVersion);
}