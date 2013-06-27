using UnityEngine;
using System.Collections;

/// <summary>
/// Top down camera control script.
/// </summary>
[ExecuteInEditMode]
public class TopDownCamera : RuntimeCameraControl
{
	/// <summary>
	/// The camera control parameter for player camera.
	/// </summary>
	public TopDownCameraControlParameter topDownCameraParameter;
	
	public Transform Focus = null;
	
	/// <summary>
	/// The character which the camera is currenting viewing on
	/// </summary>
	private CharacterController PlayerCharacter = null;
	private Vector3 dampingVelocity = new Vector3 ();
	private bool ShouldAutoAdjustCamera = true;

	public float shake_decay = 0.002f;
	public float shake_intensity = 1.0f;
	public float shake_interval = 0.02f;
	private bool isShaking = false;
	private Vector3 originPosition;
	private float CameraDampInterval = 0.02f;
	private float CameraLastDampTime = 0;
	
	/// <summary>
	/// The current top down camera parameter.
	/// Can be overrided at runtime.
	/// By default, the predefined parameter is used.
	/// </summary>
	public TopDownCameraControlParameter CurrentTopDownCameraParameter = null;
	
	void Awake ()
	{
		PlayerCharacter = transform.root.GetComponentInChildren<CharacterController> ();
		CurrentTopDownCameraParameter = topDownCameraParameter;
	}

	// Use this for initialization
	void Start ()
	{
		//restore the original position
		originPosition = transform.position;
	}

	void LateUpdate ()
	{
		if (Working) {
			//avoid too frequent update
			if (Time.time - CameraLastDampTime >= CameraDampInterval) {
				if(Focus == null)
				{
				   ApplyCameraControlParameter (true, GetCharacterCenter (), this.CurrentTopDownCameraParameter);
				}
				else 
				{
				   ApplyCameraControlParameter (true, Focus.position, this.CurrentTopDownCameraParameter);
				}
				CameraLastDampTime = Time.time;
			}
		}
	}

	private Vector3 GetCharacterCenter ()
	{
		if (PlayerCharacter == null)
			PlayerCharacter = transform.root.GetComponentInChildren<CharacterController> ();
		return PlayerCharacter.transform.position + PlayerCharacter.center;
	}
	
	/// <summary>
	/// Applies the camera control parameter to the current camera.
	/// smoothDamp - if smoothly damp from current camera position ? if false, the camera control parameter takes effect immediately.
	/// CharacterCenter - the camera center.
	/// topdownCameraControlParameter - the topDownCameraControlParameter.
	/// </summary>
	public virtual void ApplyCameraControlParameter (bool SmoothDamp, 
		                                             Vector3 CharacterCenter, 
		                                             TopDownCameraControlParameter topdownCameraControlParameter)
	{
		if (LevelManager.Instance != null && LevelManager.Instance.ControlDirectionPivot != null) {
			Vector3 NewPositionOffset = LevelManager.Instance.ControlDirectionPivot.TransformDirection (Vector3.back) * topdownCameraControlParameter.DynamicDistance;
			NewPositionOffset += Vector3.up * topdownCameraControlParameter.DynamicHeight;
			Vector3 newPosition = (SmoothDamp) ?
                                   Vector3.SmoothDamp (transform.position, CharacterCenter + NewPositionOffset, ref dampingVelocity, CurrentTopDownCameraParameter.smoothLag) 
                                   : CharacterCenter + NewPositionOffset;
			newPosition = AdjustLineOfSight (newPosition, CharacterCenter);
			transform.position = newPosition;
		} else 
			Debug.LogWarning ("Warning! NO Level manager instance found in scene, can not set position of the camera");
	}


	/// <summary>
	/// If current camera sign being obstacled by object in layer, return the closet unhidden point
	/// </summary>
	/// <param name="newPosition"></param>
	/// <param name="target"></param>
	/// <returns></returns>
	protected Vector3 AdjustLineOfSight (Vector3 newPosition, Vector3 target)
	{
		RaycastHit hit;
		if (Physics.Linecast (target, newPosition, out hit, CurrentTopDownCameraParameter.lineOfSightMask.value)) {
			dampingVelocity = Vector3.zero;
			return hit.point;
		}
		return newPosition;
	}

	void OnEnable ()
	{
		//Debug.Log ("Position damp immediately!");
		ApplyCameraControlParameter (false, GetCharacterCenter (), this.CurrentTopDownCameraParameter);
		transform.LookAt (PlayerCharacter.transform);
	}
	
	/// <summary>
	/// Shake from
	/// </summary>
	public IEnumerator Shake ()
	{
		isShaking = true;
		if (shake_intensity > 0) {
			transform.position = originPosition + Random.insideUnitSphere * shake_intensity;
			shake_intensity -= shake_decay;
		}
		yield return new WaitForSeconds(shake_interval);
		isShaking = false;
	}
}
