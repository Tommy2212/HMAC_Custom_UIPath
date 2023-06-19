// using System.Activities;
// using Hansae.HMAC.Activities;
// using Hansae.HMAC.Enums;

// // ReSharper disable UnusedVariable

// namespace UnitTests;

// internal static class Program2
// {
//     public static void Main(string[] args)
//     {
//         UnitTest();
//     }
//     private static void UnitTest()
//     {    
        
//         #region Your activity properties get set in this section.
        
//         // Anything that is a proper InArgument, OutArgument, or InOutArgument
//         // get set here as part of a data dictionary.
//         var arguments = new Dictionary<string, object>
//         {   
//             { "DataKey", "42f014507de3f8a1f976832af5798983bfc726d0" },
//             { "AccessKeyID", "100dacmZc9pnqqpaZai" },
//             { "SecretAccessKey", "fkksdo25phq3wyfrenambopvn7miv3jt2clfuva45a6og52wa5rvamiskeyn6kiy7kspeibogfe63gh5h6d4i7jw2l4qj3nf2tjajbq" },
//             { "UserID", "DataApiAgent@5717989018077705" },
//             { "Url", "https://network.infornexus.com/rest/3.1/OrderDetail/280110436665" },
//             { "FingerPrint", "fbc225984165f2df85ec35be25090055" },
//             { "Method", "GET" },
//             { "Data", "Example"}
            
            
//         };

//         // Everything else like enums and types get set here and pass directly into the activity
//         // and do not need to be retrieved via the .Get(CancellationToken) like argument values.
//         var obj = new HmacRequest(true)
//         {
//             // RelativePronoun = TestEnum.This,
//             // DataType = typeof(string)
//         };
        
//         #endregion

//         // This line is where ExecuteAsync gets invoked.
//         var response = WorkflowInvoker.Invoke(obj, arguments, new TimeSpan(0,0,0, 5));

//         // These are where your OutArguments and InOutArgument values are retrieved. 
//         var result = response["Result"];

//         Console.WriteLine("result2 -------------->" + result);






//     }
// }
