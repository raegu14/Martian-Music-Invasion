using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Concrete implementation of the ILogWriter interface which writes
/// log entries to a [remote] web service via HTTP POST protocol.
/// </summary>
/// Change History:
/// 2013/07/17: Upgrade to Unity 4 broke logging, replaced body of code with
///             Beanstalk logger which is based on working code model from
///             the CS2N API.
/// 2013/03/04: Fixed bug in HelloWorld URL
/// 2012/11/08: Added/updated comments.
/// </remarks>
public class HttpWriter : ILogWriter
{
	private enum ReadyState {
		/// <summary>
		/// Starting state
		/// </summary>
		Initializing,
		/// <summary>
		/// Called logging service, waiting for reply
		/// </summary>
		WakingServiceUp,
		/// <summary>
		/// Logging service replied to initial wake up call.
		/// </summary>
		CommunicationVerified,
		/// <summary>
		/// HttpWriter is ready for logging.
		/// </summary>
		Ready,
		/// <summary>
		/// Logging service failed to reply to wake up call, log writing disabled.
		/// </summary>
		TimeoutGivingUp
	}
	
    /// <summary>
    /// The shared secret is a string which is known by both the remote web service and 
    /// the local application intended to prevent casual spamming of the web service
    /// via drive-by attacks.  Since it is sent in the clear, it can be easily gleaned 
    /// using a wire sniffer.  If we use HTTPS, we gain some protection here, but also 
    /// have SSL overhead and certificate issues to deal with.
    /// </summary>
    private const string SHARED_SECRET = "UgkLuhakJbBSAczdSisJHdn0PM02mK6W";

    /*public static string Url()
    {
        string result = Application.absoluteURL;

        if (result.Length < 4)
        {
            return "http://127.0.0.1:8000";
        }

        //removing "/MyGame.unity3D" 
        for (int i = result.Length - 1; i >= 0; --i)
        {
            if (result[i] == '/')
            {
                return result.Remove(i);
            }
        }

        return "";
    }*/

    public static string Url()
    {
        string result = Application.absoluteURL;
        int i;
        for (i = 8; i < result.Length; i++)
        {
            if (result[i] == '/')
            {
                break;
            }
        }
        result = result.Remove(i);
        return result + "/cgi-bin/";
    }

    private enum LogDestination
    {
        ExternalServerPHP,
        ExternalServerCGI,
        InternalServerCGI
    };

    // // // // // // // // // // // // // // // // // // //
    // // // // // // //
    //
    private static LogDestination Dest = LogDestination.ExternalServerCGI;
    //
    // // // // // //
    // // // // // // // // // // // // // // // // // // //

    private static Dictionary<LogDestination, string> DestToUrl = new Dictionary<LogDestination, string>()
    {
        {LogDestination.ExternalServerCGI, "http://cbarker.net/cgi-bin"},
        {LogDestination.ExternalServerPHP, "http://cbarker.net/mmi/log/message.php"},
        //{LogDestination.InternalServerCGI, "http://208.40.145.43/cgi-bin/"}
        //{LogDestination.InternalServerCGI, Url()}
    };

    private string LOGGING_SERVICE_URL = DestToUrl[Dest];

    /// <summary>
    /// The number of seconds the system will queue messages before
    /// assuming the server will be able to handle them.
    /// </summary>
    private float timeout = 20.0f;
	
	/// <summary>
	/// WWW instance used to check for communications readiness.
	/// </summary>
	private WWW helloWorld;
	
	/// <summary>
	/// Indicates the current state of the HttpWriter
	/// </summary>
	private ReadyState state = ReadyState.Initializing;
	
	/// <summary>
	/// Holds messages waiting to be sent to the log server.
	/// </summary>
	private Queue<string> messageQueue = new Queue<string>();
	
	public HttpWriter (){
        startTime = DateTime.Now.Ticks;
    }

    public static long AverageWaitTime
    {
        get
        {
            if (totalRequests == 0)
            {
                return 0;
            } else
            {
                return (totalReceive - totalSend) / (totalRequests * TimeSpan.TicksPerMillisecond);
            }
        }
    }

    public static long totalRequests = 0;
    public static long totalSend = 0;
    public static long totalReceive = 0;
    public static long startTime;

    /// <summary>
    /// Writes the given message to the web service. 
    /// </summary>
    /// <param name="message">The string to be written to the web service.</param>
    public void Write(string message) {
        
        //Debug.Log(LOGGING_SERVICE_URL);
        if (this.state == ReadyState.Initializing) {
            Debug.Log("Trying to wake up...");
			WakeUpService();
			this.state = ReadyState.WakingServiceUp;
		}
		
		if (this.state == ReadyState.WakingServiceUp) {
			if (Time.time > this.timeout) {
				// Drat!  Didn't hear back from service, so give up and don't queue
				// messages further, clear queue, don't log, don't try to recover.
				this.state = ReadyState.TimeoutGivingUp;
				this.messageQueue.Clear();
			}
			else { 
				// keep state as WakingServiceUp until our timeout reached or we get ack 
				// back from service moving us to state of CommunicationVerified:
				// queue messages during this period
				this.messageQueue.Enqueue(message);
			}
		}
		else if (this.state == ReadyState.CommunicationVerified) {
			// Service is there.  Dequeue all that we have and send those off, 
			// then mark state as Ready so that queue no longer needs to be used.
			while (this.messageQueue.Count > 0) {				
				this.SendMessage(this.messageQueue.Dequeue());
			}
			this.state = ReadyState.Ready;
		}
		
		if (this.state == ReadyState.Ready) {
			this.SendMessage(message);
		}
	}


