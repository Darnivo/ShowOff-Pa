using UnityEngine;

public class ObjectThing : MonoBehaviour
{
    public Material[] state_Materials;
    private ObjectState _currState = ObjectState.UNDETECTED;


    void Update()
    {

    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Sight"))
        {
            _currState = ObjectState.DETECTED;
            switchMaterial();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Sight"))
        {
            _currState = ObjectState.UNDETECTED;
            switchMaterial();
        }
    }
    private void switchMaterial(){
        if (_currState == ObjectState.UNDETECTED)
        {
            GetComponent<Renderer>().material = state_Materials[0];
        }
        else if(_currState == ObjectState.DETECTED)
        {
            GetComponent<Renderer>().material = state_Materials[1];
        }

    }

    public enum ObjectState{
        UNDETECTED,
        DETECTED
    }
}
