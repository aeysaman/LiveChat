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
            try {
                start().Wait();
            }
            catch(System.AggregateException e)
            {
                Console.WriteLine("Cannot write to file, please close output file and rerun");
            }
            Console.ReadKey();
        }
        static async Task start()
        {
            LiveChatApi.Api Api = new LiveChatApi.Api("a.tomic@bc.edu", "3302ee3a4cfd5ba8cadb2515df64168e");
            FullAgent[] agents = await getAgents(Api);
            Console.WriteLine("agent count: " + agents.Length);
            Dictionary<String, Chat> allChats = new Dictionary<string, Chat>();
            foreach (FullAgent f in agents)
            {
                Console.WriteLine("{0};{1}", f.name, f.login);
                Chat[] current = await getChats(Api, f.login);
                foreach(Chat foo in current)
                {
                    if (!allChats.ContainsKey(foo.id))
                    {
                        allChats.Add(foo.id, foo);
                    }
                }
            }
            exportChats(allChats);
            //getChat(Api).Wait();
        }
        static async Task<FullAgent[]> getAgents(LiveChatApi.Api Api)
        {
            String result = await Api.Agents.List();
            DataContractJsonSerializer json = new DataContractJsonSerializer(typeof(FullAgent[]));
            MemoryStream ms = new MemoryStream(System.Text.ASCIIEncoding.ASCII.GetBytes(result));
            FullAgent[] agents = (FullAgent[])json.ReadObject(ms);
            
            return agents;
        }
        //gathers Chats from API, serializes from Json, and exports this ChatGroup
        static async Task<Chat[]> getChats(LiveChatApi.Api Api, String agent)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("date_from", "2015-01-01");
            parameters.Add("agent", agent);
            
            string result = await Api.Archives.Get(parameters);

            DataContractJsonSerializer json = new DataContractJsonSerializer(typeof(ChatGroup));
            MemoryStream ms = new MemoryStream(System.Text.ASCIIEncoding.ASCII.GetBytes(result));
            ChatGroup chats = (ChatGroup)json.ReadObject(ms);
            
            Console.WriteLine("getChats " + chats.chats.Length);
            return chats.chats;
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
        static void exportChats(Dictionary<String, Chat> foo)
        {
            Chat[] chats = foo.Values.ToArray();
            Console.WriteLine("export" + chats.Length);
            var csv = new StringBuilder();
            csv.AppendLine("Chat ID,Visitor Name,Visitor ID,Visitor Email,Visitor Country,Visitor Region,Visitor City,Start Date,End Date,Agents, Messages Text");
            foreach (Chat c in chats)
            {
                String s = exportChat(c);
                //Console.WriteLine(s);
                csv.AppendLine(s);
            }
            File.WriteAllText("chats.csv", csv.ToString());
            Console.WriteLine("All Done!");
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
            DateTime date = DateTime.Parse(d);
            return date.ToString("yyyy/MM/dd HH:mm");
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
            strFinal = strFinal.Replace(",", "*").Replace("\n", " ").Replace("\t", " ").Replace("\r", " ");
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
            return String.Format("{0},{1},{2},{3},{4},{5}",
                v.name, v.id, v.email, v.country, v.region, v.city);
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
        [DataContract]
        public class FullAgent
        {
            [DataMember]
            public string name;
            [DataMember]
            public string permission;
            [DataMember]
            public string avatar;
            [DataMember]
            public string login;
            [DataMember]
            public string status;
            [DataMember]
            public string[] group_ids;
        }
    }
}