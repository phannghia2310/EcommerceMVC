using System;
using System.Collections.Generic;

namespace EcommerceMVC.Data;

public partial class Message
{
    public int Id { get; set; }

    public string FromUser { get; set; } = null!;

    public string ToUser { get; set; } = null!;

    public string Message1 { get; set; } = null!;

    public DateTime Timestamp { get; set; }

    public int? IsRead { get; set; }
}
