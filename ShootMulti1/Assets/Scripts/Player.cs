﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;



// https://www.youtube.com/watch?v=jdv8erC7ML8&t=360s


[RequireComponent (typeof (PlayerController))]
[RequireComponent (typeof (GunController))]

public class Player : LivingEntity {

	public float moveSpeed = 5;
	PlayerController controller;
	Camera viewCamera;
	GunController gunController;


	protected override void Start () {
        base.Start();
		controller = GetComponent<PlayerController> ();
        gunController = GetComponent<GunController>();
		viewCamera = Camera.main;
	}
	
	
	void Update ()
    {
        // movement input
        Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        Vector3 moveVelocity = moveInput.normalized * moveSpeed;
        controller.Move(moveVelocity);

        //look input
        Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
        Plane groupPlane = new Plane(Vector3.up, Vector3.zero);
        float rayDistance;

        if (groupPlane.Raycast(ray, out rayDistance))
        {
            Vector3 point = ray.GetPoint(rayDistance);

            //Debug.DrawLine(ray.origin, point, Color.red);

            controller.LookAt(point);
        }

        //weapon input
        if (Input.GetMouseButton(0)){
            gunController.Shoot();
        }


    }

    
}
