using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent(typeof (Predator3rdPersonMovementController))]
[RequireComponent(typeof (PredatorPlayerStatus))]
public class Joystick_Predator : JoyButton {
	
	public Texture ForeGroundTexture;
	public float ForeGroundTextureSize = 25;
	
    public ThirdPersonFollowCamera_Predator camera_controller = null;
    public GameGUIHelper.RectPosition Location = GameGUIHelper.RectPosition.BottomLeft;

    private Predator3rdPersonMovementController PredatorMovementController = null;
    private MovementControlMode playerControlMode;

    void Awake()
    {
        this.JoyButtonName = "Joystick_Predator";
        PredatorMovementController = this.GetComponent<Predator3rdPersonMovementController>();
        ValueOffsetModifier = ((float)JoyButtonSize / 2) - 15;
        JoyButtonBound = GameGUIHelper.GetSquareOnGUICoordinate(Location, JoyButtonSize);
        playerControlMode = GetComponent<PredatorPlayerStatus>().PlayerControlMode;
    }

    void Start()
    {
        JoyButtonBound = GameGUIHelper.GetSquareOnGUICoordinate(Location, JoyButtonSize);
    }

    void Update()
    {
        JoyButtonBound = GameGUIHelper.GetSquareOnGUICoordinate(Location, JoyButtonSize);
    }

    public override void onTouchBegin(Touch t)
    {
        base.onTouchBegin(t);
    }

    private void SetDirectionInCharacterRelativeMode()
    {
        Vector3 worldDirection = LevelManager.Instance.ControlDirectionPivot.TransformDirection
			(new Vector3(Joybutton_Right_Value, 0, Joybutton_Up_Value));
        PredatorMovementController.MoveDirection_CharacterRelative = worldDirection.normalized;
    }

    private void SetDirectionInCameraRelativeMode()
    {
        float absUp = Mathf.Abs(Joybutton_Up_Value);
        float absRight = Mathf.Abs(Joybutton_Right_Value);
        //Up >>> right - ignore right
        if (absUp >= absRight * 2)
        {
            PredatorMovementController.MoveForwardModifier = Joybutton_Up_Value;
            PredatorMovementController.MoveRightModifier = 0;
        }
        else if (absRight >= absUp * 2)
        {
            PredatorMovementController.MoveForwardModifier = 0;
            PredatorMovementController.MoveRightModifier = Joybutton_Right_Value;
        }
        else
        {
            PredatorMovementController.MoveForwardModifier = Joybutton_Up_Value;
            PredatorMovementController.MoveRightModifier = Joybutton_Right_Value;
        }
    }

    public override void onTouchMove(Touch t)
    {
        Vector2 direction = (t.position - TouchStartPosition).normalized;
        Joybutton_Up_Value = direction.y;
        Joybutton_Right_Value = direction.x;
        //Debug.Log("onTouchMove : up:" + Joybutton_Up_Value + " right: " + Joybutton_Right_Value);
        switch (playerControlMode)
        {
            case MovementControlMode.CameraRelative:
                SetDirectionInCameraRelativeMode();
                break;
            case MovementControlMode.CharacterRelative:
                SetDirectionInCharacterRelativeMode();
                break;
        }
        JoyButtonBoundOffset.x = direction.x * ValueOffsetModifier;
        JoyButtonBoundOffset.y = -direction.y * ValueOffsetModifier;
    }

    public override void onTouchEnd(Touch t)
    {
        //Debug.Log("onTouchEnd");
        base.onTouchEnd(t);

        PredatorMovementController.MoveForwardModifier = 0;
        PredatorMovementController.MoveRightModifier = 0;

        PredatorMovementController.MoveDirection_CharacterRelative = Vector3.zero;

        JoyButtonBoundOffset.x = 0;
        JoyButtonBoundOffset.y = 0;
    }

    public override void onTouchStationary(Touch t)
    {
        this.onTouchMove(t);
    }

    void OnGUI()
    {
        Rect backgroundRectArea = new Rect(JoyButtonBound.x, JoyButtonBound.y,JoyButtonSize, JoyButtonSize);
		GUI.DrawTexture(backgroundRectArea, ButtonTexture);
		
		
        Rect foregroundRectArea = new Rect(JoyButtonBound.center.x - ForeGroundTextureSize/2 + JoyButtonBoundOffset.x, 
			JoyButtonBound.center.y - ForeGroundTextureSize/2 + JoyButtonBoundOffset.y,
      ForeGroundTextureSize, ForeGroundTextureSize);
        GUI.DrawTexture(foregroundRectArea, ForeGroundTexture);
    }
}
