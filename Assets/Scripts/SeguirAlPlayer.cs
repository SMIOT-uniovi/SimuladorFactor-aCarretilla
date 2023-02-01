using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeguirAlPlayer : MonoBehaviour
{
    
    public Vector3 offset = new Vector3(0, 5, 0);
    public Vector3 offAngulo = new Vector3(20,270,0);

    // objeto al que voy a seguir
    public GameObject player;
    
    // Start is called before the first frame update
    void Start()
    {
        //this.transform.rotation = player.transform.rotation;
        Quaternion offRotation = new Quaternion();
        offRotation.eulerAngles = offAngulo;
        this.transform.rotation= player.transform.rotation * offRotation;
    }

    // Update is called once per frame
    void Update()
    {
        //Me pongo en la posición del player
        this.transform.position = player.transform.position+offset;
        //this.transform.rotation = player.transform.rotation;
        Quaternion offRotation = new Quaternion();
        offRotation.eulerAngles = offAngulo;
        this.transform.rotation= player.transform.rotation * offRotation;
        
       
    }
}
