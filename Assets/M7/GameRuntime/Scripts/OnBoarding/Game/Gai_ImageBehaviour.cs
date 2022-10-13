using System.Collections.Generic;
using UnityEngine;

namespace M7.GameRuntime.Scripts.OnBoarding.Game
{
    public class Gai_ImageBehaviour : MonoBehaviour
    {
        [SerializeField] private List<ParticleSystem> particles;

        [SerializeField] private List<Transform> objects;

        public void Set()
        {
            // change layer to on boarding including its children
            foreach (var particle in particles) particle.gameObject.layer = LayerMask.NameToLayer("OnBoarding");
            foreach (var obj in objects) obj.gameObject.layer = LayerMask.NameToLayer("OnBoarding");
            
            // change vfx sorting layer id to onboarding and order in layer to 1
            foreach (var particle in particles)
            {
                particle.GetComponent<ParticleSystemRenderer>().sortingLayerName = "OnBoarding";
                particle.GetComponent<ParticleSystemRenderer>().sortingOrder = 0;
            }
            
            // add canvas to parent, set override sorting to true and set to 1
            var canvas = transform.gameObject.AddComponent<Canvas>();
            canvas.overrideSorting = true;
            canvas.sortingOrder = 1;
            canvas.sortingLayerName = "OnBoarding";
        }
    }
}
