// Copyright (c) 2026 chobitech
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php


using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Chobitech.DemoKit
{
    /// <summary>
    /// A base component that provides side-drawer functionality for UI panels.
    /// It handles smooth animations, flexible positioning with margins, and state transitions (open/close).
    /// This is used as the foundation for the Log and Description areas in the DemoKit.
    /// </summary>
    internal partial class BaseDrawer : MonoBehaviour
    {
        [Header("Basic Settings")]
        [SerializeField]
        private float topMargin = 100f;

        internal float TopMargin => topMargin;

        [SerializeField]
        private float bottomMargin = 80f;

        internal float BottomMargin => bottomMargin;

        [SerializeField]
        private bool openFromRight = false;

        [SerializeField]
        private float animationDurationSec = 0.15f;

        [Header("Events")]
        [SerializeField]

        internal UnityEvent<bool> onDrawerStateChanged;

        [SerializeField]
        internal UnityEvent<float> onDrawerMoving;

        [Header("Components")]
        [SerializeField]
        private RectTransform baseContainer;
        
        [SerializeField]
        private RectTransform drawerArea;

        internal bool IsOpened { get; private set; }

        internal void SetMargin(float top, float bottom)
        {
            topMargin = top;
            bottomMargin = bottom;

            this.RunIfUnityObjectIsNotNull(baseContainer, area =>
            {
                var anchorX = openFromRight ? 1f : 0f;
                area.anchorMin = new(anchorX, 0f);
                area.anchorMax = new(anchorX, 1f);
                area.pivot = new(anchorX, area.pivot.y);
                area.offsetMin = new(area.offsetMin.x, bottomMargin);
                area.offsetMax = new(area.offsetMax.x, -topMargin);
                area.anchoredPosition = new(0f, area.anchoredPosition.y);
            });
        }

        private void InitDrawerArea()
        {
            SetMargin(topMargin, bottomMargin);
        }

        private Coroutine _drawerMoveCoroutine;

        private IEnumerator DrawerMoveRoutine(float toX, bool deactivateDrawerAreaOnFinished)
        {
            if (drawerArea == null)
            {
                yield break;
            }

            var areaObj = drawerArea.gameObject;

            if (!areaObj.activeSelf)
            {
                areaObj.SetActive(true);
            }
            
            var aPos = drawerArea.anchoredPosition;
            var opening = toX == 0f;

            float GetMovingRate(float f)
            {
                return opening ? f : (1f - f);
            }

            if (animationDurationSec <= 0f)
            {
                aPos.x = toX;
                drawerArea.anchoredPosition = aPos;
            }
            else
            {
                var startX = aPos.x;
                var elapsedSec = 0f;
                while (elapsedSec < animationDurationSec)
                {
                    var rate = elapsedSec / animationDurationSec;
                    aPos.x = Mathf.Lerp(startX, toX, Mathf.Sqrt(rate));
                    drawerArea.anchoredPosition = aPos;
                    onDrawerMoving?.Invoke(GetMovingRate(rate));
                    elapsedSec += Time.deltaTime;
                    yield return null;
                }

                aPos.x = toX;
                drawerArea.anchoredPosition = aPos;
                onDrawerMoving?.Invoke(GetMovingRate(1f));
            }


            if (deactivateDrawerAreaOnFinished)
            {
                areaObj.SetActive(false);
            }

            _drawerMoveCoroutine = null;
        }

        private void StopDrawerMoveCoroutine()
        {
            if (_drawerMoveCoroutine != null)
            {
                StopCoroutine(_drawerMoveCoroutine);
                _drawerMoveCoroutine = null;
            }
        }

        private void OpenOrCloseDrawer(bool open, bool animate)
        {
            StopDrawerMoveCoroutine();

            this.RunIfUnityObjectIsNotNull(drawerArea, area =>
            {
                var toX = open ? 0f : area.rect.width;
                if (!openFromRight)
                {
                    toX *= -1f;
                }
                
                if (animate)
                {
                    _drawerMoveCoroutine = StartCoroutine(DrawerMoveRoutine(toX, !open));
                }
                else
                {
                    var aPos = area.anchoredPosition;
                    aPos.x = toX;
                    area.anchoredPosition = aPos;
                    if (!open)
                    {
                        area.gameObject.SetActive(false);
                    }
                }

                IsOpened = open;
                onDrawerStateChanged?.Invoke(IsOpened);
            });
        }

        internal void OpenDrawer(bool animate = true)
            => OpenOrCloseDrawer(true, animate);
        
        internal void CloseDrawer(bool animate = true)
            => OpenOrCloseDrawer(false, animate);
        
        internal void ToggleOpenOrClose(bool animate = true)
        {
            if (IsOpened)
            {
                CloseDrawer(animate);
            }
            else
            {
                OpenDrawer(animate);
            }
        }

        protected virtual void Awake()
        {
            InitDrawerArea();
        }
    }
}