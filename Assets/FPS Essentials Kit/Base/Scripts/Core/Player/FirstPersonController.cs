/*
 * Copyright (c) 2017 The Asset Lab. All rights reserved.
 * https://www.theassetlab.com/
*/

using System;
using Essentials.Input;
using UnityEngine;
using System.Collections;

namespace Essentials
{
    namespace Controllers
    {
        public enum MotionState
        {
            Idle,
            Walking,
            Running,
            Crouched,
            Flying
        }

        [AddComponentMenu("FPS Essentials/Controllers/First Person Controller")]
        [RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(CapsuleCollider)), DisallowMultipleComponent]
        public sealed class FirstPersonController : MonoBehaviour
        {
            #region MOVEMENT

            [SerializeField]
            [Range(0.1f, 10)]
            private float m_WalkingSpeed = 3.5f;

            [SerializeField]
            [Range(0.1f, 5)]
            private float m_CrouchSpeed = 2f;

            [SerializeField]
            [MinMax(0, Mathf.Infinity)]
            private float m_RunMultiplier = 2.5f; // Run speed = WalkingSpeed * RunMultiplier

            [SerializeField]
            [Range(0, 1)]
            private float m_AirControlPercent = 0.5f;

            [SerializeField]
            [MinMax(0, Mathf.Infinity)]
            private float m_JumpForce = 10f;

            [SerializeField]
            [Range(0.05f, 0.5f)]
            private float m_StepOffset = 0.2f; // Maximum height of each step

            [SerializeField]
            [Range(1, 90)]
            private float m_SlopeLimit = 60; // Maximum slope

            private float m_GroundRelativeAngle;

            #endregion

            #region MOUSE LOOK

            [SerializeField]
            private MouseLook m_MouseLook;

            [SerializeField]
            [NotNull]
            private Camera m_MainCamera;

            #endregion

            #region STAMINA

            [SerializeField]
            private bool m_Stamina = true;

            [SerializeField]
            [MinMax(0, Mathf.Infinity)]
            private float m_MaxStaminaAmount = 100;

            [SerializeField]
            [MinMax(0, Mathf.Infinity)]
            private float m_DecrementRatio = 7.5f;

            [SerializeField]
            private AudioClip m_BreathSound;

            private float m_StaminaAmount;

            #endregion

            #region FALL

            [SerializeField]
            [MinMax(0, Mathf.Infinity)]
            private float m_HeightThreshold = 5.0f; // The max height that player can fall without hurting yourself


            [SerializeField]
            [MinMax(0, Mathf.Infinity)]
            private float m_DamageMultiplier = 4.0f; // The damage multiplier increases the damage of the fall

            private Vector3 m_FallingStartPos;
            private float m_FallDistance;

            #endregion

            #region FOOTSTEPS

            [SerializeField]
            private bool m_Footsteps = true;

            [SerializeField]
            [Range(0, 1)]
            private float m_WalkingVolume = 0.2f;

            [SerializeField]
            [Range(0, 1)]
            private float m_CrouchVolume = 0.1f;

            [SerializeField]
            [Range(0, 1)]
            private float m_RunningVolume = 0.4f;

            [SerializeField]
            private AudioClip m_JumpSound; // The sound played when character leaves the ground.

            [SerializeField]
            [Range(0, 1)]
            private float m_JumpLandingVolume = 0.2f;

            [SerializeField]
            private AudioClip m_CrouchDownSound;

            [SerializeField]
            private AudioClip m_CrouchUpSound;

            private float m_NextStep;

            #endregion

            #region VAULT

            [SerializeField]
            private bool m_Vault;

            [SerializeField]
            [MinMax(0, Mathf.Infinity)]
            private float m_InteractionRange = 1;

