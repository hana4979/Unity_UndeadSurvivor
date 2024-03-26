using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("#BGM")]
    public AudioClip bgmClip;
    public float bgmVolume;
    AudioSource bgmPlayer;
    AudioHighPassFilter bgmEffect;

    [Header("#SFX")]
    public AudioClip[] sfxClips;
    public float sfxVolume;
    public int channels; // 동시다발적으로 많은 오디오소스 재생을 위한 채널 개수 변수
    AudioSource[] sfxPlayers;
    int channelIndex; // 마지막으로 재생된 채널을 담음

    public enum Sfx { Dead, Hit, LevelUp=3, Lose, Melee, Range=7, Select, Win }

    void Awake()
    {
        instance = this;
        Init();
    }

    void Init()
    {
        // 배경음 플레이어 초기화
        GameObject bgmObject = new GameObject("BgmPlayer");
        bgmObject.transform.parent = transform; // 배경음을 담당하는 자식 오브젝트
        bgmPlayer = bgmObject.AddComponent<AudioSource>(); // AddComponent 함수로 오디오 소스를 생성하고 변수에 저장
        bgmPlayer.playOnAwake = false; // 게임시작 즉시 실행 여부
        bgmPlayer.loop = true; // 반복재생
        bgmPlayer.volume = bgmVolume;
        bgmPlayer.clip = bgmClip;
        bgmEffect = Camera.main.GetComponent<AudioHighPassFilter>();

        // 효과음 플레이어 초기화
        GameObject sfxObject = new GameObject("SfxPlayer");
        sfxObject.transform.parent = transform;
        sfxPlayers = new AudioSource[channels]; // 채널 개수만큼 오디오소스 배열 초기화

        // 반복문으로 모든 효과음 오디오소스 생성하면서 저장
        for(int index = 0; index < sfxPlayers.Length; index++)
        {
            sfxPlayers[index] = sfxObject.AddComponent<AudioSource>();
            sfxPlayers[index].playOnAwake = false;
            sfxPlayers[index].bypassListenerEffects = true; // 필터 영향을 받지 않도록 함
            sfxPlayers[index].volume = sfxVolume;
        }
    }

    public void PlayBgm(bool isPlay)
    {
        if (isPlay)
        {
            bgmPlayer.Play();
        }
        else
        {
            bgmPlayer.Stop();
        }
    }

    public void EffectBgm(bool isPlay)
    {
        bgmEffect.enabled = isPlay;
    }

    public void PlaySfx(Sfx sfx)
    {
        // 재생하지 않고 있는 플레이어를 집어서 재생
        for (int index = 0; index < sfxPlayers.Length; index++)
        {
            // 채널 개수만큼 순회하도록 채널인덱스 변수 활용
            int loopIndex = (index + channelIndex) % sfxPlayers.Length;

            if (sfxPlayers[loopIndex].isPlaying) // 플레이 중이라면
                continue; // 다음 루프로 건너뜀

            // 효과음 2개 이상인 것은 랜덤 인덱스를 더하는 것으로 처리
            int ranIndex = 0;
            if (sfx == Sfx.Hit || sfx == Sfx.Melee)
            {
                ranIndex = Random.Range(0, 2);
            }

            channelIndex = loopIndex;
            sfxPlayers[loopIndex].clip = sfxClips[(int)sfx + ranIndex];
            sfxPlayers[loopIndex].Play();
            break; // 여러 개의 플레이어가 똑같은 효과음을 재생하는 오류 방지
        }
    }
}
