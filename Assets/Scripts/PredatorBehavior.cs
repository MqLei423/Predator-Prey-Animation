using UnityEngine;

namespace MengQiLei
{
    public class PredatorBehavior : MonoBehaviour
    {
        private float moveSpeed = 4f;
        private float rotationSpeed = 5f;
        private float viewAngle = 30f;
        private float chaseDistance = 12f;

        private float avoidDistance = 3f;

        private GameObject prey;
        private Vector3 preyLastKnownPosition;

        private void Update()
        {
            AvoidObstacle();

            if (!wallAhead())
            {
                // Check for nearby prey
                if (prey != null && Vector3.Distance(transform.position, prey.transform.position) <= chaseDistance)
                {
                    ChasePrey();
                }
                else
                {
                    SearchForPrey();
                }
            }

            // Move forward
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);

            Vector3 leftDirection = Quaternion.Euler(0, -viewAngle / 2, 0) * transform.forward;
            Vector3 rightDirection = Quaternion.Euler(0, viewAngle / 2, 0) * transform.forward;
            Debug.DrawRay(transform.position, leftDirection * chaseDistance, Color.red);
            Debug.DrawRay(transform.position, rightDirection * chaseDistance, Color.red);
        }

        private void SearchForPrey()
        {
            // Cast rays to detect prey
            RaycastHit[] hits = Physics.RaycastAll(transform.position, transform.forward, chaseDistance);

            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.CompareTag("Prey"))
                {
                    // Prey detected, start chasing
                    prey = hit.collider.gameObject;
                    preyLastKnownPosition = prey.transform.position;
                    ChasePrey();
                    return;
                }
            }
        }

        private void ChasePrey()
        {
            // Rotate towards the last known position of the prey
            Vector3 directionToPrey = (preyLastKnownPosition - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(directionToPrey);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
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
            Vector3 leftDirection = Quaternion.Euler(0, -45, 0) * transform.forward;
            Vector3 rightDirection = Quaternion.Euler(0, 45, 0) * transform.forward;
            Debug.DrawRay(transform.position, leftDirection * avoidDistance, Color.green);
            Debug.DrawRay(transform.position, rightDirection * avoidDistance, Color.green);

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
                }
                else
                {
                    // Rotate the character 180 degrees around the y-axis
                    transform.Rotate(Vector3.up, 180f);
                }
            }
        }

        public bool isPredator()
        {
            return true;
        }
    }
}
