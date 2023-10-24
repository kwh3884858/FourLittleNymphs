
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class VoiceBooster : UdonSharpBehaviour
{
    public float m_boosterRadius = 5.0f;
    //public SphereCollider m_boosterArea = null;
    [Tooltip("This one need to set as player layer")]
    public LayerMask m_areaLayerMask;
    private VRCPlayerApi[] m_currentControlledPlayer;

    void Start()
    {
        //if (m_boosterArea == null)
        //{
        //    return;
        //}
        

    }

    private void OnTriggerEnter(Collider other)
    {
        
    }

    void Update()
    {
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
