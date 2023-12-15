using GameNetcodeStuff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace conchaSuMare.Objects
{
    internal class PlayerSoundStatus
    {
        public bool _status;
        public PlayerControllerB _player;

        ///<summary>
        ///Define the status of the player refered to the sound and death management
        ///false -> Death by fall and pending to reproduce sound 
        ///true -> Death by fall and already reproduced sound
        ///</summary>
        public bool Status   
        {
            get { return _status; }
            set { _status = value; }
        }
        private PlayerControllerB Player
        {
            get { return _player; }
            set {  _player = value; }
        }

        public PlayerSoundStatus(PlayerControllerB player)
        {
            Player = player;
            Status = false;
        }
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            PlayerSoundStatus other = (PlayerSoundStatus)obj;
            return Player.Equals(other.Player);
        }

    }
}
