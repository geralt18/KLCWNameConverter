using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KLCWNameConverter
{
   class Program
   {
        private const string folderPath = @"E:\!KLCW";

        static void Main(string[] args)
      {
         WebRequest req = WebRequest.Create("https://pl.wikiradio.org/wiki/Klub_Ludzi_Ciekawych_Wszystkiego");
         WebResponse res = req.GetResponse();
         using (var reader = new StreamReader(res.GetResponseStream()))
         {
            string content = reader.ReadToEnd();

            foreach (var f in Directory.GetFiles(folderPath))
            {

               //<li> 2013 - 06 - 02 Brzechwa bał się dzieci!Zaczął pisać bajki, by uwieść przedszkolankę?
               //<a class="external text" rel="nofollow" href="http://www.polskieradio.pl/be15efad-98c2-4abd-835b-af497723cf73.mp3">MP3</a>
               string p1 = string.Format(@"<li>(?<name>.*)<a.*{0}.*<\/a>", Path.GetFileName(f));
               Regex pattern = new Regex(p1);

               MatchCollection mp1 = pattern.Matches(content);
               if (mp1.Count > 0)
               {
                  string gname = mp1[0].Groups["name"].Value;
                  string nameNew = string.Format("{0}.mp3", CleanFileName(gname).Trim());

                  File.Move(f, Path.Combine(Path.GetDirectoryName(f), nameNew));
                  continue;
               }



               //<td> 2015-08-01
               //</td>
               //<td> 13-letni łącznik Powstania. Wspomnienia świadków
               //</td>
               //<td> <a rel="nofollow" class="external text" href="http://www.polskieradio.pl/553a277c-f8fb-44b2-a54a-640e3f02338f.mp3">MP3</a>
               //</td>
               string p2 = string.Format(@"<td>(?<date>.*\n)<\/td>\n<td>(?<name>.*\n)<\/td>\n<td>(.*){0}(.*)<\/a>", Path.GetFileName(f));
               pattern = new Regex(p2);

               MatchCollection mp2 = pattern.Matches(content);
               if (mp2.Count > 0)
               {
                  string gname = mp2[0].Groups["name"].Value;
                  string gdate = mp2[0].Groups["date"].Value;
                  string nameNew = string.Format("{0} {1}.mp3", CleanFileName(gdate).Trim(), CleanFileName(gname).Trim());

                  File.Move(f, Path.Combine(Path.GetDirectoryName(f), nameNew));
                  continue;
               }
            }
         }
      }

      static string CleanFileName(string name)
      {
         foreach (char c in System.IO.Path.GetInvalidFileNameChars())
            name = name.Replace(c.ToString(), "");
         return name;
      }
   }
}
