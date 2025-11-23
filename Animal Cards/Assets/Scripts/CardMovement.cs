using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class CardMovement : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    private InputAction click;
    private InputAction pointer;

    private RectTransform rectTransform;
    private Canvas canvas;
    private Vector2 originalLocalPointerPosition;
    private Vector3 originalPanelLocalPosition;
    private Vector3 originalScale;
    private int currentState = 0;
    private Quaternion originalRotation;
    private Vector3 originalPosition;

    [SerializeField] private float selectScale = 1.1f;      //a card pops bigger when hovered
    [SerializeField] private Vector2 cardPlay;      //when does a card snap into play?
    [SerializeField] private Vector3 playPosition;      //where does it snap to?
    [SerializeField] private GameObject glowEffect;
    [SerializeField] private GameObject playArrow;




    void Awake()
    {
        click = InputSystem.actions.FindAction("Click");
        pointer = InputSystem.actions.FindAction("Point");

        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
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
        if (currentState == 1)
        {
            currentState = 2;
            //get correct position according to the camera's view of the world...
            //...as originalLocalPointerPosition
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.GetComponent<RectTransform>(), eventData.position, eventData.pressEventCamera, out originalLocalPointerPosition);
            originalPanelLocalPosition = rectTransform.localPosition;
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
                
                //if within the Y range for a card entering play...
                if (rectTransform.localPosition.y > cardPlay.y)
                {
                    currentState = 3;       //...flag that and...
                    playArrow.SetActive(true);
                    rectTransform.localPosition = playPosition;     //snap to position
                }
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
        rectTransform.localPosition = playPosition;
        rectTransform.localRotation = Quaternion.identity;

        //if card, still clicked, is dragged below play area...
        if (pointer.ReadValue<Vector2>().y < cardPlay.y)
        {
            currentState = 2;       //...flag that it is no longer over play area
            playArrow.SetActive(false);
        }
        
    }
}
