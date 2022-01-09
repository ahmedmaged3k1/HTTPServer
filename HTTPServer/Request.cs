using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTTPServer
{
    public enum RequestMethod
    {
        GET,
        POST,
        HEAD
    }

    public enum HTTPVersion
    {
        HTTP10, 
        HTTP11,
        HTTP09
    }
    
    class Request
    {
        string[] requestLines;
         RequestMethod method;
        public string requestMethod; 
        public string relativeURI;
        Dictionary<string, string> headerLines = new Dictionary<string, string> { };
         public string contentLines;
        public string contentType;


        public Request(string requestString)
        {
            this.requestString = requestString;
        }
        public Dictionary<string, string> HeaderLines
        {
            get { return headerLines; }
        }
        public string http;
        public HTTPVersion httpVersion;
        public  string requestString;
        


        public bool ParseRequest()
        {
            string[] stringSeparators = new string[] { "\r\n" };
            requestLines = requestString.Split(stringSeparators, StringSplitOptions.None);
          


            LoadHeaderLines();
            
            if (requestLines.Length >= 3 )
            {
                
                if (ParseRequestLine()) {
                    LoadContentLines();
                    return true; }
            }

            return false;

        }

        private bool ParseRequestLine()
        {
            try
            {
                string[] stringSeparators = new string[] { " " };
                String[]  parts = requestLines[0].Split(stringSeparators, StringSplitOptions.None);
                

                if (parts[0].Trim() == "GET") { method = RequestMethod.GET; }
                else if (parts[0].Trim() == "POST") { method = RequestMethod.POST; }
                else if (parts[0].Trim() == "HEAD") { method = RequestMethod.HEAD; }
                requestMethod=parts[0].Trim();

                relativeURI = parts[1].Trim();
                string [] uri = relativeURI.Split('/');
                relativeURI = uri[1];
               


                if (parts[2].Trim().ToLower() == "http/1.1") { httpVersion = HTTPVersion.HTTP11; http = "HTTP/1.1"; }
                else if (parts[2].Trim().ToLower() == "http/1.0") { httpVersion = HTTPVersion.HTTP10; http = "HTTP/1.0"; }
                else if (parts[2].Trim().ToLower() == "http/0.9") { httpVersion = HTTPVersion.HTTP09; http = "HTTP/0.9"; }
                
                return true;
            }
            catch (Exception ex) {
                Logger.LogException(ex);

                return false; }
        }
        private bool LoadContentLines()
        {
            try
            {

               int  contentLineStartingIndex = requestLines.Length - 1;
                if (requestLines[contentLineStartingIndex] != null)
                {
                    requestLines[contentLineStartingIndex].Trim();
                    contentLines = requestLines[contentLineStartingIndex].Trim();
                    Console.WriteLine(contentLines);
                }
            }
            catch (Exception e)
            {
                Logger.LogException(e);
                return false;
            }
            return true;
        }



        private bool LoadHeaderLines()
        {
            try
            {

                for (int i = 1; i < requestLines.Length; i++)
                {
                    if (requestLines[i] == "") break;

                    string[] splitted = requestLines[i].Split(':');
                    if (splitted[0] == "content-type")
                    {
                        contentType = splitted[1];
                       // Console.WriteLine("Content type isss   :   "+contentType); 
                    }
                    headerLines.Add(splitted[0], splitted[1]);
                    

                }

            }
            catch
            {
                return false;
            }

            return true;
        }

    }
   

}
