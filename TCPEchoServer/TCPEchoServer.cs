/*
 * TCPEchoServer
 *
 * Author Michael Claudius, ZIBAT Computer Science
 * Version 1.0. 2014.02.10
 * Copyright 2014 by Michael Claudius
 * Revised 2014.09.01, 2017.09.01
 * All rights reserved
 */


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace TCPEchoServer
{
    class TCPEchoServer
    {

        public static void Main(string[] args)
        {
            string serverCertificateFile = "C:/Certificates/ServerSSL.cer";
            bool clientCertificateRequired = false;
            bool checkCertificateRevocation = true;
            SslProtocols enabledSSLProtocols = SslProtocols.Tls;

            X509Certificate serverCertificate =
                new X509Certificate(serverCertificateFile, "mysecret");


            IPAddress ip = IPAddress.Parse("127.0.0.1");

            TcpListener serverSocket = new TcpListener(ip, 6789);
            serverSocket.Start();
            Console.WriteLine("Server started");

            TcpClient connectionSocket = serverSocket.AcceptTcpClient();
            //Socket connectionSocket = serverSocket.AcceptSocket();
            Console.WriteLine("Server activated");

            Stream unsecureStream = connectionSocket.GetStream();
            bool leaveInnerStreamOpen = false;
            SslStream sslStream = new SslStream(unsecureStream, leaveInnerStreamOpen);
            sslStream.AuthenticateAsServer(serverCertificate, clientCertificateRequired,

                enabledSSLProtocols, checkCertificateRevocation);
            // Stream ns = connectionSocket.GetStream();
           // Stream ns = new NetworkStream(connectionSocket);

            StreamReader sr = new StreamReader(sslStream);
            StreamWriter sw = new StreamWriter(sslStream);
            sw.AutoFlush = true; // enable automatic flushing

            string message = sr.ReadLine();
            string answer = "";
            while (message != null && message != "")
            {

                Console.WriteLine("Client: " + message);
                answer = message.ToUpper();
                sw.WriteLine(answer);
                message = sr.ReadLine();

            }

            sslStream.Close();
            connectionSocket.Close();
            serverSocket.Stop();

        }
    }
    
}
