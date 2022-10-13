using System.Collections;
using System.Collections.Generic;
using Gamelogic.Grids;
using UnityEngine;

namespace M7.Match
{
    public class HintReceiver : MonoBehaviour
    {
        [SerializeField] ParticleSystem hintSwipeParticle;
        [SerializeField] ParticleSystem hintParticle;
        [SerializeField] float durationPerLoop;
        [SerializeField] AnimationCurve lerpCurve;

        MatchGrid matchGrid => PuzzleBoardManager.Instance.ActiveGrid;

        IMap3D<RectPoint> MapGrid { get { return matchGrid.Map; } }

        private void Start()
        {
            PuzzleBoardManager.Instance.onMatchSwipeStarted += StopHint;
        }

        public void OnRandomPossibleMove(PossibleMove possibleMove)
        {
            if (!this.enabled)
                return;

            StopHint();
            ShowHint(possibleMove);
        }

        void ShowHint(PossibleMove possibleMove)
        {
            if (hintSwipeParticle != null)
            {
                var rad = RadBetweenVector2(MapGrid[possibleMove.TargetCell.CurrentRectPoint], MapGrid[possibleMove.DestinationCell.CurrentRectPoint]);
                var mainParticle = hintSwipeParticle.main;
                mainParticle.startRotation = -rad;
                hintSwipeParticle.transform.position = (MapGrid[possibleMove.TargetCell.CurrentRectPoint] + MapGrid[possibleMove.DestinationCell.CurrentRectPoint]) / 2;
                hintSwipeParticle.Clear();
                hintSwipeParticle.Play();
            }
            if (hintParticle != null && possibleMove.TargetCell != null)
            {
                hintParticle.transform.position = matchGrid.GetMatchGridTile(possibleMove.TargetCell.CurrentRectPoint).transform.position + Vector3.back;
                hintParticle.Play();
            }
        }

        public void StopHint()
        {
            if (hintSwipeParticle != null)
                hintSwipeParticle.Stop();
            if (hintParticle != null)
                hintParticle.Stop();
        }

        private float RadBetweenVector2(Vector2 vec1, Vector2 vec2)
        {
            return Mathf.Atan2(vec2.y - vec1.y, vec2.x - vec1.x);
        }

        private float AngleBetweenVector2(Vector2 vec1, Vector2 vec2)
        {
            Vector2 diference = vec2 - vec1;
            float sign = (vec2.y < vec1.y) ? -1.0f : 1.0f;
            return Vector2.Angle(Vector2.right, diference) * sign;
        }

        private void OnDisable () {
            StopHint();
        }
    }
}
