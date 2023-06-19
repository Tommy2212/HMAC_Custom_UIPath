using System;
using System.Activities;
using System.Collections.Generic;

using Hansae.HMAC.Activities;

namespace UnitTests
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            var rs = UnitTest();

            var rs2 = UnitTest2(rs);
            Console.WriteLine("result456 -------------->" + rs2);
        }

        private static string UnitTest2(string data)
        {
            var arguments = new Dictionary<string, object>
            {
                { "DataKey", "42f014507de3f8a1f976832af5798983bfc726d0" },
                { "AccessKeyID", "100dacmZc9pnqqpaZai" },
                { "SecretAccessKey", "fkksdo25phq3wyfrenambopvn7miv3jt2clfuva45a6og52wa5rvamiskeyn6kiy7kspeibogfe63gh5h6d4i7jw2l4qj3nf2tjajbq" },
                { "UserID", "DataApiAgent@5717989018077705" },
                { "Url", "https://network.infornexus.com/rest/3.1/OrderDetail/280115860466/actionSet/wf_approve" },
                { "FingerPrint", "e190a0f431f86942c2095e9b639ebb64" },
                { "Method", "POST" },
                { "Data", data }
            };

            var obj = new HmacRequest();

            var response = WorkflowInvoker.Invoke(obj, arguments, TimeSpan.FromSeconds(5));
            var result = response["Result"];
            var status = response["Status"];
            Console.WriteLine("result -------------->" + result);
            Console.WriteLine("status -------------->" + status);
            return Convert.ToString(result);
        }

        private static string UnitTest()
        {
            var arguments = new Dictionary<string, object>
            {
                { "DataKey", "42f014507de3f8a1f976832af5798983bfc726d0" },
                { "AccessKeyID", "100dacmZc9pnqqpaZai" },
                { "SecretAccessKey", "fkksdo25phq3wyfrenambopvn7miv3jt2clfuva45a6og52wa5rvamiskeyn6kiy7kspeibogfe63gh5h6d4i7jw2l4qj3nf2tjajbq" },
                { "UserID", "DataApiAgent@5717989018077705" },
                { "Url", "https://network.infornexus.com/rest/3.1/OrderDetail/280115860466" },
                { "FingerPrint", "e190a0f431f86942c2095e9b639ebb64" },
                { "Method", "GET" },
                { "Data", "Example" }
            };

            var obj = new HmacRequest();

            var response = WorkflowInvoker.Invoke(obj, arguments, TimeSpan.FromSeconds(5));
            var result = response["Result"];            
            var status = response["Status"];

            Console.WriteLine("result -------------->" + result);
            Console.WriteLine("status -------------->" + status);
            return Convert.ToString(result);
        }
    }
}
