using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Animations;

public class ComprobarColision : MonoBehaviour
{

    public GameObject destino;
    private Vector3 posicion;
    
    Quaternion offRotation = new Quaternion();
    private Vector3 rotacionDerecha = new Vector3(0,0,0);
    private Vector3 rotacionIzquierda =new Vector3(0,180,0);

    
    private void OnCollisionEnter(Collision otro)
    {
        // termino el juego
        if (otro.collider.CompareTag("Destino"))
        {
            Debug.Log("Llego al destino y genero uno nuevo");
            
            // borro el destino actual
            Destroy(otro.collider.gameObject);
            
            // defino un nuevo destino aleatoriamente
            
            int posicionZ = Random.Range(1, 6);
            int posicionX = Random.Range(0,2);
        
            if (posicionX!=0)
            {
                //determino la posición
                posicion = new Vector3(-75, -1, -20*posicionZ);
                // determino la rotación
                offRotation.eulerAngles = rotacionDerecha;
                // creo el destino para la navegación de la carretilla
                Instantiate(destino, posicion, destino.transform.rotation);
                Debug.Log("Situación del destino"+destino.transform.rotation.ToString());
            }
            else
            {
                // determino la posición
                posicion = new Vector3(0, -1, -20*posicionZ);
                // determino la rotación
                offRotation.eulerAngles = rotacionIzquierda;
                // creo el destino para la navegación de la carretilla
                Instantiate(destino, posicion, offRotation);
                Debug.Log("Situación del destino"+destino.transform.rotation.ToString());
            }

                
            
        }
        

    }
    
    
}
