using UnityEngine;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
public class SaveLoad 
{
    public static List<EstadisticasJugador> savedGames = new List<EstadisticasJugador>();

    //it's static so we can call it from anywhere 
    public static void Save()
    {
        SaveLoad.savedGames.Add(EstadisticasJugador.current);
        BinaryFormatter bf = new BinaryFormatter();
        //Application.persistentDataPath is a string, so if you wanted you can put that into debug.log if you want to know where save games are located 
        FileStream file = File.Create(Application.persistentDataPath + "/savedStatistics.gd"); //you can call it anything you want 
        bf.Serialize(file, SaveLoad.savedGames);
        file.Close();
        SaveLoad.savedGames[0] = EstadisticasJugador.current;
    }

    public static void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/savedStatistics.gd"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/savedStatistics.gd", FileMode.Open);
            SaveLoad.savedGames = (List<EstadisticasJugador>)bf.Deserialize(file);
            file.Close();
            EstadisticasJugador.current = SaveLoad.savedGames[0];
        }
    }
}
