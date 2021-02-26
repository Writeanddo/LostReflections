using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatController : MonoBehaviour
{
    public Animator swordAnimator;
    public Transform attackPoint;

    public float attackRange = 0.6f;
    public float attackDuration = 0.35f;
    public float attackCooldown = 0f;
    public LayerMask enemyLayers;

    public float hitLaunchForce = 100f;
    public float hitDuration = 0.5f;
    public float invincibilityDuration = 1f;
    public float invincibilityFlashPeriod = 0.1f;

    private PlayerCharacterController movement;
    private PlayerInputController input;
    private PlayerAudioController speaker;

    private float attackStart;
    private List<string> enemiesHitThisSwing;

    private float hitStart;
    private bool invincible, dead;
    private SpriteRenderer sprite;

    public int lives = 3;

    void Start()
    {
        movement = GetComponent<PlayerCharacterController>();
        input = GetComponent<PlayerInputController>();
        speaker = GetComponent<PlayerAudioController>();
        sprite = GetComponent<SpriteRenderer>();

        attackStart = -1;
        invincible = false;
        dead = false;
        enemiesHitThisSwing = new List<string>();
    }

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

            // Check if we can move again
            if(Time.time > hitStart + hitDuration)
            {
                input.SetFrozen(false);
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(attackStart != -1)
        {
            if(Time.time < attackStart + attackDuration)
            {
                Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
                foreach(Collider2D enemy in hitEnemies)
                {
                    if(!enemiesHitThisSwing.Contains(enemy.name))
                    {
                        enemiesHitThisSwing.Add(enemy.name);

                        enemy.gameObject.GetComponent<EnemyCombatController>().Hit();
                    }

                }
            }
            else if(Time.time > attackStart + attackDuration + attackCooldown)
            {
                attackStart = -1;
                enemiesHitThisSwing.Clear();
            }
        }
    }

    public void Combat(bool attack)
    {
        if(attack && attackStart == -1)
        {
            invincible = false;
            sprite.color = new Color(1, 1, 1, 1);

            attackStart = Time.time;
            swordAnimator.SetTrigger("Attack");
            speaker.PlaySwordNoise();
        }
    }

    public void Hit(float point, float multiplier = 1)
    {
        if(!dead && !invincible)
        {
            lives--;
            LevelManager.GetInstance().UpdateLivesUI(lives);
            if(lives > 0)
            {
                Vector2 force = new Vector2(hitLaunchForce * (point < transform.position.x ? 1 : -1) * multiplier, hitLaunchForce * 0.5f * multiplier);
                movement.Launch(force);
                input.SetFrozen(true);
                hitStart = Time.time;
                invincible = true;
                speaker.PlayHitNoise();
            }
            else
            {
                speaker.PlayDeathNoise();
                LevelManager.GetInstance().StartRespawn();
                input.SetFrozen(true);
                movement.Die();
                dead = true;
            }
        }
    }

    public void FullHeal()
    {
        lives = LevelManager.GetInstance().maxHearts;
        LevelManager.GetInstance().UpdateLivesUI(lives);
        dead = false;
    }
}
