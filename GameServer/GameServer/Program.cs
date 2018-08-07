using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GameServer
{
    class Program
    {
        //Succes/fail messages
        private const string USER_CREATION_FAILED = "Creation failed.";

        //Possible request commands
        private const string CREATE_USER = "createuser";
        private const string LOGIN = "login";

        //Possible request parameters
        private const string NAME = "name";
        private const string USER_ID = "userid";
        private const string PASSWORD = "password";
        private const string POSITION = "position";
        private const string FRIEND_IDS = "friendsids";
        

        private static byte[] _buffer = new byte[1024];
        private static List<Socket> _clientSockets = new List<Socket>();
        private static Socket _serverSocker = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        static void Main(string[] args)
        {
            Console.Title = "Server";
            StartDatabase();
            SetupServer();
            Console.ReadLine();
        }

        private static void StartDatabase()
        {
            Console.WriteLine("Starting Database...");
            MyDataBase.Instance.LoadUserList();
            MyDataBase.Instance.LoadRouteList();
        }

        private static void SetupServer()
        {
            Console.WriteLine("Setting up server...");
            _serverSocker.Bind(new IPEndPoint(IPAddress.Any, 100));
            _serverSocker.Listen(1);
            _serverSocker.BeginAccept(new AsyncCallback(AcceptCallback), null);
        }

        private static void AcceptCallback(IAsyncResult AR)
        {
            Socket socket = _serverSocker.EndAccept(AR);
            _clientSockets.Add(socket);
            Console.WriteLine("Client Connected");
            socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
            _serverSocker.BeginAccept(new AsyncCallback(AcceptCallback), null);
        }

        private static void ReceiveCallback(IAsyncResult AR)
        {

            try
            {
                Socket socket = (Socket)AR.AsyncState;
                int received = socket.EndReceive(AR);
                byte[] dataBuf = new byte[received];
                Array.Copy(_buffer, dataBuf, received);

                string request = Encoding.ASCII.GetString(dataBuf);
                Console.WriteLine("Text received: " + request);

                string response = ProcessRequest(request);

                byte[] data = Encoding.ASCII.GetBytes(response);
                socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);
                socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
            }
            catch (SocketException)
            {
                Console.WriteLine("A client closed it's connection.");
                Console.ReadLine();
                Environment.Exit(0);
            }
        }

        private static void SendCallback(IAsyncResult AR)
        {
            Socket socket = (Socket)AR.AsyncState;
            socket.EndSend(AR);
        }
        
        private static string ProcessRequest(string request)
        {
            string response = String.Empty;

            string[] requestParts = request.Split(':');

            switch (requestParts[0].ToLower())
            {
                case CREATE_USER:                        
                    response = CreateUser(requestParts[1]);
                    break;
                case LOGIN:
                    response = LoginUser(requestParts[1]);
                    break;
                default:
                    response = "Invalid Request!";
                    break;
            }

            return response;
        }

        private static string LoginUser(string parameters)
        {
            Dictionary<string, object> parsedArguments = ParseRequest(parameters);

            string response = String.Empty;

            try
            {
                if (parsedArguments.ContainsKey(USER_ID))
                {
                    response = MyDataBase.Instance.GetUserByID(parsedArguments[USER_ID].ToString()).ToString();
                }
                else
                {
                    response = MyDataBase.Instance.CheckForUserWithPassword(parsedArguments[NAME].ToString(), parsedArguments[PASSWORD].ToString()).ToString();
                }
            }catch(Exception e)
            {
                response = e.Message;
            }

            return response;
        }

        private static string CreateUser(string parameters)
        {
            Dictionary<string, object> parsedArguments = ParseRequest(parameters);

            User user;

            if (!parsedArguments.ContainsKey("userid"))
            {
                user = new User(parsedArguments[NAME].ToString(),
                    parsedArguments[PASSWORD].ToString(),
                    parsedArguments.ContainsKey(POSITION) ? (Position) parsedArguments[POSITION] : new Position(),
                    parsedArguments.ContainsKey(FRIEND_IDS) ? (List<string>) parsedArguments[FRIEND_IDS] : new List<string>());
                try
                {
                    MyDataBase.Instance.AddCustomUser(user);
                }catch(Exception e)
                {
                    return e.Message;
                }
            }
            else
            { 
                user = new User(parsedArguments[NAME].ToString(),
                    parsedArguments[USER_ID].ToString(),
                    parsedArguments[PASSWORD].ToString(),
                    parsedArguments.ContainsKey(POSITION) ? (Position)parsedArguments[POSITION] : new Position(),
                    parsedArguments.ContainsKey(FRIEND_IDS) ? (List<string>)parsedArguments[FRIEND_IDS] : new List<string>());
                MyDataBase.Instance.AddFacebookUser(user);
            }

            return user.ToString();
        }

        private static Dictionary<string, object> ParseRequest(string parameters)
        {
            Dictionary<string, object> parsedArguments = new Dictionary<string, object>();

            string[] parameterParts = parameters.Split(';');
            
            foreach (string attribute in parameterParts)
            {
                string keyword = attribute.Split('=')[0].ToLower();
                string[] value = attribute.Split('=')[1].Split(',');

                switch (keyword)
                {
                    case POSITION:
                        Position position = new Position(float.Parse(value[0]), float.Parse(value[1]));
                        parsedArguments.Add(keyword, position);
                        break;
                    case FRIEND_IDS:
                        List<string> friendsIds = new List<string>();
                        foreach(string friend in value)
                        {
                            friendsIds.Add(friend);
                        }
                        parsedArguments.Add(keyword, friendsIds);
                        break;
                    default:
                        parsedArguments.Add(keyword, value[0]);
                        break;
                }
            }

            return parsedArguments;
        }
    }
}