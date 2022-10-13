using UnityEngine;

namespace M7.GameRuntime.Scripts.Misc
{
    public class GameObjectFollower : MonoBehaviour
    {
        [SerializeField] private bool followWorldPosition;
        [SerializeField] private Transform objectToFollow;

        [SerializeField] private Vector3 offset;

        private Vector3 _pos;
    
        // Update is called once per frame
        private void Update()
        {
            if (followWorldPosition)
            {
                FollowWorldPos();
                return;
            }
        
            FollowLocalPos();
        }

        private void FollowWorldPos()
        {
            var position = objectToFollow.position;
            _pos.x = position.x + offset.x;
            _pos.y = position.y + offset.y;
            _pos.z = position.z + offset.z;
            transform.position = _pos;
            print($"Object To Follow {objectToFollow.position}, pos {_pos}, transform position {transform.position}");
        }
    
        private void FollowLocalPos()
        {
            var position = objectToFollow.localPosition;
            _pos.x = position.x + offset.x;
            _pos.y = position.y + offset.y;
            _pos.z = position.z + offset.z;
            transform.localPosition = _pos;
        }
    }
}
