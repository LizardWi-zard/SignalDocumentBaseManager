using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SignalDocumentBaseManager.Classes
{
    static public class DataBaseInteracter
    {
        static public async Task<DocumentFile[]> GetDocumentsAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                var result = await client.GetAsync("https://localhost:7231/Documents").ConfigureAwait(false);

                return await result.Content.ReadFromJsonAsync<DocumentFile[]>();
            }
        }

        static public async Task PostDocumentsAsync(DocumentFile documentToPost)
        {
            using (HttpClient client = new HttpClient())
            {
                var json = JsonConvert.SerializeObject(documentToPost);

                string param = $"json={json}";

                var content = new StringContent(param, Encoding.UTF8, "application/json");

                string ct = content.ToString();

                try
                {
                    var result = await client.PostAsync("https://localhost:7231/Documents?" + param, content);

                    result.EnsureSuccessStatusCode();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Exception heppend: " + ex);
                }
            }
        }

    }
}
