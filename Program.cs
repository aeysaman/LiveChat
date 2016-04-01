using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
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
            //getAgent(Api).Wait();
            getChat(Api).Wait();
            //getTotalChats(Api).Wait();
        }
        static async Task getAgents(LiveChatApi.Api Api)
        {
            string result = await Api.Agents.List();
            Console.WriteLine(result);
            Console.ReadKey();
        }
        static async Task getTotalChats(LiveChatApi.Api Api)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("date_from", "2015-10-01");
            parameters.Add("date_to", "2016-01-31");
            parameters.Add("agent", "a.tomic@bc.edu");
            parameters.Add("group_by", "hour");
            string result = await Api.Reports.ChatSources(parameters);

            Console.WriteLine(result);
            Console.ReadKey();
        }
        static async Task getChats(LiveChatApi.Api Api)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("date_from", "2015-03-01");
            parameters.Add("query", "test");
            parameters.Add("agent", "a.tomic@bc.edu");
            string result = await Api.Archives.Get(parameters);

            Console.WriteLine(result);
            Console.ReadKey();
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

            //Console.WriteLine(result);
            Console.ReadKey();
        }
        static async Task getAgent(LiveChatApi.Api Api)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("login", "a.tomic@bc.edu");
            string result = await Api.Agents.Add(parameters);
            DataContractJsonSerializer test = new DataContractJsonSerializer(typeof(Agent));

            Console.WriteLine(result);
            Console.ReadKey();
        }
        [DataContract]
        public class Chat
        {
            [DataMember]
            public string type;
            [DataMember]
            public string id;
            [DataMember]
            public string[] tickets;
            [DataMember]
            public string visitor_name;
            [DataMember]
            public string visitor_ip;
            [DataMember]
            public string message_id;
            [DataMember]
            public Visitor visitor;
            [DataMember]
            public Message[] messages;
            [DataMember]
            public Agent[] agents;
            [DataMember]
            public string rate;
            [DataMember]
            public int duration;
            [DataMember]
            public string chat_start_url;
            [DataMember]
            public string referrer;
            [DataMember]
            public bool pending;
            [DataMember]
            public string timezone;
            [DataMember]
            public string started;
            [DataMember]
            public string started_timestamp;
            [DataMember]
            public string ended_timestamp;
            [DataMember]
            public string ended;
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