using UnityEngine;
using System.Collections;

// Author: Jonathan Dymond
// Class: ITCS 4236

public class KinematicArrive : MonoBehaviour {

	// Data type used to store orientation data of the player
	public class Steering
	{
		public Vector3 velocity;
		public Quaternion rotation;

		public Steering() {
			velocity = new Vector3();
			rotation = new Quaternion();
		}
	}

	public Rigidbody rb;
	[SerializeField] Vector3 target;

	public float maxSpeed = 10f;
	public float radius = 0f;
	public float timeToTarget = 1f;
	public Vector3 targetOffset = new Vector3 (0, .5f, 0);
	public float turnSpeed = .1f;
	public float updateTimer = .1f;
	public float updateTime = .01f;

	public Ray lastRay = new Ray(); 

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody> ();
	}

	Steering getSteering() {
		
		Steering steering = new Steering();
		
		// get direction to the target
		steering.velocity = target - transform.position;
		
		// check if in radius
		if (steering.velocity.magnitude < radius) 
			return steering;
		
		// we need to move target. get there in timeToTarget seconds
		steering.velocity /= timeToTarget;

		// limit speed to max
		if( steering.velocity.magnitude > maxSpeed )
		{
			steering.velocity.Normalize();
			steering.velocity *= maxSpeed;
		}

		// populate steering data with default rotation, and update the rotation to face target
		steering.rotation = transform.rotation;
		steering.rotation.SetLookRotation (target - transform.position);
		
		
		return steering;
	}

	void setOrientation(Steering steering) {
		// update the player velocity
		rb.velocity = steering.velocity;
		// Slowly update the rotation over the timeToTarget time
		transform.rotation = Quaternion.Slerp (transform.rotation, steering.rotation, Time.deltaTime * turnSpeed );
	}

	void getTarget() {
		// Check for mouse click && limit Raycast code to timed Interval
		if (Input.GetMouseButton (0) && updateTimer <= 0) {

			updateTimer = updateTime;

			// Use the camera to create a raycast from the screen to the plane
			Ray dir = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;

			// Check if ray hits the plane, if so, store in target
			if (Physics.Raycast (dir, out hit)) 
			if (Vector3.Distance (transform.position, hit.point) > .1f)
				target = hit.point + targetOffset;
			
			lastRay = dir;
		} else {
			// Hmm fix this?
			updateTimer -= Mathf.Clamp(updateTime * Time.deltaTime, .001f, updateTime);
		}
	}

	// Each frame
	void Update () {

		// Update the target variable
		getTarget ();

		// Update the orientation (position, rotation) of the player, based on the steering input
		setOrientation (getSteering ());

	}

	// Display the target for debugging
	void OnDrawGizmos() {
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere (target, .1f);
		Gizmos.DrawLine(lastRay.origin, target);
	}

}
