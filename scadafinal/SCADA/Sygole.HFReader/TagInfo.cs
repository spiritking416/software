using System;
using System.Collections.Generic;
using System.Text;

namespace Sygole.HFReader
{
    public class TagInfo
    {
        public byte InformationFlag;
        public byte[] UID = new byte[8];
        public byte DSFID; 
        public byte AFI; 
        public byte BlockCnt; 
        public byte BlockSize; 
        public byte ICrefcerence;
    }
}
