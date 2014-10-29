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
    string reponse="";
    bool appeler = false;
    // Use this for initialization
    void Start()
    {
    }
    public IEnumerator wait(int i)
    {
        yield return new WaitForSeconds(i);
    }
    void Awake()
    {
        envoyerMessage("trouver partie");
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
    //----------------------------TEESSTTTTTT--------------------------------------------------////////
    private void recevoirResultatNonBloquant()
    {
        try
        {
            StateObject state = new StateObject();
            state.workSocket = connexionServeur.sck;

            connexionServeur.sck.BeginReceive(state.buffer, 0, state.buffer.Length, 0, new AsyncCallback(ReceiveCallback), state);
        }
        catch (Exception e) { Console.WriteLine(e.ToString()); }
    }
    private void ReceiveCallback(IAsyncResult result)
    {
        StateObject state = (StateObject)result.AsyncState;
        Socket client = state.workSocket;
        int lenght = client.EndReceive(result);
        if (lenght > 0)
        {
            // There might be more data, so store the data received so far.
            state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, lenght));
            //  Get the rest of the data.
            client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(ReceiveCallback), state);
        }
        else
        {
            if (state.sb.Length > 1)
                reponse = state.sb.ToString();
            appeler = false;
        }
    }
    private string test()
    { 
        connexionServeur.sck.Blocking=false;
        System.Text.StringBuilder message = new System.Text.StringBuilder();
        int bytesRead=-1;
        while (bytesRead != 0)
        {
            byte c = 0;
            byte[] buff = new byte[connexionServeur.sck.SendBufferSize];
            SocketError err;
            // read a character.
            bytesRead = connexionServeur.sck.Receive(buff, 0, 1, SocketFlags.None, out err);
            // checking what happened
            if (SocketError.Success == err)
            {
                // read a byte!  Let's process it
                if (bytesRead > 0)
                {
                    // found a null character -- in this case it makes the end of a message.
                    if (c == 0)
                    {
                        // null terminated message received
                        break;
                    }
                    else
                        message.Append((char)c);
                }
            }
            else if (SocketError.WouldBlock != err)
            {
                break;

            }
        }
        connexionServeur.sck.Blocking=true;
        return message.ToString();
    }
}
