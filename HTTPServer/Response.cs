using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{

    public enum StatusCode
    {
        OK = 200,
        InternalServerError = 500,
        NotFound = 404,
        BadRequest = 400,
        Redirect = 301
    }

    class Response
    {
        string responseString;
       
        public string ResponseString
        {
            get
            {
                return responseString;
            }
        }
        
        List<string> headerLines = new List<string>();
        public Response(string http , StatusCode code, string contentType, string content, string redirectoinPath)
        {
            
            // TODO: Add headlines (Content-Type, Content-Length,Date, [location if there is redirection])
            string statusLine = GetStatusLine(code);
           
            //HTTP/1.1
            responseString = http +" "+200+ " "+code + "\r\nContent_Type:" + contentType + "\r\nContent_Length:" + content.Length + "\r\nDate:" + DateTime.Now + "\r\n"+ "\r\n" + content;
            Console.WriteLine(responseString);
            
            // TODO: Create the request string
            if (redirectoinPath != "") { responseString = responseString + "Location: " + redirectoinPath; }


        }

        private string GetStatusLine(StatusCode code)
        {
            // TODO: Create the response status line and return it
            string statusLine = string.Empty;

            return statusLine;
        }
    }
}
