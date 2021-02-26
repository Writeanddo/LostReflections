using UnityEngine;

public class MirrorController : MonoBehaviour
{
    public GameObject player, reflection, orb, crack;
    public Animator playerAnim, playerSwordAnim, reflectAnim, reflectSwordAnim;
    public float delay;
    public int move;
    public string text;

    private bool isStatic;
    private float mirrorLine;
    private float triggerStart;
    private AudioSource audioSource; 

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        isStatic = true;
        triggerStart = -1;
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector2.Distance(transform.position, player.transform.position) < 20)
        {
            if(!reflection.activeSelf) reflection.SetActive(true);

            if(!isStatic)
            {
                reflection.transform.position = new Vector2((2 * mirrorLine) - player.transform.position.x, player.transform.position.y);
                reflection.transform.localScale = new Vector2(player.transform.localScale.x * transform.localScale.x * -1, 1);

                string playerState = playerAnim.GetCurrentAnimatorClipInfo(0)[0].clip.name;
                string currentAnim = reflectAnim.GetCurrentAnimatorClipInfo(0)[0].clip.name;
                
                if(currentAnim != playerState) reflectAnim.SetTrigger(playerState);

                playerState = playerSwordAnim.GetCurrentAnimatorClipInfo(0)[0].clip.name;
                currentAnim = reflectSwordAnim.GetCurrentAnimatorClipInfo(0)[0].clip.name;

                if(playerState == "Attack" && currentAnim != "Attack") reflectSwordAnim.SetTrigger("Attack");
            }
            else if(triggerStart != -1) // Player is frozen
            {
                if(Time.time > triggerStart + delay)
                {
                    crack.SetActive(true);
                    MusicManager.GetInstance().IncreaseMusicIntensity();
                    MusicManager.GetInstance().TurnUpBackground(2);
                    //if(move == 1) LevelManager.GetInstance().StartMusic();
                    LevelManager.GetInstance().SetRespawnPoint(player.transform.position, player.transform.localScale.x);
                    mirrorLine = reflection.transform.position.x + (player.transform.position.x - reflection.transform.position.x) / 2;
                    isStatic = false;
                    triggerStart = -1;
                    PlayerInputController playerInput = player.GetComponent<PlayerInputController>();
                    playerInput.EnableMove(move);
                    playerInput.SetFrozen(false);
                    orb.GetComponent<SymbolFollow>().StartFollow();
                    LevelManager.GetInstance().ShowPrompt(text, 8);
                }
            }
        }
        else
        {
            if(reflection.activeSelf) reflection.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag == "Player")
        {
            player.GetComponent<PlayerCombatController>().FullHeal();
            if(isStatic)
            {
                player.GetComponent<PlayerInputController>().SetFrozen(true);
                MusicManager.GetInstance().HushBackground(0.4f);
                audioSource.Play();
                orb.SetActive(true);
                triggerStart = Time.time;
            }
        }
    }
}

