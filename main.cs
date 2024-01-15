// main.cs
using System;
using System.Threading;
using System.Net;



class Program
{
    static void Main()
    {
        var apiManager = new ApiManager();

        using (HttpListener listener = new HttpListener())
        {
            listener.Prefixes.Add("http://localhost:8080/");
            listener.Start();

            Console.WriteLine("API en cours d'exécution sur http://localhost:8080/");

            while (true)
            {
                var context = listener.GetContext();
                ThreadPool.QueueUserWorkItem(o => apiManager.HandleRequest(context));
            }
        }
    }
}
