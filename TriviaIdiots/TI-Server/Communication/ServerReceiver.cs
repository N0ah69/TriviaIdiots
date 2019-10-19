﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TI_Server.Players;

namespace TI_Server.Communication
{
    class ServerReceiver : IReceiver
    {
        public Server Server;
        public ServerClient client;
        public ServerReceiver(Server server, ServerClient client)
        {
            this.Server = server;
            this.client = client;
        }

        public async Task handlePackage(string message)
        {
            string[] data = Regex.Split(message, "``");

            switch (data[0])
            {
                case "Connect":
                    string name = data[1];
                    client.player = new Player(this.client, name);
                    break;
                case "QuestionRequest":
                    string room1 = data[1];

                    ApiRequest request = new ApiRequest();
                    Question q1 = await request.getQuestion();

                    ServerRoom roomA = getRoom(room1);
                    foreach (Player player in roomA.players)
                    {
                        player.sendQuestion(q1);
                    }
                    break;
                case "RoomConnect":
                    string room2 = data[1];

                    if (this.Server.RoomExists(room2))
                    {
                        ServerRoom roomConnect = this.Server.GetRoom(room2);
                        roomConnect.AddPlayer(this.client.player);
                    }

                    break;
                case "RoomShow":
                    Server.sendRoomNamesToClients();
                    break;
                case "RoomCreate":
                    this.client.CreateRoom();
                    Server.sendRoomNamesToClients();
                    break;
                case "RoomStart":
                    string room3 = data[1];

                    ServerRoom roomB = getRoom(room3);
                    roomB.startRoom();

                    ApiRequest request1 = new ApiRequest();
                    Question q2 = await request1.getQuestion();

                    this.client.player.sendQuestion(q2);

                    break;
                case "Answer":
                    string player1 = data[1];
                    string question = data[2];
                    string givenAnswer = data[3];



                    break;
            }
        }

        public ServerRoom getRoom(string roomName)
        {
            if (this.Server.RoomExists(roomName))
            {
                return this.Server.GetRoom(roomName);
                
            }
            return null;
        }
    }
}
