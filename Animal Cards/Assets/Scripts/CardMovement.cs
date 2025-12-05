using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using TMPro;
using System.Linq;
using UnityEngine.XR;

public class CardMovement : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    private InputAction click;
    private InputAction pointer;

    private RectTransform rectTransform;
    private BoxCollider2D boxColl;
    private Canvas canvas;
    private Vector2 originalLocalPointerPosition;
    private Vector3 originalPanelLocalPosition;
    private Vector3 originalScale;
    private int currentState = 0;
    private Quaternion originalRotation;
    private Vector3 originalPosition;

    [SerializeField] private float selectScale = 1.1f;      //a card pops bigger when hovered
    [SerializeField] private int playPosition;      //which spot does it snap to?
    [SerializeField] private GameObject glowEffect;
    [SerializeField] private GameObject playArrow;
    [SerializeField] private RectTransform playspot1;
    [SerializeField] private RectTransform playspot2;




    void Awake()
    {
        click = InputSystem.actions.FindAction("Click");
        pointer = InputSystem.actions.FindAction("Point");

        rectTransform = GetComponent<RectTransform>();
        boxColl = GetComponent<BoxCollider2D>();
        canvas = GetComponentInParent<Canvas>();
        
        GameObject[] PSes = GameObject.FindGameObjectsWithTag("Playspots");
        playspot1 = PSes.ElementAt<GameObject>(1).GetComponent<RectTransform>();
        playspot2 = PSes.ElementAt<GameObject>(0).GetComponent<RectTransform>();

        originalScale = rectTransform.localScale;
        originalPosition = rectTransform.localPosition;
        originalRotation = rectTransform.localRotation;
    }



    void Update()
    {
        switch (currentState)
        {
            case 0:     //if card is in hand
                break;
            case 1:     //if pointer is over card
                HandleHoverState();
                break;
            case 2:     //if card is clicked
                HandleDragState();
                if (!click.IsPressed())   //if mouse button is released
                {
                    TransitionToState0();
                }
                break;
            case 3:     //if card held over play area
                HandlePlayState();
                if (!click.IsPressed())   //if mouse button is released
                {
                    rectTransform.SetParent(playPosition == 1 ? playspot1 : playspot2);
                    GetComponent<CardStats>().state = CardStats.PlayState.InPlay;
                    FindFirstObjectByType<HandManager>().cardsInHand.Remove(gameObject);
                    
                    originalPosition = rectTransform.localPosition;
                    originalRotation = Quaternion.identity;
                    TransitionToState0();
                }
                break;
            default:
                TransitionToState0();
                break;
        }  
    }



    private void TransitionToState0()   //reset card to original state
    {
        currentState = 0;
        rectTransform.localScale = originalScale;
        rectTransform.localPosition = originalPosition;
        rectTransform.localRotation = originalRotation;
        glowEffect.SetActive(false);
        playArrow.SetActive(false);
    }



    //currentState 0 -> 1
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (currentState == 0)
        {
            //it starts where it is
            originalPosition = rectTransform.localPosition;
            originalRotation = rectTransform.localRotation;
            originalScale = rectTransform.localScale;
            currentState = 1;
        }
    }


    //currentState 1 -> 0
    public void OnPointerExit(PointerEventData eventData)
    {
        if (currentState == 1)
        {
            TransitionToState0();
        }
    }


    //currentState 1 -> 2
    public void OnPointerDown(PointerEventData eventData)
    {
        if (GameManager.Instance.TurnCount == 1 && GameManager.Instance.PhaseCount == 1 && GetComponent<CardStats>().state == CardStats.PlayState.InHand)
        {
            if (currentState == 1)
            {
                currentState = 2;
                //get correct position according to the camera's view of the world...
                //...as originalLocalPointerPosition
                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.GetComponent<RectTransform>(), eventData.position, eventData.pressEventCamera, out originalLocalPointerPosition);
                originalPanelLocalPosition = rectTransform.localPosition;
            }
        }
    }


    //currentState 2 -> 3, if over play area
    public void OnDrag(PointerEventData eventData)
    {
        if (currentState == 2)
        {
            //get correct position according to the camera's view of the world...
            //...as localPointerPosition
            //if this position is inside rectagle...
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.GetComponent<RectTransform>(), eventData.position, eventData.pressEventCamera, out Vector2 localPointerPosition))
            {
                //respect scale
                localPointerPosition /= canvas.scaleFactor;
                //follow mouse location, not mouse movement 
                Vector3 offsetToOriginal = localPointerPosition - originalLocalPointerPosition;
                //move it!
                rectTransform.localPosition = originalPanelLocalPosition + offsetToOriginal;
                
                //if within a playspot...
                if (boxColl.OverlapPoint(playspot1.position) && playspot1.childCount == 0)
                {
                    currentState = 3;       //...flag that and...
                    playArrow.SetActive(true);
                    rectTransform.position = playspot1.position;     //snap to position
                    playPosition = 1;
                }
                else if (boxColl.OverlapPoint(playspot2.position) && playspot2.childCount == 0)
                {
                    currentState = 3;       //...flag that and...
                    playArrow.SetActive(true);
                    rectTransform.position = playspot2.position;     //snap to position
                    playPosition = 2;
                }
            }
        }
        //if card, still clicked, is dragged out of a playspot...
        else if (currentState == 3)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.GetComponent<RectTransform>(), eventData.position, eventData.pressEventCamera, out Vector2 localPointerPosition);
            localPointerPosition /= canvas.scaleFactor;
            if (!boxColl.OverlapPoint(localPointerPosition))
            {
                Vector3 offsetToOriginal = localPointerPosition - originalLocalPointerPosition;
                rectTransform.localPosition = originalPanelLocalPosition + offsetToOriginal;
                currentState = 2;       //...flag that it is no longer over play area
                playArrow.SetActive(false);
                playPosition = 0;
            }
        }
    }



    private void HandleHoverState()
    {
        glowEffect.SetActive(true);
        rectTransform.localScale = originalScale * selectScale;
    }
    

    private void HandleDragState()
    {
        rectTransform.localRotation = Quaternion.identity;  //set rotation to 0
    }

    
    private void HandlePlayState()
    {
        rectTransform.localRotation = Quaternion.identity;

        if (playPosition == 1) { rectTransform.position = playspot1.position; }
        else if (playPosition == 2) { rectTransform.position = playspot2.position; }     
    }
}
