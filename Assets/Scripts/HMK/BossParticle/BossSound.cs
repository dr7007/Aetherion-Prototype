using UnityEngine;

public class BossSound : MonoBehaviour
{
    [SerializeField] AudioClip attack;
    [SerializeField] AudioClip bangsound;
    [SerializeField] AudioClip houling;
    [SerializeField] AudioClip step6;
    [SerializeField] AudioClip healing;
    [SerializeField] AudioClip break3;
    [SerializeField] AudioClip baum;
    [SerializeField] AudioClip kkang1;
    
    private void First()
    {
        AudioSource.PlayClipAtPoint(attack, gameObject.transform.position, 3f);
        AudioSource.PlayClipAtPoint(houling, gameObject.transform.position, 3f);
    }
    private void FirstDetect()
    {
        AudioSource.PlayClipAtPoint(attack, gameObject.transform.position, 3f);
    }
    private void break1()
    {
        AudioSource.PlayClipAtPoint(break3, gameObject.transform.position, 3f);
    }
    
    private void ResponAttack()
    {
        AudioSource.PlayClipAtPoint(attack, gameObject.transform.position, 3f);
    }
    private void No1()
    {
        AudioSource.PlayClipAtPoint(attack, gameObject.transform.position, 3f);
    }
    private void No3()
    {
        AudioSource.PlayClipAtPoint(attack, gameObject.transform.position, 3f);
    }
    private void heal()
    {
        AudioSource.PlayClipAtPoint(healing, gameObject.transform.position, 3f);
    }
    private void step()
    {
        AudioSource.PlayClipAtPoint(step6, gameObject.transform.position, 3f);
    }
    private void step1()
    {
        AudioSource.PlayClipAtPoint(step6, gameObject.transform.position, 3f);
    }
    private void step3()
    {
        AudioSource.PlayClipAtPoint(step6, gameObject.transform.position, 3f);
    }
    private void No2()
    {
        AudioSource.PlayClipAtPoint(attack, gameObject.transform.position, 3f);
    }
    private void bang()
    {
        AudioSource.PlayClipAtPoint(bangsound, gameObject.transform.position, 3f);
    }
    private void take1()
    {
        AudioSource.PlayClipAtPoint(baum, gameObject.transform.position, 3f);
    }
    private void take3()
    {
        AudioSource.PlayClipAtPoint(baum, gameObject.transform.position, 3f);
    }
    private void take2()
    {
        AudioSource.PlayClipAtPoint(baum, gameObject.transform.position, 3f);
    }
    private void kkang()
    {
        AudioSource.PlayClipAtPoint(kkang1, gameObject.transform.position, 3f);
    }
    private void hitt()
    {
        AudioSource.PlayClipAtPoint(baum, gameObject.transform.position, 3f);
    }

}
