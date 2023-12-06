using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class RubyController : MonoBehaviour
{
    public float speed = 3.0f;
    
    public int maxHealth = 5;
    
    public GameObject projectilePrefab;
    public GameObject stunProjectile;
    public GameObject winText;
    public GameObject lossText;

    public ParticleSystem healthEffect;
    public ParticleSystem hitEffect;
    
    public AudioClip throwSound;
    public AudioClip hitSound;

    public AudioClip chargeSound;
    public AudioClip stunThrowSound;
    
    public int health { get { return currentHealth; }}
    int currentHealth;
    
    public static int fixedRobotsAmount;
    public int maxRobots = 2;

    public static int thanksForTalking;
    public int thankedForTalking = 1;
    
    public float timeInvincible = 2.0f;
    public bool stunnerOn = false;
    bool isInvincible;
    float invincibleTimer;
    float stunCogsLeft = 3;
    float holdDownStartTime;
    float holdDownEndTime;

    bool gameOver;
    bool noCogs;
    
    Rigidbody2D rigidbody2d;
    float horizontal;
    float vertical;
    
    Animator animator;
    Vector2 lookDirection = new Vector2(1,0);
    
    AudioSource audioSource;
    
    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        
        currentHealth = maxHealth;

        lossText.SetActive(false);
        winText.SetActive(false);

        fixedRobotsAmount = 0;
        thanksForTalking = 0;

        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        
        Vector2 move = new Vector2(horizontal, vertical);
        
        if(!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }
        
        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude);
        
        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
                isInvincible = false;
        }

        if (health <= 0)
        {
            gameOver = true;
            lossText.SetActive(true);
        }
        
        if (Input.GetKeyDown(KeyCode.C))
        {
            holdDownStartTime = Time.time;

            if (stunCogsLeft == 0)
            {
                noCogs = true;
            }

            if (stunnerOn == true && noCogs != true)
            {
                PlaySound(chargeSound);
            }
        }
        
            
        if (Input.GetKeyUp(KeyCode.C))
        {
            holdDownEndTime = Time.time;

            audioSource.Stop();

            if(holdDownEndTime - holdDownStartTime > 1f)
            {
                if (stunnerOn == true && noCogs != true)
                {
                    Launch(stunProjectile);
                    ChangeStunBar(-1);
                }
            }
                else
            {
                Launch(projectilePrefab);
            }
        }
        
        if (Input.GetKeyDown(KeyCode.X))
        {
            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
            if (hit.collider != null)
            {
                NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
                if (character != null)
                {
                    character.DisplayDialog();
                    thanksForTalking = 1;
                }
            }
        }
        
        if (gameOver == true)
        {
            invincibleTimer = 1.0f;
            speed = 0.0f;

            if (Input.GetKeyDown(KeyCode.R))
            {
                // Reload the current scene
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }
    
    void FixedUpdate()
    {
        Vector2 position = rigidbody2d.position;
        position.x = position.x + speed * horizontal * Time.deltaTime;
        position.y = position.y + speed * vertical * Time.deltaTime;

        rigidbody2d.MovePosition(position);
    }

    public void ChangeHealth(int amount)
    {
        if (amount > 0)
        {
            Instantiate(healthEffect, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
        }
        
        if (amount < 0)
        {
            if (isInvincible)
                return;
            
            isInvincible = true;
            invincibleTimer = timeInvincible;
            Instantiate(hitEffect, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
            PlaySound(hitSound);
        }
        
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        
        UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);
    }
    
    void Launch(GameObject bulletType)
    {
        GameObject projectileObject = Instantiate(bulletType, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(lookDirection, 300);

        animator.SetTrigger("Launch");

        if(bulletType == stunProjectile)
        {
            PlaySound(stunThrowSound);
        }
        else
        {
            PlaySound(throwSound);
        }
    }

    public void ChangeStunBar(float amount)
    {
        if(stunCogsLeft == 0)
        {
            stunnerOn = false;
        }

        stunCogsLeft = Mathf.Clamp(stunCogsLeft + amount, 0, 3);
        UIPowerupBar.instance.SetValue(stunCogsLeft / (float)3f);
    }
    
    public void winScreen()
    {
        if (gameOver == false)
        {
            gameOver = true;
            winText.SetActive(true);
        }
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
}
