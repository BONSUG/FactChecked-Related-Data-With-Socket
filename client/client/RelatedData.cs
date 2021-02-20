using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace client
{
    class RelatedData
    {
        private string input;
        public string InputData { get { return input; } set { input = value; } }
        
        private string output;
        public string OutputData { get { return output; } set { output = value; } }

        private string title;
        public string Title { get { return title; } set { title = value; } }

        private string type;
        public string Type { get { return type; } set { type = value; } }

        private string accuracy;
        public string Accuracy { get { return accuracy; } set { accuracy = value; } }

        private string link;
        public string Link { get { return link; } set { link = value; } }

        public RelatedData() {
            input = output = title = type = accuracy = link = null;
        }

        public void StripData(RelatedData r) {
            r.OutputData=r.OutputData.Substring(2, r.OutputData.Length-4);
            char sp = '@';
            string[] spstring = r.OutputData.Split(sp);
            r.Title=spstring[0];
            r.Type = spstring[1];
            r.Accuracy = spstring[2];
            r.Link = spstring[3];
        }
    }
}
