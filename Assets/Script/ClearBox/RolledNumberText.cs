using System.Collections;
using System.Collections.Generic;
using Script;
using TMPro;
using UnityEngine;

public class RolledNumberText : MonoBehaviour
{
    private TextMeshPro TextMeshPro;
    public static int[] rollNumber;
    void Start()
    {
        TextMeshPro = GetComponent<TextMeshPro>();
    }

    void Update()
    {
        if(rollNumber!=null)
        {
            var answer = "";
            foreach (var answerNumber in rollNumber)
            {
                answer += answerNumber + " ";

            }

            TextMeshPro.text = answer;
        }
    }
}
