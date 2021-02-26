using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{
    public GameObject locked;
    private Animator lockedAnim;
    private AudioSource lockedSound;

    private int moveLeft, moveRight;
    private bool jump;
    private bool attack;
    private PlayerCharacterController movement;
    private PlayerCombatController combat;

    private bool[] moves = {true, false, false, false}; // left, right, jump, attack, wall jump
    // private bool[] moves = {true, true, true, true}; // left, right, jump, attack, wall jump
    private int lastRight;
    private bool frozen = false;

    // Start is called before the first frame update
    void Start()
    {
        movement = GetComponent<PlayerCharacterController>();
        combat = GetComponent<PlayerCombatController>();

        lockedAnim = locked.GetComponent<Animator>();
        lockedSound = locked.GetComponent<AudioSource>();

        moveLeft = 0;
        moveRight = 0;
        lastRight = 0;
        jump = false;
        attack = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(!frozen)
        {
            movement.Move(moveLeft + moveRight, jump, true);
            combat.Combat(attack);
        }
        else
        {
            movement.Move(0, false, false);
            combat.Combat(false);
        }
        jump = false;
        attack = false;
    }

    public void OnJump()
    {
        if(LevelManager.GetInstance().IsPaused()) return;
        
        if(!moves[2]) PlayLocked();
        else jump = true;
    }

    public void OnMoveLeft(InputValue input)
    {
        if(LevelManager.GetInstance().IsPaused()) return;
        
        if(moves[0]) moveLeft = (int) input.Get<float>();
    }

    public void OnMoveRight(InputValue input)
    {
        if(LevelManager.GetInstance().IsPaused()) return;
        
        int newRight = (int) input.Get<float>();
        if(moves[1]) moveRight = newRight;
        else if(lastRight == 0 && newRight != 0) PlayLocked();
        lastRight = newRight;
    }

    public void OnAttack()
    {
        if(LevelManager.GetInstance().IsPaused()) return;

        if(!moves[3]) PlayLocked();
        else attack = true;
    }

    public void OnPause()
    {
        LevelManager.GetInstance().TogglePause();
    }

    public void EnableMove(int move)
    {
        moves[move] = true;
    }

    public void SetFrozen(bool val)
    {
        frozen = val;
    }

    private void PlayLocked()
    {
        if(!movement.IsProne() && !frozen)
        {
            lockedAnim.SetTrigger("Locked");
            lockedSound.Play();
        }
    }
}
