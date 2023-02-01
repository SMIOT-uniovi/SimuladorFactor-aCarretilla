using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;
using Random = UnityEngine.Random;


public class AvisoADAS : MonoBehaviour
{
    // enlaces con el gameManager
    private GameObject gameManager;
    private GameManager scriptGameManager;
    // enlace a la carretilla
    private GameObject carretilla;
    
    // variables para el registro del evento
    private float verticalInput;
    private bool obstaculoOculto =false;
    private DateTime tiempoActivacionADAS; 
    private String instanteEventoObstaculoOculto;
    private String instanteFrenada;
    private Vector3 posicionInicial;
    private Vector3 posicionFrenada;
    private float verticalInputInicial;
    private int tipoMensaje;
    
    private void OnTriggerEnter(Collider otro)
    {
        // si el objeto que llega es la carretilla
        if (otro.CompareTag("Carretilla"))
        {
            // lanzamos el aviso con el adas
            Debug.Log("Aviso de ADAS: "+name);
            //scriptGameManager.mensajeConductor("Adas activado");
            
            // genero valores entre 1 y 6 y luego le añado el modo
            tipoMensaje = Random.Range(1, 5)+10*ControladorAdas.modo;
            scriptGameManager.mensajeDeActivacionAlADAS(tipoMensaje);
            // activamos que hemos entrado en el área de un obstáculo oculto
            obstaculoOculto = true;
            // recojo el instante de tiempo y la posición de la carretilla
            tiempoActivacionADAS = System.DateTime.Now;
            instanteEventoObstaculoOculto = System.DateTime.Now.ToString("hh:mm:ss.fff");
            posicionInicial = carretilla.transform.position;
            // recojo la posición del acelerador
            verticalInputInicial = Input.GetAxis("Vertical");

        }

    }
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        // conectamos con el gameManager
        gameManager = GameObject.FindWithTag("GameManager");
        scriptGameManager =gameManager.GetComponent<GameManager>();
        // obtengo una referencia a la carretilla
        carretilla = GameObject.FindWithTag("Carretilla");
        // creo el tiempo que voy a utilizar para analizar los eventos
        tiempoActivacionADAS = new DateTime();
    }

    // Update is called once per frame
    void Update()
    {
        verticalInput = Input.GetAxis("Vertical");
        // si detectamos colisión/obstáculo oculto y dejamos de moverlos entonces tenemos que registrar los datos         
        if (obstaculoOculto)
        {
            // si dejamos de acelerar
            if (verticalInput == 0)
            {
                instanteFrenada =  System.DateTime.Now.ToString("hh:mm:ss.fff");
                posicionFrenada = carretilla.transform.position;
                // registramos el evento
                scriptGameManager.registroEvento("[Obstaculo-"+name.ToString()+"]["+instanteEventoObstaculoOculto+"]["+instanteFrenada+"]["+posicionInicial.x+"|"+posicionInicial.z+"]["+posicionFrenada.x+"|"+posicionFrenada.z+"]["+verticalInputInicial+"]["+tipoMensaje+"]");
                
                // salimos del estado obstáculo oculto
                obstaculoOculto = false;    
                scriptGameManager.mensajeConductor("");
                // avisamos al ADAS para que se desactive
                scriptGameManager.mensajeDeDesactivacionAlADAS();
            }
            // seguimos acelerando
            else
            {
                // compruebo si en 3 segundos no ha frenado
                int  segundosTranscurridos = (int)(System.DateTime.Now-tiempoActivacionADAS).TotalSeconds;
                if (segundosTranscurridos >= 3)
                {
                    // registramos el evento
                    scriptGameManager.registroEvento("[Obstaculo-"+name.ToString()+"]["+instanteEventoObstaculoOculto+"][NO]["+posicionInicial.x+"|"+posicionInicial.y+"][NO|NO]");
                    // salimos del estado obstáculo oculto
                    obstaculoOculto = false;
                    scriptGameManager.mensajeConductor("");
                    // avisamos al ADAS para que se desactive
                    scriptGameManager.mensajeDeDesactivacionAlADAS();
                }    
            }
                
        }
        
        
    }
    
    
    
    
    
}
