using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgmController : MonoBehaviour
{
    public static BgmController Instance
    {
        get
        {
            if (instance != null)
                return instance;

            instance = FindObjectOfType<BgmController>();

            if (instance != null)
                return instance;

            Create ();

            return instance;
        }
    }
    protected static BgmController instance;

    private FMOD.Studio.Bus m_Master;
    private static float masterVolume = 1f;

    public static BgmController Create ()
    {
        GameObject sceneControllerGameObject = new GameObject("BgmController");
        instance = sceneControllerGameObject.AddComponent<BgmController>();

        return instance;
    }

    private void Awake()
    {
        if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

        m_Master = FMODUnity.RuntimeManager.GetBus("bus:/Master");
        SetMasterVolume(PlayerPrefs.GetFloat("MasterVolume", 1));
    }

    private void Update()
    {
        m_Master.setVolume(masterVolume);
    }

    public void SetMasterVolume(float volume)
    {
        masterVolume = volume;
    }
}
