using System.Diagnostics;

namespace HTTP_Status
{
    internal class HTTPStatus
    {
        private readonly HttpClient client = new HttpClient();

        private string Path {get;}       

        public HTTPStatus(string path)
        { 
            Path = path.Trim();          
            CheckStatusAsync(ReadUrlsFromFile()).GetAwaiter().GetResult();
        }
   
        private List<string> ReadUrlsFromFile()
        {
            List<string> urls = new List<string>();

            if (!File.Exists(Path))
                throw new FileNotFoundException("File not found");

            using (StreamReader file = new StreamReader(Path))
            {
                string? line;
                while ((line = file.ReadLine()) != null)
                {
                    if (line.StartsWith("https://"))
                        urls.Add(line);
                    else
                        throw new ArgumentException("Invalid URL format");
                }                 
            }     
            
            return urls;
        }

        private void WritingToFile(List<string> urls, int currentUrls, string httpStatus, long leadTime)
        {
            using(FileStream stream = new FileStream(Path.Replace(".txt", "Status.csv"), FileMode.Append))
            using (StreamWriter writer = new StreamWriter(stream))
                writer.WriteLine($"[{httpStatus}]," + $"{urls[currentUrls]}," + $"{leadTime}ms");
        }

        private async Task CheckStatusAsync(List<string> urls)
        {                  
            for (int i = 0; i < urls.Count; i++)
            {
                HttpResponseMessage response = new HttpResponseMessage();
                var stopwatch = Stopwatch.StartNew(); 

                try
                {
                    response.StatusCode = (await client.GetAsync(urls[i])).StatusCode;
                    stopwatch.Stop();
                }
                catch (Exception)
                {
                    response.StatusCode = System.Net.HttpStatusCode.NotFound;
                }
                
                WritingToFile(urls, i, response.StatusCode.ToString(), stopwatch.ElapsedMilliseconds);
            }                         
        }
    }    
}
