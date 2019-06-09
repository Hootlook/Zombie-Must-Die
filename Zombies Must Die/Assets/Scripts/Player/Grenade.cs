﻿using BeardedManStudios.Forge.Networking.Generated;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : WeaponBase
{
    public float explosionRadius = 12;
    public float proximity = 50;
    int layerMask = 1 << 11;
    bool oneTime = false;

    protected override void NetworkStart()
    {
        base.NetworkStart();

        layerMask = ~layerMask;

        gameObject.name = "Grenade";
    }

    void Update()
    {
        if (networkObject == null) return;

        if (isEquipped)
        {
            if (isArmed)
            {
                if (!oneTime)
                {
                    transform.SetParent(null);
                    networkObject.position = transform.position;
                    networkObject.rotation = transform.rotation;
                    networkObject.positionInterpolation.target = transform.position;
                    networkObject.rotationInterpolation.target = transform.rotation;
                    networkObject.SnapInterpolations();

                    a.Play();

                    if (networkObject.IsOwner)
                    {
                        rb.isKinematic = false;
                        rb.AddForce(Camera.main.transform.forward * 500);
                        rb.AddTorque(transform.right * 500);
                        networkObject.Destroy(4000);
                    }

                    FxManager.EmitParticleOnDestroy("WFX_Explosion", transform);
                    FxManager.EmitSoundOnDestroy("grenade_explosion", transform, 2);

                    oneTime = true;
                }
            }

            isShooting = i.isShooting;
        }
    }

    private void OnDestroy()
    {
        if (isArmed)
        {
            Collider[] objectsInRange = Physics.OverlapSphere(transform.position, explosionRadius, layerMask);
            foreach (Collider col in objectsInRange)
            {
                if (Physics.Raycast(transform.position, (col.transform.position - transform.position), out RaycastHit hit, explosionRadius, layerMask))
                {
                    if (hit.collider.tag == "Player")
                    {
                        float distance = Vector3.Distance(hit.collider.transform.position, transform.position);
                        proximity /= distance;

                        EZCameraShake.CameraShaker.Instance.ShakeOnce(5, proximity, 0, 1);
                    }
                }
            }
        }
    }
}
