﻿using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : weaponsBehavior
{
	public int pellets = 6;
	public float range = 30;
	[SerializeField]
	GameObject impact;
	public float maximumSpread = 0.3f;
    public float fireTimer;
    public float fireRate;
    Inputs i;

    protected override void NetworkStart()
    {
        base.NetworkStart();

        i = GetComponentInParent<Inputs>();
    }

    void Update()
    {
        if (networkObject == null) return;

        if (fireTimer < fireRate) fireTimer += Time.fixedDeltaTime;

        RaycastHit hit;

		if (i.isShooting)
		{
            if (fireTimer <= fireRate) return;

			for (int i = 0; i < pellets; i++)
			{
                Random.InitState(i);
				float spreadX = Random.Range(-maximumSpread, maximumSpread);
				float spreadY = Random.Range(-maximumSpread, maximumSpread);
				float spreadZ = 0f; //Don't adjust depth of spread.

                Vector3 spread = transform.TransformDirection(new Vector3(spreadX, spreadY, spreadZ));
				Vector3 direction = (transform.forward + spread).normalized;


                //Vector3 direction = Random.insideUnitCircle * spread;  THE OLD WAY
                if (Physics.Raycast(transform.position, transform.forward + direction, out hit, range))
				{
					Debug.Log(hit.transform.name);
					Instantiate(impact, hit.point, Quaternion.LookRotation(hit.normal));
				}                
			}

            fireTimer = 0;
        }
    }
}
