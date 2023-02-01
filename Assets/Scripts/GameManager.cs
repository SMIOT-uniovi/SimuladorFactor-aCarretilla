using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Net.Sockets;
using System.Threading;
using TMPro;
using Random = UnityEngine.Random;
using System.Threading.Tasks;
using Assets.Scripts;

public class GameManager : MonoBehaviour
{
    
   
    // duración máxima de la simulación
    public int duracionSimulacion=300;
    
    // número de fase
    [Range(-1,10)]
    public int fase = -1;
    
    // variables para el inicio de fase
    private bool inicioDeFase=false;
    private float tiempoInicioDeFase;
    
    //enlaces al interfaz    
    public TextMeshProUGUI interfaceFase;
    public TextMeshProUGUI interfaceTitulo;
    public TextMeshProUGUI interfaceContador;

    // objetos y enlaces a la carretilla
    private Vector3 posicion;
    private GameObject carretilla;

    // dirección ip del ADAS
    public String ipADAS ="192.168.0.20";
    // puerto del ADAS
    public int puertoADAS = 65432;
    // socket con la raspberry - ADAS
    private TcpClient socketConnection;
    // variable para la activación de los mensajes al ADAS
    public bool activarADAS=false;
    //variable para definir el tipo de mensaje al ADAS
    private int tipoDeMensajeAlADAS;
    // variable para activar comunicación con el ADAS (0 nada, 1 activar, 2 desactivar)
    private int mensajeADAS=0;
    
    

    // fichero para guardar el registro de actividad
    private StreamWriter registroDeLog;
    // fichero para guardar errorer y trazas
    private StreamWriter registroDeError;
    

    public void mensajeConductor(String mensaje)
    {
        interfaceTitulo.text = mensaje;
    }
    
    
    // método para cambiar de fase
    public void cambioDeFase()
    {
        
        inicioDeFase = true;
        tiempoInicioDeFase = 10.0f;
        
        //aumentamos la fase
        fase++;
        interfaceFase.text="Fase: "+fase.ToString();
        interfaceTitulo.text = "¡Dirígete a la salida¡";
        // pongo la carretilla en la posición
        switch (fase)
        {
            case 0:
                posicion = new Vector3(0,0,0);
                break;
            case 1:
                posicion = new Vector3(82,0,12);
                break;
            case 2:
                posicion = new Vector3(-100,0,135);
                break;
            case 3:
                posicion = new Vector3(125,0,10);
                break;
            case 4:
                posicion = new Vector3(-145,0,-40);
                break;
            case 5:
                posicion = new Vector3(-80,0,120);
                break;
            case 6:
                posicion = new Vector3(72,0,98);
                break;

            default:
                posicion = new Vector3(0,0,0);
                break;
                
        }
        carretilla.transform.position=posicion;
        carretilla.GetComponent<MovimientoVehiculo>().speed = carretilla.GetComponent<MovimientoVehiculo>().speed / 30;


    }
    

    public void finalDePartida()
    {

        String instanteLlegada;
        
        interfaceTitulo.text = "¡Has terminado tu ejercicio, gracias por tu colaboración¡";
        interfaceFase.text = "";
        Debug.Log("Termino la simulación");
        instanteLlegada =  System.DateTime.Now.ToString("hh:mm:ss.fff");
        registroEvento("[Final][Fase: "+fase+"]["+instanteLlegada+"]");
        
        // paro el tiempo
        Time.timeScale = 0;
        
       // finalizo la partida
        Application.Quit();
    }

    
    // Start is called before the first frame update
    void Start()
    {

        // activar las pantallas delantera y trasera
        if (Display.displays.Length > 1)
            Display.displays[1].Activate();
        if (Display.displays.Length > 2)
            Display.displays[2].Activate();
        
        
        // consigo la referencia a la carretilla
        carretilla= GameObject.FindWithTag("Carretilla");
        
        cambioDeFase();
      
        // asigno el fichero al registro de log
        registroDeLog = new StreamWriter("RegistroDeLogSimuladorCarretilla-"+System.DateTime.Now.Year.ToString()+"-"+System.DateTime.Now.DayOfYear.ToString()+"-"+System.DateTime.Now.Hour.ToString()+"-"+System.DateTime.Now.Minute.ToString()+ ".txt", false);

        // asigno el fichero de registro de errores y eventos
        registroDeError = new StreamWriter("Error.txt",false);
        
        // activo la corrutina para controlar el fin de la simulación
        StartCoroutine(FinDeLaSimulacion());
        // activo la corrutina para controlar al ADAS
        StartCoroutine(ComunicacionConADAS());
                             
        
        // registro el evento de inicio de fase
        String instanteLlegada;
        instanteLlegada =  System.DateTime.Now.ToString("hh:mm:ss.fff");
        registroEvento("[ADAS][Modo: "+ControladorAdas.modo+"]");
        registroEvento("[Inicio][Fase: "+fase+"]["+instanteLlegada+"]");
    }

