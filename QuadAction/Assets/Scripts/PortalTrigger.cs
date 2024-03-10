using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalTrigger : MonoBehaviour
{
    private HashSet<GameObject> playersInPortal = new HashSet<GameObject>();
    public int totalPlayers = 1; // �ʿ��� �÷��̾� ��, ���� ������ ���� ����

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
            // ��� �÷��̾ ��Ż�� �������� �� ������ ����
            DungeonSceneManager.Instance.EnterDungeon();
        }
    }
}
