//using System.Underworld;
using System.Net.Http;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using System;

namespace TheFinalLoadTest
{
    struct LoadTestTestSettings
    {

    }

    public interface TestSettings
    {
        Uri Addres { get; }
        int RequestCount { get; }
        int Timeout { get; }
        int ThreadsCount { get; }
    }

    public class CreateTestSettings : TestSettings
    {
        public Uri Addres { get; }
        public int RequestCount { get; set; } = 1000;
        public int Timeout { get; set; } = 1000;
        public int ThreadsCount { get; set; } = 10;

        public CreateTestSettings(Uri addres)
        {
            Addres = addres;
        }

        public CreateTestSettings(string addres)
        {
            Addres = new Uri(addres);
        }

    }

    public class GetTestSettings : TestSettings
    {
        public Uri Addres { get; }
        public int RequestCount { get; set; } = 100000;
        public int Timeout { get; set; } = 1000;
        public int ThreadsCount { get; set; } = 100;

        public GetTestSettings(Uri addres)
        {
            Addres = addres;
        }

        public GetTestSettings(string addres)
        {
            Addres = new Uri(addres);
        }

    }

    public struct TestReult{
        public int requestsFailed;
        public int requestsTimeout;
        public long avgTime;
    }

    class Program
    {
        static GetTestSettings getSettings = new GetTestSettings(@"https://127.0.0.1:5001/api/MeetRead")
        {
            RequestCount = 10000,
            ThreadsCount = 10
        };
        static CreateTestSettings createSettings = new CreateTestSettings(@"https://127.0.0.1:5003/api/MeetCreation")
        {
            RequestCount = 100,
            ThreadsCount = 2
        };

        static HttpClient httpCli;

        static void Main(string[] args)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            ServicePointManager.ServerCertificateValidationCallback = (senderX, certificate, chain, sslPolicyErrors) => { return true; };
            httpCli = new HttpClient();
            GetTester gt = new GetTester(httpCli);
            CreateTester ct = new CreateTester(httpCli);
            Task<TestReult>[] arr = new Task<TestReult>[2];
            arr[0] = gt.StartTest(getSettings);
            arr[1] = ct.StartTest(createSettings);
            //arr[0] = ct.StartTest(createSettings);

            Task.WaitAll(arr);

