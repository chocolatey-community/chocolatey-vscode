///////////////////////////////////////////////////////////////////////////////
// HELPER METHODS
///////////////////////////////////////////////////////////////////////////////

public void SendMessageToTwitter()
{
    try
    {
        Information("Sending message to Twitter...");

        TwitterSendTweet(parameters.Twitter.ConsumerKey,
                         parameters.Twitter.ConsumerSecret,
                         parameters.Twitter.AccessToken,
                         parameters.Twitter.AccessTokenSecret,
                         parameters.TwitterMessage);

        Information("Message successfully sent.");
    }
    catch(Exception ex)
    {
        Error("{0}", ex);
    }
}
