using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class ImpactFlash : MonoBehaviour
{
    public static ImpactFlash Instance;
    public Image flashImage;
    public float flashDuration = 0.05f;

    private void Awake()
    {
        Instance = this;
        flashImage.gameObject.SetActive(false);
    }

    public void PlayFlash()
    {
        StartCoroutine(Flash());
    }

    private IEnumerator Flash()
    {
        flashImage.gameObject.SetActive(true);
        yield return new WaitForSeconds(flashDuration);
        flashImage.gameObject.SetActive(false);
    }
}
