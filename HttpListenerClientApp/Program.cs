using System.Net;

namespace HttpListenerClientApp
{
    internal class Program
    {
        private static readonly HttpClient httpClient = new HttpClient();
    private const string baseUrl = "http://localhost:8889/";

        static async Task Main(string[] args)
        {
            Console.WriteLine("HTTP Client Application");
            Console.WriteLine("Make sure the HTTP Listener is running before testing");
            Console.WriteLine();

            try
            {
                // Task 1: URL - Get My Name
                await Task1_GetMyName();

                // Task 2: HTTP Status Messages
                await Task2_HttpStatusMessages();

                // Task 3: Header
                await Task3_Header();

                // Task 4: Cookies
                await Task4_Cookies();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                httpClient.Dispose();
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        // Task 1: URL - Request to get name
        private static async Task Task1_GetMyName()
        {
            Console.WriteLine("=== Task 1: URL - Getting My Name ===");
            try
            {
                string url = baseUrl + "MyName/";
                HttpResponseMessage response = await httpClient.GetAsync(url);
                string content = await response.Content.ReadAsStringAsync();
                
                Console.WriteLine($"URL: {url}");
                Console.WriteLine($"Response: {content}");
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Task 1 Error: {ex.Message}");
                Console.WriteLine();
            }
        }

        // Task 2: HTTP Status Messages
        private static async Task Task2_HttpStatusMessages()
        {
            Console.WriteLine("=== Task 2: HTTP Status Messages ===");
            
            string[] endpoints = {
                "Information/",
                "Success/",
                "Redirection/",
                "ClientError/",
                "ServerError/"
            };

            foreach (string endpoint in endpoints)
            {
                try
                {
                    string url = baseUrl + endpoint;
                    HttpResponseMessage response = await httpClient.GetAsync(url);
                    
                    Console.WriteLine($"URL: {url}");
                    Console.WriteLine($"Status Code: {(int)response.StatusCode} - {response.StatusCode}");
                    Console.WriteLine($"Status Description: {response.ReasonPhrase}");
                    
                    string content = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Content: {content}");
                    Console.WriteLine();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error with {endpoint}: {ex.Message}");
                    Console.WriteLine();
                }
            }
        }

        // Task 3: Header
        private static async Task Task3_Header()
        {
            Console.WriteLine("=== Task 3: Header - Getting My Name from Header ===");
            try
            {
                string url = baseUrl + "MyNameByHeader/";
                HttpResponseMessage response = await httpClient.GetAsync(url);
                
                Console.WriteLine($"URL: {url}");
                
                // Get the X-MyName header value
                if (response.Headers.TryGetValues("X-MyName", out var headerValues))
                {
                    string myName = headerValues.FirstOrDefault() ?? "Header not found";
                    Console.WriteLine($"X-MyName Header Value: {myName}");
                }
                else
                {
                    Console.WriteLine("X-MyName header not found");
                }
                
                string content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Response Content: {content}");
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Task 3 Error: {ex.Message}");
                Console.WriteLine();
            }
        }

        // Task 4: Cookies
        private static async Task Task4_Cookies()
        {
            Console.WriteLine("=== Task 4: Cookies - Getting My Name from Cookies ===");
            try
            {
                // Create a cookie container to handle cookies
                var cookieContainer = new CookieContainer();
                using var clientWithCookies = new HttpClient(new HttpClientHandler()
                {
                    CookieContainer = cookieContainer
                });

                string url = baseUrl + "MyNameByCookies/";
                HttpResponseMessage response = await clientWithCookies.GetAsync(url);
                
                Console.WriteLine($"URL: {url}");
                
                // Get cookies from the response
                var cookies = cookieContainer.GetCookies(new Uri(baseUrl));
                var myNameCookie = cookies["MyName"];
                
                if (myNameCookie != null)
                {
                    Console.WriteLine($"MyName Cookie Value: {myNameCookie.Value}");
                }
                else
                {
                    Console.WriteLine("MyName cookie not found");
                }
                
                string content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Response Content: {content}");
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Task 4 Error: {ex.Message}");
                Console.WriteLine();
            }
        }
    }
}
