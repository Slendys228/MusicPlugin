using HarmonyLib;
using System.Collections;
using UnityEngine;

namespace MusicPlugin
{
    public class MusicHandler : MonoBehaviour
    {
        private static GameObject IntroObject;
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
        private void Start()
        {
            GameObject IntroObject = GameObject.Find("IntroScreen");
            if (IntroObject)
            {
                IntroObject.GetComponent<AudioSource>().loop = true;
                LoadAndPlayAudio(IntroObject.GetComponent<AudioSource>());
            }
        }
        public void LoadAndPlayAudio(AudioSource _audioSource)
        {
            StartCoroutine(LoadAndPlayAudioCoroutine(_audioSource));
        }
        private IEnumerator LoadAndPlayAudioCoroutine(AudioSource audioSource)
        {
            if (File.Exists("C:/Program Files (x86)/Steam/steamapps/common/Content Warning/Plugins/MusicPlugin/mainmusic.mp3"))
            {
                using (var www = new WWW("file:///" + "C:/Program Files (x86)/Steam/steamapps/common/Content Warning/Plugins/MusicPlugin/mainmusic.mp3"))
                {
                    yield return www;

                    if (!string.IsNullOrEmpty(www.error))
                    {
                        Debug.LogError("Error load File: " + www.error);
                    }
                    else
                    {
                        audioSource.clip = www.GetAudioClip(false, true, AudioType.MPEG);
                        audioSource.Play();
                    }
                }
            }
            else
            {
                Debug.LogError("File not found.");
            }
        }
    }
    [ContentWarningPlugin("MusicPlugin", "0.1", false)]
    public class PluginTy
    {
        static PluginTy()
        {
            Debug.Log("Hello from MusicPlugin! This is called on plugin load");
        }
    }
    [HarmonyPatch(typeof(IntroScreenAnimator))]
    [HarmonyPatch("Start")]
    public class Patch
    {
        static bool Prefix()
        {
            GameObject handler = GameObject.Find("MusicHandler");
            if (!handler)
            {
                handler = new GameObject("MusicHandler");
                handler.AddComponent<MusicHandler>();
            }
            return true;
        }
    }
}