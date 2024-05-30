using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    //Data Acquired from SQL Database and Stored Locally
    
    //Player Names
    public static string username;

    //Player Weapons 
    public static int upgrade1; //Semi-Auto Upgrade
    public static int upgrade2; //Shotgun Upgrade
    public static int upgrade3; //ChaingGun Upgrade
    public static int scrap; 

    //team number
    public static int team;

    public static bool LoggedIn { get { return username != null; } }

    public static void LogOut()
    {
        username = null;
        upgrade1 = 0;
        upgrade2 = 0;
        upgrade3 = 0;
        scrap = 0;
        team = -1;
    }


}
