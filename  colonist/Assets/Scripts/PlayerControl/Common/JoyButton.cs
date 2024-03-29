using UnityEngine;
using System.Collections;

/// <summary>
/// AdaptiveLengthMode defines how length adapt to current screen resolution.
/// </summary>
public enum AdaptiveLengthMode
{
    /// <summary>
    /// Length value is a fixed value.
    /// </summary>
    Fixed = 0,
    /// <summary>
    /// Length is relative to screen width.
    /// </summary>
    RelativeToScreenWidth = 1,
    /// <summary>
    /// Length is relative to screen height.
    /// </summary>
    RelativeToScreenHeight = 2,

    /// <summary>
    /// Relative to referrence button's width.
    /// </summary>
    RelativeToReferrenceButtonWidth = 3,

    /// <summary>
    /// Relative to referrence button's height.
    /// </summary>
    RelativeToReferrenceButtonHeight = 4,
}

/// <summary>
/// AdaptiveLength represents a length which can self-adapt to screen resolution.
/// </summary>
[System.Serializable]
public class AdaptiveLength
{
    public AdaptiveLengthMode adaptiveLengthMode = AdaptiveLengthMode.Fixed;
    /// <summary>
    /// FixedSize when mode = fixed.
    /// </summary>
    public float FixedValue = 1;
    /// <summary>
    /// Aspect to screen width (when mode = relative to screen width) or aspect to screen height (when mode = relative to screen height).
    /// Note: the lengthAspect should between 0 and 1.
    /// </summary>
    public float Aspect = 0.1f;

    public float GetValue(Vector2 referrenceGUISize = new Vector2())
    {
        switch (adaptiveLengthMode)
        {
            case AdaptiveLengthMode.RelativeToReferrenceButtonWidth:
                return referrenceGUISize.x * Aspect;
            case AdaptiveLengthMode.RelativeToReferrenceButtonHeight:
                return referrenceGUISize.y * Aspect;
            case AdaptiveLengthMode.RelativeToScreenHeight:
                return Screen.height * Aspect;
            case AdaptiveLengthMode.RelativeToScreenWidth:
                return Screen.width * Aspect;
            case AdaptiveLengthMode.Fixed:
            default:
                return FixedValue;
        }
    }
}

/// <summary>
/// AdaptivePointMode defines how point adapt to current screen resolution.
/// </summary>
public enum AdaptivePointMode
{
    /// <summary>
    /// Point is fixed.
    /// </summary>
    Fixed = 0,
    /// <summary>
    /// Relative to screen left border.
    /// </summary>
    RelativeToScreenLeft = 1,

    /// <summary>
    /// Relative to screen top border.
    /// </summary>
    RelativeToScreenTop = 2,

    /// <summary>
    /// Relative to referrence button anchor point's left.
    /// The offset unit is defined in AdaptiveLength.
    /// </summary>
    RelativeToReferrenceButtonLeft = 3,

    /// <summary>
    /// Relative to referrence button anchor point's top.
    /// The offset unit is defined in AdaptiveLength.
    /// </summary>
    RelativeToReferrenceButtonTop = 4,
	
	/// <summary>
	/// Final value = Screen width - Value
	/// </summary>
	ScreenWidthMinusFixedValue = 5,
	/// <summary>
	/// Final value = Screen height - Value
	/// </summary>
	ScreenHeightMinusFixedValue = 6,
}

/// <summary>
/// AdaptivePoint represents an anchor which can self-adapt to screen resolution.
/// </summary>
[System.Serializable]
public class AdaptiveAnchor
{
    public AdaptivePointMode adaptivePointMode = AdaptivePointMode.Fixed;
    /// <summary>
    /// The fixed value, when mode = fixed.
    /// </summary>
    public float FixedValue = 1;
    /// <summary>
    /// When mode = relative to RelativeToScreenLeft or RelativeToScreenTop.
    /// The aspect to screen left border or top border
    /// Note: aspect should between 0 and 1.
    /// </summary>
    public float Aspect = 0.1f;

