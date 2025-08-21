using System.Net;
using System.Text;

namespace HttpListenerApp
{
    internal class Program
    {
        private static HttpListener? listener;
        private static bool running = true;
        private const string url = "http://localhost:8889/";

        static async Task Main(string[] args)
        {
            Console.WriteLine("Starting HTTP Listener on " + url);
            Console.WriteLine("Press 'q' to quit");

            // Start the listener
            await StartListener();
        }

        private static async Task StartListener()
        {
            listener = new HttpListener();
            listener.Prefixes.Add(url);
            
            try
            {
                listener.Start();
                Console.WriteLine("Listener started successfully");

                // Start background task to handle quit command
                _ = Task.Run(HandleQuitCommand);

                // Main listener loop
                while (running)
                {
                    try
                    {
                        var context = await listener.GetContextAsync();
                        _ = Task.Run(() => ProcessRequest(context));
                    }
                    catch (ObjectDisposedException)
                    {
                        // Listener was stopped
                        break;
                    }
                    catch (HttpListenerException ex)
                    {
                        Console.WriteLine($"Listener error: {ex.Message}");
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error starting listener: {ex.Message}");
            }
            finally
            {
                listener?.Stop();
                Console.WriteLine("Listener stopped");
            }
        }

        private static void HandleQuitCommand()
        {
            while (running)
            {
                var key = Console.ReadKey(true);
                if (key.KeyChar == 'q' || key.KeyChar == 'Q')
                {
                    running = false;
                    listener?.Stop();
                    break;
                }
            }
        }

        private static async Task ProcessRequest(HttpListenerContext context)
        {
            var request = context.Request;
            var response = context.Response;

            try
            {
                // Parse the resource path
                string resourcePath = request.Url?.AbsolutePath?.Trim('/') ?? "";
                
                Console.WriteLine($"Received request: {request.HttpMethod} {request.Url}");
                Console.WriteLine($"Resource path: {resourcePath}");

                // Route to appropriate handler
                switch (resourcePath.ToLower())
                {
                    case "myname":
                        await GetMyName(response);
                        break;
                    case "information":
                        await HandleInformation(response);
                        break;
                    case "success":
                        await HandleSuccess(response);
                        break;
                    case "redirection":
                        await HandleRedirection(response);
                        break;
                    case "clienterror":
                        await HandleClientError(response);
                        break;
                    case "servererror":
                        await HandleServerError(response);
                        break;
                    case "mynamebyheader":
                        await GetMyNameByHeader(response);
                        break;
                    case "mynamebycookies":
                        await MyNameByCookies(response);
                        break;
                    default:
                        await HandleNotFound(response);
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing request: {ex.Message}");
                response.StatusCode = 500;
            }
            finally
            {
                response.Close();
            }
        }

        // Task 1: GetMyName method
        private static async Task GetMyName(HttpListenerResponse response)
        {
            string myName = "GitHub Copilot";
            byte[] buffer = Encoding.UTF8.GetBytes(myName);
            
            response.ContentLength64 = buffer.Length;
            response.ContentType = "text/plain";
            response.StatusCode = 200;
            
            await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
        }

        // Task 2: HTTP Status Messages
        private static async Task HandleInformation(HttpListenerResponse response)
        {
            response.StatusCode = 102; // Processing (1xx)
            response.StatusDescription = "Processing";
            string message = "1xx - Information: Processing";
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            response.ContentLength64 = buffer.Length;
            response.ContentType = "text/plain";
            await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
        }

        private static async Task HandleSuccess(HttpListenerResponse response)
        {
            response.StatusCode = 200; // OK (2xx)
            response.StatusDescription = "OK";
            string message = "2xx - Success: OK";
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            response.ContentLength64 = buffer.Length;
            response.ContentType = "text/plain";
            await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
        }

        private static async Task HandleRedirection(HttpListenerResponse response)
        {
            response.StatusCode = 302; // Found (3xx)
            response.StatusDescription = "Found";
            response.Headers.Add("Location", "http://localhost:8888/Success/");
            string message = "3xx - Redirection: Found";
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            response.ContentLength64 = buffer.Length;
            response.ContentType = "text/plain";
            await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
        }

        private static async Task HandleClientError(HttpListenerResponse response)
        {
            response.StatusCode = 400; // Bad Request (4xx)
            response.StatusDescription = "Bad Request";
            string message = "4xx - Client Error: Bad Request";
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            response.ContentLength64 = buffer.Length;
            response.ContentType = "text/plain";
            await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
        }

        private static async Task HandleServerError(HttpListenerResponse response)
        {
            response.StatusCode = 500; // Internal Server Error (5xx)
            response.StatusDescription = "Internal Server Error";
            string message = "5xx - Server Error: Internal Server Error";
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            response.ContentLength64 = buffer.Length;
            response.ContentType = "text/plain";
            await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
        }

        // Task 3: Header
        private static async Task GetMyNameByHeader(HttpListenerResponse response)
        {
            string myName = "GitHub Copilot";
            response.Headers.Add("X-MyName", myName);
            response.StatusCode = 200;
            response.ContentType = "text/plain";
            
            string message = "Check the X-MyName header for my name";
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            response.ContentLength64 = buffer.Length;
            await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
        }

        // Task 4: Cookies
        private static async Task MyNameByCookies(HttpListenerResponse response)
        {
            string myName = "GitHub Copilot";
            
            // Add cookie
            Cookie cookie = new Cookie("MyName", myName);
            response.Cookies.Add(cookie);
            
            response.StatusCode = 200;
            response.ContentType = "text/plain";
            
            string message = "Check the MyName cookie for my name";
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            response.ContentLength64 = buffer.Length;
            await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
        }

        private static async Task HandleNotFound(HttpListenerResponse response)
        {
            response.StatusCode = 404;
            response.StatusDescription = "Not Found";
            string message = "404 - Not Found";
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            response.ContentLength64 = buffer.Length;
            response.ContentType = "text/plain";
            await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
        }
    }
}
