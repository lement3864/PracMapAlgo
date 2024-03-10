using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalTrigger : MonoBehaviour
{
    private HashSet<GameObject> playersInPortal = new HashSet<GameObject>();
    public int totalPlayers = 1; // 필요한 플레이어 수, 게임 설정에 따라 조정

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playersInPortal.Add(other.gameObject);
            CheckAllPlayersInPortal();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playersInPortal.Remove(other.gameObject);
        }
    }

    private void CheckAllPlayersInPortal()
    {
        if (playersInPortal.Count == totalPlayers)
        {
            // 모든 플레이어가 포탈에 도달했을 때 실행할 로직
            DungeonSceneManager.Instance.EnterDungeon();
        }
    }
}
