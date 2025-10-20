namespace Witmina_MarineManagement
{
    public enum PortState
    {
        Idle,
        WaitingRequest,
        Working,
        WaitingLeave,
    }
    public enum BoatType
    {
        Regular,
        Event,
        Vip,
    }

    public enum PowerUpType
    {
        Score,
        Speed,
        Time,
        Boat,
    }

    public enum RequestType
    {
        Repair,
        Battery,
        Oil,
        Fuel,
        Cleaning,
        Food,
        Water,
        Waste,
        Security,
        Communication,
        Emergency,
        PassengerR,
        PassengerY,
        PassengerB,
    }

    public enum AudioFxType
    {
        CorrectPlay,
        FailPlay,
        ShipNear,
    }
}
