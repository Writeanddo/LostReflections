using UnityEngine;
using UnityEngine.Events;

public class PlayerCharacterController : MonoBehaviour
{
	[SerializeField] private float m_JumpForce = 400f;							// Amount of force added when the player jumps.
	[Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;	// How much to smooth out the movement
	[SerializeField] private bool m_AirControl = false;							// Whether or not a player can steer while jumping;
	[SerializeField] private LayerMask m_WhatIsGround;							// A mask determining what is ground to the character
	[SerializeField] private Transform m_GroundCheck;							// A position marking where to check if the player is grounded.

	public float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
	public float jumpTimeout = 0.1f;
	private bool m_Grounded;            // Whether or not the player is grounded.
	private Rigidbody2D m_Rigidbody2D;
    private Animator m_Animator;
	private bool m_FacingRight;  // For determining which way the player is currently facing.
	private Vector3 m_Velocity = Vector3.zero;
	private bool prone;
	private float lastJump;

	[Header("Events")]
	[Space]

	public UnityEvent OnLandEvent;

	[System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }

	private void Awake()
	{
		m_Rigidbody2D = GetComponent<Rigidbody2D>();
        m_Animator = GetComponent<Animator>();

		if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();

		m_FacingRight = transform.localScale.x == 1;
		prone = true;
		lastJump = 0;
	}

	private void FixedUpdate()
	{
		bool wasGrounded = m_Grounded;
		m_Grounded = false;

		// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
		// This can be done using layers instead but Sample Assets will not overwrite your project settings.
		Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
			{
				m_Grounded = true;
				if (!wasGrounded)
					OnLandEvent.Invoke();
			}
		}
        m_Animator.SetBool("Ground", m_Grounded);
        m_Animator.SetFloat("vSpeed", m_Rigidbody2D.velocity.y);
	}


	public void Move(float move, bool jump, bool lockMovement)
	{
		if(prone && move != 0)
		{
			m_Animator.SetTrigger("Get Up");
			string anim = m_Animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
			prone = anim == "Prone" || anim == "Get Up";
			return;
		}
		//only control the player if grounded or airControl is turned on
		if (m_Grounded || (m_AirControl && lockMovement))
		{
            float speed = move * 10f;
            m_Animator.SetFloat("Speed", Mathf.Abs(speed));
			// Move the character by finding the target velocity
			Vector3 targetVelocity = new Vector2(speed, m_Rigidbody2D.velocity.y);
			// And then smoothing it out and applying it to the character
			m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

			// If the input is moving the player right and the player is facing left...
			if (move > 0 && !m_FacingRight)
			{
				// ... flip the player.
				Flip();
			}
			// Otherwise if the input is moving the player left and the player is facing right...
			else if (move < 0 && m_FacingRight)
			{
				// ... flip the player.
				Flip();
			}
		}
		// If the player should jump...
		if (m_Grounded && jump && Time.time > lastJump + jumpTimeout)
		{
			// Add a vertical force to the player.
			m_Grounded = false;
            m_Animator.SetBool("Ground", false);
            m_Animator.SetFloat("vSpeed", 0.1f);
			m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
			lastJump = Time.time;
		}
	}

	public void Launch(Vector2 force)
	{
		if((!m_FacingRight && force.x < 0) || (m_FacingRight && force.x > 0)) Flip();
		m_Rigidbody2D.velocity = new Vector2();
		m_Rigidbody2D.AddForce(force);
		m_Animator.SetTrigger("Hit");
	}

	public void Die()
	{
		m_Animator.SetTrigger("Die");
	}

	public void Reset(Vector3 pos, Vector3 scale)
	{
        transform.position = pos;
        transform.localScale = scale;
		m_Animator.ResetTrigger("Die");
		m_Animator.Play("Idle");
		m_FacingRight = transform.localScale.x == 1;
	}

	public bool IsProne()
	{
		return prone;
	}

	private void Flip()
	{
		// Switch the way the player is labelled as facing.
		m_FacingRight = !m_FacingRight;

		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}
}