using UnityEngine;

namespace MengQiLei
{
    public class PreyBehavior : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 3f;
        [SerializeField] private GameObject target;
        private float avoidDistance = 3f;
        private float rotationSpeed = 5f;
        private int viewAngle = 90;
        private Vector3 targetPos;

        private void Start()
        {
            targetPos = new Vector3(target.transform.position.x, 1f, target.transform.position.z);
        }

        private void Update()
        {


            AvoidObstacle();

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

        private bool wallAhead()
        {
            RaycastHit hit;
            return Physics.Raycast(transform.position, transform.forward, out hit, avoidDistance);
        }


        private void AvoidObstacle()
        {
            // Cast rays to detect obstacles
            RaycastHit hit;
            Vector3 avoidanceDirection = Vector3.zero;

            // Calculate the angle of prey's view
            Vector3 leftDirection = Quaternion.Euler(0, -viewAngle / 2, 0) * transform.forward;
            Vector3 rightDirection = Quaternion.Euler(0, viewAngle / 2, 0) * transform.forward;

            Debug.DrawRay(transform.position, leftDirection * avoidDistance, Color.red);
            Debug.DrawRay(transform.position, rightDirection * avoidDistance, Color.red);

            int count = 0;
            // Check for obstacles on the left side
            if (Physics.Raycast(transform.position, leftDirection, out hit, avoidDistance))
            {
                avoidanceDirection += -leftDirection.normalized * (1f / hit.distance); // Add avoidance force
                count++;
            }

            // Check for obstacles on the right side
            if (Physics.Raycast(transform.position, rightDirection, out hit, avoidDistance))
            {
                avoidanceDirection += -rightDirection.normalized * (1f / hit.distance); // Add avoidance force
                count++;
            }

            // Apply avoidance direction if needed
            if (avoidanceDirection != Vector3.zero)
            {
                if (count < 2)
                {
                    // Normalize the avoidance direction
                    avoidanceDirection.Normalize();

                    // Adjust the character's rotation to face the avoidance direction
                    Quaternion targetRotation = Quaternion.LookRotation(avoidanceDirection);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                } else
                {
                    // Rotate the character 180 degrees around the y-axis
                    transform.Rotate(Vector3.up, 180f);
                }
            }
        }


    }
}
