using UnityEngine;

public class animationmanager : MonoBehaviour
{
    public GameObject playerHands;
    public GameObject shootAnim;
    public void PlayAnimation()
    {
        shootAnim.SetActive(false);
        playerHands.GetComponent<Animator>().SetTrigger("SpellCast");
        shootAnim.SetActive(true);
    }
}
