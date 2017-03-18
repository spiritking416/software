using System;
using System.Collections.Generic;
using ServiceStack.Redis;
using HNCAPI.Data;


namespace HNCAPI
{
    class LocalSampleDataWriter
    {
        private object _lock;
        private String _ip;
        private Int32 _RedisLocalPort;
        private ServiceStack.Redis.PooledRedisClientManager _Manager;
        private Int32 _CurrentDB;
        public int CurrentDB
        {
            get
            {
                return _CurrentDB;
            }

            set
            {
                _CurrentDB = value;
            }
        }

        public LocalSampleDataWriter()
        {
            _lock = new object();
            _ip = "127.0.0.1";// System.Configuration.ConfigurationManager.AppSettings["RedisUri"];
            _RedisLocalPort = 6379;
        }
         public  bool RegisterDB(String MachineSN)
        {
            try
            {
                
                RedisClient client = getClient(_RedisLocalPort);
                if (client != null)
                {
                    for (Int32 i = 0; i < 23; i++)
                    {
                        client.Db = i;
                        if (client.DbSize == 0) continue;
                        String SN = client.Get<String>("Machine");
                        if (SN == MachineSN)
                        {
                            _CurrentDB = i;
                            return true;

                        }

                    }
                    for (Int32 i = 0; i < 23; i++)
                    {
                        client.Db = i;
                        if (client.DbSize != 0) continue;
                        client.Set<String>("Machine", MachineSN);
                        _CurrentDB = i;
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("RegisterDB" + ex.Message);
            }
           
            return false;

        }
        private RedisClient getClient(Int32 port)
        {
            try {
                if (_Manager == null)
                {
                    _Manager = new PooledRedisClientManager(2, 3000, new String[] { _ip + ":" + _RedisLocalPort.ToString() });
                    //_Manager.Start();
                }
                RedisClient client = (RedisClient)_Manager.GetClient();
               
                if (client.Ping()) return client;
            }
            catch(Exception e)
            {
                Console.WriteLine("getClientLocal" + e.Message);
            }
            return null;

        }
        public void WriteToDatabase(List<SampleCode> codes)
        {
            const Int32 SampleDataReserve = 10000;

            try
            {
                    using (RedisClient LocalClient = getClient(_RedisLocalPort))
                    {
                        if (LocalClient == null)
                        {
                            return;
                        }
                        if (LocalClient != null)
                            LocalClient.Db = _CurrentDB;
                        Int32 len = LocalClient.LLen("SampleData");
                        if (SampleDataReserve < len + codes.Count)//当本地缓存大于1万条记录时，减小数据库。将来可以做本地磁盘缓存。
                        {
                            LocalClient.LTrim("SampleData",len+codes.Count- SampleDataReserve, -1);
                        }
                        for (int i = 0; i < codes.Count; i++)
                        {
                            String value = Newtonsoft.Json.JsonConvert.SerializeObject(codes[i]);
                            LocalClient.AddItemToList("SampleData", value);
                        }
                    }
                
            }
            catch (Exception ex)
            {
                Console.Write("WriteToDatabase" + ex.Message);
            }
            finally
            {
               
            }
        }
       

    }
}
