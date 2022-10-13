/*
 * TweenCycleVideoPlayerVolume.cs
 * Author: Cristjan Lazar
 */

using UnityEngine;
using UnityEngine.Video;

using DG.Tweening;

public class TweenCycleVideoPlayerVolume : TweenCycleBase {

    [SerializeField] private VideoPlayer videoPlayer;

    [SerializeField] private int trackIndex;

    [SerializeField] private float to = 1f;

    private ushort TrackIndex { get { return (ushort)trackIndex; } }

    protected override Tweener InitTween () {
        return DOTween.To(() => videoPlayer.GetDirectAudioVolume(TrackIndex), 
                          v => videoPlayer.SetDirectAudioVolume(TrackIndex, v), 
                          to, 
                          duration);
    }

    protected override void Reset () {
        videoPlayer = videoPlayer ?? GetComponent<VideoPlayer>();
    }

}

