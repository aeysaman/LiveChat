using System;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Web;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            LiveChatApi.Api Api = new LiveChatApi.Api("a.tomic@bc.edu", "3302ee3a4cfd5ba8cadb2515df64168e");
            //getAgent(Api).Wait();
            getChats(Api).Wait();
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
            //Chat foo = (Chat)JavaScriptConvert.DeserializeObjext(result);

            Console.WriteLine(result);
            Console.ReadKey();
        }
        static async Task getChat(LiveChatApi.Api Api)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            string chatID = "NXV13D0HIN";
            string result = await Api.Archives.Get(chatID);
            //Chat foo = (Chat)JavaScriptConvert.DeserializeObjext(result);
            //DataContractJsonSerializer test = new DataContractJsonSerializer(typeof(Chat));

            Console.WriteLine(result);
            Console.ReadKey();
        }
        static async Task getAgent(LiveChatApi.Api Api)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("login", "a.tomic@bc.edu");
            string result = await Api.Agents.Add(parameters);
            //DataContractJsonSerializer test = new DataContractJsonSerializer(typeof(Agent));

            Console.WriteLine(result);
            Console.ReadKey();
        }
        //[DataContract]
        public class Chat
        {
            public string type { get; set; }
            public string id { get; set; }
            public string[] tickets { get; set; }
            public string visitor_name { get; set; }
            public string visitor_ip { get; set; }
            public string message_id { get; set; }
            public Visitor visitor;
            public Message[] messages { get; set; }
            public Agent[] agents { get; set; }
            public string rate { get; set; }
            public int duration { get; set; }
            public string chat_start_url { get; set; }
            public string referrer { get; set; }
            public bool pending { get; set; }
            public string timezone { get; set; }
            public string started { get; set; }
            public string started_timestamp { get; set; }
            public string ended_timestamp { get; set; }
            public string ended { get; set; }
        }
        public class Message
        {
            public string author_name { get; set; }
            public string text { get; set; }
            public string date { get; set; }
            public string timestamp { get; set; }
            public string agent_id { get; set; }
            public string type { get; set; }
        }
        public class Visitor
        {
            public string id { get; set; }
            public string name { get; set; }
            public string ip { get; set; }
            public string city { get; set; }
            public string region { get; set; }
            public string country { get; set; }
            public string country_code { get; set; }
            public string timezone { get; set; }
        }
        public class Agent
        {
            public string display_name { get; set; }
            public string email { get; set; }
        }
    }
}
