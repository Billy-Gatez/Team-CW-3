using UnityEngine;
using System.Collections;

 public class playerController : MonoBehaviour, IDamage
 {
     [SerializeField] LayerMask ignoreLayer;
     [SerializeField] CharacterController controller;

     [SerializeField] int HP;
     [SerializeField] int speed;
     [SerializeField] int sprintMod;
     [SerializeField] int jumpSpeed;
     [SerializeField] int jumpMax;
     [SerializeField] int gravity;

     [SerializeField] int shootDamage;
     [SerializeField] int shootDist;
     [SerializeField] float shootRate;


    [SerializeField] int crouchSpeed;
    [SerializeField] float crouchHeight;

     int jumpCount;
     int HPOrig;

     float shootTimer;

     Vector3 moveDir;
     Vector3 playerVel;

     bool isSprinting;
     bool isCrouching;

    float originalHeight;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
     {
         HPOrig = HP;
         updatePlayerUI();
         originalHeight = controller.height;
    }

     //  Update is called once per frame
     void Update()
     {
         Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.red);

         movement();

         sprint();

         crouch();

    }
     void movement()
     {
         if(controller.isGrounded)
         {
             jumpCount = 0;
             playerVel = Vector3.zero;
         }

         //moveDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        moveDir = (Input.GetAxis("Horizontal") * transform.right) +
                   (Input.GetAxis("Vertical") * transform.forward);
        //transform.position += moveDir * speed * Time.deltaTime;


       controller.Move(moveDir * speed * Time.deltaTime);

         jump();

         playerVel.y -= gravity * Time.deltaTime;
         controller.Move(playerVel * speed * Time.deltaTime);

         shootTimer += Time.deltaTime;

         if(Input.GetButton("Fire1") && shootTimer >= shootRate)
         {
             shoot();
         }

     }

     void jump()
     {
         if(Input.GetButtonDown("Jump") && jumpCount < jumpMax)
         {
             jumpCount++;
             playerVel.y = jumpSpeed;
         }
     }

     void sprint()
     {
         if(Input.GetButtonDown("Sprint"))
         {
             speed *= sprintMod;
         }
         else if (Input.GetButtonUp("Sprint"))
         {
             speed /= sprintMod;
         }
     }

    void crouch()
    {
        if (Input.GetButtonDown("Crouch"))
        {
            isCrouching = true;
            controller.height = crouchHeight;
            speed = crouchSpeed;
        }
        else if (Input.GetButtonUp("Crouch"))
        {
            isCrouching = false;
            controller.height = originalHeight;
            speed = HPOrig;
        }

    }
        void shoot()
     {
         shootTimer = 0;

         RaycastHit hit;
         if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDist, ~ignoreLayer))
         {
             Debug.Log(hit.collider.name);

             IDamage dmg = hit.collider.GetComponent<IDamage>();

             if (dmg != null)
             {
                 dmg.takeDamage(shootDamage);
             }
         }
     }

     public void takeDamage(int amount)
     {
        HP -= amount;
        updatePlayerUI();
        StartCoroutine(flashDamageScreen());

         if (HP < 0)
         {
              // You lose!!
             gamemanager.instance.youlose();
         }
     }
    public void updatePlayerUI()
    {
        gamemanager.instance.playerHPBar.fillAmount = (float)HP / HPOrig;
    }
    IEnumerator flashDamageScreen()
    {
        gamemanager.instance.playerDamageScreen.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        gamemanager.instance.playerDamageScreen.SetActive(false);
    }
}