    // Update is called once per frame
    void Update()
    {
    
        
        // terminar el juego si pulso escape
        if (Input.GetKey("escape"))
           Application.Quit();

        if (inicioDeFase)
        {
            
            tiempoInicioDeFase = tiempoInicioDeFase - Time.deltaTime;
            interfaceContador.text =tiempoInicioDeFase.ToString("N0");
            if (tiempoInicioDeFase < 0)
            {
                inicioDeFase = false;
                interfaceContador.text = "";
                interfaceTitulo.text = "";
                carretilla.GetComponent<MovimientoVehiculo>().speed = carretilla.GetComponent<MovimientoVehiculo>().speed * 30;
                registroError("Aviso: terminando cuenta de inicio de fase");
            }
        }
    
       
    }

    // método para el registro de eventos para posterior análisis
    public void registroEvento(String mensaje)
    {
        registroDeLog.WriteLine(mensaje);
        registroDeLog.Flush();
        Debug.Log(mensaje);
    }

    public void registroError(String mensaje)
    {
        registroDeError.WriteLine(mensaje);
        registroDeError.Flush();
        Debug.Log(mensaje);
    }
    
    
    // método para la generación del mensajes al ADAS
    public void mensajeDeActivacionAlADAS(int tipo)
    {
        
        Debug.Log("activación del ADAS");
        registroError("Aviso: mandando mensaje de activación del adas");
        // activar el adas
        mensajeADAS = 1;
        // asigno el tipo de mensaje al adas
        tipoDeMensajeAlADAS = tipo;
        registroError("Aviso: cambiando adas a estado activar: "+tipo);
        
    }

    // método para la generación del mensajes al ADAS
    public void mensajeDeDesactivacionAlADAS()
    {
        
        Debug.Log("desactivación del ADAS");
        registroError("Aviso: mandando mensaje de desactivación del adas");
        // desactivar el adas
        mensajeADAS = 2;

    }
    
    
    // corrutina para comunicarse con el ADAS
    private IEnumerator ComunicacionConADAS()
    {

        while (true)
        {
            //registroError("Aviso: dentro de la corrutina, mensajeADAS: "+mensajeADAS);
            
        //--------------------- activo el ADAS o desactivo el ADAS -----------------------------
        if (mensajeADAS == 1)
        {
            if (activarADAS == true)
            {
                registroError("Aviso: estableciendo conexión para activar adas");
                // establezco la conexión con la raspberry del vehículo
                socketConnection = new TcpClient(ipADAS, puertoADAS);
                // si se estableció la conexión entonces envío el mensaje
                if (socketConnection.Connected)
                {
                    // cada cierto tiempo envío un aviso de un obstáculo oculto
                    using (NetworkStream stream = socketConnection.GetStream())
                    {
                        // mando un mensaje con el tipo que me pasan por parámetro
                        int posicionObstaculo = tipoDeMensajeAlADAS;
                        var mensaje = new byte[1];
                        mensaje[0] = (byte) posicionObstaculo;
                        stream.Write(mensaje, 0, 1);
                        //interfaceTitulo.text = "¡Activo ADAS¡";
                    }
                }    
            }
            mensajeADAS = 0;
        }
        else if (mensajeADAS == 2)
        {
            if (activarADAS == true)
            {
                registroError("Aviso: estableciendo conexión para desactivar adas");
                // establezco la conexión con la raspberry del vehículo
                socketConnection = new TcpClient("192.168.0.20", 65432);
                // si se estableció la conexión entonces envío el mensaje
                if (socketConnection.Connected)
                {
                    // cada cierto tiempo envío un aviso de un obstáculo oculto
                    using (NetworkStream stream = socketConnection.GetStream())
                    {
                        // mando un mensaje con el tipo que me pasan por parámetro
                        int posicionObstaculo = 5;
                        var mensaje = new byte[1];
                        mensaje[0] = (byte) posicionObstaculo;
                        stream.Write(mensaje, 0, 1);
                        //interfaceTitulo.text = "¡Desactivo ADAS¡";
                    }
                }
    
            }
            
            mensajeADAS = 0;
        }
        
        yield return new WaitForSeconds(0.2f);
            
        }
        
        
    }
    
    
    // corrutina para finalizar la simulación
    private IEnumerator FinDeLaSimulacion()
    {
        yield return new WaitForSeconds(duracionSimulacion);
        Debug.Log("Termino la simulación");
        finalDePartida();
    }

    
    
}
