using System;
using System.Activities;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Hansae.HMAC.Activities.Properties;
using UiPath.Shared.Activities;
using UiPath.Shared.Activities.Code;
using UiPath.Shared.Activities.Localization;
using Hansae.HMAC.Enums;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Net;
using System.Net.Http;
using System.Text;
// ReSharper disable CheckNamespace MemberCanBePrivate.Global PropertyCanBeMadeInitOnly.Global UnusedAutoPropertyAccessor.Global

namespace Hansae.HMAC.Activities;


/// <summary>
/// This is a regular (not scoped) activity for UiPath.
/// </summary>
[LocalizedDisplayName(nameof(Resources.HmacRequest_DisplayName))]
[LocalizedDescription(nameof(Resources.HmacRequest_Description))]
public class HmacRequest : ContinuableAsyncCodeActivity
{
    #region Properties - Everything in this section shows up in the 'Properties' panel in UiPath.

    /// <summary>
    /// ***OPTIONAL*** If set, continue executing the remaining activities even if the current activity has failed.
    /// </summary>
    [LocalizedCategory(nameof(Resources.Common_Category))]
    [LocalizedDisplayName(nameof(Resources.ContinueOnError_DisplayName))]
    [LocalizedDescription(nameof(Resources.ContinueOnError_Description))]
    public override InArgument<bool> ContinueOnError { get; set; }

    /// <summary>
    /// ***OPTIONAL*** If set, write debug log to the specified text file. The intention of this property is to be used during/// development, testing and remediation of bugs for this activity.  When testing from UiPath there is no stepping/// through activity code and this allows an activity developer to narrow down where defects are located within the/// code of the activity. It should not be used in production setting to record data pertaining to a particular RPA.
    /// </summary>
    [LocalizedCategory(nameof(Resources.Debug_Category))]
    [LocalizedDisplayName(nameof(Resources.DebugLog_DisplayName))]
    [LocalizedDescription(nameof(Resources.DebugLog_Description))]
    public InArgument<string> DebugLog { get; set; }

    /// <summary>
    /// ***OPTIONAL*** If set, write debug log to the specified text file. The intention of this property is to be used during/// development, testing and remediation of bugs for this activity.  When testing from UiPath there is no stepping/// through activity code and this allows an activity developer to narrow down where defects are located within the/// code of the activity. It should not be used in production setting to record data pertaining to a particular RPA.
    /// </summary>
    [LocalizedCategory(nameof(Resources.Input_Category))]
    [LocalizedDisplayName(nameof(Resources.Data_DisplayName))]
    [LocalizedDescription(nameof(Resources.Data_Description))]
    public InArgument<string> Data { get; set; }

    /// <summary>
    /// ***REQUIRED*** A sample string in argument named TextInput.
    /// </summary>
    [RequiredArgument]
    [LocalizedDisplayName(nameof(Resources.DataKey_DisplayName))]
    [LocalizedDescription(nameof(Resources.DataKey_Description))]
    [LocalizedCategory(nameof(Resources.Input_Category))]
    public InArgument<string> DataKey{ get; set; }

    /// <summary>
    /// ***REQUIRED*** A sample string in argument named TextInput.
    /// </summary>
    [RequiredArgument]
    [LocalizedDisplayName(nameof(Resources.UserID_DisplayName))]
    [LocalizedDescription(nameof(Resources.UserID_Description))]
    [LocalizedCategory(nameof(Resources.Input_Category))]
    public InArgument<string> UserID { get; set; }

    /// <summary>
    /// ***REQUIRED*** A sample string in argument named FolderPath.
    /// </summary>
    [RequiredArgument]
    [LocalizedDisplayName(nameof(Resources.AccessKeyID_DisplayName))]
    [LocalizedDescription(nameof(Resources.AccessKeyID_Description))]
    [LocalizedCategory(nameof(Resources.Input_Category))]
    public InArgument<string> AccessKeyID { get; set; }

    /// <summary>
    /// ***REQUIRED*** A sample string in argument named FilePath.
    /// </summary>
    [RequiredArgument]
    [LocalizedDisplayName(nameof(Resources.SecretAccessKey_DisplayName))]
    [LocalizedDescription(nameof(Resources.SecretAccessKey_Description))]
    [LocalizedCategory(nameof(Resources.Input_Category))]
    public InArgument<string> SecretAccessKey { get; set; }

    /// <summary>
    /// ***REQUIRED*** A sample string in argument named FilePath.
    /// </summary>
    [RequiredArgument]
    [LocalizedDisplayName(nameof(Resources.Url_DisplayName))]
    [LocalizedDescription(nameof(Resources.Url_Description))]
    [LocalizedCategory(nameof(Resources.Input_Category))]
    public InArgument<string> Url { get; set; }

