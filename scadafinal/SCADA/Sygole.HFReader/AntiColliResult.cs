using System;
using System.Collections.Generic;
using System.Text;

namespace Sygole.HFReader
{
    public class TagReadResult
    {
        public byte[] UID = new byte[8];
        public Antenna_enum antenna { get; set; }

        public TagReadResult(byte[] uid, Antenna_enum ant)
        {
            antenna = ant;
            Array.Copy(uid, UID, 8);
        }
    }

    public class AntiColliResult
    {
        public List<TagReadResult> UidList = new List<TagReadResult>();

        public AntiColliResult()
        {
        }

        public AntiColliResult(AntiColliResult tagList)
        {
            foreach (TagReadResult tmp in tagList.UidList)
            {
                this.add(tmp);
            }
        }

        public void add(TagReadResult tag)
        {
            foreach (TagReadResult tmp in UidList)
            {
                if (tool.ByteCmp(tmp.UID, 0, tag.UID, 0, 8) &&
                    (tmp.antenna == tag.antenna))
                {
                    return;
                }
            }
            UidList.Add(tag);
        }

        public void add(byte[] uid, Antenna_enum ant)
        {
            foreach (TagReadResult tmp in UidList)
            {
                if (tool.ByteCmp(tmp.UID, 0, uid, 0, 8) &&
                    (tmp.antenna == ant))
                {
                    return;
                }
            }
            UidList.Add(new TagReadResult(uid, ant));
        }
    }
}
