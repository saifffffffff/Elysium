using System;
using System.Collections.Generic;
using System.Text;

namespace Elysium.Domain.Models.Enums;

enum AiChatMessageStatus : byte
{
    Pending = 0,
    Answered = 1 ,
    Failed = 2 
}
