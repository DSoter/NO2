using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class DialogLine
{
    public string conditionKey; //Referencia para la funcion desde un diccionario
    public string text;
}

[Serializable]
public class DialogSequence
{
    public List<DialogLine> lines;
}

[CreateAssetMenu(fileName = "DialogTextData", menuName = "Scriptable Objects/DialogTextData")]
public class DialogTextData : ScriptableObject
{
    [SerializeField] private List<DialogSequence> listOfTexts;
    [SerializeField] private List<DialogSequence> repetitionTexts;

    public List<DialogSequence> ListOfTexts => listOfTexts;
    public List<DialogSequence> RepetitionTexts => repetitionTexts;
}


    //¿Como funciona este data?
    //Es una lista anidada que tiene todos los dialogos de una instancia de personaje
    //Por ejemplo, la primera vez que te encuentras a un personaje tendrá un mensaje de presentación,
    //Que no tendrá otras veces que te lo encuentres
    //En esa instancia de personaje la primera vez que hables con él el dialogo será
    //La primera lista de strings, la segunda vez la segunda...
    //Si hablas con él un numero de veces mayor al tamaño de la lista,
    //se escogerá aleatoriamente un dialogo de la lista repetitionTexts

    //Dentro de una lista de strings, cada string ocupa un cuadro de dialogo entero. 
    //Hablas con el personaje y se muestra list[0].item2, le das a botón de confirmar entonce se muestra list[1].item2


    //En cada tupla el item1 es una función ubicada en otro script, antes de mostrar el item2 de esa posicion se debe de comprobar si la funcion devuelve true,
    //si lo devuelve se muestra, si no se pasa al siguiente y se vuelve a comprobar

