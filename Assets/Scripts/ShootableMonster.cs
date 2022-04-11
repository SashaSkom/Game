﻿using UnityEngine;
using System.Linq;

public class ShootableMonster : Monster
{
    [SerializeField]
    private float rate = 2.0F;
    [SerializeField]
    private Color SpearColor = Color.white;
    [SerializeField]
    private float speed = 2.0F;

    private Spear spear;
    private Vector3 direction;

    private bool isFacingLeft = true;
    public Transform groundCheck;
    public LayerMask groundLayers;
    public Rigidbody2D rb;


    private SpriteRenderer sprite;

    protected void Awake()
    {
        spear = Resources.Load<Spear>("Spear");
        sprite = GetComponentInChildren<SpriteRenderer>();
    }

    protected void Start()
    {
        InvokeRepeating("Shoot", rate, rate);
        direction = -transform.right;
    }
    protected void Update()
    {
        Move();
    }

    private void Shoot()
    {
        Vector3 position = transform.position;          position.y += 0.5F;
        Spear newSpear = Instantiate(spear, position, spear.transform.rotation);

        newSpear.Parent = gameObject;
        newSpear.Sprite.flipX = !isFacingLeft;
        newSpear.Direction = -newSpear.transform.right * speed/2;
        newSpear.Color = SpearColor;
    }

    protected override void OnTriggerEnter2D(Collider2D collider)
    {
        Unit unit = collider.GetComponent<Unit>();

        if (unit && unit is Character)
        {
            if (Mathf.Abs(unit.transform.position.x - transform.position.x) < 0.3F) ReceiveDamage();
            else unit.ReceiveDamage();
        }
    }

    private void Move()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position + transform.up * 0.5F + transform.right * direction.x * 0.5F, 0.1F);

        RaycastHit2D groundInfo = Physics2D.Raycast(groundCheck.position, Vector2.down, 1f, groundLayers);

        if (colliders.Length > 0 && colliders.All(x => !x.GetComponent<Character>() && !x.GetComponent<Spear>()) || groundInfo.collider == false)
        {
            isFacingLeft = !isFacingLeft;
            speed = -speed;
            direction *= -1.0F;
            transform.localScale = new Vector2(-transform.localScale.x, 1f);
        }
        rb.velocity = new Vector2(-speed, rb.velocity.y);

    }
}