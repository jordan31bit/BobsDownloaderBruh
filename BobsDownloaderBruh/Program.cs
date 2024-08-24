using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;

namespace BobsDownloaderBruh {
    internal class Program {

        public static string[] filterEpisodesForEng(XmlNodeList itemList) {
            // Filter items for english and build array of english only anime.
            string[] englishAnimeList = new string[itemList.Count];
            for (int i = 0; i < itemList.Count; i++) {
                if (GetCatagory(itemList, i)) {
                    // build a new list of english only anime
                    englishAnimeList[i] = itemList.Item(i).OuterXml;
                }
            }
            return englishAnimeList;
        }

        // Check if item's category is anime-english
        public static bool GetCatagory(XmlNodeList itemList, int i) {
            int category = 9;
            if (itemList[i].ChildNodes[category].InnerText == "Anime - English-translated") {
                return true;
            }
            else {
                return false;
            }
        }

        public static string parseUploaderName(string[] englishAnimeList, int count) {
            string temp = "";
            string sStringTemp = "";
            
            for(int i = 0; i < count; i++) {
                temp = englishAnimeList.ElementAt(i);
                if (englishAnimeList == null || temp == null)
                    break;
                for(int j = 0; j < temp.Length; j++) {
                    if (temp[j] == '[') {
                        for(int k = 0; k < temp.Length; k++) {
                            if (temp[k] == ']') {
                                int tmp = 0;
                                tmp = k - j;
                                sStringTemp = temp.Substring(j+1, tmp-1);
                                return sStringTemp;
                            }
                        }
                    }
                }
            }
            return sStringTemp;
        }

        public static Boolean checkUploaderName(string uploaderName) {
            if ( uploaderName == "Yameii") {
                Console.WriteLine("Found Him");
                return true;
            }
            else {
                return false;
            }
        }

        async public static void searchForSeries() {
            
            HttpClient httpClient = new HttpClient();
            HttpResponseMessage response = await httpClient.GetAsync("https://nyaa.si/user/Yameii?f=0&c=0_0&q=");
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseBody);
        }

        static void Main(string[] args) {
            XmlDocument bob = new XmlDocument();
            //bob.PreserveWhitespace = true;
            bob.Load("https://nyaa.si/?page=rss");
            XmlNode firstChild = bob.FirstChild;
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(bob.NameTable);
            nsmgr.AddNamespace("nyaa", "https://nyaa.si/xmlns/nyaa");
            nsmgr.PushScope();
            nsmgr.LookupNamespace("nyaa");

            // targets first item               
            //Console.WriteLine(firstChild.ChildNodes[0].ChildNodes[4].FirstChild.InnerText);
            XmlNodeList itemList = bob.SelectNodes("/rss/channel/item");
            XmlNodeList titleList = bob.SelectNodes("/rss/channel/item/title");
            XmlNode catagoryID = bob.SelectSingleNode("nyaa:categoryId", nsmgr);
            XmlNode englishShow = bob.SelectSingleNode("nyaa:category", nsmgr);
            string[] englishAnimeList = new string[itemList.Count]; // hold all english only anime items.
            int count = itemList.Count;
            string uploaderName;
            englishAnimeList = filterEpisodesForEng(itemList);
            for (int i = 0; i < englishAnimeList.Length; i++) {
                uploaderName = parseUploaderName(englishAnimeList, count);
                Console.WriteLine(uploaderName);
                Console.WriteLine(checkUploaderName(uploaderName));
            }
            searchForSeries();
           /* for(int i = 0; i < itemList.Count; i++) {
                if (titleList[i].InnerText == "[BBF] Dark Gathering - 01 [WEB 1080p SUB ITA][8F2DBA69].mkv") {
                    Console.WriteLine("Found it");
                    Console.WriteLine(titleList[i].InnerText);
                    break;
                }
                //Console.WriteLine(title2[i].InnerText);

            }*/
           
            Console.Read();
            
        }
    }
}
