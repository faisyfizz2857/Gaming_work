using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject[] AllTowers; //initialization of array for towers
    public int RingNumber;         //Assigning Ring number
    public bool detected = false;
    private Vector3 offset;
    public Vector3 initialPosition;
    private Vector3 screenpoint;
    private Vector3 worldpoint;
    private float startTime;
	public Sprite isNotCollidedSprite;
	public Sprite OriginalSprite;
	public Sprite onClickChange;
    GameObject currentTower;
    [SerializeField] Transform particlePosition;
    [SerializeField] GameObject particleOnPlacement;

    bool isClicked;

    // Use this for initialization
    void Start()
    {
        isClicked = false;
		initialPositionBeforeMove = initialPosition = transform.localPosition;
    }
	
	//movement of an object
	void Move(){
	 if (isClicked)
        {
			// Using math.lerp for linearly interpolation if isClicked==true
            startTime += Time.smoothDeltaTime;
            if (startTime < 0.2f)
            {
                transform.localPosition = new Vector2(Mathf.Lerp(transform.localPosition.x, initialPosition.x, startTime), Mathf.Lerp(transform.localPosition.y, initialPosition.y, startTime));
                
            }
            else
            {
               // Debug.Log("item reached at position");
                //spawning of particles and destroying after 2 seconds
                transform.localPosition = initialPosition;
				initialPositionBeforeMove = initialPosition;
                Destroy(Instantiate(particleOnPlacement, particlePosition.position, new Quaternion()), 2);
                startTime = 0;
                isClicked = false;
            }
        }	
	}

    //Checking here if Ring can move 
    void RingCanMove()
    {
        for (int i = 0; i < AllTowers.Length; i++)
        {
            if (AllTowers[i].GetComponent<BoxCollider>().bounds.Intersects(GetComponent<Collider>().bounds)) //Ring collision with the tower
            {

                if (RingNumber == AllTowers[i].GetComponent<Tower_Handling>().Tower_Number)
                {
                    //Adding AudioSource if not present before and assigning sound to it
                    if (!GetComponent<AudioSource>())
                    {
                        gameObject.AddComponent<AudioSource>();
                        GetComponent<AudioSource>().clip = Resources.Load<AudioClip>("Sounds/item");
                        GetComponent<AudioSource>().playOnAwake = false;
                    }
					GetComponent<AudioSource>().clip = Resources.Load<AudioClip>("Sounds/item");
                    GetComponent<AudioSource>().Play();

                    if (!detected)
                    {
                        transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);   //transforming scale of sprite on click 
                        GetComponent<SpriteRenderer>().sprite = onClickChange; //Changing sprite on Click
                    }
                    currentTower = AllTowers[i];
					transform.localPosition = initialPositionBeforeMove;
                    StopAllCoroutines();
                    initialPosition = transform.localPosition;
                    GetComponent<SpriteRenderer>().sortingOrder = 100; //changing layer 
                    offset = new Vector2(transform.position.x - Utility.getPositionVector2().x, transform.position.y - Utility.getPositionVector2().y);
                    StartCoroutine(Moveup(transform, initialPosition, new Vector3(0, 4, transform.localPosition.z)));
                }

            }
        }
    }


    void CanRingPlacedOnTower()
    {
        transform.localScale = new Vector3(1f, 1f, 1f);
        GetComponent<SpriteRenderer>().sprite = OriginalSprite;
        detected = false;
        GetComponent<SpriteRenderer>().sortingOrder = 6;
        bool iscollided = false;
		// If ring can be placed on other tower or not 
        for (int i = 0; i < AllTowers.Length; i++)
        {
            if (AllTowers[i].GetComponent<BoxCollider>().bounds.Intersects(GetComponent<Collider>().bounds) && transform.parent != AllTowers[i].transform)
            {
                if (CanPlaced(AllTowers[i]))
                {
                    iscollided = true;
					initialPosition = new Vector3(0, AllTowers[i].GetComponent<Tower_Handling>().destinationPosition.y + (1.34f * AllTowers[i].transform.childCount), 0);
                    transform.parent = AllTowers[i].transform;
                    if (currentTower.transform.childCount > 0)
                    {
                        currentTower.GetComponent<Tower_Handling>().Tower_Number = currentTower.transform.GetChild(currentTower.transform.childCount - 1).GetComponent<GameController>().RingNumber;
                    }
                    else
                    {
                        currentTower.GetComponent<Tower_Handling>().Tower_Number = -1;
                    }
                    currentTower = AllTowers[i];
                }
            }
        }

        if (!iscollided)
        {
			GetComponent<AudioSource>().clip = Resources.Load<AudioClip>("Sounds/wrongSound");  	//Wrong sound will be played if placed on wrong tower.
            GetComponent<AudioSource>().Play();
            GetComponent<SpriteRenderer>().sprite = isNotCollidedSprite; 							//Assigning sprite if not collided with tower
        }
    }

    // Update is called once per frame
    void Update()
    {
        Move();

        //touch start
        if (Utility.isClicked_3D(GetComponent<Collider>()) && !isClicked)
        {
            //  Debug.Log("Touch start"); 
			RingCanMove();		//calling RingCanMove function here
        }

        if (detected)
        {
            // Debug.Log("Touch continue");
            drag();				//calling drag function here
        }

        //touch ended
        if ((Utility.getTouched_Phase2D(0) == TouchPhase.Ended && detected ) ||( Utility.getTouched_Phase2D(0) == TouchPhase.Ended && isMoving))
        {
           // Debug.Log("Touch End");

            isMoving = false;
            detected = false;

            StopAllCoroutines();
			CanRingPlacedOnTower();		//calling CanRingPlacedOnTower function here

            isClicked = true;
            //  Debug.Log(iscollided);
        }
    }


	// Checking here Rings can be placed on towers or not

    bool isMoving = false;
	Vector3 initialPositionBeforeMove; 			// initializing a vector variable
    bool CanPlaced(GameObject Tower)
    {
        bool isPlaceable = false;
		if (Tower.GetComponent<Tower_Handling>().Tower_Number == -1)
        {
            isPlaceable = true;
			Tower.GetComponent<Tower_Handling>().Tower_Number = RingNumber;
        }
		if (Tower.GetComponent<Tower_Handling>().Tower_Number >= RingNumber)
        {
            isPlaceable = true;
			Tower.GetComponent<Tower_Handling>().Tower_Number = RingNumber;
        }
		return isPlaceable;

    }
  

	// Draging of an object Function
    void drag()
    {
        transform.position = new Vector3(offset.x + Utility.getPositionVector2().x, Mathf.Clamp(offset.y + Utility.getPositionVector2().y, 2.3f, 5), transform.position.z);
    }
	//Functoin for moving the object upward while touching
    IEnumerator Moveup(Transform objectToMove, Vector3 start, Vector3 end)

    {
        initialPositionBeforeMove = start;
        GetComponent<AudioSource> ().Play ();
        float distance = 0;
        Vector3 initialposition = start;
        Vector3 finalPosition = end;
        isMoving = true;
        float desiredspeed = 0.015f;
        float distanceBaseSpeed = (finalPosition.y - initialposition.y) * desiredspeed;
        do
        {
            distance += distanceBaseSpeed;
            objectToMove.localPosition = Vector3.Lerp(initialposition, finalPosition, distance);
            yield return new WaitForSeconds(Time.deltaTime);

        } while (distance <= 1);
		// active detected bool allowing to move object

        detected = true;
        isMoving = false;

    }
}
