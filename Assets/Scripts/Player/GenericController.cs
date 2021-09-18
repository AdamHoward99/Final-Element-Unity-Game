using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GenericController : MonoBehaviour
{
    //Player Variables
    private int CurrentJumps { get; set; }
    private int Stock { get; set; }
    public int playerType;
    private int PlayerType{ get { return playerType; }  }
    private const int TotalJumps = 1;
    private const int MaxStock = 5;

    private float Speed { get; set; }
    private float JumpForce { get; set; }
    private float MovementForce { get; set; }
    private float Damage { get; set; }
    private float Strength { get; set; }

    private Rigidbody PlayerRigidbody { get; set; }
    private string InputAxisName { get; set; }
    private bool Grounded { get; set; }
    private Vector3 StartingSpawnPosition { get; set; }
    private Animator Anim { get; set; }
    private Dictionary<string, KeyCode> inputCommands;

    public AudioSource[] Audio;

    //Projectile Variables
    public Transform projectileSpawnPoint;     //Assigned in scene editor
    public Rigidbody projectileRb; 

    private const float attackDelay = 1.0f;
    private const float projectileSpeed = 5.0f;

    private float AttackCooldown { get; set; }

    //Text Variables
    public Text damageTxt;
    public Text stockText;

    private Color PreviousTextColour = new Color(0, 0, 0, 0);

    private void Start()
    {
        Speed = 1.5f;
        JumpForce = 5.0f;
        Strength = 8.0f;
        Stock = 3;
        Damage = 0.0f;
        CurrentJumps = TotalJumps;
        stockText.text = string.Concat("Stock: ", Stock.ToString());
        damageTxt.text = string.Concat("Stun: ", Damage.ToString());

        Anim = GetComponent<Animator>();
        PlayerRigidbody = GetComponent<Rigidbody>();
        StartingSpawnPosition = transform.position;

        GetInputCommands();
    }

    private void InputAction()
    {
        if (Input.GetKeyDown(inputCommands["Jump"]) && CurrentJumps > 0)
           Jump();

        //Projectile 
        if (Input.GetKey(inputCommands["Projectile"]) && Grounded && CheckAttackCooldown() && Time.timeScale > 0f)
            Fire();

        //Attack 
        if (Input.GetKey(inputCommands["Attack"]))
        {
            Anim.SetBool("Attacking", true);
            StartCoroutine(AttackAnimationDelay("Attacking"));
        }
    }

    private void Update()
    {
        if (Input.anyKey)
            InputAction();

        Move();

        if (MovementForce < 0) ChangeRotation(-90.0f);
        else if (MovementForce > 0) ChangeRotation(90.0f);

        if (Grounded)
        {
            ResetJumps();
            
            //Moving animation
            if (MovementForce != 0)
                Anim.SetBool("Running", true);
            else
                Anim.SetBool("Running", false);
        }

        Color c = SetDamageTextColour();
        if (PreviousTextColour != c)
        {
            damageTxt.color = c;       
            PreviousTextColour = c;     //Stores previous value so unless its changed, its ignored
        }
    }

    private void Move()
    {
        MovementForce = Input.GetAxis(InputAxisName);
        PlayerRigidbody.velocity = new Vector2(MovementForce * Speed, PlayerRigidbody.velocity.y);
    }

    private void ResetJumps()
    {
        CurrentJumps = TotalJumps;
        Anim.SetBool("Jumping", false);
    }

    private void Jump()
    {
        Anim.SetBool("Jumping", true);
        PlayerRigidbody.velocity = Vector2.up * JumpForce;
        CurrentJumps--;
    }

    private Color SetDamageTextColour()
    {
        if(Damage <= 50)
            return new Color(0, 1, 0, 1);

        else if(Damage <= 100)
            return new Color(1, 1, 0, 1);

        else
        return new Color(1, 0, 0, 1);
    }

    private void ChangeRotation(float dir) => transform.rotation = Quaternion.Euler(transform.rotation.x, dir, transform.rotation.z);

    private bool CheckAttackCooldown() => Time.time > AttackCooldown;

    private void Fire()
    {
        Rigidbody projectile = Instantiate(projectileRb, projectileSpawnPoint.position, Quaternion.identity);

        Vector2 movement = transform.rotation.y < 0 ? new Vector2(-1, 0) : new Vector2(1, 0);
        Audio[2].Play();        //Projectile Sfx

        projectile.GetComponent<Rigidbody>().AddForce(movement * projectileSpeed);
        AttackCooldown = Time.time + attackDelay;
        Anim.SetBool("Attacking", true);
        StartCoroutine(AttackAnimationDelay("Attacking"));
    }

    private void UseItem(string itemName)
    {
        if (itemName == "HealthItem")    //Heart Container (Decreases Damage)
        {
            Damage -= 20;
            if (Damage < 0) Damage = 0;
            damageTxt.text = string.Concat("Stun: ", Damage.ToString());
        }

        else if (itemName == "StrengthItem") //Weight (Increases strength)
        {
            Strength *= 1.5f;
            if (Strength > 30.0f) Strength = 30.0f;
        }

        else if (itemName == "StockItem") //Potion bottle (Extra life/was invincible)
        {
            Stock++;
            if (Stock > 5) Stock = 5;
            stockText.text = string.Concat("Stock: ", Stock.ToString());
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        //Colliding with items
        if (collision.gameObject.tag == "Item")
        {
            Audio[3].Play();
            UseItem(collision.gameObject.name);
            Destroy(collision.transform.parent.gameObject);
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
            Grounded = true;

        //Losing a life when going off the stage
        if(collision.gameObject.tag == "End Zone")
        {
            Audio[1].Play(); //Death sfx
            Stock--;
            stockText.text = string.Concat("Stock: ", Stock.ToString());
            if (Stock > 0)
            {
                this.transform.position = StartingSpawnPosition;
                this.Damage = 0.0f;
                damageTxt.text = string.Concat("Stun: ", Damage.ToString());
            }
            else
            {
                SceneManager.LoadScene("WinMenu");
                StateChange.CurrentLevel = SceneManager.GetActiveScene().name;
                StateChange.VictoriousPlayer = PlayerType == 1 ? 2 : 1;
            }
        }

        //Collision with projectile
        if(collision.gameObject.tag == "Projectile")
        {
            Damage += Strength;
            damageTxt.text = string.Concat("Stun: ", Damage.ToString());
            bool positionCheck = collision.transform.position.x > transform.position.x;     //True if projectile is to the right of this player
            PushBack(positionCheck ? -0.2f : 0.2f);
            Destroy(collision.gameObject);
        }
    }

    private void PushBack(float num) => transform.Translate(num - (Damage / 100.0f), 0.45f + (Damage / 100.0f), 0.0f, Space.World);     //Collision with other player projectile

    private void PushBack(float num, Collision col) => col.transform.Translate(num, 0.45f, 0.0f, Space.World);      //Collision when attacking other player

    private void OnCollisionStay(Collision collision)
    {
        //Attacking when the player is near
        if(collision.gameObject.tag == "Player")
        {
            if(Input.GetKey(inputCommands["Attack"]) && CheckAttackCooldown())
            {
                GenericController scr = collision.gameObject.GetComponent<GenericController>(); 
                scr.Damage += Strength;
                scr.damageTxt.text = string.Concat("Stun: ", scr.Damage.ToString());
                Audio[0].Play();        //HitSfx
                bool positionCheck = collision.transform.position.x < transform.position.x;     //True if projectile is to the left of this player
                PushBack(positionCheck ? -0.8f : 0.8f, collision);

                //push back of other player
                AttackCooldown = Time.time + attackDelay;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
            Grounded = false;
    }

    private IEnumerator AttackAnimationDelay(string boolName)
    {
        yield return new WaitForSeconds(0.5f);
        Anim.SetBool(boolName, false);
    }

    private void GetInputCommands()
    {
        if (Input.GetJoystickNames().Length >= PlayerType)  
        {
            InputAxisName = $"Player {playerType} Horizontal (Controller)";
            if (PlayerType == 1) inputCommands = new Dictionary<string, KeyCode> { { "Jump", KeyCode.Joystick1Button1 }, { "Projectile", KeyCode.Joystick1Button2 }, { "Attack", KeyCode.Joystick1Button0 } };
            else if (PlayerType == 2) inputCommands = new Dictionary<string, KeyCode> { { "Jump", KeyCode.Joystick2Button1 }, { "Projectile", KeyCode.Joystick2Button2 }, { "Attack", KeyCode.Joystick2Button0 } };
        }
        else
        {
            InputAxisName = $"Player {PlayerType} Horizontal";
            if (PlayerType == 1) inputCommands = new Dictionary<string, KeyCode> { { "Jump", KeyCode.W }, { "Projectile", KeyCode.Space }, { "Attack", KeyCode.M } };
            else if (PlayerType == 2) inputCommands = new Dictionary<string, KeyCode> { { "Jump", KeyCode.UpArrow }, { "Projectile", KeyCode.KeypadEnter }, { "Attack", KeyCode.KeypadPlus } };
        }
    }
}
