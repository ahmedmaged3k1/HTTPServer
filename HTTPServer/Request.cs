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
        HTTP10, // HTTP/1.0
        HTTP11,
        HTTP09
    }
    
    class Request
    {
        string[] requestLines;
        RequestMethod method;
        public string relativeURI;
        Dictionary<string, string> headerLines;


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

        /// <summary>
        /// Parses the request string and loads the request line, header lines and content, returns false if there is a parsing error
        /// </summary>
        /// <returns>True if parsing succeeds, false otherwise.</returns>
        public bool ParseRequest()
        {
            


            //TODO: parse the receivedRequest using the \r\n delimeter   

            //requestString = requestString.Replace("\r\n", "\n");
            requestLines = requestString.Split(' ');
          

            bool isBlanckLine = ValidateBlankLine();
            

            // check that there is atleast 3 lines: Request line, Host Header, Blank line (usually 4 lines with the last empty line for empty content)

            if (requestLines.Length >= 3 && isBlanckLine == true)
            {
                //ValidateIsURI(relativeURI);
                // Parse Request line
                bool isParseRequestLine = ParseRequestLine();
               // bool isLoadHeaderLines = LoadHeaderLines();

                if (isParseRequestLine ) {
                    

                    return true; }
            }

            return false;


            

        }

        private bool ParseRequestLine()
        {
            try
            {
                //String[] parts = requestLines[0].Split(' ');
                String[] parts = requestLines;

                if (parts[0].Trim() == "GET") { method = RequestMethod.GET; }
                else if (parts[0].Trim() == "POST") { method = RequestMethod.POST; }
                else if (parts[0].Trim() == "HEAD") { method = RequestMethod.HEAD; }


                relativeURI = parts[1].Trim();
                string [] uri = relativeURI.Split('/');
                relativeURI = uri[1];

                
              
                Console.WriteLine("Triiimmmmm" + parts[2].Trim());
                if (parts[2].Trim().ToLower() == "http/1.1") { httpVersion = HTTPVersion.HTTP11; http = "HTTP/1.1"; }
                else if (parts[2].Trim().ToLower() == "http/1.0") { httpVersion = HTTPVersion.HTTP10; http = "HTTP/1.0"; }
                else if (parts[2].Trim().ToLower() == "http/0.9") { httpVersion = HTTPVersion.HTTP09; http = "HTTP/0.9"; }
                
                return true;
            }
            catch (Exception ex) { return false; }
        }

       /* private bool ValidateIsURI(string uri)
        {
            return Uri.IsWellFormedUriString(uri, UriKind.RelativeOrAbsolute);
        }
        */
        /*private bool LoadHeaderLines()
        {
            try
            {
                for (int i = 1; i < requestLines.Length; i++)
                {
                    if (requestLines[i] == "") { break; }
                    //String[] header = requestLines[i].Split(':');
                  //  headerLines.Add(header[0].Trim(), header[1].Trim());
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }*/

        private bool ValidateBlankLine()
        {
            for (int i = 0; i < requestLines.Length; i++)
            {
                if (requestLines[i] == "") { return false; }
            }
            return true;
        }

    }
   

}
