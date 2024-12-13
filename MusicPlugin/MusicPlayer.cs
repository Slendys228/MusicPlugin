using HarmonyLib;
using System.Collections;
using UnityEngine;

namespace MusicPlugin
{
    public class MusicHandler : MonoBehaviour
    {
        private static GameObject IntroObject;
        private static string playerMusicPath = Directory.GetCurrentDirectory().Replace("\\", "/") + "/Plugins/MusicPlugin/Music";
        private string musicPath = Directory.GetCurrentDirectory().Replace("\\", "/") + "/Plugins/MusicPlugin/";
        private string[] musicList;
        private AudioSource musicSource;
        private Vector2 scrollPosition;
        private bool viewPlayer = false;
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            Debug.Log(musicPath);
        }
        private void Start()
        {
            GameObject IntroObject = GameObject.Find("IntroScreen");
            if (IntroObject)
            {
                IntroObject.GetComponent<AudioSource>().loop = true;
                LoadAndPlayAudio(IntroObject.GetComponent<AudioSource>());
            }
            if (Directory.Exists(playerMusicPath))
            {
                musicSource = gameObject.AddComponent<AudioSource>();
                musicList = Directory.GetFiles(playerMusicPath);
            }
        }
        private void OnGUI()
        {
            if (GUILayout.Button("View Player"))
            {
                viewPlayer = !viewPlayer;
            }
            if (viewPlayer)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                GUILayout.BeginArea(new Rect(Screen.width / 2 - 150, Screen.height / 2 - 100, 300, 200));
                GUILayout.BeginVertical("box");
                scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(300), GUILayout.Height(150));
                if (musicList != null)
                {
                    GUILayout.Box($"Volume: {musicSource.volume}");
                    foreach (var Music in musicList)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Box(Path.GetFileName(Music));
                        if (GUILayout.Button("Play"))
                        {
                            gameObject.GetComponent<MusicHandler>().LoadAndPlayAudio(musicSource, Music);
                        }
                        GUILayout.EndHorizontal();
                    }
                }
                GUILayout.EndScrollView();
                musicSource.volume = GUILayout.HorizontalSlider(musicSource.volume, 0, 1);
                if (GUILayout.Button("Stop"))
                {
                    musicSource.Stop();
                }
                GUILayout.EndVertical();
                GUILayout.EndArea();
            }
        }
        public void LoadAndPlayAudio(AudioSource _audioSource, string FileName = "mainmusic.mp3")
        {
            StartCoroutine(LoadAndPlayAudioCoroutine(_audioSource));
        }
        private IEnumerator LoadAndPlayAudioCoroutine(AudioSource audioSource, string FileName = "mainmusic.mp3")
        {
            if (File.Exists(musicPath + FileName))
            {
                using (var www = new WWW("file:///" + musicPath + FileName))
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
    [ContentWarningPlugin("MusicPlugin", "1.2.1", false)]
    public class PluginTy
    {
        static PluginTy()
        {
            Debug.Log("MusicPlugin load.");
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