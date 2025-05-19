using UnityEngine;

public class ObjectThing : MonoBehaviour
{
    private ObjectState _currState = ObjectState.UNDETECTED;


    void Update()
    {

    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Sight"))
        {
            _currState = ObjectState.DETECTED;
            switchState();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Sight"))
        {
            _currState = ObjectState.UNDETECTED;
            switchState();
        }
    }
    private void switchState(){
        if (_currState == ObjectState.UNDETECTED)
        {
            enabled = false;
        }
        else if(_currState == ObjectState.DETECTED)
        {
            enabled = true; 
        }

    }

    public enum ObjectState{
        UNDETECTED,
        DETECTED
    }
}
