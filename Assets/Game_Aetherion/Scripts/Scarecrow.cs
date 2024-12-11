using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScareCrow : MonoBehaviour
{
    private HashSet<Collider> hitTargets = new HashSet<Collider>();
    public Material scareMat = null;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerAttack")) // �÷��̾� ���⿡ ���� ���ݹ��� ���
        {
            if (!hitTargets.Contains(other))
            {
                PlayerBattle playerAttack = other.GetComponentInParent<PlayerBattle>();

                // �浹�� ����� HitTracker�� �߰�
                hitTargets.Add(other);

                // ��ٿ� �� ��� ����
                StartCoroutine(CoolDown(other));
            }
        }
    }

    private IEnumerator CoolDown(Collider other)
    {
        scareMat.EnableKeyword("_EMISSION");

        PlayerAnim playerHit = other.GetComponentInParent<PlayerAnim>();

        while (true)
        {
            if (playerHit.hitCombo == false) break;
            yield return null;
        }

        scareMat.DisableKeyword("_EMISSION");

        if (hitTargets.Contains(other))
        {
            hitTargets.Remove(other);
        }
    }


}
