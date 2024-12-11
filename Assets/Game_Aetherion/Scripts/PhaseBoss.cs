using System.Collections;
using UnityEngine;

public class PhaseBoss : MonoBehaviour
{
    public bool phaseChanged = false;
    public GameObject particle;

    private void Update()
    {
        if (phaseChanged)
        {
            particle.GetComponent<ParticleSystem>().Play();
            StartCoroutine(PauseParticle());
        }
    }
    private void PhaseChange()
    {
        phaseChanged = true;
    }

    private IEnumerator PauseParticle()
    {
        yield return new WaitForSeconds(4f);

        particle.SetActive(false);
    }
}
