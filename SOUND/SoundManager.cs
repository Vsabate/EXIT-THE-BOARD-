using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class SoundManager : MonoBehaviour
{
    #region Singleton
    public static SoundManager instance;

    private void Awake()
    {

        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }
    #endregion

    #region VARIABLES
    [Header("SOUND STUFF")]
    [HideInInspector] [Range(0, 1)] public float OverallVolume_Master;
    [HideInInspector] [Range(0, 1)] public float OverallVolume_Music;
    [HideInInspector] [Range(0, 1)] public float OverallVolume_SFX;

    [Space]
    [Header("FMOD STUFF")]
    private FMOD.Studio.VCA s_VCA_General;
    private FMOD.Studio.VCA s_VCA_Music;
    private FMOD.Studio.VCA s_VCA_SFX;
    [HideInInspector] public string s_VCA1_Name = "vca:/Master_VCA";
    [HideInInspector] public string s_VCA2_Name = "vca:/Music_VCA";
    [HideInInspector] public string s_VCA3_Name = "vca:/SFX_VCA";


    [Space]
    [Header("MUSIC INSTANCES")]
    [HideInInspector] public FMOD.Studio.EventInstance Music_MainMenu;
    [HideInInspector] public FMOD.Studio.EventInstance Music_Board;

    [Space]
    [Header("BOOLS")]
    [HideInInspector] public bool s_CanPlayDrums;
    [HideInInspector] public bool s_IsPlayingDrums;
    #endregion

    private void Start()
    {
        s_VCA1_Name = "vca:/Master_VCA";
        s_VCA2_Name = "vca:/Music_VCA";
        s_VCA3_Name = "vca:/SFX_VCA";

        s_VCA_General = FMODUnity.RuntimeManager.GetVCA(s_VCA1_Name);
        s_VCA_Music = FMODUnity.RuntimeManager.GetVCA(s_VCA2_Name);
        s_VCA_SFX = FMODUnity.RuntimeManager.GetVCA(s_VCA3_Name);

        Volume_SetInitial();
        UI_Manager.instance.Volume_SetInitialText();

        Music_MainMenu = FMODUnity.RuntimeManager.CreateInstance("event:/Music_Events/Menu-Theme");
        Music_Board = FMODUnity.RuntimeManager.CreateInstance("event:/Music_Events/Board-Theme");

        s_CanPlayDrums = false;
        s_IsPlayingDrums = false;
    }

    // FMOD STUFF
    #region SOUND_SETTINGS_FUNCTIONS 
    public void Volume_SetInitial()
    {
        OverallVolume_Music = 0.6f;
        OverallVolume_SFX = 0.6f;
        OverallVolume_Master = 0.6f;
        SetVolume();
    }
    public void Up_GeneralVol(float Value)
    {
        if (0.0f <= OverallVolume_Master && OverallVolume_Master < 1.0f)
        {
            OverallVolume_Master += Value;
            
        }
        else if (OverallVolume_Master >= 1.0f)
        {
            OverallVolume_Master = 1.0f;
        }
        OverallVolume_Master = (float)Math.Round(OverallVolume_Master, 1);

        // EXTRA
        if (0.0f <= OverallVolume_Music && OverallVolume_Music < 1.0f)
        {
            OverallVolume_Music += Value;
        }
        else if (OverallVolume_Music >= 1.0f)
        {
            OverallVolume_Music = 1.0f;
        }
        OverallVolume_Music = (float)Math.Round(OverallVolume_Music, 1);
        if (0.0f <= OverallVolume_SFX && OverallVolume_SFX < 1.0f)
        {
            OverallVolume_SFX += Value;
        }
        else if (OverallVolume_SFX >= 1.0f)
        {
            OverallVolume_SFX = 1.0f;
        }
        OverallVolume_SFX = (float)Math.Round(OverallVolume_SFX, 1);
        // EXTRA

        SetVolume();
    }
    public void Down_GeneralVol(float Value)
    {
        if (0.1f < OverallVolume_Master && OverallVolume_Master <= 1.0f)
        {
            OverallVolume_Master -= Value;
        }
        else if (OverallVolume_Master <= 0.1f)
        {
            OverallVolume_Master = 0.0f;
        }
        OverallVolume_Master = (float)Math.Round(OverallVolume_Master, 1);

        // EXTRA
        if (0.1f < OverallVolume_Music && OverallVolume_Music <= 1.0f)
        {
            OverallVolume_Music -= Value;
        }
        else if (OverallVolume_Music <= 0.1f)
        {
            OverallVolume_Music = 0.0f;
        }
        OverallVolume_Music = (float)Math.Round(OverallVolume_Music, 1);
        if (0.1f < OverallVolume_SFX && OverallVolume_SFX <= 1.0f)
        {
            OverallVolume_SFX -= Value;
        }
        else if (OverallVolume_SFX <= 0.1f)
        {
            OverallVolume_SFX = 0.0f;
        }
        OverallVolume_SFX = (float)Math.Round(OverallVolume_SFX, 1);
        // EXTRA

        SetVolume();
    }
    public void Up_MusicVol(float Value)
    {
        if (0.0f <= OverallVolume_Music && OverallVolume_Music < 1.0f)
        {
            OverallVolume_Music += Value;
        }
        else if (OverallVolume_Music >= 1.0f)
        {
            OverallVolume_Music = 1.0f;
        }
        OverallVolume_Music = (float)Math.Round(OverallVolume_Music, 1);
        SetVolume();
    }
    public void Down_MusicVol(float Value)
    {
        if (0.1f < OverallVolume_Music && OverallVolume_Music <= 1.0f)
        {
            OverallVolume_Music -= Value;
        }
        else if (OverallVolume_Music <= 0.1f)
        {
            OverallVolume_Music = 0.0f;
        }
        OverallVolume_Music = (float)Math.Round(OverallVolume_Music, 1);
        SetVolume();
    }
    public void Up_SFXVol(float Value)
    {
        if (0.0f <= OverallVolume_SFX && OverallVolume_SFX < 1.0f)
        {
            OverallVolume_SFX += Value;
        }
        else if (OverallVolume_SFX >= 1.0f)
        {
            OverallVolume_SFX = 1.0f;
        }
        OverallVolume_SFX = (float)Math.Round(OverallVolume_SFX, 1);
        SetVolume();
    }
    public void Down_SFXVol(float Value)
    {
        if (0.1f < OverallVolume_SFX && OverallVolume_SFX <= 1.0f)
        {
            OverallVolume_SFX -= Value;
        }
        else if (OverallVolume_SFX <= 0.1f)
        {
            OverallVolume_SFX = 0.0f;
        }
        OverallVolume_SFX = (float)Math.Round(OverallVolume_SFX, 1);
        SetVolume();
    }
    public void SetVolume()
    {
        s_VCA_General.setVolume(OverallVolume_Master);
        s_VCA_Music.setVolume(OverallVolume_Music);
        s_VCA_SFX.setVolume(OverallVolume_SFX);
    }
    #endregion

    public void PlaySound(string path)
    {
        FMODUnity.RuntimeManager.PlayOneShot(path);
    }

    #region SOUND_SFX_SOUNDS
    public void Sound_WELCOME()
    {
        StartCoroutine(WELCOME());
    }
    private IEnumerator WELCOME()
    {
        PlaySound("event:/Menu/WELCOME");
        yield return new WaitForSeconds(2.25f);
        Music_MainMenu.start();
        FadeController.instance.FadeOut_Black(1.0f);
    }
    public void Sound_StartGame() // Cuando los players le dan a continue para ir al tablero
    {
        PlaySound("event:/Menu/Comencar a jugar");
    }
    public void Sound_MinigameEnds() // PONER ESTO EN LOS 3 MINIGAME MANAGERS, CUANDO APARECE EL RANKING
    {
        PlaySound("event:/Menu/MinigameEnds");
    }
    public void Sound_ShowMinigameName()
    {
        PlaySound("event:/Menu/Comencar minijoc");
    } // Aparece el nombre del minijuego
    public void Sound_Teleport()
    {
        PlaySound("event:/Taulell/Canvi de posicio");
    } // Cuando dos personajes se teletransportan
    public void Sound_PositiveEffect()
    {
        PlaySound("event:/Taulell/PositiveEffect");
    } // Cuando un character cae en casilla de efecto positivo
    public void Sound_NegativeEffect() // Cuando un character cae en casilla de efecto negativo
    {
        PlaySound("event:/Taulell/NegativeEffect");
    }
    public void Sound_Drums()
    {
        StartCoroutine(PlayDrums());
    } // Cuando hay cuenta atrás de 3,2,1... en minijuegos
    private IEnumerator PlayDrums()
    {
        s_IsPlayingDrums = false;
        PlaySound("event:/Taulell/Timbal");
        yield return new WaitForSeconds(1f);
        PlaySound("event:/Taulell/Timbal");
        yield return new WaitForSeconds(1f);
        PlaySound("event:/Taulell/Timbal");
        yield return null;
    } //
    public void Sound_BeginMinigame()
    {
        PlaySound("event:/Taulell/Apariciominijoc");
    } // Trompa de guerra para cuando aparece la palabra BEGIN
    public void Sound_WitchButton() // Cuando un botón aparece en pantalla
    {
        PlaySound("event:/Taulell/WitchButton");
    }
    public void Sound_WitchLaugh() // Cuando comienza el minijuego de la bruja
    {
        PlaySound("event:/MiniJoc2/WitchLaugh");
    }
    public void Sound_Boing() // cuando rebotan las pelotas del minijuego
    {
        PlaySound("event:/MiniJoc1/Boing");
    }
    public void Sound_EndGame() // cuando un jugador gana
    {
        PlaySound("event:/Taulell/So final joc (Guanya)");
    }
    #endregion
}
