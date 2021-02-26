using UnityEngine;

public class FinalMirrorController : MonoBehaviour
{
    public GameObject player, crack;
    public SymbolFollow[] orbs;
    public float delay = 3;

    private float triggerStart;
    private AudioSource audioSource; 

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        triggerStart = -1;
    }

    // Update is called once per frame
    void Update()
    {
        if(triggerStart != -1) // Player is frozen
        {
            if(Time.time > triggerStart + delay)
            {
                crack.SetActive(true);
                triggerStart = -1;
                LevelManager.GetInstance().TransitionScene();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag == "Player")
        {
            triggerStart = Time.time;
            player.GetComponent<PlayerInputController>().SetFrozen(true);
            MusicManager.GetInstance().HushBackground(0.4f);
            audioSource.Play();
            foreach(SymbolFollow orb in orbs)
            {
                orb.FinalSpot();
            }
        }
    }
}