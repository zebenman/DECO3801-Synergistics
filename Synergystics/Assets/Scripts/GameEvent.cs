using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class GameEvent
{
    public int EventID { get; protected set; } = -1;
    public string EventDescriptor { get; protected set; } = "Default event descriptor";

    public GameEvent(int eid, string descriptor)
    {
        EventID = eid;
        EventDescriptor = descriptor;
    }
}
