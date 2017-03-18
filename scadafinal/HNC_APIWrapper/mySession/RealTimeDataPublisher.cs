using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Runtime.Remoting.Messaging;
using HNCAPI.Data;
namespace HNCAPI
{
    class RealTimeDataPublisher
    {
        private String _ip;
        private Int32 _port;
        RealTimeDataSubscriber _sub;
        private delegate void UploadHandler(List<SampleCode> codes);
        private String _Channel;
        public System.String Channel
        {
            get { return _Channel; }
            set { _Channel = value; }
        }
        public RealTimeDataPublisher(RealTimeDataSubscriber sub)
        {
            _ip = System.Configuration.ConfigurationManager.AppSettings["RedisUri"];
            _port = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["RedisPort"]);
            _sub = sub;
            _Channel = sub.KeyName;
        }
        public void Publish(List<SampleCode> codes)
        {
            List<SampleCode> sendData = _sub.ETL(codes);
            UploadHandler handler = new UploadHandler(Upload);
            IAsyncResult result = handler.BeginInvoke(sendData,new AsyncCallback(UploadFinished), "published:"+_Channel);
        }
        private void Upload(List<SampleCode> codes)
        {
            try
            {
                using (ServiceStack.Redis.RedisClient client = new ServiceStack.Redis.RedisClient(_ip, _port))
                {
                    if (client.Ping())
                    {
                        //using (var trans = client.CreatePipeline()) 
                        //{
                        //    for (int i = 0; i < codes.Count; i++)
                        //    {
                        //        String value = Newtonsoft.Json.JsonConvert.SerializeObject(codes[i]);
                        //        trans.QueueCommand(r => r.AddItemToList(_Channel, value));
                        //    }
                        //    trans.Flush();
                        //}
                        for (int i = 0; i < codes.Count; i++)
                        {
                            HNCAPI.Data.HncMessage<SampleCode> msg = new HncMessage<SampleCode>();
                            msg.Header = "SampleData";
                            msg.Body = codes[i];
                            String message = Newtonsoft.Json.JsonConvert.SerializeObject(msg);
                            client.PublishMessage(_Channel, message);

                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("publish Upload:" + ex.Message);
            }
        }
        private void UploadFinished(IAsyncResult result)
        {
            UploadHandler handler = (UploadHandler)((AsyncResult)result).AsyncDelegate;
            handler.EndInvoke(result);
            //Console.WriteLine(result.AsyncState);
        }
    }
}
