using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

	public LayerMask CollisionMask;
	float speed = 10;
	
	public void SetSpeed(float newSpeed){
		speed = newSpeed;
	}

	void Update () {
		float moveDistance = speed * Time.deltaTime;
		CheckCollisions (moveDistance);
		transform.Translate (Vector3.forward * moveDistance);		
	}

	void CheckCollisions(float moveDistance){
		Ray ray = new Ray(transform.position,transform.forward);
		RaycastHit hit;

		if(Physics.Raycast(ray, out hit, moveDistance, CollisionMask, QueryTriggerInteraction.Collide)){

		}
	}

	void OnHitObject(RaycastHit hit){
		print(hit.collider.name);
		GameObject.Destroy (gameObject);
	}
}
