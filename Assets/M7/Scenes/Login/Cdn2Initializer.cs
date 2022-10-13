using UnityEngine;
using M7.CDN;
using TMPro;

public class Cdn2Initializer : MonoBehaviour
{
    [SerializeField] string secondCdnLabel = "CDN2";
    [SerializeField] TextMeshProUGUI progress;
    [SerializeField] GameObject parentUI;
    
    public void StartDownloadCDN()
    {
        StartCoroutine(CDNDownloader.Execute(secondCdnLabel, OnStatusUpdate));
    }

    private void OnStatusUpdate(CdnStatus status)
    {
        switch (status.CurrenStatus)
        {
            case CdnStatus.Status.DownloadStarting:
                parentUI.SetActive(true);
                break;
            case CdnStatus.Status.Downloading:
                progress.text = $"{(int)(status.DownloadStatus.Percent * 100)}%";
                break;
            case CdnStatus.Status.Done:
                parentUI.SetActive(false);
                break;
        }
    }
}