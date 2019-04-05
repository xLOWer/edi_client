using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace updater
{
    class Program
    {
        static void Main(string[] args)
        {
            //https://github.com/xLOWer/edi_client/raw/master/EdiClient2/bin/x86/Release/EdiClient.zip

            using (WebClient wc = new WebClient())
            {
                wc.DownloadFile( new Uri( "http://192.168.1.20:8080/updater_last.zip" ),
                "EdiClient.zip" );
            }
        }
    }
}
