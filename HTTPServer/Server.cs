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
        bool isNotBadErorr;


        public Server(int portNumber, string redirectionMatrixPath)
        {
            //TODO: call this.LoadRedirectionRules passing redirectionMatrixPath to it
            this.redirectionMatrixPath=redirectionMatrixPath;
            //TODO: initialize this.serverSocket
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Any, portNumber);
             serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(iPEndPoint);
           
        }

        public void StartServer()
        {
            serverSocket.Listen(2);
            while (true)
            {
                //Accept a client Socket (will block until a client connects)
                Socket clientSocket = this.serverSocket.Accept();
                //RemoteEndPoint Gets the IP address and Port number of the client
                Console.WriteLine("New client accepted:" + clientSocket.RemoteEndPoint);
                //Create a thread that works on ClientConnection.HandleConnection 	method
                Thread newthread = new Thread(new ParameterizedThreadStart
        (HandleConnection));
                //Start the thread
                newthread.Start(clientSocket);
            }

            }

            public void HandleConnection(object obj)
        {
            // TODO: Create client socket 
            Socket clientSock = (Socket)obj;
            // set client socket ReceiveTimeout = 0 to indicate an infinite time-out period
            clientSock.ReceiveTimeout = 0; 
            //Send an initial message to the client 
            string welcome = "Welcome to my http server";
            byte[] data = Encoding.ASCII.GetBytes(welcome);
            //clientSock.Send(data);
            // TODO: receive requests in while true until remote client closes the socket.
            int len = 1; 
            while (true)
            {
                try
                {
                    Console.WriteLine("Welcome to my http server");
                    //Bad Request 
                    // TODO: Receive request
                    data = new byte[1024];
                    len = clientSock.Receive(data);
                    // string header = Encoding.ASCII.GetString(data).Split('\n');
                    Console.WriteLine("Well Formated "+ Encoding.ASCII.GetString(data));
                    string[] stringSeparators = new string[] { "\n" };
                    string []header  = Encoding.ASCII.GetString(data).Split(stringSeparators, StringSplitOptions.None);
                    Console.WriteLine("lennn Headdeerrr : "+ header[0]);
                   
                    // TODO: break the while loop if receivedLen==0
                     if (len == 0)
                     {
                         Console.WriteLine("Client: {0}  ended the connection", clientSock.RemoteEndPoint);
                      break;
                     }
                    // TODO: Create a Request object using received request string
                    // Request request = new Request("GET aboutus2.html http/1.1");
                    Request request = new Request(header[0]);



                    Debug.WriteLine(request.requestString);
                    // TODO: Call HandleRequest Method that returns the response
                    Response response = HandleRequest(request);
                    // TODO: Send Response back to client
                    clientSock.Send(Encoding.ASCII.GetBytes(response.ResponseString), 0, response.ResponseString.Length, SocketFlags.None);
                   
                }
                catch (Exception ex)
                {
                    // TODO: log exception using Logger class
                    Logger.LogException(ex);
                }
                break;

            }

            // TODO: close client socket
            clientSock.Close();

        }

        Response HandleRequest(Request request)
        {
           
            string content;
            
            try
            {
                
                LoadRedirectionRules(@"D:\Desktop\Projects\Project Data\Template[2021-2022]\HTTPServer\bin\redirectionRules.txt");

                try
                {
                    request.ParseRequest();
                    Console.WriteLine(request.ParseRequest());
                }
                catch(Exception ex)
                {
                    Logger.LogException(ex);
                    request.relativeURI = Configuration.BadRequestDefaultPageName;                              
                }
                //TODO: map the relativeURI in request to get the physical path of the resource.
                String physicalPath = Configuration.RootPath + "\\" + request.relativeURI;
                Console.WriteLine("Physical path : "+physicalPath);


                //TODO: check for redirect
                String redirectedPhysicalPath =  GetRedirectionPagePathIFExist(request.relativeURI);

                // string checkForBadRequest = request.relativeURI;
                 isNotServerErorr = request.relativeURI.Contains(".html");
                Console.WriteLine("el bad a5baro eh " + isNotServerErorr);

                //Match match = Regex.Match(request.relativeURI, regex ,RegexOptions.IgnoreCase);
               
                Console.WriteLine("Redirected Physical Path + "+ redirectedPhysicalPath);



                //TODO: check file exists
               // StreamWriter streamWriter = new StreamWriter(File.Open(physicalPath, FileMode.Open));
                //streamWriter.Close();
                //TODO: read the physical file
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

                // Create OK response
                Console.WriteLine("el cooodeee   " + code);
                return new Response(request.http, code, "text/html",content,redirectedPhysicalPath);

            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Logger.LogException(ex);
                //TODO: check for bad request  Not Found 404 
                if (isNotServerErorr)
                {
                    request.relativeURI = Configuration.NotFoundDefaultPageName;

                    code = StatusCode.NotFound;
                    byte[] fileData1 = new byte[1000];
                    fileData1 = File.ReadAllBytes(Configuration.RootPath + "\\" + request.relativeURI);
                    content = Encoding.ASCII.GetString(fileData1).Trim();
                    Console.WriteLine("File Content " + content);

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

                    return new Response(request.http, code, "text/html", content, "");
                }
                // TODO: in case of exception, return Internal Server Error. 

                request.relativeURI = Configuration.InternalErrorDefaultPageName;
                
                code = StatusCode.InternalServerError;
                byte[] fileData = new byte[1000];
                fileData = File.ReadAllBytes(Configuration.RootPath + "\\" + request.relativeURI);
                content = Encoding.ASCII.GetString(fileData).Trim();
                Console.WriteLine("File Content " + content);

                return new Response(request.http, code, "text/html", content, "");

            }
        }

        private string GetRedirectionPagePathIFExist(string relativePath)
        {
            // using Configuration.RedirectionRules return the redirected page path if exists else returns empty
            try
            {
                //Console.WriteLine("helloooo  " + Configuration.RedirectionRules["aboutus.html"]);
               // Console.WriteLine("helloooo  ela " + relativePath);


                return Configuration.RedirectionRules[relativePath];
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                return string.Empty;
            }

            
        }

        private string LoadDefaultPage(string defaultPageName)
        {
            string filePath = Path.Combine(Configuration.RootPath, defaultPageName);
            // TODO: check if filepath not exist log exception using Logger class and return empty string
            try
            {
                // else read file and return its content
                String fileContent = File.ReadAllText(filePath);
                return fileContent;
            }
            catch (Exception ex)
            {
                //if filepath not exist log exception using Logger class and return empty string
                Logger.LogException(ex);
                return string.Empty;
            }
           
        }

        private void LoadRedirectionRules(string filePath)
        {
            try
            {

                // TODO: using the filepath paramter read the redirection rules from file 
                String[] fileData = File.ReadAllLines(filePath);
                
                foreach (String elem in fileData)
                {
                    
                    String[] redrectString = elem.Split(',');
                    // then fill Configuration.RedirectionRules dictionary 
                    Configuration.RedirectionRules = new Dictionary<string, string>
                    {
                        { redrectString[0], redrectString[1] }
                    };
                   // Console.WriteLine("helloooo  " + Configuration.RedirectionRules["aboutus.html"]);
                    
                    // Configuration.RedirectionRules = new Dictionary<string, string>; 
                    // Configuration.RedirectionRules.Add("aboutus.html", "aboutus2.html");
                }

            }

            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Logger.LogException(ex);
                Environment.Exit(1);
            }
        }
    }
}
