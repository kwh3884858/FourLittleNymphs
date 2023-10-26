
using System.Collections.Generic;
using UdonSharp;
using UnityEngine;
using UnityEngine.Assertions;
using VRC.SDK3.Data;
using VRC.SDKBase;
using VRC.Udon;

public class VoiceBooster : UdonSharpBehaviour
{
    public float m_boosterRadius = 5.0f;
    //public SphereCollider m_boosterArea = null;
    [Tooltip("This one need to set as player layer")]
    public LayerMask m_areaLayerMask;

    //public VRCPlayerApi[] m_allPlayers = new VRCPlayerApi[80];

    [UdonSynced]
    private string _json;
    //private DataList _list;

    public override void OnPreSerialization()
    {
        if (VRCJson.TrySerializeToJson(m_currentControlledPlayerID, JsonExportType.Minify, out DataToken result))
        {
            _json = result.String;
        }
        else
        {
            Debug.LogError(result.ToString());
        }
    }

    public override void OnDeserialization()
    {
        if (VRCJson.TryDeserializeFromJson(_json, out DataToken result))
        {
            m_currentControlledPlayerID = result.DataList;
        }
        else
        {
            Debug.LogError(result.ToString());
        }
    }

    //[UdonSynced]
    private DataList m_currentControlledPlayerID = new DataList();

    public void DoYodo_HapticSwitchOn()
    {
        SetupPlayersVoice(true);
    }

    public void DoYodo_HapticSwitchOff()
    {
        CloseAllPlayersVoice();
    }

    private void SetupPlayersVoice(bool isTurnOnBooster)
    {
        for (int i = 0; i < m_currentControlledPlayerID.Count; i++)
        {
            m_currentControlledPlayerID.TryGetValue(i, out DataToken playerVaule);
            //Assert.IsTrue(,
            //    $"Attempting to get value {i}, hit {playerVaule.Error}");
            if (playerVaule.TokenType == TokenType.Int)
            {
                int playerID = (int)playerVaule;
                VRCPlayerApi player = VRCPlayerApi.GetPlayerById(playerID);
                if (isTurnOnBooster)
                {
                    player.SetAvatarAudioGain(40);
                    player.SetAvatarAudioFarRadius(10000);
                }
                else
                {
                    player.SetAvatarAudioGain(10);
                    player.SetAvatarAudioFarRadius(25);
                }
            }
        }

    }

    private void CloseAllPlayersVoice()
    {
        //VRCPlayerApi.GetPlayers(m_allPlayers);
        for (int i = 0; i < VRCPlayerApi.GetPlayerCount(); i++)
        {
            VRCPlayerApi player = VRCPlayerApi.GetPlayerById(i);
            player.SetAvatarAudioGain(10);
            player.SetAvatarAudioFarRadius(25);
        }
    }

    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        Debug.Log(gameObject.activeInHierarchy);
        base.OnPlayerTriggerEnter(player);
        int playerID = player.playerId;
        if (m_currentControlledPlayerID.Contains(playerID))
        {

            m_currentControlledPlayerID.Add(playerID);
        }
    }

    public override void OnPlayerTriggerExit(VRCPlayerApi player)
    {
        Debug.Log(gameObject.activeInHierarchy);
        base.OnPlayerTriggerExit(player);
        int playerID = player.playerId;

        if (m_currentControlledPlayerID.Contains(playerID))
        {

            m_currentControlledPlayerID.Remove(playerID);
        }

    }

    void Update()
    {
        //Debug.Log(gameObject.activeInHierarchy);
        //RaycastHit[] playerInBoosterArea = Physics.SphereCastAll(transform.position, m_boosterRadius, Vector3.up, Mathf.Infinity, m_areaLayerMask);
        //foreach (var player in playerInBoosterArea)
        //{
        //    Debug.Log(player);
        //}
    }


#if !COMPILER_UDONSHARP && UNITY_EDITOR


    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, m_boosterRadius);
    }
#endif
}
