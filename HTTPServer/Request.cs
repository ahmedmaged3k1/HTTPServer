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
        public string relativeURI;
        Dictionary<string, string> headerLines = new Dictionary<string, string> { };


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
        string[] contentLines;


        public bool ParseRequest()
        {
            string[] stringSeparators = new string[] { "\r\n" };
            requestLines = requestString.Split(stringSeparators, StringSplitOptions.None);
            Console.WriteLine("HEELLLOO  " + requestLines.Length);
            Console.WriteLine("HEELLLOO  " + requestLines[0]);


            LoadHeaderLines();
            
            if (requestLines.Length >= 3 )
            {
                bool isParseRequestLine = ParseRequestLine();
                if (isParseRequestLine ) {

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
                Console.WriteLine("HEELLLOO  "+ parts[0]);


                if (parts[0].Trim() == "GET") { method = RequestMethod.GET; }
                else if (parts[0].Trim() == "POST") { method = RequestMethod.POST; }
                else if (parts[0].Trim() == "HEAD") { method = RequestMethod.HEAD; }


                relativeURI = parts[1].Trim();
                string [] uri = relativeURI.Split('/');
                relativeURI = uri[1];
                Console.WriteLine("thee relativee uri"+relativeURI);
                
              
                Console.WriteLine("Triiimmmmm" + parts[2].Trim());
                if (parts[2].Trim().ToLower() == "http/1.1") { httpVersion = HTTPVersion.HTTP11; http = "HTTP/1.1"; }
                else if (parts[2].Trim().ToLower() == "http/1.0") { httpVersion = HTTPVersion.HTTP10; http = "HTTP/1.0"; }
                else if (parts[2].Trim().ToLower() == "http/0.9") { httpVersion = HTTPVersion.HTTP09; http = "HTTP/0.9"; }
                
                return true;
            }
            catch (Exception ex) { return false; }
        }

 
   
        private bool LoadHeaderLines()
        {
            try
            {

                for (int i = 1; i < requestLines.Length; i++)
                {
                    if (requestLines[i] == "") break;

                    string[] splitted = requestLines[i].Split(':');
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
