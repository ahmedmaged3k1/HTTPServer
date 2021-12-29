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

            string statusLine = GetStatusLine(code);
            ;

            responseString = http +" "+statusLine+ " "+code + "\r\nContent_Type:" + contentType + "\r\nContent_Length:" + content.Length + "\r\nDate:" + DateTime.Now + "\r\n"+ "\r\n" + content;
            Console.WriteLine(responseString);
            
     
            if (redirectoinPath != "") { responseString = responseString + "Redirected To : " + redirectoinPath; }


        }

        private string GetStatusLine(StatusCode code)
        {
     
            string statusLine = string.Empty;
            if (code == StatusCode.OK) { return "200"; }
            else if (code == StatusCode.NotFound) { return "404"; }
            else if (code == StatusCode.BadRequest) { return "400"; }
            else if (code == StatusCode.Redirect) { return "301"; }
            else if (code == StatusCode.InternalServerError) { return "500"; }



            return statusLine;
        }
    }
}
