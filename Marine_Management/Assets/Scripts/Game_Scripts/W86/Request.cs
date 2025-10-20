using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Witmina_MarineManagement
{
    public class Request
    {
        public Boat Boat;
        public Port Port;
        public RequestType Type;
        public int Tier;
        public float Timer;

        public Request(Boat boat, Port port, RequestType type, float timer)
        {
            Boat = boat;
            Port = port;
            Type = type;
            // Tier = tier;
            Timer = timer;
        }
    }
}
