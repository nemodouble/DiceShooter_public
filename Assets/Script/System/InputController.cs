using System;
using System.Collections.Generic;
using System.Linq;
using FMODUnity;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

namespace Script
{
    public class InputController : MonoBehaviour
    {
        public static InputController Instance
        {
            get
            {
                if (instance != null)
                    return instance;

                instance = FindObjectOfType<InputController>();

                if (instance != null)
                    return instance;

                Create ();

                return instance;
            }
        }
        public static InputController instance;

        public static InputController Create ()
        {
            GameObject sceneControllerGameObject = new GameObject("InputController");
            instance = sceneControllerGameObject.AddComponent<InputController>();

            return instance;
        }

        public bool IsPaused
        {
            set
            {
                OnDeselect();
                m_IsPaused = value;
            }
        }
        private bool m_IsPaused;
        
        public enum SelectState
        {
            None,
            SelectGun,
            SelectDice,
            AimingGun,
        }

        public SelectState nowSelectState;
        private GameObject m_SelectedObject;
        public bool isMouseInput;

        private void Awake()
        {
            if (Instance != this)
            {
                Destroy(gameObject);
            }
            DontDestroyOnLoad(this);

            nowSelectState = SelectState.None;
        }
        void Update()
        {
            if (m_IsPaused) return;
            
            if (isMouseInput && m_SelectedObject != null)
            {
                if (nowSelectState == SelectState.AimingGun)
                    m_SelectedObject.GetComponent<Gun>().mousePos =
                        Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
                else
                    m_SelectedObject.transform.position =
                        (Vector2) Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            }
            
        }

