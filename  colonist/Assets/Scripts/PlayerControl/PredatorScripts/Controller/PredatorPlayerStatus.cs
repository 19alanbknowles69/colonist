using UnityEngine;
using System.Collections;



/// <summary>
/// A class used for its static member to expose current status of the predator protagonist
/// </summary>
public class PredatorPlayerStatus : MonoBehaviour{

    //public MovementControlMode PlayerControlMode = MovementControlMode.CharacterRelative;

    /// <summary>
    /// Assign the predator bone reference at this beheavior,
    /// </summary>
    public Transform root = null;
    public Transform pelvis = null;
    public Transform spine = null;
    public Transform body = null;
    public Transform frontLeftUpperLeg = null;
    public Transform frontRightUpperLeg = null;
    public Transform rearLeftUpperLeg = null;
    public Transform rearRightUpperLeg = null;
    public Transform leftUpperClaw = null;
    public Transform rightUpperClaw = null;
	
    /// <summary>
    /// When DisableUserMovement = true, the movement controller will ignore the user movement command in Update()
    /// </summary>
    [HideInInspector]
    public bool DisableUserMovement = false;

    public static bool isBusy
    {
        get
        {
            return isFetching || isAttacking || isMoving;
        }
    }


    private static bool isFetching = false;
    private static bool isAttacking = false;
    private static bool isMoving = false;
    private static bool isJumping = false;
	
    public static bool IsFetching
    {
        get
        {
            return isFetching;
        }
    }

    public static bool IsAttacking
    {
        get
        {
            return isAttacking;
        }
    }

    public static bool IsMoving
    {
        get
        {
            return isMoving;
        }
    }
	
	public static bool IsJumping
    {
        get
        {
            return isJumping;
        }
    }

    private Predator3rdPersonMovementController movementController;
    private Predator3rdPersonalAttackController attackController;
    //private Predator3rdPersonalFetchController fetchController;
    private Predator3rdPersonalJumpController JumpController;
    void Awake()
    {
        movementController = this.GetComponent<Predator3rdPersonMovementController>();
        attackController = this.GetComponent<Predator3rdPersonalAttackController>();
        //fetchController = this.GetComponent<Predator3rdPersonalFetchController>();
        JumpController = this.GetComponent<Predator3rdPersonalJumpController>();
    }

    private void UpdateStatus()
    {
        isAttacking = attackController.IsPlayingAttack() ;//|| fetchController.isPlayingFetchAnimation();
        isMoving = !Mathf.Approximately(movementController.MoveDirection.magnitude, 0); 
		isJumping = JumpController.IsJumping;
    }

    void Update()
    {
        UpdateStatus();
    }
	
}
