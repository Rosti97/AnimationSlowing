using UnityEngine;

public class animationmanager : MonoBehaviour
{
    public GameObject playerHands;
    public GameObject shootAnim;
    public GameObject wandGlow;
    public void PlayAnimation()
    {
        shootAnim.SetActive(false);
        playerHands.GetComponent<Animator>().SetTrigger("SpellCast");
        shootAnim.SetActive(true);
    }

    public void StartGlowing()
    {
        wandGlow.SetActive(true);
    }

    public void StopGlowing()
    {
        wandGlow.SetActive(false);
    }
}