    /// <summary>
    /// When mode = RelativeToReferrenceButtonLeft or RelativeToReferrenceButtonTop, 
    /// assign the adaptiveLength the variables.
    /// </summary>
    public AdaptiveLength RelativeReferrenceLength = null;

    public float GetValue()
    {
        switch (adaptivePointMode)
        {
            case AdaptivePointMode.RelativeToScreenLeft:
                return Screen.width * Aspect;
            case AdaptivePointMode.RelativeToScreenTop:
                return Screen.height * Aspect;
            case AdaptivePointMode.RelativeToReferrenceButtonTop:
            case AdaptivePointMode.RelativeToReferrenceButtonLeft:
                throw new System.Exception("Call GetValue(ReferrencePoint) instead!");
		case AdaptivePointMode.ScreenWidthMinusFixedValue:
			return Screen.width - FixedValue;
			break;
		case AdaptivePointMode.ScreenHeightMinusFixedValue:
			return Screen.height - FixedValue;
			break;
            case AdaptivePointMode.Fixed:
            default:
                return FixedValue;
        }
    }

    public float GetValue(AdaptiveRect referrenceGUI)
    {
        float ret = 0;
        switch (adaptivePointMode)
        {
            case AdaptivePointMode.RelativeToScreenLeft:
                return Screen.width * Aspect;
            case AdaptivePointMode.RelativeToScreenTop:
                return Screen.height * Aspect;
            case AdaptivePointMode.RelativeToReferrenceButtonTop:
                return referrenceGUI.GetAnchor().y + this.RelativeReferrenceLength.GetValue(referrenceGUI.GetSize());
                break;
            case AdaptivePointMode.RelativeToReferrenceButtonLeft:
                return referrenceGUI.GetAnchor().x + this.RelativeReferrenceLength.GetValue(referrenceGUI.GetSize());
                break;
            case AdaptivePointMode.Fixed:
            default:
                return FixedValue;
        }
        return ret;
    }
}

/// <summary>
/// An abstract class of JoyButton
/// Offspring should implement methods:
/// - onTouchBegin
/// - onTouchMove
/// - onTouchEnd
/// </summary>
public abstract class JoyButton : MonoBehaviour
{
    /// <summary>
    /// Name of the JoyButton
    /// </summary>
    public string JoyButtonName;

    /// <summary>
    /// Use this feature to control, when there're two joybuttons overlapped on screen, 
    /// which one can get touch check first.
    /// </summary>
    public int Priority = 0;

    /// <summary>
    /// The adaptive bound of this button.
    /// </summary>
    public AdaptiveRect adaptiveBound = new AdaptiveRect();

    /// <summary>
    /// Texture of the JoyButton
    /// </summary>
    public Texture2D ButtonTexture;
    /// <summary>
    /// The Vertical-Value when user finger slip vertically on the button
    /// Positive value - upwards
    /// Negative value - downwards
    /// </summary>
    [HideInInspector]
    public float Joybutton_Up_Value = 0;
    /// <summary>
    /// The Horizontal-Value when user finger slip horizontally on the button
    /// Positive value - right
    /// Negative value - left
    /// </summary>
    [HideInInspector]
    public float Joybutton_Right_Value = 0;

    [HideInInspector]
    public Rect GUIBound;

    public virtual void PlayerControlOn()
    {
        this.enabled = true;
    }

    public virtual void PlayerControlOff()
    {
        this.enabled = false;
    }

    /// <summary>
    /// The button's bound offset , the actual display position on screen = JoyButtonBound + JoyButtonBoundOffset
    /// </summary>
    [HideInInspector]
    public Vector2 JoyButtonRuntimeOffset = new Vector2();
    /// <summary>
    /// User Touch start position on this button
    /// </summary>
    [HideInInspector]
    public Vector2 TouchStartPosition;

    /// <summary>
    /// has a finger on this button?
    /// </summary>
    [HideInInspector]
    public bool hasFingerOnJoyButton;
    /// <summary>
    /// The finger id on this button
    /// </summary>
    protected int fingerID = -1;

