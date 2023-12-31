using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemyController : MonoBehaviour
{
    public float speed;
    public bool vertical;
    public float changeTime = 3.0f;

    public ParticleSystem smokeEffect;

    public TextMeshProUGUI fixedText;
    public TextMeshProUGUI stunnedText;

    public GameObject playerCharacter;

    private RubyController rubyController;
    
    Rigidbody2D rigidbody2D;

    public AudioClip fixedSound;

    float timer;
    float storedSpeed;
    int direction = 1;
    bool broken = true;
    bool stunned = false;
    
    Animator animator;
    
    // Start is called before the first frame update
    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        timer = changeTime;
        animator = GetComponent<Animator>();
        GameObject rubyControllerObject = GameObject.FindWithTag("RubyController");
        storedSpeed = speed;

        if (rubyControllerObject != null)
        {
            rubyController = rubyControllerObject.GetComponent<RubyController>();
        }

        if (RubyController.fixedRobotsAmount == 0)
        {
            fixedText.text = RubyController.fixedRobotsAmount.ToString();
        }
    }

    void Update()
    {
        //remember ! inverse the test, so if broken is true !broken will be false and return won’t be executed.
        if(!broken)
        {
            return;
        }
        
        timer -= Time.deltaTime;

        if (timer < 0)
        {
            direction = -direction;
            timer = changeTime;
        }

        if(stunned == true)
        {
            speed = 0f;
        }
    }
    
    void FixedUpdate()
    {
        //remember ! inverse the test, so if broken is true !broken will be false and return won’t be executed.
        if(!broken)
        {
            return;
        }
        
        Vector2 position = rigidbody2D.position;
        
        if (vertical)
        {
            position.y = position.y + Time.deltaTime * speed * direction;
            animator.SetFloat("Move X", 0);
            animator.SetFloat("Move Y", direction);
        }
        else
        {
            position.x = position.x + Time.deltaTime * speed * direction;
            animator.SetFloat("Move X", direction);
            animator.SetFloat("Move Y", 0);
        }
        
        rigidbody2D.MovePosition(position);
    }
    
    void OnCollisionEnter2D(Collision2D other)
    {
        RubyController player = other.gameObject.GetComponent<RubyController >();

        if (player != null)
        {
            player.ChangeHealth(-1);
        }
    }
    
    //Public because we want to call it from elsewhere like the projectile script
    public void Fix()
    {
        broken = false;
        rigidbody2D.simulated = false;
        //optional if you added the fixed animation
        animator.SetTrigger("Fixed");
        smokeEffect.Stop();
        rubyController.PlaySound(fixedSound);

        RubyController.fixedRobotsAmount = RubyController.fixedRobotsAmount +1;
        fixedText.text = RubyController.fixedRobotsAmount.ToString();
        if(RubyController.fixedRobotsAmount == rubyController.maxRobots)
        {
            rubyController.winConRobot = true;
        }
    }

    public void Stun()
    {
        stunned = true;
        rubyController.winConStun = true;
        StartCoroutine(StunTime());
    }

    IEnumerator StunTime()
    {
        yield return new WaitForSeconds(3);
        stunned = false;
        speed = storedSpeed;
    }
}
