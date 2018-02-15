using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
namespace CompleteProject
{
    public class PlayerHealth : NetworkBehaviour
    {
        public int startingHealth = 100;                            // The amount of health the player starts the game with.

        [SyncVar(hook = "OnChangeHealth")]                          //Synchronize on the network the health bar
        public int currentHealth;                                   // The current health the player has.
        public Slider healthSlider;                                 // Reference to the UI's health bar.
        public Image damageImage;                                   // Reference to an image to flash on the screen on being hurt.
        public AudioClip deathClip;                                 // The audio clip to play when the player dies.
        public float flashSpeed = 5f;                               // The speed the damageImage will fade at.
        public Color flashColour = new Color(1f, 0f, 0f, 0.1f);     // The colour the damageImage is set to, to flash.


        Animator anim;                                              // Reference to the Animator component.
        AudioSource playerAudio;                                    // Reference to the AudioSource component.
        PlayerShooting playerShooting;                              // Reference to the PlayerShooting script.
        public bool isDead;                                                // Whether the player is dead.
        bool damaged;                                               // True when the player gets damaged.


        void Awake ()
        {
            // Setting up the references.
            anim = GetComponent <Animator> ();
            playerAudio = GetComponent <AudioSource> ();
            playerShooting = GetComponentInChildren <PlayerShooting> ();

            // Set the initial health of the player.
            //currentHealth = startingHealth;
        }


        void Update ()
        {
            // If the player has just been damaged...
            if(damaged)
            {
                // ... set the colour of the damageImage to the flash colour.
                damageImage.color = flashColour;
            }
            // Otherwise...
            else
            {
                // ... transition the colour back to clear.
                damageImage.color = Color.Lerp (damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
            }

            // Reset the damaged flag.
            damaged = false;
        }

        [ServerCallback]
        void OnEnable()
        {
            currentHealth = startingHealth;
        }

        [Server]
        public void TakeDamage (int amount)
        {
            // Set the damaged flag so the screen will flash.
            damaged = true;
            Debug.Log("Taking damage " + amount);
            // Reduce the current health by the damage amount.
            currentHealth -= amount;

            // Set the health bar's value to the current health.
            //healthSlider.value = currentHealth;

            // Play the hurt sound effect.
            playerAudio.Play ();

            // If the player has lost all it's health and the death flag hasn't been set yet...
            if(currentHealth <= 0 && !isDead)
            {
                // ... it should die.
                RpcDeath ();
                PlayerManager.instance.DeadPlayer();
            }
        }

        [ClientRpc]
        void RpcDeath ()
        {
            // Set the death flag so this function won't be called again.
            isDead = true;

            // Turn off any remaining shooting effects.
            //playerShooting.DisableEffects ();

            // Tell the animator that the player is dead.
            anim.SetTrigger ("Die");

            // Set the audiosource to play the death clip and play it (this will stop the hurt sound from playing).
            playerAudio.clip = deathClip;
            playerAudio.Play ();
        }


        public void RestartLevel ()
        {
            // Reload the level that is currently loaded.
            //SceneManager.LoadScene (0);
        }

        void OnChangeHealth(int healthValue)
        {
            currentHealth = healthValue;
            if (isLocalPlayer)
            {
                healthSlider.value = currentHealth;
                if(currentHealth <= 0 && !isDead)
                {
                    DeathView(true);

                }else if( currentHealth == startingHealth)
                {
                    DeathView(false);
                }
            }
        }

        private void DeathView(bool dead)
        {
            Camera cam = GetComponentInChildren<Camera>();
            //GetComponent<ScoreScript>().setScore(0);
            if(cam != null)
            {
                cam.GetComponent<GrayscaleEffect>().enabled = dead;
            }
            GetComponent<Respawner>().SetActiveSpawnPoint(dead);
        }

        [Command]
        public void CmdRespawn()
        {
            //TODO inform the master
            isDead = false;
            currentHealth = startingHealth;
        
            // Tell the animator that the player respawns.
            anim.SetTrigger("Respawn");
            PlayerManager.instance.RespawnPlayer();
        }


        [ClientRpc]
        void RpcRespawn()
        {
            // Set the death flag so this function won't be called again.
            isDead = false;

            // Turn off any remaining shooting effects.
            //playerShooting.DisableEffects ();

            // Tell the animator that the player is dead.
            anim.SetTrigger("Die");           
        }
    }
}