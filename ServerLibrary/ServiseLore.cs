﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace ServerLibrary
{

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class LoreServise : IServiseForServer
    {
        int nextId = 1;
        List<ServerUser> users = new List<ServerUser>();

        public void Connect(string connectlogin, string connectpassword)
        {
            ServerUser user = new ServerUser()
            {
                Id = nextId,
                Login = connectlogin,
                Password = connectpassword
            };
            nextId++;
            SendStringMessage(DateTime.Now.ToString() + " Hello " + user.Login);
            users.Add(user);
        }
        public void Disconnect(int userId)
        {
            var user = users.Find(x => x.Id == userId);
            if (user != null)
            {
                users.Remove(user);
                SendStringMessage(DateTime.Now.ToString() + " Bye " + user.Login);
            }
        }

        public void DoWork()
        {
        }

        public void SendStringMessage(string message)
        {
            foreach (var user in users)
            {
                user.OperContext.GetCallbackChannel<IServiseForServerCallback>().ReceiveLoreMessage(message);
            }
        }
    }
}
