﻿using BeardedManStudios.Forge.Networking.Generated;
using UnityEngine;

public class PlayerSetup : PlayerBehavior
{
    public GameObject cam;
    public bool isGrounded;
    public float vertical;
    PlayerMovements pm;
    CharacterController cc;
    CameraController cameraController;
	public delegate void playerInstance();
	public static event playerInstance PlayerLoaded;

	protected override void NetworkStart()
    {
        base.NetworkStart();

        if (networkObject.IsOwner)
        {
            gameObject.tag = "Player";
            Instantiate(cam);
        }
        cc = GetComponent<CharacterController>();
        pm = GetComponent<PlayerMovements>();
        cameraController = GameObject.Find("Camera(Clone)").GetComponent<CameraController>();

        PlayerLoaded();
	}
   
    void Update()
    {
        if (networkObject != null)
        {
            if (networkObject.IsOwner)
            {
                networkObject.position = transform.position;
                networkObject.rotation = transform.rotation;
                networkObject.spine = pm.spine.rotation;
                networkObject.isGrounded = cc.isGrounded;
                networkObject.cameraVertical = cameraController.vertical;
            }
            else
            {
                transform.position = networkObject.position;
                transform.rotation = networkObject.rotation;
                pm.spine.rotation = networkObject.spine;
                isGrounded = networkObject.isGrounded;
                vertical = networkObject.cameraVertical;
            }
        }        
    }
}
