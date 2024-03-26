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
    public int channels; // ���ôٹ������� ���� ������ҽ� ����� ���� ä�� ���� ����
    AudioSource[] sfxPlayers;
    int channelIndex; // ���������� ����� ä���� ����

    public enum Sfx { Dead, Hit, LevelUp=3, Lose, Melee, Range=7, Select, Win }

    void Awake()
    {
        instance = this;
        Init();
    }

    void Init()
    {
        // ����� �÷��̾� �ʱ�ȭ
        GameObject bgmObject = new GameObject("BgmPlayer");
        bgmObject.transform.parent = transform; // ������� ����ϴ� �ڽ� ������Ʈ
        bgmPlayer = bgmObject.AddComponent<AudioSource>(); // AddComponent �Լ��� ����� �ҽ��� �����ϰ� ������ ����
        bgmPlayer.playOnAwake = false; // ���ӽ��� ��� ���� ����
        bgmPlayer.loop = true; // �ݺ����
        bgmPlayer.volume = bgmVolume;
        bgmPlayer.clip = bgmClip;
        bgmEffect = Camera.main.GetComponent<AudioHighPassFilter>();

        // ȿ���� �÷��̾� �ʱ�ȭ
        GameObject sfxObject = new GameObject("SfxPlayer");
        sfxObject.transform.parent = transform;
        sfxPlayers = new AudioSource[channels]; // ä�� ������ŭ ������ҽ� �迭 �ʱ�ȭ

        // �ݺ������� ��� ȿ���� ������ҽ� �����ϸ鼭 ����
        for(int index = 0; index < sfxPlayers.Length; index++)
        {
            sfxPlayers[index] = sfxObject.AddComponent<AudioSource>();
            sfxPlayers[index].playOnAwake = false;
            sfxPlayers[index].bypassListenerEffects = true; // ���� ������ ���� �ʵ��� ��
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
        // ������� �ʰ� �ִ� �÷��̾ ��� ���
        for (int index = 0; index < sfxPlayers.Length; index++)
        {
            // ä�� ������ŭ ��ȸ�ϵ��� ä���ε��� ���� Ȱ��
            int loopIndex = (index + channelIndex) % sfxPlayers.Length;

            if (sfxPlayers[loopIndex].isPlaying) // �÷��� ���̶��
                continue; // ���� ������ �ǳʶ�

            // ȿ���� 2�� �̻��� ���� ���� �ε����� ���ϴ� ������ ó��
            int ranIndex = 0;
            if (sfx == Sfx.Hit || sfx == Sfx.Melee)
            {
                ranIndex = Random.Range(0, 2);
            }

            channelIndex = loopIndex;
            sfxPlayers[loopIndex].clip = sfxClips[(int)sfx + ranIndex];
            sfxPlayers[loopIndex].Play();
            break; // ���� ���� �÷��̾ �Ȱ��� ȿ������ ����ϴ� ���� ����
        }
    }
}
