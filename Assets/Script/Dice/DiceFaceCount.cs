using System;
using Script;
using TMPro;
using UnityEngine;

public class DiceFaceCount : MonoBehaviour 
{
    public Dice.DiceFaceType faceType;

    public int count = 0;

    private TextMeshPro m_TextMeshPro;

    void Start () 
    {
        m_TextMeshPro = GetComponent<TextMeshPro>();
        m_TextMeshPro.text = "";
        SetFaceType(faceType);
    }

    private void Update()
    {
        transform.rotation = Quaternion.identity;
    }

    public void DisplayNumber(bool doDisplay = true)
    {
        m_TextMeshPro.text = doDisplay ? count.ToString() : "";
    }

    public void CountUp(int upNum)
    {
        count += upNum;
        m_TextMeshPro.text = count.ToString();
    }
    
    public void SetFaceType(Dice.DiceFaceType ft)
    {
        switch (ft)
        {
            case Dice.DiceFaceType.Left:
                transform.rotation = Quaternion.Euler(0,0, 0);
                break;
            case Dice.DiceFaceType.Right:
                transform.rotation = Quaternion.Euler(0,0, 180);
                break;
            case Dice.DiceFaceType.Up:
                transform.rotation = Quaternion.Euler(0,0, 270);
                break;
            case Dice.DiceFaceType.Down:
                transform.rotation = Quaternion.Euler(0,0, 90);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}