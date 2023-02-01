using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LlegadaPuerta : MonoBehaviour
{
    
    private GameObject gameManager;
    private GameManager scriptGameManager;
    
    private String instanteLlegada;
    
    private void OnTriggerEnter(Collider otro)
    {
        // si el objeto que llega es la carretilla
        if (otro.CompareTag("Carretilla"))
        {
            //registro el evento
            instanteLlegada =  System.DateTime.Now.ToString("hh:mm:ss.fff");
            scriptGameManager.registroEvento("[LlegadaPuerta][Fase: "+scriptGameManager.fase+"]["+instanteLlegada+"]");
            // cambiamos de fase
            scriptGameManager.cambioDeFase();    
          
            
        }

    }


    // Start is called before the first frame update
    void Start()
    {
        
        gameManager = GameObject.FindWithTag("GameManager");
        scriptGameManager =gameManager.GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
