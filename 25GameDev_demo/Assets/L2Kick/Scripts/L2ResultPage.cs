using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class L2ResultPage : MonoBehaviour
{
    L2gameController controller;

    enum level
    {
        SS,
        S,
        A,
        B
    }
    level myLevel;
    int score;
    //int bestCombo;

    public Image image_level;
    public Sprite sprite_level_S;
    public Sprite sprite_level_A;
    public Sprite sprite_level_B;
    public Text text_score;
    public Text text_bestCombo;


    // Start is called before the first frame update
    void Start()
    {
        controller = FindObjectOfType<L2gameController>();
        score = controller.score;
        //text_bestCombo.text = "best combo " + bestCombo;
        text_score.text = "score " + score;
        if (controller.health > 0)
        {
            if (score / controller.totalScore >= 1)
            {
                myLevel = level.SS;
                image_level.sprite = sprite_level_S;
            }
            else if (score / controller.totalScore >= 0.8)
            {
                myLevel = level.S;
                image_level.sprite = sprite_level_S;
            }
            else
            {
                myLevel = level.A;
                image_level.sprite = sprite_level_A;
            }
        }
        else
        {
            myLevel = level.B;
            image_level.sprite = sprite_level_B;
        }
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickRestartButton()
    {
        SceneManager.LoadScene("L2Start");
    }
}