            [SerializeField]
            private AnimationCurve m_VaultAnimationCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));

            [SerializeField]
            [MinMax(0, Mathf.Infinity)]
            private float m_VaultDuration = 0.5f;

            private bool m_CanVault;
            private Vector3 m_ObstaclePosition;

            #endregion

            private int m_Weight = 0; // Current weight the player is carrying [0,5]

            private Rigidbody m_RigidBody;
            private CapsuleCollider m_Capsule;
            private Vector3 m_GroundContactNormal;
            private Vector3 m_GroundContactPoint;

            private MotionState m_State = MotionState.Idle;
            private SurfaceIdentifier m_Surface;

            private bool m_Jump;
            private bool m_PreviouslyGrounded;
            private bool m_PreviouslyJumping;
            private bool m_Jumping;
            private bool m_Sliding;
            private bool m_Running;
            private PlayerAudioSource m_PlayerBreathSource;
            private PlayerAudioSource m_PlayerFootstepsSource;

            //Events
            public event Action<float> LandingEvent;
            public event Action JumpEvent;
            public event Action VaultEvent;

            #region CONTROLLER PROPERTIES

            public bool Grounded { get; private set; }
            public bool Controllable { get; set; }

            public bool ReceiveMouseInput
            {
                get { return m_MouseLook.Controllable; }
                set { m_MouseLook.Controllable = value; }
            }

            public int Weight
            {
                get { return m_Weight; }
                set { m_Weight = Mathf.Clamp(value, 0, 5); }
            }

            public MotionState State { get { return m_State; } }
            public Vector3 Velocity { get { return m_RigidBody.velocity; } }
            public float StaminaPercent { get { return m_StaminaAmount / m_MaxStaminaAmount; } }

            public bool IsAiming { get; set; }
            public bool LowerBodyDamaged { get; set; }

            public bool IsCrouched { get; private set; }
            public bool CanVault { set; get; }

            public float CurrentTargetSpeed
            {
                get
                {
                    if (IsCrouched)
                    {
                        return m_CrouchSpeed;
                    }

                    if (m_Running)
                    {
                        return WalkingSpeed * (m_Stamina ? 1 + (m_StaminaAmount * (m_RunMultiplier - 1)) / m_MaxStaminaAmount : m_RunMultiplier);
                    }

                    return WalkingSpeed;
                }
            }

            private float WalkingSpeed
            {
                get
                {
                    if (LowerBodyDamaged)
                    {
                        return m_WalkingSpeed * 0.7f;
                    }

                    if (!m_Stamina)
                        return m_WalkingSpeed;

                    switch (m_Weight)
                    {
                        case 0:
                            return m_WalkingSpeed;
                        case 1:
                            return m_WalkingSpeed * 0.9f;
                        case 2:
                            return m_WalkingSpeed * 0.85f;
                        case 3:
                            return m_WalkingSpeed * 0.8f;
                        case 4:
                            return m_WalkingSpeed * 0.75f;
                        case 5:
                            return m_WalkingSpeed * 0.7f;
                        default:
                            return m_WalkingSpeed;
                    }
                }
            }

            #endregion

            private void Start ()
            {
                m_RigidBody = GetComponent<Rigidbody>();
                m_RigidBody.mass = 10;
                m_RigidBody.collisionDetectionMode = CollisionDetectionMode.Continuous;
                m_RigidBody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

                m_Capsule = GetComponent<CapsuleCollider>();
                m_MouseLook.Init(transform, m_MainCamera.transform);

                m_StaminaAmount = m_MaxStaminaAmount;

                JumpEvent += PlayJumpSound;
                LandingEvent += PlayLandingSound;

                ReceiveMouseInput = true;
                Controllable = true;

                // Instead of invoking these methods once per frame, it's more efficient to update them at a lower frequency
                InvokeRepeating("UpdateState", 0, 0.05f);
                InvokeRepeating("CheckGroundStatus", 0, 0.05f);
                InvokeRepeating("CheckObstaclesAhead", 0, 0.1f);
                InvokeRepeating("CheckGroundAngle", 0, 0.2f);

                m_PlayerBreathSource = AudioManager.Instance.RegisterSource("PlayerBreathSource", AudioManager.Instance.transform);
                m_PlayerFootstepsSource = AudioManager.Instance.RegisterSource("PlayerFootstepsSource", AudioManager.Instance.transform);
            }

            private void Update ()
            {
                m_MouseLook.UpdateRotation(transform, m_MainCamera.transform, IsAiming);

                if (Grounded)
                {
                    if (Controllable)
                        HandleInput();

                    if (m_Footsteps && m_State != MotionState.Idle && m_State != MotionState.Flying)
                        FootStepCycle(); // Plays footsteps

                    // Update stamia amount
                    if (m_Stamina)
                    {
                        m_StaminaAmount = Mathf.MoveTowards(m_StaminaAmount, m_Running && Velocity.sqrMagnitude > CurrentTargetSpeed * CurrentTargetSpeed * 0.1f
                            ? 0 : m_MaxStaminaAmount, Time.deltaTime * m_DecrementRatio);

                        m_PlayerBreathSource.Play(m_BreathSound, 0);
                        m_PlayerBreathSource.CalculateVolumeByPercent(m_MaxStaminaAmount * 0.4f, m_StaminaAmount, 0.5f);
                    }

                    ScaleCapsuleForCrouching();
                }
                else
                {
                    if (m_PreviouslyGrounded)
                    {
                        // Set falling start position
                        m_FallingStartPos = transform.position;
                        m_PreviouslyGrounded = false;
                    }
                }

                if (Grounded && !m_PreviouslyGrounded)
                {
                    // Calculates the vertical fall distance
                    m_FallDistance = m_FallingStartPos.y - transform.position.y;

                    if (m_FallDistance > m_HeightThreshold && !m_Sliding)
                    {
                        LandingEvent.Invoke(Mathf.Round(m_DamageMultiplier * -Physics.gravity.y * (m_FallDistance - m_HeightThreshold)));
                    }
                    else if (m_FallDistance >= m_StepOffset + 1 || m_PreviouslyJumping)
                    {
                        LandingEvent.Invoke(0);
                    }

                    m_FallDistance = 0;
                    m_PreviouslyGrounded = true;
                    m_Sliding = false;
                }
            }

            private void HandleInput ()
            {
                CheckRunning();

                if (InputManager.GetButtonDown("Jump") && !m_Jump && !IsCrouched && !LowerBodyDamaged && !PreventStandingInLowHeadroom())
                {
                    // Check if there is any obstacle ahead and try to vault, otherwise just jump upwards
                    if (m_Vault && CanVault && m_CanVault && GetInput().y > Mathf.Epsilon && IsValidObstacle())
                    {
                        StartCoroutine(Vault(transform.forward * (m_ObstaclePosition.z + m_Capsule.radius * 2) +
                            transform.up * (m_ObstaclePosition.y + (m_Capsule.height / 2 + 0.05f) - transform.position.y)));

                        if (VaultEvent != null)
                            VaultEvent.Invoke();
                    }
                    else
                    {
                        m_Jump = true;
                    }
                }

                if (GameplayManager.Instance.CrouchStyle == GameplayManager.ActionMode.Press)
                {
                    if (InputManager.GetButtonDown("Crouch") && !IsCrouched && !m_Running)
                    {
                        IsCrouched = true;
                    }
                    else
                    {
                        IsCrouched &= !m_Running && !InputManager.GetButtonDown("Crouch") || PreventStandingInLowHeadroom();
                    }
                }
                else
                {
                    if (IsCrouched && PreventStandingInLowHeadroom() && !m_Running)
                    {
                        IsCrouched = true;
                    }
                    else
                    {
                        IsCrouched = InputManager.GetButton("Crouch");
                    }
                }
            }

            private void CheckObstaclesAhead ()
            {
                if (!CanVault || !m_Vault)
                {
                    m_CanVault = false;
                    m_ObstaclePosition = Vector3.zero;
                    return;
                }

                RaycastHit hitInfo;
                if (Physics.SphereCast(transform.position + Vector3.up * (m_Capsule.height / 20), m_Capsule.radius / 2, transform.TransformDirection(Vector3.forward), 
                    out hitInfo, m_InteractionRange / 2, Physics.AllLayers, QueryTriggerInteraction.Ignore))
                {
                    if (hitInfo.collider.GetComponent<Rigidbody>())
                        return;

                    // Check surface angle and size
                    float angle = Vector3.Angle(transform.up, hitInfo.normal);
                    m_CanVault = hitInfo.collider != null && angle > 85 && angle < 100 && (hitInfo.collider.bounds.max.y - hitInfo.point.y) <= m_Capsule.height / 4 + 0.05f;

                    if (m_CanVault)
                    {
                        m_ObstaclePosition = new Vector3(0, hitInfo.collider.bounds.max.y, hitInfo.distance);
                    }
                }
                else
                {
                    m_CanVault = false;
                }
            }

            private bool IsValidObstacle ()
            {
                RaycastHit hitInfo;
                Ray ray = new Ray(transform.position + Vector3.up, transform.TransformDirection(Vector3.forward));

                bool isBlocked = Physics.Raycast(ray, m_InteractionRange, Physics.AllLayers, QueryTriggerInteraction.Ignore);
                return !isBlocked && !Physics.SphereCast(ray, m_Capsule.radius, out hitInfo, m_InteractionRange, Physics.AllLayers, QueryTriggerInteraction.Ignore);
            }

            private IEnumerator Vault (Vector3 targetPosition)
            {
                Vector3 initialPos = transform.position;
                Vector3 destination = transform.position + targetPosition;

                Controllable = false;

                // Make the character move to target position
                for (float t = 0f; t <= m_VaultDuration; t += Time.deltaTime)
                {
                    transform.position = Vector3.Lerp(initialPos, destination, t / m_VaultDuration * m_VaultAnimationCurve.Evaluate(t / m_VaultDuration));
                    yield return new WaitForFixedUpdate();
                }

                Controllable = true;
            }

            private bool PreventStandingInLowHeadroom ()
            {
                RaycastHit hitInfo;
                Ray ray = new Ray(transform.position, transform.TransformDirection(Vector3.up));

                return Physics.SphereCast(ray, m_Capsule.radius * 0.9f, out hitInfo, 2, Physics.AllLayers, QueryTriggerInteraction.Ignore);
            }

            private void FixedUpdate ()
            {
                Vector2 input = GetInput();

                if ((Mathf.Abs(input.x) > float.Epsilon || Mathf.Abs(input.y) > float.Epsilon))
                {
                    // Calculates movement direction
                    Vector3 desiredMove = transform.forward * input.y + transform.right * input.x;
                    desiredMove = (desiredMove.sqrMagnitude > 1) ? Vector3.ProjectOnPlane(desiredMove, m_GroundContactNormal).normalized : Vector3.ProjectOnPlane(desiredMove, m_GroundContactNormal);

                    desiredMove.x = desiredMove.x * (Grounded ? CurrentTargetSpeed : CurrentTargetSpeed * m_AirControlPercent);
                    desiredMove.z = desiredMove.z * (Grounded ? CurrentTargetSpeed : CurrentTargetSpeed * m_AirControlPercent);
                    desiredMove.y = desiredMove.y * (Grounded ? CurrentTargetSpeed : CurrentTargetSpeed * m_AirControlPercent);

                    if (m_RigidBody.velocity.sqrMagnitude < (CurrentTargetSpeed * CurrentTargetSpeed))
                    {
                        m_RigidBody.AddForce(desiredMove, ForceMode.Impulse);
                    }
                }

                if (Grounded)
                {
                    m_RigidBody.drag = 5f;

                    if (m_Jump)
                    {
                        m_RigidBody.drag = 0f;
                        m_RigidBody.velocity = new Vector3(m_RigidBody.velocity.x, 0f, m_RigidBody.velocity.z);
                        m_RigidBody.AddForce(new Vector3(0f, m_JumpForce * 10, 0f), ForceMode.Impulse);
                        m_Jumping = true;
                        JumpEvent.Invoke();
                    }

                    if (!m_Jumping && Mathf.Abs(input.x) < float.Epsilon && Mathf.Abs(input.y) < float.Epsilon && m_RigidBody.velocity.magnitude < 1f)
                    {
                        m_RigidBody.Sleep();
                    }
                }
                else
                {
                    m_RigidBody.drag = 0f;

                    if (m_RigidBody.velocity.magnitude < Mathf.Abs(Physics.gravity.y * m_RigidBody.mass / 2))
                    {
                        m_RigidBody.AddForce(Physics.gravity, ForceMode.Impulse);
                    }

                    if (m_PreviouslyGrounded && !m_Jumping)
                    {
                        StickToGroundHelper();
                    }
                }
                m_Jump = false;
            }

            private void ScaleCapsuleForCrouching ()
            {
                if (IsCrouched)
                {
                    if (Mathf.Abs(m_Capsule.height - 1.2f) > Mathf.Epsilon)
                    {
                        m_PlayerFootstepsSource.ForcePlay(m_CrouchDownSound, 0.15f);
                        m_NextStep = m_CrouchDownSound.length + Time.time;
                    }

                    m_Capsule.height = 1.2f;
                    m_Capsule.center = new Vector3(0, -0.4f, 0);

                    m_MainCamera.transform.localPosition = Vector3.MoveTowards(m_MainCamera.transform.localPosition, new Vector3(0, 0.05f, 0), Time.deltaTime * 5);
                }
                else
                {
                    if (Mathf.Abs(m_Capsule.height - 2) > Mathf.Epsilon)
                    {
                        m_PlayerFootstepsSource.ForcePlay(m_CrouchUpSound, 0.15f);
                        m_NextStep = m_CrouchUpSound.length + Time.time;
                    }

                    m_Capsule.height = 2f;
                    m_Capsule.center = Vector3.zero;

                    m_MainCamera.transform.localPosition = Vector3.MoveTowards(m_MainCamera.transform.localPosition, new Vector3(0, 0.8f, 0), Time.deltaTime * 5);
                }
            }

            private void UpdateState ()
            {
                bool idle = GetInput() == Vector2.zero || Velocity.sqrMagnitude < CurrentTargetSpeed * CurrentTargetSpeed * 0.1f;
                bool running = m_Running;

                if (Grounded)
                {
                    if (!running && !IsCrouched && !idle)
                    {
                        m_State = MotionState.Walking;
                        return;
                    }
                    if (running && !IsCrouched && !idle)
                    {
                        m_State = MotionState.Running;
                        return;
                    }
                    if (IsCrouched && !idle)
                    {
                        m_State = MotionState.Crouched;
                        return;
                    }
                    m_State = MotionState.Idle;
                    return;
                }
                if (m_FallingStartPos.y - transform.position.y >= m_StepOffset + m_Capsule.height)
                    m_State = MotionState.Flying;
            }

            private void StickToGroundHelper ()
            {
                RaycastHit hitInfo;
                if (Physics.SphereCast(transform.position, m_Capsule.radius * 0.9f, Vector3.down, out hitInfo, (1 - m_Capsule.radius) + 0.1f, Physics.AllLayers, QueryTriggerInteraction.Ignore))
                {
                    if (Mathf.Abs(Vector3.Angle(hitInfo.normal, Vector3.up)) > m_SlopeLimit)
                    {
                        m_RigidBody.velocity = Vector3.ProjectOnPlane(m_RigidBody.velocity, hitInfo.normal);
                    }
                }
            }

            public void CheckGroundAngle ()
            {
                if (Grounded)
                {
                    RaycastHit hitInfo;
                    if (Physics.Raycast(transform.position, Vector3.down, out hitInfo, m_Capsule.height + 0.1f, Physics.AllLayers, QueryTriggerInteraction.Ignore))
                    {
                        m_GroundRelativeAngle = Mathf.Abs(Vector3.Angle(hitInfo.normal, Vector3.up));
                    }
                }
            }

            public Vector2 GetInput ()
            {
                if (!Controllable)
                    return Vector2.zero;

                Vector2 input = new Vector2
                {
                    x = InputManager.GetAxis("Horizontal"),
                    y = InputManager.GetAxis("Vertical")
                };

                return input;
            }

            private void CheckRunning ()
            {
                if (!LowerBodyDamaged && GetInput().y > 0 && !IsAiming)
                {
                    if (GameplayManager.Instance.SprintStyle == GameplayManager.ActionMode.Hold)
                    {
                        m_Running = InputManager.GetButton("Run");
                    }
                    else
                    {
                        if (InputManager.GetButtonDown("Run"))
                        {
                            m_Running = !m_Running;
                        }
                    }
                }
                else
                {
                    m_Running = false;
                }
            }

            private void FootStepCycle ()
            {
                if (m_Surface != null)
                {
                    switch (m_State)
                    {
                        case MotionState.Walking:
                            OnFootStrike(m_WalkingVolume, (11 - CurrentTargetSpeed) * 0.07f);
                            break;
                        case MotionState.Running:
                            OnFootStrike(m_RunningVolume, 0.15f + m_WalkingSpeed / CurrentTargetSpeed * 0.25f);
                            break;
                        case MotionState.Crouched:
                            OnFootStrike(m_CrouchVolume, (6 - m_CrouchSpeed) * 0.2f);
                            break;
                        default:
                            return;
                    }
                }
            }

            private void PlayJumpSound ()
            {
                if (!m_Footsteps)
                    return;

                if (m_JumpSound != null)
                {
                    m_PlayerFootstepsSource.ForcePlay(m_JumpSound, m_JumpLandingVolume);
                    m_NextStep = m_JumpSound.length + Time.time;
                }
            }

            private void PlayLandingSound (float fallDamage)
            {
                if (!m_Footsteps || m_Surface == null)
                    return;

                AudioClip land = m_Surface.GetSurfaceData(m_GroundContactPoint).GetRandomLandingSound();

                if (land != null)
                {
                    m_PlayerFootstepsSource.ForcePlay(land, m_JumpLandingVolume);
                    m_NextStep = land.length + Time.time;
                }
            }

            private void OnFootStrike (float volume, float stepLength)
            {
                if (m_NextStep < Time.time)
                {
                    m_NextStep = stepLength + Time.time;

                    AudioClip footStep = m_State == MotionState.Running ? m_Surface.GetSurfaceData(m_GroundContactPoint).GetRandomSprintingFootsteps()
                        : m_Surface.GetSurfaceData(m_GroundContactPoint).GetRandomFootsteps();

                    m_PlayerFootstepsSource.ForcePlay(footStep, volume);
                }
            }

            private void CheckGroundStatus ()
            {
                m_PreviouslyGrounded = Grounded;
                m_PreviouslyJumping = m_Jumping;
                m_Sliding = m_GroundRelativeAngle > m_SlopeLimit;
                float offset = (1 - m_Capsule.radius) + (m_Sliding ? 0.05f : m_StepOffset);

                RaycastHit hitInfo;
                if (Physics.SphereCast(transform.position, m_Capsule.radius * 0.9f, Vector3.down, out hitInfo, offset, Physics.AllLayers, QueryTriggerInteraction.Ignore))
                {
                    Grounded = true;
                    m_GroundContactNormal = hitInfo.normal;
                    m_GroundContactPoint = hitInfo.point;

                    m_Surface = hitInfo.collider.GetSurface();
                }
                else
                {
                    Grounded = false;
                    m_GroundContactNormal = Vector3.up;
                }
                if (!m_PreviouslyGrounded && Grounded && m_Jumping)
                {
                    m_Jumping = false;
                }
            }

            public IEnumerator AdrenalineShot (float duration)
            {
                if (m_Stamina)
                {
                    m_Stamina = false;
                    StartCoroutine(RegenStaminaProgressively());
                    yield return new WaitForSeconds(duration);
                    m_Stamina = true;
                }
            }

            private IEnumerator RegenStaminaProgressively (float duration = 1)
            {
                for (float t = 0f; t <= duration; t += Time.deltaTime)
                {
                    m_StaminaAmount = Mathf.Lerp(m_StaminaAmount, m_MaxStaminaAmount, t / duration);
                    yield return new WaitForFixedUpdate();
                }
            }
        }
    }
}
