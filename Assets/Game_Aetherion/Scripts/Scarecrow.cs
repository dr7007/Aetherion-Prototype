using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScareCrow : MonoBehaviour
{
    private HashSet<Collider> hitTargets = new HashSet<Collider>();
    public Material scareMat = null;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerAttack")) // 플레이어 무기에 의해 공격받은 경우
        {
            if (!hitTargets.Contains(other))
            {
                PlayerBattle playerAttack = other.GetComponentInParent<PlayerBattle>();

                // 충돌한 대상을 HitTracker에 추가
                hitTargets.Add(other);

                // 쿨다운 후 대상 제거
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
