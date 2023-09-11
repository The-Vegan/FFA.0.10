using Godot;
using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace FFA.Empty.Empty.Network.Client
{
    public class LocalClient
    {
        public Global global;

        public bool connected = false;
        public long ping = 0;

        public bool pingPrint = false;

        private BaseClient client;

        public byte clientID = 0;

        private PlayerInfo[] players = new PlayerInfo[16];

        public PlayerInfo[] GetPlayersInfo() { return players; }

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
        public LocalClient(string ip)
        {
            client = new BaseClient();
            client.DataRecievedEvent += DataRecived;
            client.ClientDisconnectedEvent += Disconnected;
            client.ConnectToServer(ip, 1404);
            connected = true;
            GD.Print("[LocalClient] Connected to server successfully");
        }

        private void Disconnected(object sender)
        {
            global.ResetNetworkConfigAndGoBackToMainMenu();
            this.Disconnect();
        }

        private void DataRecived(object sender, byte[] data, NetworkStream stream)
        {
            if (data[0] == PING)
            {
                client.SendDataToServer(data);
                ping = (data[1] << 24) + (data[2] << 16) + (data[3] << 8) + data[4];
                return;
            }
            //try
            {
                if (global.isPlayingLevel)
                {
                    if (global.hasServer) return; //prevents server from self sabotage
                    switch (data[0])
                    {
                        case GAME_OVER:
                            GD.Print("[LocalClient] GAME OVER Recieved");
                            break;
                        case GAME_SOON_OVER:
                            GD.Print("[LocalClient] GAME SOON OVER Recieved");
                            break;
                        case SET_MOVES:
                            byte entityID = data[1];
                            short entityPacket = (short)((data[2] << 8) + data[3]);
                            float time = BitConverter.ToSingle(data, 4);

                            global.gMap.SetEntityPacketOnLevel(entityID, entityPacket, time);
                            break;
                        case SYNC_ENTITIES:
                            List<SyncEntityPacket> packets = SyncEntityPacket.ToSyncPacketList(data);
                            global.gMap.ResyncEntitiesInLevel(packets);
                            global.gMap.LevelTimerSync(this);
                            break;
                        case ITEM_GIVEN_BY_SERVER:
                            GD.Print("[LocalClient] Server gave you a thingy :3");
                            break;
                        case BLUNDERED_BY_SERVER:
                            GD.Print("[LocalClient] Server fliped you off");
                            break;
                        default:
                            GD.Print("[LocalClient] Error : Unkown protocol : " + data[0]);
                            return;
                    }
                }
                else
                {
                    switch (data[0])
                    {
                        case ABOUT_TO_LAUNCH:
                            GD.Print("[LocalClient] ABOUT_TO_LAUNCH recieved");
                            global.launchAborted = false;
                            global.CountDownTimer();
                            break;
                        case ABORT_LAUNCH:
                            GD.Print("[LocalClient] ABORT_LAUNCH recieved");
                            global.launchAborted = true;
                            break;
                        case LAUNCH:
                            GD.Print("[LocalClient] LAUNCH recieved");
                            if (data[1] != 0)
                            {
                                global.LoadMapFromID(data[1]);
                                Dictionary<byte, Vector2> IDToCoords = new Dictionary<byte, Vector2>(); ;
                                ushort offset = 2;
                                while ((data[offset] != 0) && ((offset + 5) < data.Length))
                                {
                                    byte id = data[offset];

                                    short x = (short)((data[offset + 1] << 8) + (data[offset + 2]));
                                    short y = (short)((data[offset + 3] << 8) + (data[offset + 4]));
                                    offset += 5;

                                    IDToCoords.Add(id, new Vector2(x, y));
                                }
                                SignalReady();
                            }
                            else throw new NotImplementedException("bruh,  I don't serialize maps yet");
                            break;
                        case SET_CLIENT_OR_ENTITY_ID:
                            GD.Print("[LocalClient] SET_CLIENT_OR_ENTITY_ID recieved ; clientID : " + data[1] + "\tcharID : " + data[2]);
                            this.clientID = data[1];
                            if (data[2] != 0) this.players[clientID - 1].characterID = data[2];
                            players[clientID - 1] = new PlayerInfo();
                            players[clientID - 1].clientID = clientID;
                            players[clientID - 1].characterID = 0;
                            SendCharIDAndName(players[clientID - 1].name, players[clientID - 1].characterID);
                            break;
                        case SEND_NAME_LIST:
                            GD.Print("[LocalClient] SEND_NAME_LIST recieved");
                            players = PlayerInfo.DeserialiseInfoArray(data);
                            global.DisplayPlayerList(players);
                            break;
                        case SET_LEVEL_CONFIG:
                            GD.Print("[LocalClient] SET_LEVEL_CONFIG recieved");
                            break;
                        case SERVER_FULL:
                            GD.Print("[LocalClient] Connexion Denied, Server full");
                            break;
                        case GAME_LAUNCHED:
                            GD.Print("[LocalClient] Connexion Denied, Game launched");
                            break;
                        default:
                            GD.Print("[LocalClient] recieved \"" + data[0] + "\"");
                            break;

                    }
                }

            }
            //catch (Exception e)
            {
            //    GD.Print("[LocalClient] Incoherent data,protocole : " + data[0] + " threw exception : " + e);
            }
        }

        //Menu
        //-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-\\
        public void SendCharIDAndName(string name, byte charID)
        {
            if (clientID == 0) return;
            try
            {
                if (String.IsNullOrEmpty(name)) name = "bob";
                else if (name.Length > 24) name = name.Substring(0, 24);

                GD.Print("[LocalClient] id:" + clientID + "players:" + players);
                players[clientID - 1].name = name;
                players[clientID - 1].characterID = charID;
                byte[] stream = new byte[8_192];
                stream[0] = SET_CHARACTER;
                byte[] playerAsBytes = players[clientID - 1].ToByte();
                for (ushort i = 0; i < playerAsBytes.Length && i < 24; i++) stream[i + 1] = playerAsBytes[i];

                client.SendDataToServer(stream);
            }
            catch(Exception e) { GD.Print("[LocalClient] [SendCharIDAndName]" + e); }
            
        }
        //-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-\\
        //Menu

        //Level
        //-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-\\
        public void SendPacketToServer(short move)
        {
            byte[] output = new byte[8_192];
            output[0] = MOVE;
            output[1] = clientID;
            output[2] = (byte)(move >> 8);
            output[3] = (byte)move;

            client.SendDataToServer(output);
            GD.Print("[LocalClient] Moves sent");
        }
        //-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-\\
        //Level
        public void Disconnect() { client.Disconnect(); connected = false; GD.Print("[LocalClient] Disconected"); }

        internal void SignalReady() { byte[] signal = new byte[8_192]; signal[0] = CLIENT_READY; signal[1] = clientID; client.SendDataToServer(signal); }
    }
}
