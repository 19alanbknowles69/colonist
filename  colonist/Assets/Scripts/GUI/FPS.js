// Attach this to a GUIText to make a frames/second indicator.
//
// It calculates frames/second over each updateInterval,
// so the display does not keep changing wildly.
//
// It is also fairly accurate at very low FPS counts (<10).
// We do this not by simply counting frames per interval, but
// by accumulating FPS for each frame. This way we end up with
// correct overall FPS even if the interval renders something like
// 5.5 frames.

//@script RequireComponent (GUIText)
  
var updateInterval = 0.5;

var recordTime = true;

var displayOnScreen = false;

private var accum = 0.0; // FPS accumulated over the interval
private var frames = 0; // Frames drawn over the interval
private var timeleft : float; // Left time for current interval
private var textPos : Vector2 = Vector2();
private var fps ="";
function Awake()
{
}

function Start()
{
	timeleft = updateInterval;	
}

function FixedUpdate ()
{
textPos = Vector2(0,0);
//textPos.x = -Screen.width/2;
//textPos.y = -Screen.height/2;
	//guiText.pixelOffset.x = -Screen.width/2;
	//guiText.pixelOffset.y = Screen.height/2;
}
 
function Update()
{
	timeleft -= Time.deltaTime;
	accum += Time.timeScale/Time.deltaTime;
	++frames;
	
	// Interval ended - update GUI text and start new interval
	if( timeleft <= 0.0 )
	{
		// display two fractional digits (f2 format)
		//guiText.text = "FPS:" + (accum/frames).ToString("f2");
		fps = "FPS:" + (accum/frames).ToString("f2");
		timeleft = updateInterval;
		accum = 0.0;
		frames = 0;
	} 
}

function OnGUI()
{
    if(displayOnScreen)
    {
      GUI.Label(Rect(textPos.x, textPos.y, 80,30), fps);
      if(recordTime)
      {
         GUI.Label(Rect(Screen.width/2 , 10, 80, 30), Time.time.ToString());
      }
    }
}

//@script RequireComponent (GUIText) 