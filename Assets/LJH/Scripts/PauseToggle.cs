using UnityEngine;

public class PauseToggle : MonoBehaviour
{
    public GameObject targetUI;
    public Transform headCamera;

    public void ToggleUI()
    {
        bool isActive = targetUI.activeSelf;
        bool nextState = !isActive;

        targetUI.SetActive(nextState);

        if (nextState)
        {
            Time.timeScale = 0f;

            if (headCamera != null)
            {
                targetUI.transform.position = headCamera.position + (headCamera.forward * 1.5f);
                targetUI.transform.LookAt(2 * targetUI.transform.position - headCamera.position);
            }
        }
        else
        {
            Time.timeScale = 1f;
        }
    }
}
