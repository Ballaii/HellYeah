using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 10f;
    public float jumpHeight = 1.5f;
    public float gravity = -9.81f;
    public float airControl = 0.6f;

    [Header("Bunny Hop")]
    public bool enableBhop = true;
    public float bhopJumpBoost = 1.2f;    // Additional height for consecutive jumps
    public float bhopMomentumMultiplier = 1.1f;  // Speed boost when bunny hopping
    public float maxBhopSpeed = 25f;      // Cap for bhop speed
    public float autoJumpThreshold = 0.5f; // Distance to ground for auto-jump to trigger
    public bool showDebugInfo = true;     // Show debug visuals

    [Header("Strafe Jumping")]
    public bool enableStrafeJumping = true;
    public float strafeAcceleration = 1.5f;   // How quickly you accelerate when strafing
    public float maxStrafeAngle = 75f;        // Max angle for effective strafing (degrees)
    public float minStrafeAngle = 15f;        // Min angle needed to gain speed (degrees)
    public float strafeEfficiency = 0.8f;     // How efficient your strafing is (0-1)

    [Header("Slide")]
    public bool enableSlide = true;
    public float slideDuration = 1.0f;    // How long the slide lasts
    public float slideSpeed = 15f;        // Initial slide speed
    public float slideFriction = 5f;      // How quickly the slide slows down
    public float slideYPosOffset = -0.5f; // Lower the camera during slide
    public float slideCooldown = 0.5f;    // Time before you can slide again

    [Header("Look")]
    public float mouseSensitivity = 100f;
    public Transform playerBody;
    public Transform firstPersonCamTransform;

    [Header("Headbob")]
    public float bobSpeed = 10f;
    public float bobAmount = 0.05f;

    // Private variables
    private float bobTimer;
    private Vector3 camStartLocalPos;
    private CharacterController cc;
    private Vector3 velocity;
    private float xRotation = 0f;
    private bool isSprinting;
    private InputSystem_Actions inputActions;

    // Bunny hop variables
    private bool isBhopping = false;
    private float timeSinceLastJump = 0f;
    private float lastJumpTime = -10f;
    private int consecutiveJumps = 0;
    private Vector3 bhopDirection;

    // Strafe jumping variables
    private float currentSpeed = 0f;
    private Vector3 lastFrameVelocity;
    private float lastMouseX = 0f;

    // Slide variables
    private bool isSliding = false;
    private float slideTimer = 0f;
    private float slideCooldownTimer = 0f;
    private Vector3 slideDirection;
    private float originalControllerHeight;
    private Vector3 originalCamLocalPos;

    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        cc = GetComponent<CharacterController>();
        inputActions = new InputSystem_Actions();
        camStartLocalPos = firstPersonCamTransform.localPosition;
        originalControllerHeight = cc.height;
        originalCamLocalPos = firstPersonCamTransform.localPosition;
    }

    void OnEnable() => inputActions.Player.Enable();
    void OnDisable() => inputActions.Player.Disable();

    void OnGUI()
    {
        if (showDebugInfo)
        {
            // Display current speed and jump count
            GUI.Label(new Rect(10, 10, 300, 30), $"Speed: {currentSpeed:F2} units/sec");
            GUI.Label(new Rect(10, 40, 300, 30), $"Consecutive Jumps: {consecutiveJumps}");

            // Strafe guidance
            if (!cc.isGrounded)
            {
                Vector3 velNorm = lastFrameVelocity.normalized;
                Vector2 mov = inputActions.Player.Move.ReadValue<Vector2>();
                Vector3 wishDir = (transform.right * mov.x + transform.forward * mov.y).normalized;

                float angle = Vector3.Angle(velNorm, wishDir);
                string strafeMsg = "Not strafing";

                if (angle < minStrafeAngle)
                    strafeMsg = "Turn more!";
                else if (angle > maxStrafeAngle)
                    strafeMsg = "Turn less!";
                else if (angle >= minStrafeAngle && angle <= maxStrafeAngle)
                {
                    float optimalAngle = (minStrafeAngle + maxStrafeAngle) * 0.5f;
                    float diff = Mathf.Abs(angle - optimalAngle);
                    if (diff < 5f)
                        strafeMsg = "Perfect strafe!";
                    else
                        strafeMsg = "Good strafe";
                }

                GUI.Label(new Rect(10, 70, 300, 30), $"Strafe angle: {angle:F1}° - {strafeMsg}");
            }
        }
    }

    void Update()
    {
        // Store last frame velocity for calculations
        lastFrameVelocity = new Vector3(velocity.x, 0, velocity.z);
        currentSpeed = lastFrameVelocity.magnitude;

        HandleMouseLook();

        // If not sliding, handle normal movement
        if (!isSliding)
        {
            HandleMovement();
            HandleHeadBob();
        }
        else
        {
            HandleSlide();
        }

        // Update timers
        timeSinceLastJump += Time.deltaTime;
        if (slideCooldownTimer > 0)
            slideCooldownTimer -= Time.deltaTime;

        // Debug info
        if (showDebugInfo)
        {
            // Velocity vector (blue)
            Debug.DrawRay(transform.position, new Vector3(velocity.x, 0, velocity.z).normalized * 2f, Color.blue);
            // Forward direction (green)
            Debug.DrawRay(transform.position, transform.forward * 2f, Color.green);
            // Ground check ray
            Debug.DrawRay(transform.position, Vector3.down * 1.1f, Color.yellow);

            // Show current speed on screen
            Debug.Log($"Speed: {currentSpeed:F2} - Jumps: {consecutiveJumps}");
        }
    }

    void HandleMouseLook()
    {
        Vector2 mouseInput = inputActions.Player.Look.ReadValue<Vector2>();
        float mouseX = Mathf.Clamp(mouseInput.x, -10f, 10f) * mouseSensitivity * Time.deltaTime;
        float mouseY = mouseInput.y * mouseSensitivity * Time.deltaTime;

        // Store mouseX for strafe jumping calculations
        lastMouseX = mouseX;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        firstPersonCamTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }

    void HandleMovement()
    {
        isSprinting = inputActions.Player.Sprint.IsPressed();
        float currentSpeed = isSprinting ? sprintSpeed : walkSpeed;

        Vector2 mov = inputActions.Player.Move.ReadValue<Vector2>();
        Vector3 moveDir = transform.right * mov.x + transform.forward * mov.y;

        // Check for slide initiation (sprint + crouch)
        if (enableSlide && isSprinting && inputActions.Player.Crouch.triggered &&
            cc.isGrounded && slideCooldownTimer <= 0 && moveDir.magnitude > 0.1f)
        {
            StartSlide(moveDir.normalized);
            return;
        }

        if (cc.isGrounded)
        {
            // Reset Y velocity when grounded
            if (velocity.y < 0)
                velocity.y = -2f;

            // Normal movement on ground
            velocity.x = moveDir.x * currentSpeed;
            velocity.z = moveDir.z * currentSpeed;

            // Handle jumps and bunny hop
            if (inputActions.Player.Jump.IsPressed())
            {
                // Allow more responsive jumping when holding the button
                // Only jump if we've been grounded for a moment (prevents accidental double jumps)
                if (timeSinceLastJump > 0.1f)
                {
                    Jump();

                    // Store bhop direction when jumping
                    if (moveDir.magnitude > 0.1f)
                    {
                        bhopDirection = moveDir.normalized;
                        isBhopping = true;
                    }
                    else
                    {
                        isBhopping = false;
                    }
                }
            }
            else
            {
                // If on ground and not jumping, reset bhop
                isBhopping = false;
                consecutiveJumps = 0;
            }
        }
        else
        {
            // Air control with strafe jumping
            if (!cc.isGrounded && (isBhopping || Mathf.Abs(mov.x) > 0.1f))
            {
                if (enableStrafeJumping)
                {
                    // Apply source-engine style air strafing
                    ApplyStrafeJumping(mov);
                }
                else if (isBhopping && enableBhop)
                {
                    // For bunny hop without strafing, maintain momentum in bhop direction
                    float bhopSpeed = Mathf.Min(currentSpeed * bhopMomentumMultiplier * (1 + consecutiveJumps * 0.1f), maxBhopSpeed);

                    // Blend between bhop direction and player input for some control
                    Vector3 targetVelocity = bhopDirection * bhopSpeed;
                    velocity.x = Mathf.Lerp(velocity.x, targetVelocity.x, airControl * 0.5f * Time.deltaTime);
                    velocity.z = Mathf.Lerp(velocity.z, targetVelocity.z, airControl * 0.5f * Time.deltaTime);
                }
                else
                {
                    // Standard air control
                    velocity.x = Mathf.Lerp(velocity.x, moveDir.x * currentSpeed, airControl * Time.deltaTime);
                    velocity.z = Mathf.Lerp(velocity.z, moveDir.z * currentSpeed, airControl * Time.deltaTime);
                }

                // Allow consecutive jumps if within the bhop window and jump is held
                if (enableBhop && inputActions.Player.Jump.IsPressed())
                {
                    // Check if near ground and ready for next jump
                    if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1.1f))
                    {
                        // Auto-jump when close enough to ground
                        if (hit.distance < 0.5f && timeSinceLastJump > 0.1f)
                        {
                            Jump();
                        }
                    }
                }
            }
            else
            {
                // Standard air control for non-bhop
                velocity.x = Mathf.Lerp(velocity.x, moveDir.x * currentSpeed, airControl * Time.deltaTime);
                velocity.z = Mathf.Lerp(velocity.z, moveDir.z * currentSpeed, airControl * Time.deltaTime);
            }
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;

        // Apply movement
        Vector3 move = new Vector3(velocity.x, velocity.y, velocity.z);
        cc.Move(move * Time.deltaTime);
    }

    private void Jump()
    {
        float jumpMultiplier = 1f;

        // For bunny hop, increase jump height on consecutive jumps
        if (enableBhop && isBhopping && consecutiveJumps > 0)
        {
            jumpMultiplier = Mathf.Min(bhopJumpBoost * (1 + consecutiveJumps * 0.05f), bhopJumpBoost * 1.5f);
        }

        velocity.y = Mathf.Sqrt(jumpHeight * jumpMultiplier * -2f * gravity);
        lastJumpTime = Time.time;
        timeSinceLastJump = 0f;
        consecutiveJumps++;
    }

    private void StartSlide(Vector3 direction)
    {
        if (!enableSlide) return;

        isSliding = true;
        slideTimer = slideDuration;
        slideDirection = direction;

        // Apply initial slide velocity
        velocity.x = slideDirection.x * slideSpeed;
        velocity.z = slideDirection.z * slideSpeed;

        // Reduce character controller height
        cc.height = originalControllerHeight * 0.5f;
        cc.center = new Vector3(0, -originalControllerHeight * 0.25f, 0);

        // Lower camera position
        firstPersonCamTransform.localPosition = new Vector3(
            originalCamLocalPos.x,
            originalCamLocalPos.y + slideYPosOffset,
            originalCamLocalPos.z
        );
    }

    private void EndSlide()
    {
        isSliding = false;
        slideCooldownTimer = slideCooldown;

        // Restore character controller height
        cc.height = originalControllerHeight;
        cc.center = Vector3.zero;

        // Reset camera position gradually (will be handled by head bob)
        firstPersonCamTransform.localPosition = originalCamLocalPos;
    }

    private void HandleSlide()
    {
        if (slideTimer > 0)
        {
            slideTimer -= Time.deltaTime;

            // Apply friction to slow down slide
            float frictionFactor = slideTimer / slideDuration; // Gradually increase friction
            velocity.x = Mathf.Lerp(velocity.x, 0, slideFriction * Time.deltaTime * (1 - frictionFactor));
            velocity.z = Mathf.Lerp(velocity.z, 0, slideFriction * Time.deltaTime * (1 - frictionFactor));

            // Apply gravity
            velocity.y += gravity * Time.deltaTime;

            // Move the character
            cc.Move(velocity * Time.deltaTime);

            // End slide early if player presses jump
            if (inputActions.Player.Jump.triggered)
            {
                EndSlide();
                Jump();
            }
        }
        else
        {
            EndSlide();
        }
    }

    void HandleHeadBob()
    {
        if (isSliding || !cc.isGrounded || cc.velocity.magnitude < 0.1f)
        {
            ResetHeadBob();
            return;
        }

        bobTimer += Time.deltaTime * bobSpeed;
        float bobX = 0f;
        float bobY = Mathf.Sin(bobTimer) * bobAmount;

        // Increase bob amount when sprinting
        if (isSprinting) bobY *= 1.5f;

        firstPersonCamTransform.localPosition = camStartLocalPos + new Vector3(bobX, bobY, 0f);
    }

    void ResetHeadBob()
    {
        bobTimer = 0f;
        if (!isSliding) // Don't reset camera position during slide
        {
            firstPersonCamTransform.localPosition = Vector3.Lerp(
                firstPersonCamTransform.localPosition,
                camStartLocalPos,
                Time.deltaTime * 5f
            );
        }
    }

    private void ApplyStrafeJumping(Vector2 moveInput)
    {
        if (moveInput.magnitude < 0.1f) return;

        Vector3 currentVelNorm = lastFrameVelocity.normalized;
        float speed = lastFrameVelocity.magnitude;

        Vector3 wishDir = (transform.right * moveInput.x + transform.forward * moveInput.y).normalized;

        float angle = Vector3.Angle(currentVelNorm, wishDir);
        float strafeEffectiveness = 0f;

        if (angle >= minStrafeAngle && angle <= maxStrafeAngle)
        {
            float optimalAngle = (minStrafeAngle + maxStrafeAngle) * 0.5f;
            float angleDelta = Mathf.Clamp((angle - minStrafeAngle) / (maxStrafeAngle - minStrafeAngle), 0, 1);
            float angleFactor = Mathf.SmoothStep(0, 1, 1 - Mathf.Abs(angleDelta - 0.5f) * 2);
            strafeEffectiveness = angleFactor * strafeEfficiency;

            // Mouse movement helps but no strict sign check
            if (Mathf.Abs(lastMouseX) > 0.01f && Mathf.Abs(moveInput.x) > 0.1f)
            {
                float mouseSync = Mathf.Clamp01(Mathf.Abs(lastMouseX) * 0.2f); // Smooth contribution
                strafeEffectiveness *= 1f + mouseSync * 0.5f;
            }
        }

        if (strafeEffectiveness > 0f)
        {
            float acceleration = strafeAcceleration * strafeEffectiveness * Time.deltaTime * 60f;
            Vector3 newVelocity = Vector3.Lerp(
                lastFrameVelocity,
                lastFrameVelocity + wishDir * acceleration,
                strafeEffectiveness * 0.8f
            );

            newVelocity = newVelocity.normalized * Mathf.Max(newVelocity.magnitude, speed);

            float velocityDot = Vector3.Dot(newVelocity.normalized, lastFrameVelocity.normalized);
            if (velocityDot < 0.7f)
            {
                newVelocity = Vector3.Lerp(lastFrameVelocity, newVelocity, 0.5f);
            }

            if (newVelocity.magnitude > maxBhopSpeed)
            {
                newVelocity = newVelocity.normalized * maxBhopSpeed;
            }

            velocity.x = newVelocity.x;
            velocity.z = newVelocity.z;

            if (showDebugInfo)
            {
                Debug.DrawRay(transform.position, wishDir * 2f,
                    Color.Lerp(Color.yellow, Color.red, strafeEffectiveness), 0.1f);
            }
        }
    }
}