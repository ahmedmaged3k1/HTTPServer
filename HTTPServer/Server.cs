using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;
namespace HTTPServer
{
    class Server
    {
        string redirectionMatrixPath;
        Socket serverSocket;
        StatusCode code;
        bool isNotServerErorr;       


        public Server(int portNumber, string redirectionMatrixPath)
        {

            this.redirectionMatrixPath=redirectionMatrixPath;

            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Any, portNumber);
             serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(iPEndPoint);
            Console.WriteLine("Server is Runing on Port : " + portNumber);
               
        }

        public void StartServer()
        {
            
            serverSocket.Listen(2);

            while (true)
            {
               
                Socket clientSocket = this.serverSocket.Accept();
               
                Console.WriteLine("New client accepted : " + clientSocket.RemoteEndPoint);
           
                Thread newthread = new Thread(new ParameterizedThreadStart
        (HandleConnection));
           
                newthread.Start(clientSocket);
            }

            }

            public void HandleConnection(object obj)
        {
       
            Socket clientSock = (Socket)obj;
            clientSock.ReceiveTimeout = 0; 
            byte[] data = new byte[1024];
            int len = 1; 
            while (true)
            {
                try
                {   
                    len = clientSock.Receive(data);
               
                     if (len == 0)
                     {
                         Console.WriteLine("Client: {0}  ended the connection ", clientSock.RemoteEndPoint);
                      break;
                     }
                    string requestString = Encoding.ASCII.GetString(data);
                    Console.WriteLine(requestString);
                    Request request = new Request(requestString);
                    Response response = HandleRequest(request);
                  
                    clientSock.Send(Encoding.ASCII.GetBytes(response.ResponseString), 0, response.ResponseString.Length, SocketFlags.None);
                   
                }
                catch (Exception ex)
                {
                    
                    Logger.LogException(ex);
                }
                break;

            }

            clientSock.Close();

        }

        Response HandleRequest(Request request)
        {
           
            string content;
            bool isHead = false;

            bool isPost = false;

            try
            {
                LoadRedirectionRules(redirectionMatrixPath);
                try
                {
                    request.ParseRequest();
                    if (request.requestMethod.ToLower() == "post")
                    {
                      isPost = true;
                    }
                    else if (request.requestMethod.ToLower() == "head")
                    {
                       isHead = true;
                    }
                }
                catch(Exception ex)
                {
                    Logger.LogException(ex);
                    request.relativeURI = Configuration.InternalErrorDefaultPageName;
                    code = StatusCode.InternalServerError;
                    byte[] fileDataa = new byte[1000];
                    fileDataa = File.ReadAllBytes(Configuration.RootPath + "\\" + request.relativeURI);
                    content = Encoding.ASCII.GetString(fileDataa).Trim();
                    Console.WriteLine("File Content " + content);
                    if (isHead)
                    {
                        content = "";
                        return new Response(request.http, code, "text/html", content, "");
                    }
                    else if (isPost)
                    {
                        string requestBody = request.contentLines.Trim();
                        
                        content = "The User Sent The Following Data : " + requestBody;

                        return new Response(request.http, code, request.contentType, content, "");
                    }
                    return new Response(request.http, code, "text/html", content, "");

                }

                String physicalPath = Configuration.RootPath + "\\" + request.relativeURI;
                Console.WriteLine("Physical path : "+physicalPath);
                String redirectedPhysicalPath =  GetRedirectionPagePathIFExist(request.relativeURI);
                 isNotServerErorr = request.relativeURI.Contains(".html");
               // Console.WriteLine("el bad a5baro eh " + isNotServerErorr);
              //  Console.WriteLine("Redirected Physical Path + "+ redirectedPhysicalPath);


                byte[] fileData = new byte[1000];
                if (redirectedPhysicalPath != "") {
                    code = StatusCode.Redirect;
                    fileData = File.ReadAllBytes(Configuration.RootPath + "\\" + redirectedPhysicalPath);

                }
              
               
                else {
                    code = StatusCode.OK;

                    fileData = File.ReadAllBytes(physicalPath);
                }
                content = Encoding.ASCII.GetString(fileData).Trim();
                Console.WriteLine("File Content "+content);


                //  Console.WriteLine("el cooodeee   " + code);
                if (isHead)
                {
                    content = "";
                    return new Response(request.http, code, "text/html", content, "");
                }
                else if (isPost)
                {

                    string requestBody = request.contentLines.Trim();
                   
                    content = "The User Sent The Following Data : " + requestBody;

                    return new Response(request.http, code, request.contentType, content, "");
                }


                return new Response(request.http, code, "text/html",content,redirectedPhysicalPath);

            }
            catch (Exception ex)
            {
               
                Logger.LogException(ex);
                
                if (isNotServerErorr)
                {
                    request.relativeURI = Configuration.NotFoundDefaultPageName;

                    code = StatusCode.NotFound;
                    byte[] fileData1 = new byte[1000];
                    fileData1 = File.ReadAllBytes(Configuration.RootPath + "\\" + request.relativeURI);
                    content = Encoding.ASCII.GetString(fileData1).Trim();
                    Console.WriteLine("File Content " + content);
                    if (isHead)
                    {
                        content = "";
                        return new Response(request.http, code, "text/html", content, "");
                    }
                    else if (isPost)
                    {
                        string requestBody = request.contentLines.Trim();
                        
                        content = "The User Sent The Following Data : " + requestBody;

                        return new Response(request.http, code, request.contentType, content, "");
                    }
                    return new Response(request.http, code, "text/html", content, "");

                }
                else
                {
                   
                    request.relativeURI = Configuration.BadRequestDefaultPageName;

                    code = StatusCode.BadRequest;
                    byte[] fileData1 = new byte[1000];
                    fileData1 = File.ReadAllBytes(Configuration.RootPath + "\\" + request.relativeURI);
                    content = Encoding.ASCII.GetString(fileData1).Trim();
                    Console.WriteLine("File Content " + content);
                    if (isHead)
                    {
                        content = "";
                        return new Response(request.http, code, "text/html", content, "");
                    }
                    else if (isPost)
                    {
                        string requestBody = request.contentLines.Trim();
                        
                        content = "The User Sent The Following Data : " + requestBody;

                        return new Response(request.http, code, request.contentType, content, "");
                    }
                    return new Response(request.http, code, "text/html", content, "");
                }
              

            }
        }

        private string GetRedirectionPagePathIFExist(string relativePath)
        {
            
            try
            {
                return Configuration.RedirectionRules[relativePath];
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                return string.Empty;
            }

            
        }

  

        private void LoadRedirectionRules(string filePath)
        {
            try
            {

               
                String[] fileData = File.ReadAllLines(filePath);
                
                foreach (String elem in fileData)
                {
                    
                    String[] redrectString = elem.Split(',');
                    Configuration.RedirectionRules = new Dictionary<string, string>
                    {
                        { redrectString[0], redrectString[1] }
                    };
              
                }

            }

            catch (Exception ex)
            {
               
                Logger.LogException(ex);
                Environment.Exit(1);
            }
        }
    }
}
