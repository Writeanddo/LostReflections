using UnityEngine;

public class OwlAI : MonoBehaviour
{
    public Transform player;
    public float flapAmplitutde = 1f;
    public float moveSpeed = 3f;
    public Transform[] waypoints;
    public AudioClip[] clips;

    private float nextSound, lastSound;

    private EnemyCombatController combat;
    private AudioSource audioSource;

    private Vector2 pos;
    private int nWaypoints;
    public int startingIndex = 1;
    private int index;
    private BoxCollider2D col;
    private float a, b;

    void Start()
    {
        combat = GetComponent<EnemyCombatController>();
        audioSource = GetComponent<AudioSource>();
        col = GetComponent<BoxCollider2D>();

        float max = -0.1f, min = -0.43f;
        a = (max - min) / 2 - max;
        b = 2 / (max - min);
    }

    void OnEnable()
    {
        nWaypoints = waypoints.Length;
        index = startingIndex;
        if(nWaypoints == 1) pos = waypoints[0].position;
        else pos = transform.position;
        if(nWaypoints != 0) FaceTowardsGoal();
        nextSound = 0;
        lastSound = -1;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(!combat.IsAlive()) return;

        if(Vector2.Distance(player.position, pos) < 3)
        {
            if(Time.time > lastSound + nextSound)
            {
                PlayNoise();
                lastSound = Time.time;
                nextSound = Random.Range(5f, 10f);
            }
        }

        if(nWaypoints > 1)
        {
            Vector2 targetPos = waypoints[index].position;
            pos = Vector2.MoveTowards(pos, targetPos, moveSpeed * Time.deltaTime);
            if(pos == targetPos)
            {
                index = (index + 1) % nWaypoints;
                FaceTowardsGoal();
            }
        }

        Vector2 offset = new Vector2(0, (col.offset.y + a) * b * flapAmplitutde);
        transform.position = pos + offset;
    }

    private void FaceTowardsGoal()
    {
        float x = 0;
        if(waypoints[index].position.x < transform.position.x) x = -1;
        else x = 1;
        transform.localScale = new Vector2(x, 1);
    }

    private void PlayNoise()
    {
        audioSource.PlayOneShot(clips[Random.Range(0, clips.Length)]);
    }
}
