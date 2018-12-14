using System;
using System.IO;
using System.Net;
using System.Text;
using System.Collections.Generic;

namespace scrape_sofeminine
{
    class Program
    {
        static void Main(string[] args)
        {
            ScrapeSurnames( ((args.Length > 0 ) && (args[0] == "p" || args[0] == "P")) ? true : false );
        }


        static void ScrapeSurnames(bool showProgress)
        { 
            List<string> surnames = new List<string>();
            string filename = "UKSurnames.txt";
            string url;            

            for (int i = 0; i <= 57; i++)
            {
                if (showProgress)
                    Console.WriteLine("Working on page: {0}", i);

                try
                {
                    // Currently 57 pages this could be a parameter or taken from the page before the loop
                    // With almost 8500 surnames it should be enough for a useful dataset
                    url = "https://surname.sofeminine.co.uk/w/surnames/most-common-surnames-in-great-britain-" + i + ".html";
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    request.Method = "GET";
                    WebResponse response = request.GetResponse();
                    StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                    string result = reader.ReadToEnd();
                    
                    string searchString = "<td class=\"nom\">";
                    string line, surname;

                    StringReader sr = new StringReader(result);

                    do
                    {
                        line = sr.ReadLine();
                        if (line != null)
                        {
                            if (line.Contains(searchString))
                            {
                                string[] arr = line.Split('>');
                                foreach (string item in arr)
                                {
                                    if (item.Contains("</a"))
                                    {
                                        surname = item.Substring(0, item.Length - 3);
                                        if (!surnames.Contains(surname))
                                        {                                            
                                            // These tweaks are for the format of the data coming in after visual checks of initial results
                                            if ( surname.Contains(" ") )
                                            {
                                                string[] s = surname.Split(' ');     
                                                if ( s[0] != "O" )                                                    
                                                    // If surname is double barrel format it properly e.g. Berners lee -> Berners-Lee
                                                    surname = s[0] + "-" + s[1].Substring(0, 1).ToUpper() + s[1].Substring(1, s[1].Length - 1);
                                                else
                                                    // If surname is O space whatever add the apostrophe and reformat case e.g. O neil -> O'Neil
                                                    surname = s[0] + "'" + s[1].Substring(0, 1).ToUpper() + s[1].Substring(1, s[1].Length - 1);
                                            }
                                            surnames.Add(surname);                                                
                                        }
                                    }
                                }
                            }
                        }
                    }
                    while (line != null);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception: ", ex.Message);
                }
            }

            surnames.Sort();

            using (StreamWriter file = new StreamWriter(filename))
            {
                foreach (string item in surnames)
                    file.WriteLine(item);
            }

            if (showProgress)
            {
                Console.WriteLine("Completed");
                Console.ReadLine();
            }

        }

    }
}
