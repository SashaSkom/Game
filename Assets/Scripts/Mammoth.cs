using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Mammoth : Monster
{
    [SerializeField]
    private int lives = 3;
    private const float constantSpeed = 2.0F;
    private const float time = 2.0F;
    public int Lives
    {
        get { return lives; }
        set
        {
            lives = value;
        }
    }

    [SerializeField]
    private float speed = 2.0F;

    private Vector3 direction;

    private bool isFacingLeft = true;
    public Transform groundCheck;
    public LayerMask groundLayers;
    public Rigidbody2D rb;

    private SpriteRenderer sprite;

    protected void Awake()
    {
        sprite = GetComponentInChildren<SpriteRenderer>();
    }

    protected void Start()
    {
        direction = -transform.right;
    }
    protected void Update()
    {
        StartCoroutine(YourCoroutine());
        Move();

    }
    IEnumerator YourCoroutine()
    {
        yield return new WaitForSeconds(time);
        if (Mathf.Abs(speed) == constantSpeed)
            speed *= 2;
        else if (isFacingLeft)
            speed = constantSpeed;
        else
            speed = -constantSpeed;
        StopAllCoroutines();
    }

    private void Hit()
    {
    }

    protected override void OnTriggerEnter2D(Collider2D collider)
    {
        Unit unit = collider.GetComponent<Unit>();

        if (unit && unit is Character)
        {
            if (Mathf.Abs(unit.transform.position.x - transform.position.x) < 0.6F) ReceiveDamage();
            else
            {
                unit.ReceiveDamage();
                if (Mathf.Abs(speed) == constantSpeed * 2)
                    unit.ReceiveDamage();
            }
        }

        Spear spear = collider.gameObject.GetComponent<Spear>();
        if (spear)
        {
            ReceiveDamage();
        }
    }

    private void Move()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position + transform.up * 0.3F + transform.right * direction.x * 0.5F, 0.1F);

        RaycastHit2D groundInfo = Physics2D.Raycast(groundCheck.position, Vector2.down, 1f, groundLayers);

        if (colliders.Length > 3 && colliders.All(x => !x.GetComponent<Character>() && !x.GetComponent<Spear>()) || groundInfo.collider == false)
        {
            isFacingLeft = !isFacingLeft;
            speed = -speed;
            direction *= -1.0F;
            transform.localScale = new Vector2(-transform.localScale.x, 1f);
        }
        rb.velocity = new Vector2(-speed, rb.velocity.y);
    }

    public override void ReceiveDamage()
    {
        Lives--;

        rb.velocity = Vector3.zero;
        rb.AddForce(transform.up * 8.0F, ForceMode2D.Impulse);

        if (lives <= 0) Die();
    }
}