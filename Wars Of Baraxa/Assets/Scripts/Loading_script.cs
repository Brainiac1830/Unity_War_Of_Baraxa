using UnityEngine;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Collections;

public class Loading_script : MonoBehaviour {
    public GUIStyle Background;
	// Use this for initialization
	void Start () {
	}
     public IEnumerator wait(int i)
    {
        yield return new WaitForSeconds(i);           
    }
    void OnApplicationQuit()
    {
        envoyerMessage("deconnection");
    }
	// Update is called once per frame
	void Update () {
            string message = lire();
            if (message == "Partie commencer")
                Application.LoadLevel("Board");
            envoyerMessage("oui");
	}
    void OnGUI()
    {
        GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "", Background);
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
