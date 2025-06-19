using UnityEngine;

public class Defeat : MonoBehaviour
{
    [SerializeField] private GameObject Boss;
    [SerializeField]private GameObject spawner1;
    [SerializeField]private GameObject spawner2;
    [SerializeField]private GameObject spawner3;

    private void Update()
    {
        if (Boss == null)
        {
            spawner1.SetActive(false);
            spawner2.SetActive(false);
            spawner3.SetActive(false);
        }
    }
}
