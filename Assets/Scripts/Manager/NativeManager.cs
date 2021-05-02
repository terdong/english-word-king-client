using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Manager
{
    class NativeManager : MonoBehaviour
    {
        public static NativeManager Instance { get; private set; }

        void Awake()
        {
            Instance = this;
            Initialized();
        }

        void OnDestroy()
        {
            if (Instance != null)
            {
                Instance = null;
            }
        }

#if UNITY_ANDROID
        // 유니티가 동작하는 액티비티를 저장하는 변수.
        public AndroidJavaObject activity = null;

        void Initialized()
        {
            AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            activity = jc.GetStatic<AndroidJavaObject>("currentActivity");
        }
#else
        void Initialized() { }
#endif 

    }
}
