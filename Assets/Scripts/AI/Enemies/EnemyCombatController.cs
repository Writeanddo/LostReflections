using UnityEngine;

public class EnemyCombatController : MonoBehaviour
{

    public int maxLives = 1;
    public float dieDuration = 1f;
    public float invincibilityDuration = 1f;
    public float invincibilityFlashPeriod = 0.2f;
    public AudioSource deathNoise;
    public AudioClip hitNoise;
    public float hitMultiplier = 1;

    private Animator animator;
    private float hitStart, dieStart;
    private int lives;
    private bool invincible;
    private Vector3 startingPos, startingScale;
    private Quaternion startingRot;
    private SpriteRenderer sprite;
    
    // Start is called before the first frame update

    void Start()
    {
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        startingPos = transform.position;
        startingRot = transform.rotation;
        startingScale = transform.localScale;
    }

    void OnEnable()
    {
        dieStart = -1;
        hitStart = -1;
        lives = maxLives;
        GetComponent<Collider2D>().enabled = true;
        invincible = false;
    }

    // Update is called once per frame
    void Update()
    {
        
        if(invincible)
        {
            if(Time.time > hitStart + invincibilityDuration) // Check if we're done being invincible
            {
                invincible = false;
                sprite.color = new Color(1, 1, 1, 1);
                return;
            }
            
            // If not handle invinc flashing
            float t = (Time.time - hitStart) % invincibilityFlashPeriod;
            if(t < invincibilityFlashPeriod / 2) sprite.color = new Color(1, 1, 1, 0);
            else sprite.color = new Color(1, 1, 1, 1);
        }

        if(dieStart != -1)
        {

            if(Time.time > dieStart + dieDuration) Kill();
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag == "Player")
        {
            col.gameObject.GetComponent<PlayerCombatController>().Hit(transform.position.x, hitMultiplier);
        }
    }
    
    void OnTriggerStay2D(Collider2D col)
    {
        if(col.tag == "Player")
        {
            col.gameObject.GetComponent<PlayerCombatController>().Hit(transform.position.x);
        }
    }

    public void Hit()
    {
        if(!invincible)
        {
            lives -= 1;
            invincible = true;

            if(lives == 0)
            {
                animator.SetTrigger("Die");
                dieStart = Time.time;
                GetComponent<Collider2D>().enabled = false;
                deathNoise.Play();
            }
            else{
                hitStart = Time.time;
                deathNoise.PlayOneShot(hitNoise);
            }
        }
    }

    public void Kill()
    {
        transform.position = startingPos;
        transform.rotation = startingRot;
        transform.localScale = startingScale;
        gameObject.SetActive(false);
    }

    public bool IsAlive()
    {
        return lives > 0;
    }

    public bool IsInvincible()
    {
        return invincible;
    }
}
