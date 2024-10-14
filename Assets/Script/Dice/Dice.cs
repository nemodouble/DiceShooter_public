using System;
using System.Collections;
using FMODUnity;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Script
{
    public class Dice : MonoBehaviour
    {
        public DiceHole fittedDiceHole;
        
        public enum DiceFaceType
        {
            Left,
            Right,
            Up,
            Down
        }
    
        private DiceFace m_LeftFace;
        private DiceFace m_RightFace;
        private DiceFace m_UpFace;
        private DiceFace m_DownFace;

        public int leftHoleCount;
        public int rightHoleCount;
        public int upHoleCount;
        public int downHoleCount;

        public EventReference nope;
        public Material outlineMaterial;
        
        private SpriteRenderer m_SpriteRenderer;
        
        private Vector2 m_ResetPosition;
        public int rolledNumber;

        public float lastCollisionSoundTime;

        private void Start()
        {
            m_SpriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Awake()
        {
            var tf = transform;
            m_LeftFace = tf.Find("Left").GetComponent<DiceFace>();
            m_RightFace = tf.Find("Right").GetComponent<DiceFace>();
            m_UpFace = tf.Find("Up").GetComponent<DiceFace>();
            m_DownFace = tf.Find("Down").GetComponent<DiceFace>();

            m_ResetPosition = tf.position;
            transform.localScale = new Vector3(0.5f, 0.5f, 1);
        }

        public void SelectedDice()
        {
            if(fittedDiceHole != null)
            {
                ResetDice(true);
            }
            transform.localScale = new Vector3(1, 1, 1);

            gameObject.layer = LayerMask.NameToLayer("ClickDisabled");
            
            m_SpriteRenderer.material = outlineMaterial;
            DiceLayerUp();
        }

        public void MakeBreakTrigger()
        {
            m_LeftFace.MakeBreakTrigger();
            m_RightFace.MakeBreakTrigger();
            m_UpFace.MakeBreakTrigger();
            m_DownFace.MakeBreakTrigger();
        }

        public void MakeBreakNotTrigger()
        {
            m_LeftFace.MakeBreakNotTrigger();
            m_RightFace.MakeBreakNotTrigger();
            m_UpFace.MakeBreakNotTrigger();
            m_DownFace.MakeBreakNotTrigger();
        }

        public void PlaceDice(DiceHole diceHole)
        {
            fittedDiceHole = diceHole;
            m_LeftFace.diceFaceCount.DisplayNumber();
            m_RightFace.diceFaceCount.DisplayNumber();
            m_UpFace.diceFaceCount.DisplayNumber();
            m_DownFace.diceFaceCount.DisplayNumber();
            
            m_SpriteRenderer.material = new Material(Shader.Find("Sprites/Default"));
            DiceLayerDown();
        }
        
        public void ResetDice(bool isReplace = false)
        {
            var tf = transform;
            if(fittedDiceHole == null)
                DiceLayerDown();
            else
                fittedDiceHole.dice = null;
            if(!isReplace)
                tf.position = m_ResetPosition;
            fittedDiceHole = null;
            tf.localScale = new Vector3(0.5f, 0.5f, 1);
            m_LeftFace.diceFaceCount.DisplayNumber(false);
            m_RightFace.diceFaceCount.DisplayNumber(false);
            m_UpFace.diceFaceCount.DisplayNumber(false);
            m_DownFace.diceFaceCount.DisplayNumber(false);
            m_LeftFace.ActiveOutline(false);
            m_RightFace.ActiveOutline(false);
            m_UpFace.ActiveOutline(false);
            m_DownFace.ActiveOutline(false);
            
            gameObject.layer = LayerMask.NameToLayer("ClickAble");
            
            m_SpriteRenderer.material = new Material(Shader.Find("Sprites/Default"));
        }

        private void DiceLayerUp()
        {
            m_SpriteRenderer.sortingOrder += 5;
            m_DownFace.LayerUp();
            m_LeftFace.LayerUp();
            m_RightFace.LayerUp();
            m_UpFace.LayerUp();
        }

        private void DiceLayerDown()
        {
            m_SpriteRenderer.sortingOrder -= 5;
            m_DownFace.LayerDown();
            m_LeftFace.LayerDown();
            m_RightFace.LayerDown();
            m_UpFace.LayerDown();
        }

        public IEnumerator RollDice()
        {
            var randomTorquePower = Random.Range(-2000f, 2000f);
            var randomForceX = Random.Range(-1f, 1f);

            gameObject.layer = LayerMask.NameToLayer("RollingDice");
            
            var rigid = GetComponent<Rigidbody2D>();
            rigid.bodyType = RigidbodyType2D.Dynamic;
            rigid.AddTorque(randomTorquePower);
            rigid.AddForce(new Vector2(randomForceX * 2000f, 2000f));

            var stopTime = 0f;
            
            var preventInf = 0;
            while (preventInf < 10000)
            {
                preventInf++;
                yield return null;
                
                if (rigid.velocity.magnitude <= 0.1f)
                {
                    stopTime += Time.deltaTime;
                    if (stopTime >= 1f)
                        break;
                }
                else
                {
                    stopTime = 0f;
                }
            }
            const int angleDifRange = 90;
            var rotationZ = Mathf.RoundToInt(transform.rotation.eulerAngles.z);
            if (0 <= rotationZ && rotationZ <= 0 + angleDifRange / 2 || 360 - angleDifRange / 2 <= rotationZ && rotationZ <= 360)
            {
                rolledNumber = upHoleCount;
                m_UpFace.ActiveOutline();
            }
            else if (90 - angleDifRange / 2 <= rotationZ && rotationZ <= 90 + angleDifRange / 2)
            {
                rolledNumber = rightHoleCount;
                m_RightFace.ActiveOutline();
            }
            else if (180 - angleDifRange / 2 <= rotationZ && rotationZ <= 180 + angleDifRange / 2)
            {
                rolledNumber = downHoleCount;
                m_DownFace.ActiveOutline();
            }
            else if (270 - angleDifRange / 2 <= rotationZ && rotationZ <= 270 + angleDifRange / 2)
            {
                rolledNumber = leftHoleCount;
                m_LeftFace.ActiveOutline();
            }
            else
                throw new UnityException("주사위 각도 오류");
            GameController.instance.rolledDiceNumber++;
            Debug.Log($"'{gameObject.name}'의 결과 : {rotationZ}");
        }

        public void AddHoleCount(DiceFaceType diceFaceType, int damage)
        {
            if (fittedDiceHole == null)
            {
                RuntimeManager.PlayOneShot("event:/ui/ui_beep");
                return;
            }
            switch (diceFaceType)
            {
                case DiceFaceType.Left:
                    leftHoleCount += damage;
                    break;
                case DiceFaceType.Right:
                    rightHoleCount += damage;
                    break;
                case DiceFaceType.Up:
                    upHoleCount += damage;
                    break;
                case DiceFaceType.Down:
                    downHoleCount += damage;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(diceFaceType), diceFaceType, null);
            }
            ClearPercentage.instance.UpdatePercentage();
        }

        private void OnCollisionEnter2D(Collision2D col)
        {
            if(Time.time - 0.05f > lastCollisionSoundTime)
                RuntimeManager.PlayOneShot("event:/sfx/dice_crash");
            lastCollisionSoundTime = Time.time;
        }
    }
}
