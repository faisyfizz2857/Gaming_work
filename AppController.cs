/// <summary>
/// Resolutions settings for the screen
/// </summary>
using UnityEngine;
using System.Collections;

public class AppController : MonoBehaviour
{

    public static float getMWidth(float pWidth )  {
		float w_  = ((pWidth * 100f) / 1280f);
		return ((w_ /100.00f) *Screen.width);
	}
	
	public static float getMHeight(float pHeight )  {
		float h_  = ((pHeight * 100f) /720f);
		return ((h_ /100.0f) * Screen.height);
	}
	
	public static float getRWidth(float pWidth ) {
		float w_  = ((pWidth * 100f) / Screen.width);
		return ((w_ /100.0f) * 1280f);
		
	}
	
	public static float getRHeight(float pHeight ) {
		float h_  = ((pHeight * 100f) / Screen.height);
		return ((h_ /100.0f) * 720f);
	}
}