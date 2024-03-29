using UnityEngine;
using System.Collections;

[RequireComponent(typeof (Predator3rdPersonalStealthController))]
[ExecuteInEditMode]
public class JoybuttonStealth : JoyButton {
    private Predator3rdPersonalStealthController StealthController;

    void Awake()
    {
        StealthController = GetComponent<Predator3rdPersonalStealthController>();
    }

	// Use this for initialization
	void Start () {
		GUIBound = this.GetAdaptiveBound();
	}
	
	// Update is called once per frame
	void Update () {
	}
	
    public override void onTouchMove(Touch touch)
    {
    }
	
    /// <summary>
    /// Call when touch.phase = Move
    /// </summary>
    /// <param name="touch"></param>
    public override void onTouchStationary(Touch touch)
    {
    }

    public override void onTouchEnd(Touch touch)
    {
        base.onTouchEnd(touch);
        StealthController.ToggleStealth();
    }
	
    void OnGUI()
    {
//		if(Application.platform == RuntimePlatform.WindowsEditor)
//		{
//			JoyButtonBound = this.GetAdaptiveBound();
//		}
        GUI.DrawTexture(GUIBound, ButtonTexture);
    }
}
