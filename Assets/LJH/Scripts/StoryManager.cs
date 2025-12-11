using TMPro;
using UnityEngine;

public class StoryManager : MonoBehaviour
{
    [Header("UI Components")]
    public TextMeshProUGUI storyText;
    public GameObject nextButton;

    [Header("Dialogues")]
    [TextArea(3, 10)]
    public string[] sentences;

    private int currentIndex = 0;

    void Start()
    {
        if (sentences.Length > 0)
        {
            storyText.text = sentences[0];
            currentIndex = 1;
        }
    }

    public void ShowNextSentence()
    {
        if (currentIndex < sentences.Length)
        {
            storyText.text = sentences[currentIndex];
            currentIndex++;
        }
        else
        {
            Debug.Log("모든 대화가 끝났습니다.");

            gameObject.SetActive(false); 
            nextButton.SetActive(true);
        }
    }
}