    static string[] messageBatch = new string[128];
    static int batchIndex = 0;
    static bool flushing;

    public static void Flush()
    {
        flushing = true;
    }

	/// <summary>
	/// Handles the actual HTTP message transmission.
	/// </summary>
	/// <param name='message'>
	/// The message to be sent.
	/// </param>
	private void SendMessage(string message) {
        //Debug.Log(LOGGING_SERVICE_URL);

        messageBatch[batchIndex++] = message;

        if (flushing)
        {
            message = "";
            for (int i = 0; i < batchIndex; i++) {
                message += messageBatch[i];
            }
            batchIndex = 0;
            flushing = false;
        } else if (batchIndex < messageBatch.Length) {
            return;
        } else
        {
            message = string.Join("\n", messageBatch);
            batchIndex = 0;
        }

        long st = (DateTime.Now.Ticks) - startTime;

    WWWForm myForm = new WWWForm();
		myForm.AddField("secret", HttpWriter.SHARED_SECRET);
		myForm.AddField("message", message);
		byte[] byteData = myForm.data;
		Dictionary<string, string> headers = new Dictionary<string, string>(myForm.headers);
		WebService.request(LOGGING_SERVICE_URL + "/LogString", byteData, headers, 
            ((string response) => WebRequestOK(response, st)),
            ((string error) => MessageRequestError(error, message, null, st)));
	}

    private void MessageRequestOK(string response, string guid, long st)
    {
        Debug.Log(string.Format("Message successfully sent after {0} tries", (retryCount + 1) - RetryBuffer[guid]));
        RetryBuffer.Remove(guid);

        totalRequests++;
        totalSend += st;
        totalReceive += (DateTime.Now.Ticks) - startTime;

    }

	private void WebRequestOK(string response, long st) {
        totalRequests++;
        totalSend += st;
        totalReceive += (DateTime.Now.Ticks) - startTime;
    }

    private void HelloResponseOK(string message)
    {
        if (this.state == ReadyState.WakingServiceUp)
        {
            this.state = ReadyState.CommunicationVerified;
            Debug.Log("Woke up");
        }
    }

    private int retryCount = 5;

    private Dictionary<string, int> RetryBuffer = new Dictionary<string, int>();

    private void MessageRequestError(string error, string message, string guid, long st) {
        Debug.LogWarning("[Retrying] SendMessage Failed: " + error);
        Debug.LogWarning("[Retrying] -- Message: " + message);
        if (guid == null)
        {
            guid = Guid.NewGuid().ToString();
            RetryBuffer.Add(guid, retryCount);
        } else
        {
            int triesLeft = -1;
            RetryBuffer.TryGetValue(guid, out triesLeft);
            if (triesLeft <= 0)
            {
                Debug.LogWarning(string.Format("FAILED TO SEND MESSAGE AFTER {0} TRIES: {1}", retryCount, message));
                return;
            }
            else if (triesLeft == 0)
            {
                Debug.LogWarning("Bad guid for message " + message);
            } else
            {
                RetryBuffer[guid] = triesLeft - 1;
            }
        }

            WWWForm myForm = new WWWForm();
            myForm.AddField("secret", HttpWriter.SHARED_SECRET);
            myForm.AddField("message", message);
            byte[] byteData = myForm.data;
            Dictionary<string, string> headers = new Dictionary<string, string>(myForm.headers);
            WebService.request(LOGGING_SERVICE_URL + "/LogString", byteData, headers,
                ((string response) => MessageRequestOK(response, guid, st)),
                ((string e) => MessageRequestError(e, message, guid, st)));
        }

	private void WebRequestErr(string error) {
        //Debug.Log(LOGGING_SERVICE_URL);
        if (this.state == ReadyState.WakingServiceUp) {
			this.state = ReadyState.TimeoutGivingUp;
			this.messageQueue.Clear();
		}
        Debug.Log("Web request not ok: " + error);
        // NOTE:  not worrying about a lost logged message: !!!TBD!!! should we be? Only concerned about first HelloWorld success or not.
    }
		
	private void WakeUpService() {
        // SOAP Service expects a FORM payload even if no arguments need to be supplied.
        //Debug.Log(LOGGING_SERVICE_URL);
        WWWForm myForm = new WWWForm();
		myForm.AddField("", "");
        //Debug.Log(LOGGING_SERVICE_URL);

        byte[] byteData = myForm.data;
		Dictionary<string, string> headers = new Dictionary<string, string>(myForm.headers);
		WebService.request(LOGGING_SERVICE_URL + "/HelloWorld", byteData, headers, HelloResponseOK, WebRequestErr);
		
		// Set timeout counter to a point in the future.
		this.timeout += Time.time;
	}
}