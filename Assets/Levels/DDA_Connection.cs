
using System;
using System.Globalization;
using System.Net.Sockets;
using System.Text;
using UnityEngine;


public class DDA_Connection : MonoBehaviour
{
    public static DDA_Connection Instance;

    [HideInInspector]
    public bool isConnected = false;

    private TcpClient client;
    private NetworkStream stream;


    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public bool Connect()
    {
        try
        {
            if (client != null && client.Connected)
            {
                return true;
            }

            client = new TcpClient("127.0.0.1", 5005);
            stream = client.GetStream();

            return true;
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            return false;
        }
    }

    public bool Disconnect()
    {
        try
        {
            if (stream != null)
            {
                SendRaw("exit");
                stream.Close();
            }

            if (client != null)
            {
                client.Close();
            }

            return true;
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);

            return false;
        }
    }

    public void Send()
    {

    }

    public int SendState(float time, float emaTime, float deltaTime, float platDeath, 
        float emaPlatDeath, float DeltaPlatDeath, int prevActPlat, float diffPlat, float enemyDeath,
        float emaEnemyDeath, float DeltaEnemyDeath, int prevActEnemy, float diffEnemy, float done, 
        int prevAct, int act, float timePrev, float emaTimePrev, float deltaTimePrev, float platDeathPrev,
        float emaPlatDeathPrev, float DeltaPlatDeathPrev, int prevActPlatPrev, float diffPlatPrev,
        float enemyDeathPrev, float emaEnemyDeathPrev, float DeltaEnemyDeathPrev, int prevActEnemyPrev, 
        float diffEnemyPrev)
    {
        string message = string.Format(CultureInfo.InvariantCulture,
            "{0}_{1}_{2}_{3}_{4}_{5}_{6}_{7}_{8}_{9}_{10}_{11}_{12}_{13}_{14}_{15}_{16}_{17}_{18}_{19}_{20}_{21}_{22}_{23}_{24}_{25}_{26}_{27}_{28}",
            time,         
            emaTime,      
            deltaTime,      
            platDeath,       
            emaPlatDeath,     
            DeltaPlatDeath,  
            prevActPlat,      
            diffPlat,          
            enemyDeath,       
            emaEnemyDeath,    
            DeltaEnemyDeath,   
            prevActEnemy,     
            diffEnemy,     
            done,           
            prevAct,          
            act,               
            timePrev,          
            emaTimePrev,        
            deltaTimePrev,    
            platDeathPrev,    
            emaPlatDeathPrev,  
            DeltaPlatDeathPrev, 
            prevActPlatPrev,   
            diffPlatPrev,      
            enemyDeathPrev,    
            emaEnemyDeathPrev, 
            DeltaEnemyDeathPrev,
            prevActEnemyPrev,   
            diffEnemyPrev    
        );

        return SendRaw(message);
    }

    private int SendRaw(string message)
    {
        try
        {
            if (client == null || !client.Connected)
            {
                Debug.LogWarning("There is no connection to Python.");
                return -1;
            }

            byte[] data = Encoding.UTF8.GetBytes(message);
            stream.Write(data, 0, data.Length);

            byte[] buffer = new byte[256];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            string response = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();

            if (response == "Error")
            {
                Debug.LogWarning("Python returned an error.");
            }

            return int.Parse(response);
        }
        catch (Exception e)
        {
            Debug.LogError("Error when sending a message: " + e.Message);
        }

        return -1;
    }

    void OnApplicationQuit()
    {
        Disconnect();
    }
}
