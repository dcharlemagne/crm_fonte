using System;

namespace Intelbras.CRM2013.Domain.Enum
{
    [Serializable]
    public enum StatusLiveChatTracking
    {
        Vazio = 0,

        //Status: Open
        Waiting = 1,
        Transfered = 993400002,
        Chatting = 993400000,

        //Status: Completed
        Completed = 2,
        Not_Answered = 993400003,
        Out_of_work_business_hours = 993400004,
        Queue_limit_exceeded = 993400005,

        //Status: Canceled
        Canceled = 3,

        //Status: Scheduled
        Scheduled = 4
    }
}
