
using System.Collections.Generic;
using UdonSharp;
using UnityEngine;
using UnityEngine.Assertions;
using VRC.SDK3.Data;
using VRC.SDKBase;
using VRC.Udon;

public class VoiceBooster : UdonSharpBehaviour
{
    //public float m_boosterRadius = 5.0f;
    //public SphereCollider m_boosterArea = null;

    [Tooltip("Is Turn On The Booster")]
    [SerializeField]
    private bool m_currentBoosterState = false;

    //[UdonSynced]
    [SerializeField]
    private DataList m_currentControlledPlayerID = new DataList();

    [SerializeField]
    [UdonSynced] private string _json;

    [SerializeField]
    VRCPlayerApi[] players = new VRCPlayerApi[80];

    public override void OnPreSerialization()
    {
        if (Networking.IsMaster)
        {
            if (VRCJson.TrySerializeToJson(m_currentControlledPlayerID, JsonExportType.Minify, out DataToken result))
            {
                _json = result.String;
                Debug.Log(_json);
            }
            else
            {
                Debug.LogError(result.ToString());
            }
        }
    }

    public override void OnDeserialization()
    {
        Debug.Log("received");

        if (VRCJson.TryDeserializeFromJson(_json, out DataToken result))
        {
            m_currentControlledPlayerID = result.DataList;
        }
        else
        {
            Debug.LogError(result.ToString());
        }
    }

    public void Yodo_HapticSwitchOn()
    {
        m_currentBoosterState = true;
        SetControlledPlayersVoiceBooster(true);
    }

    public void Yodo_HapticSwitchOff()
    {
        m_currentBoosterState = false;
        DisableAllPlayersVoiceBoost();
    }

    private void SetControlledPlayersVoiceBooster(bool isTurnOnBooster)
    {
        for (int i = 0; i < m_currentControlledPlayerID.Count; i++)
        {
            m_currentControlledPlayerID.TryGetValue(i, out DataToken playerVaule);
            if (playerVaule.TokenType == TokenType.Int)
            {
                int playerID = (int)playerVaule;
                VRCPlayerApi player = VRCPlayerApi.GetPlayerById(playerID);

                SetPlayerVoice(player, isTurnOnBooster);
            }
        }

    }

    private void DisableAllPlayersVoiceBoost()
    {
        VRCPlayerApi.GetPlayers(players);
        for (int i = 0; i < VRCPlayerApi.GetPlayerCount(); i++)
        {
            VRCPlayerApi player = players[i];
            if (player.IsValid())
            {
                SetPlayerVoice(player, false);
            }
        }
    }

    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        base.OnPlayerTriggerEnter(player);
        int playerID = player.playerId;
        if (!m_currentControlledPlayerID.Contains(playerID))
        {
            m_currentControlledPlayerID.Add(playerID);
        }
        if (m_currentBoosterState)
        {
            SetPlayerVoice(player, true);
        }
    }

    public override void OnPlayerTriggerExit(VRCPlayerApi player)
    {
        base.OnPlayerTriggerExit(player);
        int playerID = player.playerId;

        if (m_currentControlledPlayerID.Contains(playerID))
        {
            m_currentControlledPlayerID.Remove(playerID);
        }
        if (m_currentBoosterState)
        {
            SetPlayerVoice(player, false);
        }
    }

    private void SetPlayerVoice(VRCPlayerApi player, bool isTurnBoosterOn)
    {
        if (player.IsValid())
        {
            if (isTurnBoosterOn)
            {
                Debug.Log($"[Player ID: {player.playerId}] Voice Boosted");
                player.SetVoiceGain(40);
                player.SetVoiceDistanceFar(10000);
            }
            else
            {
                Debug.Log($"[Player ID: {player.playerId}] Voice Normal");
                player.SetVoiceGain(15);
                player.SetVoiceDistanceFar(25);
            }
        }

    }

    void Update()
    {
        
    }


//#if !COMPILER_UDONSHARP && UNITY_EDITOR


//    private void OnDrawGizmos()
//    {
//        Gizmos.DrawWireSphere(transform.position, m_boosterRadius);
//    }
//#endif
}
