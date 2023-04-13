using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PathOfExile_NoSpiderMod
{
    public class FileData
    {
        public readonly string newContent;

        public readonly string whoMatched;

        public FileData(string newContent, string whoMatched)
        {
            this.newContent = newContent;
            this.whoMatched = whoMatched;
        }
    }
}