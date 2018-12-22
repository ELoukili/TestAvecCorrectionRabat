using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Exemple1
{
    public class Store
    {
        // List of stock the stores and file
        public List<string> documents = new List<string>();
        
        public string StoreName { get; set; }

        public void addDocument(string document)
        {
            documents.Add(document);
        }

        public void deleteAllDocuments()
        {
            documents.Clear();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(StoreName+":");
            if (documents.Count == 0)
                return sb.ToString() + "empty";
            foreach (string doc in documents)
            {
                sb.Append(doc+", ");
            }
            return sb.ToString().Substring(0, sb.Length - 2);
        }

    }
}