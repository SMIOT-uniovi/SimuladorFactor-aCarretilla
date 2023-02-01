using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;


public class ManagerEscenaInicial : MonoBehaviour
{
    
        // función para gestionar el botón que activa la simulación con tiras led

        public void GestorBotonSoloTiras()
        {
            // pongo el modo de funcionamiento del adas en 1
            ControladorAdas.modo = 1;
            Debug.Log("pulso el botón para arrancar la partida con ADAS -> tiras led");
            // tenemos que cargar la escena del juego
            SceneManager.LoadScene("EscenaFabrica");
        }
    
        // función para gestionar el botón que activa la simulación con tiras led y vibrador
    
        public void GestorBotonTirasYVibrador()
        {
            // pongo el modo de funcionamiento del adas en 2
            ControladorAdas.modo = 2;
            Debug.Log("pulso el botón para arrancar la partida con ADAS -> tiras led y vibrador");
        
            // tenemos que cargar la escena del juego
            SceneManager.LoadScene("EscenaFabrica");
        }
    
        // función para gestionar el botón que activa la simulación con tiras led, vibrador y matriz
    
        public void GestorBotonTirasVibradorMatriz()
        {
            // pongo el modo de funcionamiento del adas en 3
            ControladorAdas.modo = 3;
            Debug.Log("pulso el botón para arrancar la partida con ADAS -> tiras led, vibrador y matriz");
        
            // tenemos que cargar la escena del juego
            SceneManager.LoadScene("EscenaFabrica");

        }
    
    
    }


    

