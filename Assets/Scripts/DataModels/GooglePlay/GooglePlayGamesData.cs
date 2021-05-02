using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.DataModels
{
    public class GooglePlayGamesData
    {
        private bool mAuthenticating = false;

        public bool Authenticating
        {
            set
            {
                mAuthenticating = value;
            }
            get
            {
                return mAuthenticating;
            }
        }
        public bool Authenticated
        {
            get
            {
                return Social.Active.localUser.authenticated;
            }
        }

        public bool IsAuthenticate()
        {
            if (Authenticated || mAuthenticating)
            {
                Debug.LogWarning("Ignoring repeated call to Authenticate().");
                return true;
            }else
            {
                return false;
            }
        }
    }
}
