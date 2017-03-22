using System;
using System.Collections.Generic;
using System.Text;

namespace Sygole.HFReader
{
    public class CommEventCS
    {
        public event EventHandler<CommEventArgs> CommSendHandler;
        public event EventHandler<CommEventArgs> CommReceiveHandler;

        public void SendHandler(byte[] data, int length)
        {
            if (CommSendHandler != null)
            {
                CommEventArgs args = new CommEventArgs();
                if (length <= args.CommDatas.Length)
                {
                    args.CommDatasLen = length;
                    for (int i = 0; i < args.CommDatasLen; i++)
                    {
                        args.CommDatas[i] = data[i];
                    }
                    CommSendHandler.BeginInvoke(this, args, null, null);
                }
            }
        }

        public void ReceiveHandler(byte[] data, int length)
        {
            if (CommReceiveHandler != null)
            {
                CommEventArgs args = new CommEventArgs();
                if (length <= args.CommDatas.Length)
                {
                    args.CommDatasLen = length;
                    for (int i = 0; i < args.CommDatasLen; i++)
                    {
                        args.CommDatas[i] = data[i];
                    }
                    CommReceiveHandler.BeginInvoke(this, args, null, null);
                }
            }
        }

    }
}
