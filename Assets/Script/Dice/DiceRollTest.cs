using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class DiceRollTest : MonoBehaviour
{
    public float torquePower = 1000;
    private Rigidbody2D m_rigidbody2D;

    private static int num00;
    private static int num90;
    private static int num180;
    private static int num270;

    private bool printStopLog;

    private const float TOLERANCE = 0.05f;

    // Start is called before the first frame update
    void Start()
    {
        m_rigidbody2D = GetComponent<Rigidbody2D>();
        var randomTorquePower = Random.Range(-4000f, 4000f);
        var randomForceX = Random.Range(-1f, 1f);
        m_rigidbody2D.AddTorque(randomTorquePower);
        m_rigidbody2D.AddForce(new Vector2(randomForceX * 2000f, 2000f));
    }

    // Update is called once per frame
    void Update()
    {
        if (m_rigidbody2D.velocity.magnitude == 0f)
        {
            var rotationZ = Mathf.RoundToInt(transform.rotation.eulerAngles.z);
            var angleDifRange = 30;
            
            switch (rotationZ)
            {
                case var m when 90- angleDifRange/2 <= m && m <= 90 + angleDifRange/2:
                    num90++;
                    break;
                case 180:
                    num180++;
                    break;
                case 270:
                    num270++;
                    break;
                default:
                    if(!printStopLog)
                        Debug.LogError($"각도이탈 {rotationZ}"); 
                    GetComponent<SpriteRenderer>().color = Color.black;
                    printStopLog = true;
                    break;
            }

            Debug.Log($"'{gameObject.name}'의 결과 : {rotationZ}\n" +
                      $"U:{num00} / R:{num90} / D:{num180} / L:{num270}");
            Destroy(gameObject);
        }
    }
}