        /// <summary>
    /// ***REQUIRED*** A sample string in argument named FilePath.
    /// </summary>
    [RequiredArgument]
    [LocalizedDisplayName(nameof(Resources.Method_DisplayName))]
    [LocalizedDescription(nameof(Resources.Method_Description))]
    [LocalizedCategory(nameof(Resources.Input_Category))]
    public InArgument<string> Method { get; set; }
          /// <summary>
    /// ***REQUIRED*** A sample string in argument named FilePath.
    /// </summary>
    [LocalizedDisplayName(nameof(Resources.FingerPrint_DisplayName))]
    [LocalizedDescription(nameof(Resources.FingerPrint_Description))]
    [LocalizedCategory(nameof(Resources.Input_Category))]
    public InArgument<string> FingerPrint { get; set; }
   /// <summary>
    /// ***OPTIONAL*** A sample string out argument containing a Message generated by this activity.
    /// </summary>
    [LocalizedDisplayName(nameof(Resources.Result_DisplayName))]
    [LocalizedDescription(nameof(Resources.Result_Description))]
    [LocalizedCategory(nameof(Resources.Output_Category))]
    public OutArgument<string> Result { get; set; }

    [LocalizedDisplayName(nameof(Resources.Status_DisplayName))]
    [LocalizedDescription(nameof(Resources.Status_Description))]
    [LocalizedCategory(nameof(Resources.Output_Category))]
    public OutArgument<int> Status { get; set; }


    private Logger _log;
    // ReSharper disable once NotAccessedField.Local
    private readonly bool _debugMode;
    
    #endregion


    #region Constructors

    /// <summary>
    /// Default constructor instantiates a new instance of this class.
    /// </summary>
    public HmacRequest() : this(false)
    {
    }

    /// <summary>
    /// If debug mode is set to true the activity may use some alternate logic to fill in gaps missing from UiPath/// in order to function correctly.
    /// </summary>
    /// <param name="debugMode">If true debug logic is applied.</param>    
    public HmacRequest(bool debugMode)
    {
        _debugMode = debugMode;
        // ReSharper disable once RedundantJumpStatement
        if (debugMode) return;
    }

    #endregion


    #region Protected Methods

    // ReSharper disable once RedundantOverriddenMember
    /// <summary>
    /// Implementing this method is required by UiPath.
    /// </summary>
    /// <param name="metadata">MetaData is provided by UiPath</param>
    protected override void CacheMetadata(CodeActivityMetadata metadata)
    {
        /* NOTE: This line has been known to cause intermittent false positives validation errors.
         * I recommend doing using the [RequiredArgument] attribute on your properties to enforce required arguments in
         * the UIPath GUI combined with strong validation in the ExecuteAsync method for more consistent results. */
        //if (FilePath == null) metadata.AddValidationError(string.Format(Resources.ValidationValue_Error, nameof(FilePath)));

        base.CacheMetadata(metadata);
    }

