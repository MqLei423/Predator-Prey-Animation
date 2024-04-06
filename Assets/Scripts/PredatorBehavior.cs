using UnityEngine;

namespace MengQiLei
{
    public class PredatorBehavior : MonoBehaviour
    {
        private float moveSpeed = 4f;
        private float rotationSpeed = 8f;
        private float viewAngle = 30f;
        private float chaseDistance = 12f;

        private float avoidDistance = 3f;

        private void Update()
        {
     
            if (!wallAhead())
            {
                // Check for nearby prey
                for(int i = 0; i <= viewAngle; i += 10)
                {
                    SearchForPrey(i);
                }
            }

            // If wall present, prioritize wall avoiding
            for (int i = 0; i <= 180; i += 10)
            {
                AvoidObstacle(i);
            }

            // Move forward
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);

            if (transform.position.y > 1.2f) transform.position = new Vector3(transform.position.x, 1f, transform.position.z);
        }

        private void SearchForPrey(int angle)
        {
            // Cast rays to detect prey
            RaycastHit hit;
            Vector3 chaseDirection = Vector3.zero;

            // Calculate the angle of view
            Vector3 leftDirection = Quaternion.Euler(0, -angle / 2, 0) * transform.forward;
            Vector3 rightDirection = Quaternion.Euler(0, angle / 2, 0) * transform.forward;
            Debug.DrawRay(transform.position, leftDirection * chaseDistance, Color.red);
            Debug.DrawRay(transform.position, rightDirection * chaseDistance, Color.red);

            // Check for prey on the left ray
            if (Physics.Raycast(transform.position, leftDirection, out hit, chaseDistance))
            {
                if (hit.collider.CompareTag("Prey"))
                {
                    chaseDirection += leftDirection.normalized * (1f / hit.distance);
                }
            }

            // Check for obstacles on the right ray
            if (Physics.Raycast(transform.position, rightDirection, out hit, chaseDistance))
            {
                if (hit.collider.CompareTag("Prey"))
                {
                    chaseDirection += rightDirection.normalized * (1f / hit.distance);
                }
            }

            // Turn facing direction to prey
            if (chaseDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(chaseDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }

        private bool wallAhead()
        {
            RaycastHit hit;
            return Physics.Raycast(transform.position, transform.forward, out hit, avoidDistance);
        }

        private void AvoidObstacle(int angle)
        {
            // Cast rays to detect obstacles
            RaycastHit hit;
            Vector3 avoidanceDirection = Vector3.zero;

            // Calculate the angle of view
            Vector3 leftDirection = Quaternion.Euler(0, -angle/2, 0) * transform.forward;
            Vector3 rightDirection = Quaternion.Euler(0, angle/2, 0) * transform.forward;

            int count = 0;
            // Check for obstacles on the left ray
            if (Physics.Raycast(transform.position, leftDirection, out hit, avoidDistance))
            {
                if (!hit.collider.CompareTag("Prey")) {
                    avoidanceDirection += -leftDirection.normalized * (1f / hit.distance);
                    count++;
                }
            }

            // Check for obstacles on the right ray
            if (Physics.Raycast(transform.position, rightDirection, out hit, avoidDistance))
            {
                if (!hit.collider.CompareTag("Prey"))
                {
                    avoidanceDirection += -rightDirection.normalized * (1f / hit.distance);
                    count++;
                }
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
                    avoidanceDirection += transform.forward.normalized * (1f / hit.distance);
                    Quaternion targetRotation = Quaternion.LookRotation(avoidanceDirection);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                    //transform.Rotate(Vector3.up, 180f);
                }
            }
        }
    }
}
