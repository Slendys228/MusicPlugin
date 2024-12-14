using HarmonyLib;
using System.Collections;
using UnityEngine;

namespace MusicPlugin
{
    public class MusicHandler : MonoBehaviour
    {
        private static string playerMusicPath = Directory.GetCurrentDirectory().Replace("\\", "/") + "/Plugins/MusicPlugin/Music/";
        private string musicPath = Directory.GetCurrentDirectory().Replace("\\", "/") + "/Plugins/MusicPlugin/";
        private string[]? musicList;
        private string musicName = "None";
        private string? musicUrl;
        private AudioSource? musicSource;
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
                LoadAndPlayAudio(IntroObject.GetComponent<AudioSource>(), null);
            }
            if (Directory.Exists(musicPath))
            {
                musicSource = gameObject.AddComponent<AudioSource>();
                musicList = Directory.GetFiles(playerMusicPath);
                foreach (var Name in musicList)
                {
                    Debug.Log(Name);
                }
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
                GUILayout.BeginArea(new Rect(Screen.width / 2 - 150, Screen.height / 2 - 100, 310, 300));
                GUILayout.BeginVertical("box");
                GUILayout.Box($"Current:{musicName} Volume:{musicSource.volume}");
                scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(300), GUILayout.Height(150));
                if (musicList != null)
                {
                    foreach (var Music in musicList)
                    {
                        GUILayout.BeginVertical();
                        GUILayout.Box(Path.GetFileName(Music));
                        if (GUILayout.Button("Play"))
                        {
                            musicName = Path.GetFileName(Music);
                            gameObject.GetComponent<MusicHandler>().LoadAndPlayAudio(musicSource, playerMusicPath, musicName);
                        }
                        GUILayout.EndHorizontal();
                    }
                }
                GUILayout.EndScrollView();
                musicUrl = GUILayout.TextField(musicUrl);
                musicSource.volume = GUILayout.HorizontalSlider(musicSource.volume, 0, 1);
                if (GUILayout.Button("Load by URL"))
                {
                    LoadAndPlayUrl(musicSource, musicUrl);
                    musicName = Path.GetFileName(Path.GetFileName(musicUrl));
                }
                if (GUILayout.Button("Stop"))
                {
                    musicSource.Stop();
                    musicName = "None";
                }
                GUILayout.EndVertical();
                GUILayout.EndArea();
            }
        }
        public void LoadAndPlayAudio(AudioSource _audioSource, string? path, string FileName = "mainmusic.mp3")
        {
            StartCoroutine(LoadAndPlayAudioCoroutine(_audioSource, path, FileName));
        }
        public void LoadAndPlayUrl(AudioSource _audioSource, string _URL)
        {
            StartCoroutine(LoadAndPlayUrlCoroutine(_audioSource, _URL));
        }
        private IEnumerator LoadAndPlayUrlCoroutine(AudioSource audioSource, string URL)
        {
            using (var www = new WWW(URL))
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
        private IEnumerator LoadAndPlayAudioCoroutine(AudioSource audioSource, string path, string FileName = "mainmusic.mp3")
        {
            path ??= musicPath;
            if (File.Exists(path + FileName))
            {
                using (var www = new WWW("file:///" + path + FileName))
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
    [ContentWarningPlugin("MusicPlugin", "2.0.1", false)]
    public class PluginTy
    {
        static PluginTy()
        {
            Debug.Log("MusicPlugin load");
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