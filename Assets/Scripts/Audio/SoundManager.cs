using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    //=========================
    // サウンドカテゴリ定義
    //=========================
    public enum SECategory
    {
        Main,
        Serious,
        Effect,
        System,
        Bgm,
        Environment
    }

    [Serializable]
    public class AudioCategorySource
    {
        [Tooltip("カテゴリ種別")] public SECategory category;
        [Tooltip("再生に使うAudioSource")] public AudioSource source;
    }

    //=========================
    // フィールド
    //=========================
    public static SoundManager Instance { get; private set; }

    [Header("音源クリップ群")]
    [SerializeField] private AudioClip[] soundEffects;

    [Header("カテゴリ別AudioSource設定")]
    [SerializeField] private List<AudioCategorySource> categorySources = new();
    private readonly Dictionary<SECategory, AudioSource> sourceMap = new();

    [Header("AudioSourceプール設定")]
    [SerializeField] private int poolSize = 10;
    private readonly List<AudioSource> poolSources = new();

    [Header("PitchStack設定")]
    [SerializeField] private float pitchStep = 0.05f;
    [SerializeField] private float maxPitch = 2.0f;
    [SerializeField] private float resetInterval = 1.0f;

    [SerializeField, Range(0f, 1f)] private float masterVolume = 1f;
    private bool isMuted = false;
    private bool isPlayingSE = false;
    public bool IsPlayingSE => isPlayingSE;

    private readonly HashSet<SECategory> loopingCategories = new();
    private readonly Dictionary<SECategory, Coroutine> fadeOutCoroutines = new();
    private readonly Dictionary<string, bool> oncePlayFlags = new();

    private class PitchStack
    {
        public float currentPitch = 1f;
        public float lastPlayTime = 0f;
    }
    private readonly Dictionary<AudioClip, PitchStack> pitchStacks = new();

    //=========================
    // 初期化
    //=========================
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializeSourceMap();
            InitializePool();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeSourceMap()
    {
        foreach (var entry in categorySources)
        {
            if (entry.source != null && !sourceMap.ContainsKey(entry.category))
                sourceMap[entry.category] = entry.source;
        }

        if (!sourceMap.ContainsKey(SECategory.Main))
            Debug.LogWarning("SoundManager: SECategory.Main の AudioSource が未設定です。");
    }

    private void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            var src = gameObject.AddComponent<AudioSource>();
            src.playOnAwake = false;
            poolSources.Add(src);
        }
    }

    private AudioSource GetPooledSource()
    {
        foreach (var src in poolSources)
            if (!src.isPlaying) return src;

        // 全て再生中 → 最も再生時間が長いものを停止して再利用
        AudioSource longest = poolSources[0];
        float maxTime = longest.time;

        foreach (var src in poolSources)
        {
            if (src.time > maxTime)
            {
                longest = src;
                maxTime = src.time;
            }
        }

        longest.Stop();
        return longest;
    }

    //=========================
    // Clip取得
    //=========================
    public AudioClip GetClip(int index)
    {
        if (index >= 0 && index < soundEffects.Length)
            return soundEffects[index];

        Debug.LogWarning($"SoundManager: Index {index} のSEが見つかりません。");
        return null;
    }

    public AudioClip GetClip(string name)
    {
        AudioClip clip = Array.Find(soundEffects, se => se.name == name);
        if (clip == null) Debug.LogWarning($"SoundManager: SE '{name}' が見つかりません。");
        return clip;
    }

    //=========================
    // 通常再生
    //=========================
    public void PlaySE(int index, SECategory category = SECategory.Main, float volume = 1f, float pitch = 1f)
        => PlaySEInternal(GetClip(index), category, volume, pitch);

    public void PlaySE(string name, SECategory category = SECategory.Main, float volume = 1f, float pitch = 1f)
        => PlaySEInternal(GetClip(name), category, volume, pitch);

    private void PlaySEInternal(AudioClip clip, SECategory category, float volume, float pitch)
    {
        if (clip == null) return;

        if (category == SECategory.Effect || category == SECategory.System)
        {
            var src = GetPooledSource();
            src.clip = clip;
            src.pitch = pitch;
            ApplyMasterVolume(src, volume);
            src.Play();
        }
        else if (sourceMap.TryGetValue(category, out var source))
        {
            source.pitch = pitch;
            source.PlayOneShot(clip, volume * masterVolume);
        }
    }

    //=========================
    // ピッチ上昇付き再生
    //=========================
    public void PlaySE_WithPitchStack(int index, SECategory category = SECategory.Main, float volume = 1f)
        => PlaySE_WithPitchStackInternal(GetClip(index), category, volume);

    public void PlaySE_WithPitchStack(string name, SECategory category = SECategory.Main, float volume = 1f)
        => PlaySE_WithPitchStackInternal(GetClip(name), category, volume);

    private void PlaySE_WithPitchStackInternal(AudioClip clip, SECategory category, float volume)
    {
        if (clip == null) return;

        if (!pitchStacks.TryGetValue(clip, out var stack))
        {
            stack = new PitchStack();
            pitchStacks[clip] = stack;
        }

        float now = Time.time;
        if (now - stack.lastPlayTime > resetInterval)
            stack.currentPitch = 1f;

        float pitch = Mathf.Min(stack.currentPitch, maxPitch);
        PlaySEInternal(clip, category, volume, pitch);

        stack.currentPitch = Mathf.Min(stack.currentPitch + pitchStep, maxPitch);
        stack.lastPlayTime = now;
    }

    //=========================
    // 一度きり再生（条件付き）
    //=========================
    public void PlaySE_Once(Func<bool> condition, int index, SECategory category, string key = null)
    {
        if (key == null) key = $"{index}_{category}";
        if (!oncePlayFlags.ContainsKey(key)) oncePlayFlags[key] = false;

        bool prev = oncePlayFlags[key];
        bool now = condition();

        if (now && !prev)
            PlaySE(index, category);

        oncePlayFlags[key] = now;
    }

    //=========================
    // ループ・フェード
    //=========================
    public void PlaySE_Looping(int index, SECategory category = SECategory.Main)
    {
        if (!sourceMap.TryGetValue(category, out var source)) return;
        AudioClip clip = GetClip(index);
        if (clip == null) return;

        if (source.clip != clip)
        {
            source.clip = clip;
            source.loop = true;
            ApplyMasterVolume(source, 1f);
            source.Play();
            loopingCategories.Add(category);
        }
    }

    public void FadeOutAndPlaySE_Looping(int nextIndex, SECategory category = SECategory.Main, float fadeTime = 0.3f)
    {
        if (!sourceMap.TryGetValue(category, out var source)) return;
        AudioClip nextClip = GetClip(nextIndex);
        if (nextClip == null) return;

        if (fadeOutCoroutines.ContainsKey(category))
            StopCoroutine(fadeOutCoroutines[category]);

        fadeOutCoroutines[category] = StartCoroutine(FadeOutThenPlayNext(source, nextClip, category, fadeTime));
    }

    public void StopSE(SECategory category = SECategory.Main, float fadeTime = 0.5f)
    {
        if (!sourceMap.TryGetValue(category, out var source)) return;
        if (source.isPlaying)
        {
            if (fadeOutCoroutines.ContainsKey(category))
                StopCoroutine(fadeOutCoroutines[category]);

            fadeOutCoroutines[category] = StartCoroutine(FadeOutAndStop(source, category, fadeTime));
        }
        if (category == SECategory.Main) isPlayingSE = false;
    }

    public void StopAllLoopSE()
    {
        foreach (var kv in sourceMap)
        {
            if (kv.Value.isPlaying && kv.Value.loop)
            {
                kv.Value.Stop();
                kv.Value.clip = null;
                kv.Value.loop = false;
            }
        }
        isPlayingSE = false;
    }

    //=========================
    // 3D再生
    //=========================
    public void PlaySEAtPosition(int index, Vector3 position, float volume = 1f)
    {
        AudioClip clip = GetClip(index);
        if (clip == null) return;

        var go = new GameObject($"SE_{clip.name}_3D");
        go.transform.position = position;

        var src = go.AddComponent<AudioSource>();
        src.clip = clip;
        src.spatialBlend = 1f;
        src.volume = volume;
        src.Play();

        Destroy(go, clip.length);
    }

    //=========================
    // マスター音量制御
    //=========================
    public void SetMasterVolume(float value)
    {
        masterVolume = Mathf.Clamp01(value);
    }

    private void ApplyMasterVolume(AudioSource src, float localVolume)
    {
        src.volume = localVolume * masterVolume;
    }

    public void MuteAll(bool mute)
    {
        isMuted = mute;
        foreach (var src in sourceMap.Values) src.mute = mute;
        foreach (var src in poolSources) src.mute = mute;
    }

    //=========================
    // 補助コルーチン
    //=========================
    private IEnumerator FadeOutThenPlayNext(AudioSource source, AudioClip nextClip, SECategory category, float fadeTime)
    {
        yield return FadeOut(source, fadeTime);

        source.clip = nextClip;
        source.loop = true;
        source.volume = 0f;
        source.Play();
        loopingCategories.Add(category);

        yield return FadeIn(source, 1f, fadeTime);
        fadeOutCoroutines.Remove(category);
    }

    private IEnumerator FadeOutAndStop(AudioSource source, SECategory category, float fadeTime)
    {
        yield return FadeOut(source, fadeTime);
        source.Stop();
        source.clip = null;
        source.loop = false;
        fadeOutCoroutines.Remove(category);
    }

    private IEnumerator FadeOut(AudioSource source, float time)
    {
        float start = source.volume;
        float timer = 0f;
        while (timer < time)
        {
            timer += Time.deltaTime;
            source.volume = Mathf.Lerp(start, 0f, timer / time);
            yield return null;
        }
    }

    private IEnumerator FadeIn(AudioSource source, float targetVolume, float time)
    {
        float start = source.volume;
        float timer = 0f;
        while (timer < time)
        {
            timer += Time.deltaTime;
            source.volume = Mathf.Lerp(start, targetVolume, timer / time);
            yield return null;
        }
        source.volume = targetVolume;
    }
}