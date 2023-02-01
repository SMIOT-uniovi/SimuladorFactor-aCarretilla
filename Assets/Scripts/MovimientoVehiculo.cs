using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovimientoVehiculo : MonoBehaviour
{
    
    // velocidad del objeto
    [Range(0,20), TooltipAttribute("Velocidad lineal máxima del coche")]
    public float speed = 7f;
    
    [Range(0,220), TooltipAttribute("Velocidad de giro máxima del coche")]
    public float turnSpeed = 45f;
    
    private float horizontalInput, verticalInput;

    public int limitPosX, limitNegX, limitPosZ, limitNegZ;

    // gestión del sonido
    public AudioClip forkliftSound;
    private AudioSource _audioSource;
    

    // Start is called before the first frame update
    void Start()
    {
        _audioSource =GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        
       // activamos el movimiento rectilineo hacia adelante
           this.transform.Translate(speed * Time.deltaTime * Vector3.left*verticalInput);
           this.transform.Rotate(turnSpeed* Time.deltaTime*Vector3.up*horizontalInput);
           
           if ((verticalInput != 0) || horizontalInput!=0)
           {
               _audioSource.PlayOneShot(forkliftSound, 0.2f);     
           }
           
           
           // Definimos los límites en los que se va a mover la carretilla
       if (transform.position.x > limitPosX)
           transform.position = new Vector3(limitPosX, transform.position.y, transform.position.z);
       if (transform.position.x < limitNegX)
           transform.position = new Vector3(limitNegX, transform.position.y, transform.position.z);
       if (transform.position.z < limitNegZ)
           transform.position = new Vector3(transform.position.x, transform.position.y, limitNegZ);
       if (transform.position.z > limitPosZ)
           transform.position = new Vector3(transform.position.x, transform.position.y, limitPosZ);
    }
}
