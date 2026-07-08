using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogTextData", menuName = "Scriptable Objects/DialogTextData")]
public class DialogTextData : ScriptableObject
{
    [SerializeField] List<List<(Func<bool>,string)>> ListOfTexts;
    [SerializeField] List<List<(Func<bool>, string)>> repetitionTexts;

    //¿Como funciona este data?
    //Es una lista anidada que tiene todos los dialogos de una instancia de personaje
    //Por ejemplo, la primera vez que te encuentras a un personaje tendrá un mensaje de presentación,
    //Que no tendrá otras veces que te lo encuentres
    //En esa instancia de personaje la primera vez que hables con él el dialogo será
    //La primera lista de strings, la segunda vez la segunda...
    //Si hablas con él un numero de veces mayor al tamaño de la lista,
    //se escogerá aleatoriamente un dialogo de la lista repetitionTexts

    //Dentro de una lista de strings, cada string ocupa un cuadro de dialogo entero. 
    //Hablas con el personaje y se muestra list[0], le das a botón de confirmar entonce se muestra list[1]


    //Se podría hacer que según si has hecho algo tengan un dialogo adicional, vease tienes una flor desbloqueada
}
