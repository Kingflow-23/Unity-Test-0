using TMPro;
using UnityEngine;

public class Score : MonoBehaviour
{
    private TextMeshProUGUI scoreText;

    private void Awake()
    {
        scoreText = GetComponent<TextMeshProUGUI>();
        gameObject.name = "Score";
    }
    
    public void UpdateScore(int score)
    {
        scoreText.text = score.ToString("000000000");
    }
}
