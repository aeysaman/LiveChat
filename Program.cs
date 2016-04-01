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
        }
        static void exportChats(ChatGroup chats)
        {
            Console.WriteLine("export" + chats.chats.Length);
            var csv = new StringBuilder();
            foreach(Chat c in chats.chats)
            {
                String s = exportChat(c);
                Console.WriteLine(s);
                csv.AppendLine(s);
            }
            File.WriteAllText("chats.csv", csv.ToString());
            Console.ReadKey();
        }
        static async Task getChats(LiveChatApi.Api Api)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("date_from", "2015-03-01");
            parameters.Add("agent", "a.tomic@bc.edu");
            string result = await Api.Archives.Get(parameters);
            DataContractJsonSerializer json = new DataContractJsonSerializer(typeof(ChatGroup));
            MemoryStream ms = new MemoryStream(System.Text.ASCIIEncoding.ASCII.GetBytes(result));
            ChatGroup chats = (ChatGroup)json.ReadObject(ms);
            Console.WriteLine("getChats" + chats.chats.Length);
            exportChats(chats);
        }
        static async Task getChat(LiveChatApi.Api Api)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            string chatID = "NXV13D0HIN";
            string result = await Api.Archives.Get(chatID);
            DataContractJsonSerializer json = new DataContractJsonSerializer(typeof(Chat));
            MemoryStream ms = new MemoryStream(System.Text.ASCIIEncoding.ASCII.GetBytes(result));
            Chat c = (Chat)json.ReadObject(ms);
            Console.WriteLine(c.type + " " + c.visitor_ip);

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
            return c.id +"," + c.visitor + "," + c.duration + "," + c.started + "," + c.ended + "," + c.messages.Length;
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