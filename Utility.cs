/// <summary>
/// A class with controls all the touch phases in this project used for Unity Editor and Unity Android
/// </summary>
using UnityEngine;
using System.Collections;

public class Utility : MonoBehaviour
{
    private static bool status;
    private static TouchPhase touchPhase;
    private static Vector2 vector2D_getPositonVector2;
    public static bool isClicked_3D(Collider collider3D)
    {
        status = false;

#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 touchPosition3 = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            
            RaycastHit ray;

            if (Physics.Raycast(touchPosition3, Vector3.forward, out ray, 10))
            {
                if (ray.collider == collider3D)
                    status = true;
                else
                    status = false;
            }

            else
                status = false;

        }
#elif UNITY_ANDROID
		if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
		{
		 Vector3 touchPosition3 = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            
            RaycastHit ray;

            if (Physics.Raycast(touchPosition3, Vector3.forward, out ray,10))
            {
                if( ray.collider == collider3D)
                    status = true;
                else
                    status = false;
            }
                           
            else
                status = false;
		}
#endif

        return status;
    }

    public static Vector2 getPositionVector2()
    {

#if UNITY_EDITOR
        if (Input.GetMouseButton(0) || Input.GetMouseButtonUp(0) || Input.GetMouseButtonDown(0))
        {
            Vector3 touchPosition3 = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            vector2D_getPositonVector2 = new Vector2(touchPosition3.x, touchPosition3.y);
        }
      
#elif UNITY_ANDROID
		if (Input.touchCount == 1)
		{
			Vector3 touchPosition3 = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
			vector2D_getPositonVector2 = new Vector2(touchPosition3.x, touchPosition3.y);
		}

#endif

        return vector2D_getPositonVector2;

    }

    public static TouchPhase getTouched_Phase2D(int touchedIndex)
    {



#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(touchedIndex))
        {
            //print("Began");
            touchPhase = TouchPhase.Began;
        }
        else if (Input.GetMouseButtonUp(touchedIndex))
        {
            //			print("Ended");
            touchPhase = TouchPhase.Ended;
        }
        else if (Input.GetMouseButton(touchedIndex))
        {
            //			print("Moved");

            touchPhase = TouchPhase.Moved;
        }
#elif UNITY_ANDROID
		touchPhase = Input.GetTouch(touchedIndex).phase;
#endif

        return touchPhase;

    }

}