            Console.ReadKey();
        }



    }

    public class GetTester
    {
        static HttpClient httpCli;
        static int requestCount;
        static readonly object locker = new object();
        static TestReult testReult;

        public GetTester(HttpClient http)
        {
            httpCli = http;
        }

        public async Task<TestReult> StartTest(GetTestSettings settings)
        {
            testReult = new TestReult() { requestsFailed= 0, requestsTimeout = 0, avgTime = 0 };
            httpCli.Timeout = TimeSpan.FromMilliseconds(settings.Timeout);
            requestCount = settings.RequestCount;

            List<Task<TestReult>> tasksList = new List<Task<TestReult>>();

            for (int i = 0; i < settings.ThreadsCount; i++)
                tasksList.Add(StartGetTestAsync(settings.Addres));

            TestReult[] arr = await Task.WhenAll<TestReult>(tasksList);
            foreach (TestReult t in arr) {
                testReult.avgTime += t.avgTime;
                testReult.requestsFailed += t.requestsFailed;
                testReult.requestsTimeout += t.requestsTimeout;
            }
            testReult.avgTime /= (settings.RequestCount-requestCount);


            Console.WriteLine("-- GET Test done --\nAddres: {0}\nRequests: {1}\nFailed: {2}\nTimeout: {3}\navgTime: {4}",
                settings.Addres, settings.RequestCount, testReult.requestsFailed, testReult.requestsTimeout, testReult.avgTime);

            return testReult;
        }

        public static async Task<TestReult> StartGetTestAsync(Uri addres)
        {
            Console.WriteLine("Thread for GET test is hire!");
            TestReult tr = new TestReult() { requestsFailed = 0, requestsTimeout = 0, avgTime = 0 };
            Stopwatch sw = new Stopwatch();
            sw.Start();
            long start;
            while (true) {
                start = sw.ElapsedMilliseconds;
                HttpResponseMessage rm = await httpCli.GetAsync(addres);
                tr.avgTime += (sw.ElapsedMilliseconds - start);
                if (!rm.IsSuccessStatusCode)
                {
                    tr.requestsFailed++;
                    tr.requestsTimeout += rm.StatusCode == System.Net.HttpStatusCode.RequestTimeout ? 1 : 0;
                }

                lock (locker)
                {
                    requestCount--;
                    if (requestCount <= 0)
                        break;
                }
            }
            sw.Stop();
            return tr;
        }
    }

    public class CreateTester
    {
        static HttpClient httpCli;
        static int requestCount;
        static readonly object locker = new object();
        static TestReult testReult;

        const string jsonPattern = "{{\"name\":\"{0}\"," +
                "\"location\":{{\"type\":\"Point\",\"coordinates\":[{1},{2}]}}," +
                "\"subject\":\"{3}\"," +
                "\"startTime\":\"{4}\"," +
                "\"endTime\":\"{5}\"," +
                "\"maxMembers\":{6}}}";
        static string[] namesPattern = { "Meet1","Yahta","GoodWay","Piknik","Sparta", "Meet2", "Meet5", "Shodka","PizzaTime","Oleg" };
        static string[] subjectPattern = { "Sport","Eat","Games" };

        public CreateTester(HttpClient http)
        {
            httpCli = new HttpClient();
        }

        public async Task<TestReult> StartTest(CreateTestSettings settings)
        {
            testReult = new TestReult() { requestsFailed = 0, requestsTimeout = 0, avgTime = 0 };
            httpCli.Timeout = TimeSpan.FromMilliseconds(settings.Timeout);
            requestCount = settings.RequestCount;

            List<Task<TestReult>> tasksList = new List<Task<TestReult>>();

            for (int i = 0; i < settings.ThreadsCount; i++)
                tasksList.Add(StartCreateTestAsync(settings.Addres));

            TestReult[] arr = await Task.WhenAll<TestReult>(tasksList);
            foreach (TestReult t in arr)
            {
                testReult.avgTime += t.avgTime;
                testReult.requestsFailed += t.requestsFailed;
                testReult.requestsTimeout += t.requestsTimeout;
            }
            testReult.avgTime /= (settings.RequestCount - requestCount);

            Console.WriteLine("-- CREATE Test done --\nAddres: {0}\nRequests: {1}\nFailed: {2}\nTimeout: {3}\navgTime: {4}",
                settings.Addres, settings.RequestCount, testReult.requestsFailed, testReult.requestsTimeout, testReult.avgTime);

            return testReult;
        }

        public static async Task<TestReult> StartCreateTestAsync(Uri addres)
        {
            Console.WriteLine("Thread for Create test is hire!");
            TestReult tr = new TestReult() { requestsFailed = 0, requestsTimeout = 0, avgTime = 0 };
            Stopwatch sw = new Stopwatch();
            sw.Start();
            long start;
            StringContent sc;
            Random rn = new Random();
            while (true)
            {
                sc = new StringContent(String.Format(jsonPattern, 
                    namesPattern[rn.Next(10)], rn.NextDouble() * 100, rn.NextDouble() * 100,
                    subjectPattern[rn.Next(3)],
                    new DateTime(DateTime.Now.Ticks + rn.Next(50000, 90000), DateTimeKind.Utc).ToString("o"),
                    new DateTime(DateTime.Now.Ticks + rn.Next(90000, 9999999), DateTimeKind.Utc).ToString("o"),
                    rn.Next()));
                sc.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json");

                start = sw.ElapsedMilliseconds;
                HttpResponseMessage rm = await httpCli.PostAsync(addres,sc);
                tr.avgTime += (sw.ElapsedMilliseconds - start);
                if (!rm.IsSuccessStatusCode)
                {
                    tr.requestsFailed++;
                    tr.requestsTimeout += rm.StatusCode == System.Net.HttpStatusCode.RequestTimeout ? 1 : 0;
                }

                lock (locker)
                {
                    requestCount--;
                    if (requestCount <= 0)
                        break;
                }
                System.Threading.Thread.Sleep(500);
            }
            sw.Stop();
            return tr;
        }
    }
}
