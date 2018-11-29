///////////////////////////////////////////////////////////////////////////////
// HELPER METHODS
///////////////////////////////////////////////////////////////////////////////

public void SendMessageToGitterRoom()
{
    try
    {
        Information("Sending message to Gitter...");

        var postMessageResult = Gitter.Chat.PostMessage(
                    message: parameters.GitterMessage,
                    messageSettings: new GitterChatMessageSettings { Token = parameters.Gitter.Token, RoomId = parameters.Gitter.RoomId}
            );

        if (postMessageResult.Ok)
        {
            Information("Message {0} successfully sent", postMessageResult.TimeStamp);
        }
        else
        {
            Error("Failed to send message: {0}", postMessageResult.Error);
        }
    }
    catch(Exception ex)
    {
        Error("{0}", ex);
    }
}
