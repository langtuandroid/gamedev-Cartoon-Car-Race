using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.Serialization;

namespace negleft.AGS{
    public class SteerWheelMobileUI : MonoBehaviour
    {
        [FormerlySerializedAs("UI_Element")] [SerializeField] private Graphic UI_Graphic_Element;

        private RectTransform rectTTransform;
        private Vector2 centerPointVector;
        /// <summary>
        /// Maximum angle this can be rotated at?
        /// </summary>
        [FormerlySerializedAs("maximumSteeringAngle")] [SerializeField] private float maximumSteeringAngleValue = 270f;
        /// <summary>
        /// How fast the weel should snap back
        /// </summary>
        [FormerlySerializedAs("wheelReleasedSpeed")] [SerializeField] private float wheelReleasedSpeedValue = 1000f;
        /// <summary>
        /// Current wheel angle
        /// </summary>
        [FormerlySerializedAs("wheelAngle")] [SerializeField] private float wheelAngleValue = 0f;
        private float wheelPrevAngleValue = 0f;

        private bool wheelBeingHeldFlag = false;

        public float GetClampedWheelValue()
        {
            // returns a value in range [-1,1] similar to GetAxis("Horizontal")
            return wheelAngleValue / maximumSteeringAngleValue;
        }

        public float GetAngleValue()
        {
            // returns the wheel angle itself without clamp operation
            return wheelAngleValue;
        }

        private void Start()
        {
            rectTTransform = UI_Graphic_Element.rectTransform;

            InitializeEventsSystem();
            UpdateRectData();
        }

        private void Update()
        {
            // If the wheel is released, reset the rotation
            // to initial (zero) rotation by wheelReleasedSpeed degrees per second
            if (!wheelBeingHeldFlag && !Mathf.Approximately(0f, wheelAngleValue))
            {
                float deltaAngle = wheelReleasedSpeedValue * Time.deltaTime;
                if (Mathf.Abs(deltaAngle) > Mathf.Abs(wheelAngleValue))
                    wheelAngleValue = 0f;
                else if (wheelAngleValue > 0f)
                    wheelAngleValue -= deltaAngle;
                else
                    wheelAngleValue += deltaAngle;
            }

            // Rotate the wheel image
            rectTTransform.localEulerAngles = Vector3.back * wheelAngleValue;
        }

        private void InitializeEventsSystem()
        {
            //Events
            EventTrigger events = UI_Graphic_Element.gameObject.GetComponent<EventTrigger>();

            if (events == null)
                events = UI_Graphic_Element.gameObject.AddComponent<EventTrigger>();

            if (events.triggers == null)
                events.triggers = new System.Collections.Generic.List<EventTrigger.Entry>();

            EventTrigger.Entry entry = new EventTrigger.Entry();
            EventTrigger.TriggerEvent callback = new EventTrigger.TriggerEvent();
            UnityAction<BaseEventData> functionCall = new UnityAction<BaseEventData>(PressEventListener);
            callback.AddListener(functionCall);
            entry.eventID = EventTriggerType.PointerDown;
            entry.callback = callback;

            events.triggers.Add(entry);

            entry = new EventTrigger.Entry();
            callback = new EventTrigger.TriggerEvent();
            functionCall = new UnityAction<BaseEventData>(DragEventListener);
            callback.AddListener(functionCall);
            entry.eventID = EventTriggerType.Drag;
            entry.callback = callback;

            events.triggers.Add(entry);

            entry = new EventTrigger.Entry();
            callback = new EventTrigger.TriggerEvent();
            functionCall = new UnityAction<BaseEventData>(ReleaseEventListener);//
            callback.AddListener(functionCall);
            entry.eventID = EventTriggerType.PointerUp;
            entry.callback = callback;

            events.triggers.Add(entry);
        }

        private void UpdateRectData()
        {

            Vector3[] corners = new Vector3[4];
            rectTTransform.GetWorldCorners(corners);

            for (int i = 0; i < 4; i++)
            {
                corners[i] = RectTransformUtility.WorldToScreenPoint(null, corners[i]);
            }

            Vector3 bottomLeft = corners[0];
            Vector3 topRight = corners[2];
            float width = topRight.x - bottomLeft.x;
            float height = topRight.y - bottomLeft.y;

            Rect _rect = new Rect(bottomLeft.x, topRight.y, width, height);
            centerPointVector = new Vector2(_rect.x + _rect.width * 0.5f, _rect.y - _rect.height * 0.5f);
        }

        private void PressEventListener(BaseEventData eventData)
        {
            // Executed when mouse/finger starts touching the steering wheel
            Vector2 pointerPos = ((PointerEventData)eventData).position;

            wheelBeingHeldFlag = true;
            wheelPrevAngleValue = Vector2.Angle(Vector2.up, pointerPos - centerPointVector);
        }

        private void DragEventListener(BaseEventData eventData)
        {
            // Executed when mouse/finger is dragged over the steering wheel
            Vector2 pointerPos = ((PointerEventData)eventData).position;

            float wheelNewAngle = Vector2.Angle(Vector2.up, pointerPos - centerPointVector);
            // Do nothing if the pointer is too close to the center of the wheel
            if (Vector2.Distance(pointerPos, centerPointVector) > 20f)
            {
                if (pointerPos.x > centerPointVector.x)
                    wheelAngleValue += wheelNewAngle - wheelPrevAngleValue;
                else
                    wheelAngleValue -= wheelNewAngle - wheelPrevAngleValue;
            }
            // Make sure wheel angle never exceeds maximumSteeringAngle
            wheelAngleValue = Mathf.Clamp(wheelAngleValue, -maximumSteeringAngleValue, maximumSteeringAngleValue);
            wheelPrevAngleValue = wheelNewAngle;
        }

        private void ReleaseEventListener(BaseEventData eventData)
        {
            // Executed when mouse/finger stops touching the steering wheel
            // Performs one last DragEvent, just in case
            DragEventListener(eventData);

            wheelBeingHeldFlag = false;
        }
    }
}