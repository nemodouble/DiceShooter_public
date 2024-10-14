using System.Collections;
using System.Collections.Generic;
using Script;
using TMPro;
using UnityEngine;

public class AnswerText : MonoBehaviour
{
    private TextMeshPro TextMeshPro;
    void Start()
    {
        TextMeshPro = GetComponent<TextMeshPro>();
    }

    void Update()
    {
        var answer = "";
        foreach (var answerNumber in GameController.instance.answerNumbers)
        {
            answer += answerNumber + " ";
            
        }

        TextMeshPro.text = answer;
    }
}
