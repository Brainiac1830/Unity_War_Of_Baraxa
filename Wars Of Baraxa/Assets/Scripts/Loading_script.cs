using UnityEngine;
using System;
using System.Collections.Specialized;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Threading;
using warsofbaraxa;

public class Loading_script : MonoBehaviour
{
    public GUIStyle Background;
    public Texture gear;
    string reponse="";
    bool appeler = false;
    // Use this for initialization
    void Start()
    {
    }
    public IEnumerator wait(float i)
    {
        yield return new WaitForSeconds(i);
    }
    void Awake()
    {
    }
    void OnApplicationQuit()
    {
        envoyerMessage("deconnection");
    }
    // Update is called once per frame
    void Update()
    {
        string message = lire();
        if (message == "Partie Commencer")
        {
            Application.LoadLevel("Board");
        }

        envoyerMessage("oui");
    }
    void OnGUI()
    {
        GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "", Background);
        GUI.Label(new Rect(Screen.width * 0.37f, Screen.height * 0.82f, Screen.width * 0.3f, Screen.height * 0.2f), "<color=white><size=60>Loading</size></color>");
    }
    private void envoyerMessage(string message)
    {
        byte[] data = Encoding.ASCII.GetBytes(message);
        connexionServeur.sck.Send(data);
    }
    private string lire()
    {
        string message = null;
        do
        {
            message = recevoirResultat();
        } while (message == null);
        return message;
    }
    private string recevoirResultat()
    {
        byte[] buff = new byte[connexionServeur.sck.SendBufferSize];
        int bytesRead = connexionServeur.sck.Receive(buff);
        byte[] formatted = new byte[bytesRead];
        for (int i = 0; i < bytesRead; i++)
        {
            formatted[i] = buff[i];
        }
        string strData = Encoding.ASCII.GetString(formatted);
        return strData;
    }
}
