using Godot;
using System.Threading.Tasks;

public class ControllerPlayer : GenericController
{
    protected const short DOWN_MOVE =  0b0000000001;
    protected const short LEFT_MOVE =  0b0000000010;
    protected const short RIGHT_MOVE = 0b0000000100;
    protected const short UP_MOVE =    0b0000001000;
    protected const short DOWN_ATK =   0b0000010000;
    protected const short LEFT_ATK =   0b0000100000;
    protected const short RIGHT_ATK =  0b0001000000;
    protected const short UP_ATK =     0b0010000000;
    protected const short ITEM_USED =  0b0100000000;
    protected const short RESTING =    0b1000000000;

    public override void _Input(InputEvent eventInput)
    {

        if(global != null)
        {
            if (ScanInput())
            {
                short p = packet;//Having local variable prevents packet from being reset while the thread is being declared

                global.Network.client.SendPacketToServer(p); 
                packet = 0;
                GD.Print("[ControllerPlayer] Packet sent to server ### [M] -> UwU ###");
            }
        }
        else if(entity != null)
        {
            if (ScanInput())
            {
                short p = packet;

                entity.SetPacketAsync(p);
                packet = 0;
            }
        }
        else
        {
            GD.Print("[ControllerPlayer] ERROR : no valid owner found");
        }
    }

    private bool ScanInput()
    {
        bool pressed = false;
        if (Input.IsActionJustPressed("MoveDown"))
        {
            packet |= DOWN_MOVE;
            pressed = true;
        }

        if (Input.IsActionJustPressed("MoveLeft"))
        {
            packet |= LEFT_MOVE;
            pressed = true;
        }

        if (Input.IsActionJustPressed("MoveRight"))
        {
            packet |= RIGHT_MOVE;
            pressed = true;
        }

        if (Input.IsActionJustPressed("MoveUp"))
        {
            packet |= UP_MOVE;
            pressed = true;
        }
        if (Input.IsActionJustPressed("AtkDown"))
        {
            packet |= DOWN_ATK;
            pressed = true;
        }
        if (Input.IsActionJustPressed("AtkLeft"))
        {
            packet |= LEFT_ATK;
            pressed = true;
        }

        if (Input.IsActionJustPressed("AtkRight"))
        {
            packet |= RIGHT_ATK;
            pressed = true;
        }

        if (Input.IsActionJustPressed("AtkUp"))
        {
            packet |= UP_ATK;
            pressed = true;
        }


        return pressed;
    }

}
