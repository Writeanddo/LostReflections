using UnityEngine;

public class FoxAI : MonoBehaviour
{
    public float moveSpeed;
    public float runMultiplier;
    public Transform waypoint1, waypoint2;
    public GameObject player;
    public float ySight, xForwardSight, xBackSight;
    public AudioClip[] barkClips;

    private EnemyCombatController combat;
    private Animator animator;
    private int nWaypoints;
    public int startingIndex = 1;
    private int index;
    private float lastSound, nextSound;
    private BoxCollider2D col;
    private AudioSource audioSource;

    void Start()
    {
        combat = GetComponent<EnemyCombatController>();
        animator = GetComponent<Animator>();
        col = GetComponent<BoxCollider2D>();
        audioSource = GetComponent<AudioSource>();

        if(waypoint1.position.x > waypoint2.position.x)
        {
            Vector3 temp = waypoint2.position;
            waypoint2.position = waypoint1.position;
            waypoint1.position = temp;
        }
    }

    void OnEnable()
    {
        nWaypoints = 2;
        index = startingIndex;
        if(nWaypoints != 0) FaceTowardsGoal((index == 0 ? waypoint1.position : waypoint2.position).x);
        lastSound = 0;
        nextSound = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(!combat.IsAlive()) return;

        if(combat.IsInvincible())
        {
            animator.SetBool("Run", true);
            float x = player.transform.position.x;
            FaceTowardsGoal(x, true);
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(x, transform.position.y), -moveSpeed * runMultiplier * Time.deltaTime);
        }
        else if(PlayerInRange())
        {
            if(Time.time > lastSound + nextSound)
            {
                PlayBark();
                lastSound = Time.time;
                nextSound = Random.Range(3f, 8f);
            }

            animator.SetBool("Run", true);
            float x = player.transform.position.x;
            FaceTowardsGoal(x);
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(x, transform.position.y), moveSpeed * runMultiplier * Time.deltaTime);
        }
        else
        {
            nextSound = 0;
            FaceTowardsGoal((index == 0 ? waypoint1.position : waypoint2.position).x);
            animator.SetBool("Run", false);
            Vector2 targetPos = index == 0 ? waypoint1.position : waypoint2.position;
            targetPos.y = transform.position.y;
            transform.position = Vector2.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            if(((Vector2) transform.position) == targetPos) index = (index + 1) % nWaypoints;
        }
    }

    private bool PlayerInRange()
    {
        float relY = player.transform.position.y - transform.position.y;
        float x = player.transform.position.x;
        if(Mathf.Abs(relY) < ySight && x > waypoint1.position.x && x < waypoint2.position.x) return true;
        return false;
    }
    
    private void FaceTowardsGoal(float goal, bool reverse = false)
    {
        float x = 0;
        if(goal < transform.position.x) x = -1;
        else x = 1;
        transform.localScale = new Vector2(x * (reverse ? -1 : 1), 1);
    }

    private void PlayBark()
    {
        audioSource.PlayOneShot(barkClips[Random.Range(0, barkClips.Length)]);
    }
}
