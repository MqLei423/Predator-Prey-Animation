using UnityEngine;

namespace MengQiLei
{
    public class PreyBehavior : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 3f;
        [SerializeField] private GameObject target;
        [SerializeField] private GameObject origin;
        private float avoidDistance = 3f;
        private float rotationSpeed = 5f;
        private int viewAngle = 200;
        private Vector3 targetPos;
        private Vector3 respawnPos;

        private void Start()
        {
            targetPos = new Vector3(target.transform.position.x, 1f, target.transform.position.z);
            respawnPos = new Vector3(origin.transform.position.x, 1f, origin.transform.position.z);
        }

        private void Update()
        {
            // Cast rays
            for (int i = 0; i <= viewAngle; i += 10)
            {
                AvoidObstacle(i);
            }

            if (!wallAhead())
            {
                // Calculate direction towards the target
                Vector3 directionToTarget = (targetPos - transform.position).normalized;

                // Rotate towards the direction to the target
                Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);

        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Predator")) {
                // Collided with a predator, respawn
                Die();
            }
        }

        private bool wallAhead()
        {
            RaycastHit hit;
            return Physics.Raycast(transform.position, transform.forward, out hit, avoidDistance);
        }


        // Keep away from wall and predator
        private void AvoidObstacle(int viewAngle)
        {
            RaycastHit hit;
            Vector3 avoidanceDirection = Vector3.zero;

            // Calculate the angle of prey's view
            Vector3 leftDirection = Quaternion.Euler(0, -viewAngle / 2, 0) * transform.forward;
            Vector3 rightDirection = Quaternion.Euler(0, viewAngle / 2, 0) * transform.forward;

            Debug.DrawRay(transform.position, leftDirection * avoidDistance, Color.green);
            Debug.DrawRay(transform.position, rightDirection * avoidDistance, Color.green);

            // Check for obstacles on the left ray
            if (Physics.Raycast(transform.position, leftDirection, out hit, avoidDistance) && !hit.collider.CompareTag("Prey"))
            {
                avoidanceDirection += -leftDirection.normalized * (1f / hit.distance);
            }

            // Check for obstacles on the right ray
            if (Physics.Raycast(transform.position, rightDirection, out hit, avoidDistance) && !hit.collider.CompareTag("Prey"))
            {
                avoidanceDirection += -rightDirection.normalized * (1f / hit.distance);
            }

            // Apply avoidance direction if needed
            if (avoidanceDirection != Vector3.zero)
            {
                // Normalize the avoidance direction
                avoidanceDirection.Normalize();

                // Adjust the character's rotation to face the avoidance direction
                Quaternion targetRotation = Quaternion.LookRotation(avoidanceDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                
            }
        }

        private void Die()
        {
            transform.position = respawnPos;
        }
    }
}
