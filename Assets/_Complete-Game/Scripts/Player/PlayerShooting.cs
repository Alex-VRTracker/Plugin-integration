﻿using UnityEngine;
using UnitySampleAssets.CrossPlatformInput;

namespace CompleteProject
{
    public class PlayerShooting : MonoBehaviour
    {
        public int damagePerShot = 20;                  // The damage inflicted by each bullet.
        public float timeBetweenBullets = 0.15f;        // The time between each shot.
        public float range = 100f;                      // The distance the gun can fire.

        public VRTrackerTag vrGun;                      //ADDED, will link the message received from the tag
        float timer;                                    // A timer to determine when to fire.
        Ray shootRay = new Ray();                       // A ray from the gun end forwards.
        RaycastHit shootHit;                            // A raycast hit to get information about what was hit.
        int shootableMask;                              // A layer mask so the raycast only hits things on the shootable layer.
        ParticleSystem gunParticles;                    // Reference to the particle system.
        LineRenderer gunLine;                           // Reference to the line renderer.
        AudioSource gunAudio;                           // Reference to the audio source.
        Light gunLight;                                 // Reference to the light component.
		public Light faceLight;								// Duh
        float effectsDisplayTime = 0.2f;                // The proportion of the timeBetweenBullets that the effects will display for.

        void Awake ()
        {
            // Create a layer mask for the Shootable layer.
            shootableMask = LayerMask.GetMask ("Shootable");

            // Set up the references.
            gunParticles = GetComponent<ParticleSystem> ();
            gunLine = GetComponent <LineRenderer> ();
            gunAudio = GetComponent<AudioSource> ();
            gunLight = GetComponent<Light> ();
            //faceLight = GetComponentInChildren<Light> ();

            if(VRTracker.instance != null && VRTracker.instance.getTag(VRTracker.TagType.Gun))
                vrGun = VRTracker.instance.getTag(VRTracker.TagType.Gun);
            // Callback for Local layer, not server
            /*if (vrGun)
            {
                vrGun.OnDown += Shoot;
                //vrGun.OnUp += ;
            }*/
        }


        void Update ()
        {
            // Add the time since Update was last called to the timer.
            timer += Time.deltaTime;

#if !MOBILE_INPUT
            // If the Fire1 button is being press and it's time to fire...
			/*if((Input.GetButton ("Fire1")) && timer >= timeBetweenBullets && Time.timeScale != 0)
            {
                // ... shoot the gun.
                Shoot (transform.position, transform.forward);
            }*/
#else
            // If there is input on the shoot direction stick and it's time to fire...
            if ((CrossPlatformInputManager.GetAxisRaw("Mouse X") != 0 || CrossPlatformInputManager.GetAxisRaw("Mouse Y") != 0) && timer >= timeBetweenBullets)
            {
                // ... shoot the gun
                //Shoot();
            }
#endif
            // If the timer has exceeded the proportion of timeBetweenBullets that the effects should be displayed for...
            if(timer >= timeBetweenBullets * effectsDisplayTime)
            {
                // ... disable the effects.
                DisableEffects ();
            }
        }


        public void DisableEffects ()
        {
            // Disable the line renderer and the light.
            gunLine.enabled = false;
			faceLight.enabled = false;
            gunLight.enabled = false;
        }


        public int Shoot (Vector3 origin, Vector3 direction, out Vector3 destination)
        {
            Debug.Log("Shooting");
            Debug.Log(origin);
            // Reset the timer.
            /*timer = 0f;

            // Play the gun shot audioclip.
            gunAudio.Play ();

            // Enable the lights.
            gunLight.enabled = true;
			faceLight.enabled = true;

            // Stop the particles from playing if they were, then start the particles.
            gunParticles.Stop ();
            gunParticles.Play ();

            // Enable the line renderer and set it's first position to be the end of the gun.
            gunLine.enabled = true;
            gunLine.SetPosition (0, origin);
            */
            // Set the shootRay so that it starts at the end of the gun and points forward from the barrel.
            shootRay.origin = origin;
            shootRay.direction = direction;
            //destination = origin;
            int score = 0;

            // Perform the raycast against gameobjects on the shootable layer and if it hits something...
            if(Physics.Raycast (shootRay, out shootHit, range, shootableMask))
            {
                if (PlayerManager.instance.startGame)
                {
                    // Try and find an EnemyHealth script on the gameobject hit.
                    EnemyHealth enemyHealth = shootHit.collider.GetComponent<EnemyHealth>();

                    // If the EnemyHealth component exist...
                    if (enemyHealth != null)
                    {
                        // ... the enemy should take damage.
                        score = enemyHealth.TakeDamage(damagePerShot, shootHit.point);
                    }
                    else
                    {
                        Debug.Log("No ennemy");
                    }
                }
                else
                {

                    VRStandardAssets.ShootingGallery.ShootingTarget shootingTarget = shootHit.collider.GetComponent<VRStandardAssets.ShootingGallery.ShootingTarget>();
                    // If the EnemyHealth component exist...
                    if (shootingTarget != null)
                    {
                        // ... the enemy should take damage.
                        if (vrGun != null && transform.parent.parent.GetComponent<NetworkShoot>().isLocalPlayer)
                        {
                            //shootingTarget.ImReady();
                            score = -1;
                        }
                        else
                        {
                            score = -2;
                            Debug.Log("Not local player");
                        }
                    }
                    else
                    {
                        Debug.Log("No ennemy");
                    }


                }


                // Set the second position of the line renderer to the point the raycast hit.
                //gunLine.SetPosition (1, shootHit.point);
                destination = shootHit.point;
            }
            // If the raycast didn't hit anything on the shootable layer...
            else
            {
                Debug.Log("Shooting fail");

                // ... set the second position of the line renderer to the fullest extent of the gun's range.
                destination = shootRay.origin + shootRay.direction * range;
            }
            return score;
        }

        public void ShootEffects(Vector3 origin, Vector3 direction, Vector3 destination)
        {

            // Reset the timer.
            timer = 0f;

            // Play the gun shot audioclip.
            gunAudio.Play();

            // Enable the lights.
            gunLight.enabled = true;
            faceLight.enabled = true;

            // Stop the particles from playing if they were, then start the particles.
            gunParticles.Stop();
            gunParticles.Play();

            // Enable the line renderer and set it's first position to be the end of the gun.
            gunLine.enabled = true;
            gunLine.SetPosition(0, origin);
            gunLine.SetPosition(1, destination);

            // Set the shootRay so that it starts at the end of the gun and points forward from the barrel.
            //shootRay.origin = origin;
            //shootRay.direction = direction;

        }
    }
}