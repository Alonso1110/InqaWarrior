using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Input Settings")]
    [SerializeField] private Vector2 moveDir;
    [SerializeField] private bool jumpAction;

    [Header("Horizontal Movement")]
    [SerializeField] private float moveSpd = 6f; //Define Max move Speed
    [SerializeField] private float accel = 0.6f;
    [SerializeField] private float decel = 0.4f;
    [SerializeField] private float turnSpd = 0.8f;
    [SerializeField] private Transform LogicBoxesTransform;

    [Header("Vertical Movement")]
    [SerializeField] private float jumpForce = 18f; //Define initial impulse
    [SerializeField] private float jumpSoftVariation = 0.5f; //Define jump strength on air once jump binding is released
    [SerializeField] private float gravity = 3f;
    [SerializeField] private float fallingVariation = 1.5f;

    [Header("Squash & Stretch")]
    [SerializeField] private Transform visualTransform;
    [SerializeField] private Vector2 jumpStretchScale = new Vector2(0.7f, 1.3f); // X delgado, Y alto
    [SerializeField] private Vector2 landSquashScale = new Vector2(1.3f, 0.7f);  // X ancho, Y bajito
    [SerializeField] private float scaleReturnSpeed = 10f;

    private Vector3 baseScale; 
    private float currentFacingX = 1f;

    [Header("Components")]
    private Rigidbody2D rb;
    public bool completelyStop;
    
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        baseScale = visualTransform.localScale;
    }

    public void TurningX(float Xdir)
    {
        if (Xdir == 0) return;
        currentFacingX = Mathf.Sign(Xdir);

        Vector3 currentScale = visualTransform.localScale;
        visualTransform.localScale = new Vector3(Mathf.Abs(currentScale.x) * currentFacingX, currentScale.y, currentScale.z);
        LogicBoxesTransform.localScale = new Vector3(currentFacingX, 1, 1);
    }

    public void MovementX(float Xdir)
    {
        float targetSpd = Xdir * moveSpd;

        float moveRate;

        if (Mathf.Abs(targetSpd) > 0.001f)
        {
            if (Mathf.Sign(Xdir) != Mathf.Sign(rb.velocity.x) && Mathf.Abs(rb.velocity.x) > 0.01f)
            {
                moveRate = turnSpd;
            }
            else
            {
                moveRate = accel;
            }
        }
        else moveRate = decel;

        float actualXspd = Mathf.MoveTowards(rb.velocity.x, targetSpd, moveRate);

        rb.velocity = new Vector2(actualXspd, rb.velocity.y);

        completelyStop = actualXspd == 0;

    }

    public void StopAllMovement()
    {
        rb.velocity = Vector3.zero;
        completelyStop = true;
    }

    public bool stillRaising() => rb.velocity.y > 0.001f;

    public void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        rb.gravityScale = gravity * fallingVariation;

        visualTransform.localScale = new Vector3(jumpStretchScale.x * currentFacingX, jumpStretchScale.y, baseScale.z);
        
    }

    public void CutJump()
    {
        if (rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * jumpSoftVariation);
        }
    }

    public void LandSquash()
    {
        //visualTransform.localScale = new Vector3(landSquashScale.x * currentFacingX, landSquashScale.y, baseScale.z);
    }
    public void ReadjustScale()
    {
        Vector3 targetScale = new Vector3(baseScale.x * currentFacingX, baseScale.y, baseScale.z);
        visualTransform.localScale = Vector3.Lerp(visualTransform.localScale, targetScale, Time.deltaTime * scaleReturnSpeed);
    }
}