    /// <summary>
    /// This method executes the core body of logic.  Similar to the Main() method of a console application.
    /// </summary>
    /// <param name="context">The run context coming from UiPath.</param>
    /// <param name="cancellationToken">The cancellation indicator coming from UiPath</param>
    /// <returns></returns>
    /// <exception cref="NullReferenceException"></exception>
    /// <exception cref="FileNotFoundException"></exception>
    protected override async Task<Action<AsyncCodeActivityContext>> ExecuteAsync(AsyncCodeActivityContext context, CancellationToken cancellationToken)
    {
        #region Optional Logging

        var debugLog = DebugLog.Get(context);
        if (!string.IsNullOrEmpty(debugLog))
        {
            _log = new Logger(debugLog);
        }

        #endregion
        
        #region Get your input values and set them to local variables.

        var datakey = DataKey.Get(context);
        var user = UserID.Get(context);
        var accessKey = AccessKeyID.Get(context);
        var secretKey = SecretAccessKey.Get(context);
        var method = Method.Get(context);
        var apiRequestUri = Url.Get(context);
        var fingerprint = FingerPrint.Get(context);
        var data = Data.Get(context);
        string result;
        int status;
        
        #endregion
        
        #region Validations

        _log?.Write("Validating...");
        if (string.IsNullOrEmpty(user)) throw new NullReferenceException("The property 'UserID' cannot be null or empty.");
        if (string.IsNullOrEmpty(accessKey)) throw new FileNotFoundException("The property 'AccessKeyID' cannot be null or empty.");
        if (string.IsNullOrEmpty(secretKey)) throw new FileNotFoundException("The property 'SecretAccessKey' cannot be null or empty.");
        if (string.IsNullOrEmpty(method)) throw new FileNotFoundException("The property 'Method' cannot be null or empty.");
        if (string.IsNullOrEmpty(apiRequestUri)) throw new FileNotFoundException("The property 'Url' cannot be null or empty.");
        if (string.IsNullOrEmpty(datakey)) throw new FileNotFoundException("The property 'DataKey' cannot be null or empty.");
        _log?.WriteLine("...completed!");

        #endregion

        #region Added execution logic HERE

        _log?.WriteLine("Made it to execution logic.");
            string[] args = {"user", user,"accessKey",accessKey, "secret",secretKey, "url", apiRequestUri,"datakey",datakey, "method",method, "data",data};

            Dictionary<String,String> programArguements = parseArgs(args);
            Console.WriteLine("URL - " + apiRequestUri);
            String payload = null;
            if(method.ToUpper().Equals("POST")){
                if(programArguements.ContainsKey("data")) {
                    payload = programArguements["data"];
                } else {
                    Console.WriteLine("POST method requires arguement -data");
                    Environment.Exit(0);
                }
            }

            apiRequestUri = encodeUri(apiRequestUri);

            String xDapiDate = computeXDapiDate();
            String signingBase = createSigningBase(apiRequestUri, method, xDapiDate, payload);
            String signature = createSignature(signingBase, secretKey);
            String hmacAuthorization = createHmacAuthorization(user, accessKey, signature);
            Console.WriteLine("SIGNING BASE : {0}", signingBase);
            Console.WriteLine("Authorization Signature : {0}", signature);
            Console.WriteLine("Hmac Authorization : {0}", hmacAuthorization);
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            HttpClient client = new HttpClient();

            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
            client.DefaultRequestHeaders.Add("Authorization", hmacAuthorization);
            client.DefaultRequestHeaders.Add("ContentType", "application/json; charset=UTF-8");
            client.DefaultRequestHeaders.Add("datakey", datakey);
            client.DefaultRequestHeaders.Add("x-dapi-date", xDapiDate);
            


        if (method.ToUpper().Equals("POST"))
            {
                Console.WriteLine("START -- DefaultRequestHeaders");
                client.DefaultRequestHeaders.TryAddWithoutValidation("If-Match", "\"" + fingerprint + "\"" );
                Console.WriteLine("DefaultRequestHeaders------" + client.DefaultRequestHeaders);
                HttpContent content = new StringContent(data, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(apiRequestUri, content);
                HttpStatusCode statusCode = response.StatusCode;
                string resultJSON = await response.Content.ReadAsStringAsync();         
                result = resultJSON;
                Console.WriteLine("resultJSON POST------" + resultJSON);
                status= (int)statusCode;
                Console.WriteLine("statusCode POST------" + statusCode);

            }
            else
            {
                HttpResponseMessage response = await client.GetAsync(apiRequestUri);
                string resultJSON = await response.Content.ReadAsStringAsync();
                Console.WriteLine("resultJSON GET------" + resultJSON);
                HttpStatusCode statusCode = response.StatusCode;
                result = resultJSON;
                status= (int)statusCode;
                Console.WriteLine("statusCode POST------" + statusCode);

            }

        _log?.WriteLine("Compiling return message.");
        

        _log?.WriteLine("...done.");
        
        #endregion

        #region Set any output values here to return to UiPath Studio.

        _log?.Write("Returning to UiPath.");
        return (ctx) => 
        {
            Result.Set(ctx, result);
            Status.Set(ctx, status);

        };

        
        #endregion
    }

        static String createHmacAuthorization(String userId, String accessKey, String signature) {
            return "HMAC_1 " + accessKey + ":" + signature + ":" + userId;
        }

        static String createSignature(String signingBase, String secretKey) {
            byte[] k = System.Text.ASCIIEncoding.UTF8.GetBytes(secretKey);
            HMACSHA256 myhmacsha256 = new HMACSHA256 (k);
            Byte[] dataToHmac = System.Text.Encoding.UTF8.GetBytes(signingBase);
            string signature = Convert.ToBase64String(myhmacsha256.ComputeHash(dataToHmac));
            return signature;
        }

        static String createSigningBase(String url, String method, String xDapiDate, String data) {
            string baseStr = xDapiDate.ToLower() + method.ToLower();
            string path = url.Replace("https://", "");
            path = path.Substring(path.IndexOf("/"));
            baseStr += path.ToLower();
            if(method.ToUpper().Equals("POST")){
                baseStr += data.ToLower();
            }
            return baseStr;
        }

        static String computeXDapiDate(){
            String isoDate = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
            return isoDate;
        }

        static Dictionary<String,String> parseArgs(string[] args) {
            Dictionary<String,String> argMap = new Dictionary<String,String>();
            String key;
            for(int i = 0; i < args.Length - 1 ; i++) {
                if(i % 2 == 0 ) {
                    key = args[i].Replace("-", "");
                    argMap[key] = args[i+1];
                }
            }
            validateProgramArgs(argMap);
            return argMap;
        }

        static string[] expectedArgs = {"user", "accessKey", "secret", "url", "datakey", "method"};
        static void validateProgramArgs(Dictionary<String,String> programArgs) {
            for(int i =0; i < expectedArgs.Length; i++) {
                if(! programArgs.ContainsKey(expectedArgs[i])) {
                    Console.WriteLine("Missing arg " + expectedArgs[i]);
                    Environment.Exit(0);
                }
            }
        }

        static String encodeUri(String url) {
            if(url.IndexOf("oql=") > -1) {
                Regex reg = new Regex("oql=(.*?)(?=&|$)");
                String param = reg.Match(url).Value;
                Console.WriteLine(param);
                url = url.Replace(param, Uri.EscapeDataString(param));
                url = url.Replace("oql%3D", "oql=");
            }
            return url;
        }

    #endregion
}
