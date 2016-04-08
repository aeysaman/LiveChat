using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Web;

namespace LiveChat_Gathering
{
    class Program
    {
        static void Main(string[] args)
        {
            LiveChatApi.Api Api = new LiveChatApi.Api("a.tomic@bc.edu", "3302ee3a4cfd5ba8cadb2515df64168e");
            getChats(Api).Wait();
            //getChat(Api).Wait();
        }
        //gathers Chats from API, serializes from Json, and exports this ChatGroup
        static async Task getChats(LiveChatApi.Api Api)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("date_from", "2015-01-01");
            //parameters.Add("agent", "a.tomic@bc.edu");
            string result = await Api.Archives.Get(parameters);
            //Console.WriteLine(result);
            DataContractJsonSerializer json = new DataContractJsonSerializer(typeof(ChatGroup));
            MemoryStream ms = new MemoryStream(System.Text.ASCIIEncoding.ASCII.GetBytes(result));
            Console.WriteLine(result);
            ChatGroup chats = (ChatGroup)json.ReadObject(ms);
            
            Console.WriteLine("getChats" + chats.chats.Length);
            exportChats(chats);
        }
        static async Task getChat(LiveChatApi.Api Api)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            string chatID = "NWMHR9BWB6";
            string result = await Api.Archives.Get(chatID);
            DataContractJsonSerializer json = new DataContractJsonSerializer(typeof(Chat));
            MemoryStream ms = new MemoryStream(System.Text.ASCIIEncoding.ASCII.GetBytes(result));
            Chat c = (Chat)json.ReadObject(ms);
            Console.WriteLine(result);
            //Console.WriteLine(exportMessages(c.messages));

            Console.ReadKey();
        }
        [DataContract]
        public class ChatGroup
        {
            [DataMember]
            public Chat[] chats;
            [DataMember]
            public int total;
            [DataMember]
            public int pages;
        }
        public static String exportChat(Chat c)
        {
            try {
                return String.Format("{0},{1},{2},{3},{4},{5}",
                c.id, exportVisitor(c.visitor), exportDate(c.started), exportDate(c.ended), exportAgents(c.agents), exportMessages(c.messages));
            }
            catch(Exception e)
            {
                return " ";
            }
        }
        static void exportChats(ChatGroup chats)
        {
            Console.WriteLine("export" + chats.chats.Length);
            var csv = new StringBuilder();
            csv.AppendLine("Chat ID,Visitor Name,Visitor ID,Visitor Email,Visitor Country,Visitor Region,Visitor City,Start Date,End Date,Agents, Messages Text");
            foreach (Chat c in chats.chats)
            {
                String s = exportChat(c);
                //Console.WriteLine(s);
                csv.AppendLine(s);
            }
            File.WriteAllText("chats.csv", csv.ToString());
            Console.WriteLine("All Done!");
            Console.ReadKey();
        }
        [DataContract]
        public class Chat
        {
            [DataMember] public string type;
            [DataMember] public string id;
            [DataMember] public string[] tickets;
            [DataMember] public string visitor_name;
            [DataMember] public string visitor_ip;
            [DataMember] public string message_id;
            [DataMember] public Visitor visitor;
            [DataMember] public Message[] messages;
            [DataMember] public Agent[] agents;
            [DataMember] public string rate;
            [DataMember] public int duration;
            [DataMember] public string chat_start_url;
            [DataMember] public string referrer;
            [DataMember] public bool pending;
            [DataMember] public string timezone;
            [DataMember] public string started;
            [DataMember] public string started_timestamp;
            [DataMember] public string ended_timestamp;
            [DataMember] public string ended;
        }
        public static String exportDate(String d)
        {
            try {
                DateTime date = DateTime.Parse(d);
                return date.ToString("yyyy/MM/dd HH:mm");
            }
            catch (Exception e)
            {
                return " ";
            }
        }
        public static String exportMessages(Message[] ls)
        {
            Dictionary<DateTime, String> dict = new Dictionary<DateTime, string>();
            var str = new StringBuilder();
            foreach (Message m in ls)
            {
                DateTime d = DateTime.Parse(m.date);
                if (dict.ContainsKey(d))
                    dict[d] = dict[d] + "|" + m.text;
                else
                    dict.Add(d, m.text);

            }
            List<DateTime> dates = dict.Keys.ToList();
            dates.Sort();
            foreach(DateTime d in dates)
            {
                str.Append(dict[d] + "|");
                //Console.WriteLine("{0} -> {1} ", d.ToString("yyyy/MM/dd HH:mm"), dict[d]);
            }
            String strFinal = str.ToString();
            strFinal = strFinal.Replace(",", "*");
            return strFinal.Substring(0, strFinal.Length - 1) ;
        }
        [DataContract]
        public class Message
        {
            [DataMember]
            public string author_name;
            [DataMember]
            public string text;
            [DataMember]
            public string date;
            [DataMember]
            public string timestamp;
            [DataMember]
            public string agent_id;
            [DataMember]
            public string type;
        }
        public static String exportVisitor(Visitor v)
        {
            try { return String.Format("{0},{1},{2},{3},{4},{5}",
                v.name, v.id, v.email, v.country, v.region, v.city);
            }
            catch(Exception e)
            {
                return ",,,,,";
            }
        }
        [DataContract]
        public class Visitor
        {
            [DataMember]
            public string id;
            [DataMember]
            public string name;
            [DataMember]
            public string ip;
            [DataMember]
            public string email;
            [DataMember]
            public string city;
            [DataMember]
            public string region;
            [DataMember]
            public string country;
            [DataMember]
            public string country_code;
            [DataMember]
            public string timezone;
        }
        public static String exportAgents(Agent[] ls)
        {
            var str = new StringBuilder();
            foreach(Agent a in ls)
            {
                String foo = a.display_name.Trim() + ";" + a.email.Trim();
                str.Append(foo.Trim() + "|");
            }
            String result = str.ToString();
            return result.Substring(0, result.Length - 1);
        }
        [DataContract]
        public class Agent
        {
            [DataMember]
            public string display_name;
            [DataMember]
            public string email;
            
        }
    }
}