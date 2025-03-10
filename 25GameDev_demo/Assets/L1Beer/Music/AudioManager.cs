using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance { get; private set; }

    private EventInstance BGMEventInstance;
    private EventInstance BSEventInstance;

    private EventInstance SpecialBGMEventInstance;

    //new
    private FMOD.System system;
    private FMOD.Sound sound;
    private FMOD.Channel channel;
    private FMOD.ChannelGroup channelGroup;

    private EventInstance eventInstance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found More than 1 audio manager in this scene");
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

    }
    private void Start()
    {
        InitializeBGM(FMODEvents.instance.All_BGM);

        //// 获取 FMOD 系统
        //system = RuntimeManager.CoreSystem;

        //// 创建 ChannelGroup
        //FMOD.RESULT result = system.createChannelGroup("MyChannelGroup", out channelGroup);
        //Debug.Log("创建 ChannelGroup: " + result);

        //// 连接 ChannelGroup 到 MasterGroup
        //FMOD.ChannelGroup masterGroup;
        //system.getMasterChannelGroup(out masterGroup);
        //masterGroup.addGroup(channelGroup, false);

        //// 加载音效
        ////string soundPath = Application.streamingAssetsPath + "/YourSound.wav";
        //string soundPath = FMODEvents.instance.L1Beer_water.Path;
        //result = system.createSound(soundPath, FMOD.MODE.DEFAULT, out sound);
        //Debug.Log("加载音效: " + soundPath + " 结果: " + result);

        //if (result != FMOD.RESULT.OK)
        //{
        //    Debug.LogError("音效加载失败: " + result);
        //}

        // 使用 FMOD 事件实例
        eventInstance = RuntimeManager.CreateInstance(FMODEvents.instance.L1Beer_water);
    }

    private void Update()
    {
        // 确保 FMOD 系统定期更新
        //system.update();
    }

    private void OnDestroy()
    {
        // 释放资源
        //sound.release();
        //channelGroup.release();
        if (eventInstance.isValid())
        {
            eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            eventInstance.release();
        }
    }
    public void PlayOneShot(EventReference sound, Vector3 worldPos)
    {
        RuntimeManager.PlayOneShot(sound, worldPos);
    }

    //new

    public void PlayWaterSound()
    {
        //system.playSound(sound, channelGroup, false, out channel);
        //// 确保音量不是 0
        //channel.setVolume(1.0f);
        //channelGroup.setVolume(1.0f);
        if (eventInstance.isValid())
        {
            eventInstance.getPlaybackState(out PLAYBACK_STATE state);
            eventInstance.getPaused(out bool isPaused);

            if (isPaused)
            {
                // 如果是暂停状态，恢复播放
                eventInstance.setPaused(false);
                eventInstance.start();
                Debug.Log("恢复播放 FMOD 事件");
            }
            else if (state == PLAYBACK_STATE.STOPPED)
            {
                // 事件已经停止，重新播放
                eventInstance.start();
                Debug.Log("重新播放 FMOD 事件: " + FMODEvents.instance.L1Beer_water.Path);
            }
            else
            {
                Debug.Log("事件正在播放，无需重新播放");
            }
        }
        else
        {
            Debug.LogError("FMOD 事件实例无效！");
        }
    }
    public void PauseWaterSound()
    {
        //if (channel.hasHandle())
        //{
        //    bool isPaused;
        //    channel.getPaused(out isPaused);
        //    channel.setPaused(!isPaused);  // 切换暂停状态
        //}
        if (eventInstance.isValid())
        {
            //eventInstance.getPaused(out bool isPaused);
            //eventInstance.setPaused(!isPaused);            
            eventInstance.setPaused(true);
            //Debug.Log(isPaused ? "恢复播放" : "暂停播放");
        }
    }
    public EventInstance CreateInstance(EventReference er)
    {
        EventInstance ei=RuntimeManager.CreateInstance(er);
        //eventInstances.Add(ei)
        return ei;
    }

    public void InitializeBGM(EventReference bgm)
    {
        BGMEventInstance = CreateInstance(bgm);
        BGMEventInstance.start();//real
    }
    //used in L2 time donw count bgm
    public void InitializeBGMSpecial(EventReference bgm)
    {
        SpecialBGMEventInstance = CreateInstance(bgm);
        SpecialBGMEventInstance.start();
    }
    public void InitializeBSBGM(EventReference bgm)
    {
        BSEventInstance = CreateInstance(bgm);
        BSEventInstance.start();//real
    }
    public void StopBGMSpecial()
    {
        Debug.LogError("complete stop and release" );//
        SpecialBGMEventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        FMOD.RESULT result = SpecialBGMEventInstance.release();
        if (result != FMOD.RESULT.OK)
        {
            Debug.LogWarning("Failed to release FMOD event instance: " + result);
        }

    }

    public void SetSceneBGM(Scene scene)
    {
        BGMEventInstance.setParameterByName("scene", (float)scene);
    }

}
