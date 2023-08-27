using Godot;
using System;
using System.Collections.Generic;

namespace FFA.Empty.Empty.Network.Client
{
    public class SyncEntityPacket
    {
        //Packet Constants
        //-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-\\
        private const byte PING = 0;
        /*CLIENT -> SERVER*/
        private const byte MOVE = 1;
        private const byte SET_CHARACTER = 2;
        private const byte CLIENT_READY = 3;
        /*SERVER -> CLIENT*/
        private const byte SERVER_FULL = 127;
        private const byte GAME_LAUNCHED = 128;
        //Pre launch
        private const byte ABOUT_TO_LAUNCH = 255;
        private const byte ABORT_LAUNCH = 254;
        private const byte LAUNCH = 253;
        private const byte SET_CLIENT_OR_ENTITY_ID = 252;
        private const byte SEND_NAME_LIST = 251;
        private const byte SET_LEVEL_CONFIG = 250;
        //Post launch
        private const byte GAME_OVER = 249;
        private const byte GAME_SOON_OVER = 248;
        private const byte SET_MOVES = 247;
        private const byte SYNC_ENTITIES = 246;
        private const byte ITEM_GIVEN_BY_SERVER = 245;
        private const byte BLUNDERED_BY_SERVER = 244;
        //-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-\\
        //Packet Constants

        public byte entityID;
        public Vector2 position;
        public short health;
        public sbyte blunderbar;
        public sbyte itembar;
        public byte heldItemID;

        public SyncEntityPacket(Entity entity)
        {
            this.entityID = entity.id;
            this.position = entity.pos;
            this.health = entity.GetHealthPoint();
            this.blunderbar = entity.GetBlunder();
            this.itembar = entity.GetItembar();
            this.heldItemID = entity.itemID;
        }
        public SyncEntityPacket(byte[] data, ushort offset) 
        {
            this.entityID = data[offset];

            this.position = new Vector2(((data[offset + 1] << 8) + data[offset + 2]), ((data[offset + 3] << 8) + data[offset + 4]));

            this.health = (short)((data[offset + 5] << 8) + data[offset + 6]);

            this.blunderbar = (sbyte)data[offset + 7];
            this.itembar = (sbyte)data[offset + 8];
            this.heldItemID = data[offset + 9];

        }

        public byte[] ToByte()
        {
            byte[] outstream = new byte[8_192];

            outstream[0] = SYNC_ENTITIES;
            outstream[1] = entityID;

            outstream[2] = (byte)((byte)position.x >> 8); outstream[3] = (byte)position.x;
            outstream[4] = (byte)((byte)position.y >> 8); outstream[5] = (byte)position.y;

            outstream[6] = (byte)(health >> 8); outstream[7] = (byte)health;

            outstream[8] = (byte)blunderbar;
            outstream[9] = (byte)itembar;
            outstream[10] = heldItemID;

            return outstream;
        }

        public static byte[] ToByteArray(List<SyncEntityPacket> packets)
        {
            byte[] stream = new byte[8_192];

            stream[0] = SYNC_ENTITIES;
            ushort offset = 1;
            for(byte i = 0; i < packets.Count; i++)
            {
                stream[offset] = packets[i].entityID;

                stream[offset + 1] = (byte)((byte)packets[i].position.x >> 8); stream[offset + 2] = (byte)packets[i].position.x;
                stream[offset + 3] = (byte)((byte)packets[i].position.y >> 8); stream[offset + 4] = (byte)packets[i].position.y;

                stream[offset + 5] = (byte)(packets[i].health >> 8); stream[offset + 6] = (byte)packets[i].health;

                stream[offset + 7] = (byte)packets[i].blunderbar;
                stream[offset + 8] = (byte)packets[i].itembar;
                stream[offset + 9] = packets[i].heldItemID;
                offset += 10;
            }
            return stream;
        }


        public static List<SyncEntityPacket> ToSyncPacketList(byte[] data)
        {
            ushort offset = 1;
            List<SyncEntityPacket> retList = new List<SyncEntityPacket>();

            try
            {
                while (data[offset] != 0)
                {
                    retList.Add(new SyncEntityPacket(data, offset));
                    offset += 10;
                }
            }
            catch (IndexOutOfRangeException) { GD.Print("[SyncPacket] how many entities do you even have??? " + retList.Count); }

            return retList;
        }

    }
}
