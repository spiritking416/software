using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.Redis;
using System.Threading;
using System.Runtime.Remoting.Messaging;
using HNCAPI.Data;

namespace HNCAPI
{
    class RedisPubSub
    {
        public delegate void HealthDataEventHandler(HealthData hthdb);
        public event HealthDataEventHandler OnHealthData;

        public delegate void ToolBrokenEventHandler(HNCAPI.Data.ToolBrokenInfo info);
        public event ToolBrokenEventHandler OnToolBroken;
        public delegate void SubscribeSampleDataEventHandler(HNCAPI.Data.SampleMessge msg);
        public event SubscribeSampleDataEventHandler OnSubScribeSampleData;
        private delegate void StartListeningHandler();
        private String _MachineSN;
        private String _ip;
        private Int32 _port;
        ServiceStack.Redis.RedisClient _Client;
        public System.String MachineSN
	    {
		    get { return _MachineSN; }
		    set { _MachineSN = value; }
	    }
        public RedisPubSub(String MachineSN)
        {
            _MachineSN=MachineSN;
            _ip = System.Configuration.ConfigurationManager.AppSettings["RedisUri"];
            _port = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["RedisPort"]);
       
        }
        public void Start() 
        {
            StartListeningHandler handler = new StartListeningHandler(Subscribe);
            IAsyncResult result = handler.BeginInvoke(new AsyncCallback(Finished), "Finished:"+_MachineSN);
        }
        private void Finished(IAsyncResult result) 
        {
            StartListeningHandler handlerInit = (StartListeningHandler)((AsyncResult)result).AsyncDelegate;
            handlerInit.EndInvoke(result);
        }
        private void Subscribe()
        {
            try
            {
                var channelName = MachineSN;
                _Client = new RedisClient(_ip, _port);

                using (var subscription = _Client.CreateSubscription())
                {
                    subscription.OnSubscribe = channel =>
                    {

                    };
                    subscription.OnUnSubscribe = channel =>
                    {
                       
                    };
                    subscription.OnMessage = (channel, msg) =>
                    {
                        try
                        {
                            int index1 = msg.IndexOf(',');
                            String Header = msg.Substring(1, index1-1);
                            String[] HeaderArr = Header.Split(':');
                            if (HeaderArr.Length != 2) return;
                            String ValueStr = HeaderArr[1].Trim('\"');

                            switch (ValueStr)
                            {
                                case "ToolBroken":
                                    {
                                        HncMessage<HNCAPI.Data.ToolBrokenInfo> tmes = Newtonsoft.Json.JsonConvert.DeserializeObject<HncMessage<HNCAPI.Data.ToolBrokenInfo>>(msg);
                                        if (OnToolBroken != null)
                                        {
                                            OnToolBroken(tmes.Body);
                                        }
                                    }
                                    break;
                                case "Sample":
                                    {
                                        HncMessage<SampleMessge> SampleMsg = Newtonsoft.Json.JsonConvert.DeserializeObject<HncMessage<SampleMessge>>(msg);
                                        if (OnSubScribeSampleData != null)
                                        {
                                            OnSubScribeSampleData(SampleMsg.Body);
                                        }
                                    }
                                    break;

                                case "HealthData":
                                    {
                                        HncMessage<HealthData> SampleMsg = Newtonsoft.Json.JsonConvert.DeserializeObject<HncMessage<HealthData>>(msg);
                                        if (OnHealthData != null)
                                        {
                                            OnHealthData(SampleMsg.Body);
                                        }
                                        

                                        break;
                                    }
                            }

                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Subscribe"+ex.Message);
                        }
                    };
                    subscription.SubscribeToChannels(channelName); //blocking
                }

            }
            catch(Exception ex)
            {
                Console.WriteLine("Subscribe" + ex.Message);

                this.Start();
            }
           
        }
        
            
    }
}
