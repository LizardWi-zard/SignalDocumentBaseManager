using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalDocumentBaseManager.Classes
{
    public class DocumentFile
    {
        public int Id { get; set; }

        public string Type { get; set; }

        public string Name { get; set; }

        public string Number { get; set; }

        public string ReleaseDate { get; set; }

        public string EntryDate { get; set; }

        public string KeyWords { get; set; }

        public int AccessLevel { get; set; }
    }
}