    public bool PlayInterfaceAudio = false;
    public AudioClip InterfaceAudioClip = null;

    /// <summary>
    /// Check if a touch should be processed by the Joybutton.
    /// Basically, for a touch at Began phase, check if the touch is in the Button bound area
    /// For touches at other phase, check if the touch fingerid is known.
    /// offspring class should override this method for customing the chech touch logic.
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public virtual bool CheckTouch(Touch t)
    {
        switch (t.phase)
        {
            case TouchPhase.Began:
                return isTouchInsideBound(t);
            case TouchPhase.Canceled:
            case TouchPhase.Stationary:
            case TouchPhase.Ended:
            case TouchPhase.Moved:
                return this.fingerID == t.fingerId;
            default:
                Debug.Log("We find a abnormal touch !" + t.phase);
                return false;
        }
    }

    public virtual void ProcessTouch(Touch t)
    {
        switch (t.phase)
        {
            case TouchPhase.Began:
                onTouchBegin(t);
                break;
            case TouchPhase.Stationary:
                onTouchStationary(t);
                break;
            case TouchPhase.Moved:
                onTouchMove(t);
                break;
            case TouchPhase.Canceled:
            case TouchPhase.Ended:
                onTouchEnd(t);
                break;
        }
    }



    /// <summary>
    /// The time when the finger touch on the Joy button
    /// </summary>
    [HideInInspector]
    public float TouchStartTime;
    public bool isTouchInsideBound(Vector2 touchScreenCoord)
    {
        Vector2 guiCoord = GameGUIHelper.ConvertScreenTouchCoordToGUICoord(touchScreenCoord);
        bool ret = GUIBound.Contains(guiCoord);

        return ret;
    }
    public bool isTouchInsideBound(Touch t)
    {
        return this.isTouchInsideBound(t.position);
    }
    /// <summary>
    /// Call when touch.phase = Began
    /// </summary>
    /// <param name="touch"></param>
    public virtual void onTouchBegin(Touch touch)
    {
        this.hasFingerOnJoyButton = true;
        this.fingerID = touch.fingerId;
        this.TouchStartPosition = touch.position;
        this.TouchStartTime = Time.time;
    }
    /// <summary>
    /// Call when touch.phase = Move
    /// </summary>
    /// <param name="touch"></param>
    public abstract void onTouchMove(Touch touch);
    /// <summary>
    /// Call when touch.phase = Move
    /// </summary>
    /// <param name="touch"></param>
    public abstract void onTouchStationary(Touch touch);

    /// <summary>
    /// Call when touch.phase = End.
    /// Do common things to every JoyButton.
    /// Mark the flags, and clean the value, ... etc
    /// </summary>
    /// <param name="touch"></param>
    public virtual void onTouchEnd(Touch touch)
    {
        this.hasFingerOnJoyButton = false;
        this.fingerID = -1;
        Joybutton_Up_Value = 0;
        Joybutton_Right_Value = 0;
        if (this.PlayInterfaceAudio = true && this.InterfaceAudioClip != null)
        {
            AudioSource.PlayClipAtPoint(InterfaceAudioClip, this.transform.position);
        }
    }

    /// <summary>
    /// Gets the self-adaptive bound to current screen resolution.
    /// </summary>
    public virtual Rect GetAdaptiveBound()
    {
        //if HasReference flag is true, initialize the ReferrenceJoyButton variable.
        if (adaptiveBound.HasReference && adaptiveBound.ReferrenceJoyButton == null)
        {
			JoyButton[] joyButtons = GetComponents<JoyButton>();
            foreach (JoyButton b in joyButtons)
            {
                if (b != this && b.JoyButtonName == adaptiveBound.ReferrenceJoyButtonNanme)
                {
                    adaptiveBound.ReferrenceJoyButton = b;
                    break;
                }
            }
            if (adaptiveBound.ReferrenceJoyButton == null)
            {
                Debug.LogError("Not find reference button!");
                return new Rect(0, 0, 0, 0);
            }
        }
        return this.adaptiveBound.GetBound();
    }

}