        // Input Action Event
        public void OnMouseClick(InputAction.CallbackContext context)
        {
            isMouseInput = true;
            if (!context.performed) return;
            var clickWorldPosition = (Vector2) Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            if (IsOnAnyUGUI(clickWorldPosition) || m_IsPaused) return;
            
            switch (nowSelectState)
            {
                case SelectState.None:
                    TrySelect(clickWorldPosition);
                    break;
                case SelectState.SelectGun:
                    TryPlaceGun(clickWorldPosition);
                    break;
                case SelectState.SelectDice:
                    TryPlaceDice(clickWorldPosition);
                    break;
                case SelectState.AimingGun:
                    TryShootGun(clickWorldPosition);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            RuntimeManager.PlayOneShot("event:/ui/ui_click");
        }
        public void OnTouch(InputAction.CallbackContext context)
        {
            isMouseInput = false;
            var touchState = context.ReadValue<TouchState>();
            var touchWorldPosition = (Vector2)Camera.main.ScreenToWorldPoint(touchState.position);
            if (m_IsPaused) return;
            
            switch (touchState.phase)
            {
                case TouchPhase.None:
                case TouchPhase.Canceled:
                case TouchPhase.Stationary:
                    return;
                case TouchPhase.Began:
                    if (IsOnAnyUGUI(touchWorldPosition)) return;
                    switch (nowSelectState)
                    {
                        case SelectState.None:
                            TrySelect(touchWorldPosition);
                            break;
                        case SelectState.AimingGun:
                            break;
                        case SelectState.SelectGun:
                        case SelectState.SelectDice:
                            return;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
                case TouchPhase.Moved:
                    switch (nowSelectState)
                    {
                        case SelectState.SelectGun:
                        case SelectState.SelectDice:
                            m_SelectedObject.transform.position = touchWorldPosition;
                            break;
                        case SelectState.AimingGun:
                            m_SelectedObject.GetComponent<Gun>().mousePos = touchWorldPosition;
                            break;
                        case SelectState.None:
                            return;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    return;
                case TouchPhase.Ended:
                    switch (nowSelectState)
                    {
                        case SelectState.SelectGun:
                            m_SelectedObject.GetComponent<Gun>().mousePos = m_SelectedObject.transform.position;
                            TryPlaceGun(touchWorldPosition);
                            break;
                        case SelectState.SelectDice:
                            TryPlaceDice(touchWorldPosition);
                            break;
                        case SelectState.AimingGun:
                            TryShootGun(touchWorldPosition);
                            break;
                        case SelectState.None:
                            return;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            RuntimeManager.PlayOneShot("event:/ui/ui_click");
        }

        private bool TrySelect(Vector2 position)
        {
            var hit = Physics2D.Raycast(position, Vector2.zero, 1000f);
            if (hit.collider == null)
            {
                return false;
            }
            if (hit.collider.GetComponent<Dice>() != null)
            {
                if (GameController.instance.isShooted)
                {
                    RuntimeManager.PlayOneShot("event:/ui/ui_beep");
                }
                else
                {
                    hit.collider.GetComponent<Dice>().SelectedDice();
                    m_SelectedObject = hit.collider.gameObject;
                    nowSelectState = SelectState.SelectDice;
                    return true;
                }
            }
            else if (hit.collider.transform.parent.GetComponent<Gun>() != null)
            {
                var parent = hit.collider.transform.parent;
                if (parent.GetComponent<Gun>().leftBullet <= 0)
                {
                    RuntimeManager.PlayOneShot("event:/sfx/bullet_tick");
                }
                else if (!GameController.instance.CanRollDice())
                {
                    RuntimeManager.PlayOneShot("event:/ui/ui_beep");
                }
                else
                {
                    parent.GetComponent<Gun>().Selected();
                    m_SelectedObject = parent.gameObject;
                    nowSelectState = SelectState.SelectGun;
                    return true;
                }
            }
            else if (hit.collider.GetComponent<IButton>() != null)
            {
                return hit.collider.GetComponent<IButton>().Clicked();
            }
            return false;
        }
       
        private void TryPlaceDice(Vector2 position)
        {
            var hit = Physics2D.Raycast(position, Vector2.zero, 10f, LayerMask.GetMask("ClickAble"));

            if (hit.collider != null)
            {
                if (hit.collider.GetComponent<DiceHole>() != null)
                {
                    var diceHole = hit.collider.GetComponent<DiceHole>();
                    if (diceHole.HaveDice())
                    {
                        m_SelectedObject.GetComponent<Dice>().ResetDice();
                    }

                    diceHole.PlaceDice(m_SelectedObject);
                    m_SelectedObject = null;
                    nowSelectState = SelectState.None;
                }
            }
            else
            {
                m_SelectedObject.GetComponent<Dice>().ResetDice();
                m_SelectedObject = null;
                nowSelectState = SelectState.None;
            }
        }

        private void TryPlaceGun(Vector2 position)
        {
            var punch = m_SelectedObject.GetComponent<Punch>();
            var layerMask = LayerMask.GetMask("GunHit")
                            + LayerMask.GetMask("GunHitPass")
                            + LayerMask.GetMask("GunHitReflect");
            if (punch != null)
            {
                if(IsOnCancel(position))
                {
                    OnDeselect();
                    return;
                }
                
                var hit = Physics2D.Raycast(position, Vector2.zero, 10f, layerMask);
                if (hit.collider == null) return;
                if (hit.collider.GetComponent<DiceFace>() == null) return;
                
                m_SelectedObject.GetComponent<Punch>().ReloadGun();
                m_SelectedObject.transform.Find("Sprite").gameObject.SetActive(true);
                nowSelectState = SelectState.None;
                m_SelectedObject = null;
            }
            else
            {
                layerMask += LayerMask.GetMask("ClickAble");
                var hit = Physics2D.Raycast(position, Vector2.zero, 10f, layerMask);
                if (hit.collider != null)
                {
                    m_SelectedObject.GetComponent<Gun>().ResetGun();
                    nowSelectState = SelectState.None;
                    m_SelectedObject = null;
                    return;
                }
                
                m_SelectedObject.GetComponent<Gun>().ReloadGun();
                if (m_SelectedObject.GetComponent<Gun>().isSniper)
                {
                    GameController.instance.MakeBreakTrigger();
                }
                else
                {
                    GameController.instance.MakeBreakNotTrigger();
                }

                m_SelectedObject.layer = LayerMask.NameToLayer("ClickAble");
                nowSelectState = SelectState.AimingGun;
            }
        }
        
        private bool TryShootGun(Vector2 position)
        {
            if (IsOnCancel(position))
            {
                OnDeselect();
                return false;
            }

            if(m_SelectedObject.GetComponent<Gun>().ShootGun(position))
            {
                m_SelectedObject = null;
                nowSelectState = SelectState.None;
                ClearBoxText.instance.ShowGunShootReact();
                return true;
            }

            return false;
        }

        private bool IsOnCancel(Vector2 worldPosition)
        {
            var results = GetOnUGUI(worldPosition);
            return results.Any(result => result.gameObject.CompareTag("ShootCancel"));
        }

        private bool IsOnAnyUGUI(Vector2 worldPosition)
        {
            var results = GetOnUGUI(worldPosition);
            return results.Count > 0;
        }

        private List<RaycastResult> GetOnUGUI(Vector2 position)
        {
            var cameraPos = Camera.main.WorldToScreenPoint(position);
            var pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = cameraPos;
            var results = new List<RaycastResult>();
            GameObject.Find("UICanvas").GetComponent<GraphicRaycaster>().Raycast(pointerEventData, results);
            return results;
        }

        public static void OnDeselect()
        {
            switch (instance.nowSelectState)
            {
                case SelectState.SelectDice:
                    instance.m_SelectedObject.GetComponent<Dice>().ResetDice();
                    break;
                case SelectState.AimingGun:
                case SelectState.SelectGun:
                    instance.m_SelectedObject.GetComponent<Gun>().ResetGun();
                    break;
                case SelectState.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            instance.nowSelectState = SelectState.None;
            instance.m_SelectedObject = null;
        }
    }
}
