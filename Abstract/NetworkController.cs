using FFA.Empty.Empty.Network.Client;

public class NetworkController : GenericController
{
    public void PacketSetByServer(short p)
    {
        entity.SetPacketAsync(p);
    }

}
