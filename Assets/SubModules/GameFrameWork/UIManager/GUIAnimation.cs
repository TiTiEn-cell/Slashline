using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using System.Globalization;

namespace GFramework.GUI
{
    /// <summary>
    /// GUI animation type
    /// </summary>
    public enum GUIAnimType
    {
        /// <summary>
        /// Do not have any animation
        /// </summary>
        None = 0,

        MoveHorizontal = 1,
        MoveVertical = 2,
        MoveToWorldPos = 3,
        MoveToLocalPos = 4,
        RotateX = 5,
        RotateY = 6,
        RotateZ = 7,
        Zoom = 8,
        Blink = 9,
        Gradient = 10,
        ScrollInt = 11,
        ScrollFloat = 12,
    }

    /// <summary>
    /// Gui animation playing type
    /// </summary>
    public enum GUIAnimPlayType
    {
        Once = 0,
        Several = 1,
        Loop = 2,
    }

    public class GUIAnimation : MonoBehaviour
    {
        public bool autoPlay;

        [HideInInspector]
        public GUIAnimType animType = GUIAnimType.None;

        [HideInInspector]
        public float moveDistance;

        [HideInInspector]
        public Vector3 moveTo;

        [HideInInspector]
        public float rotAngle;

        [HideInInspector]
        public float zoomScale;

        [HideInInspector]
        public float showingPercent;

        [HideInInspector]
        public int fromInt;

        [HideInInspector]
        public int toInt;

        [HideInInspector]
        public float fromFloat;

        [HideInInspector]
        public float toFloat;

        [HideInInspector]
        public Text textBox;

        [HideInInspector]
        public AnimationCurve animCurve = new AnimationCurve();

        [HideInInspector]
        public GUIAnimPlayType playType = GUIAnimPlayType.Once;

        [HideInInspector]
        public int loopCount;

        [HideInInspector]
        public bool isReverseAnim = false;

        [HideInInspector]
        public bool isResetPos = true;

        [HideInInspector]
        public bool isPlayCoroutine = false;

        ///////////////////////////
        private IEnumerator animCoroutine = null;

        private Vector3 oPosition = Vector3.zero;

        private Vector3 oEulerAngles = Vector3.zero;

        private Vector3 oScale = Vector3.zero;

        ///////////////////////////
        private int playCount = 0;

        private Vector3 move = Vector3.zero;

        private Vector3 rot = Vector3.zero;

        ///////////////////////////
        private bool updateAnim = false;

        private int auPlayCount = 0;

        private Vector3 auMove = Vector3.zero;

        private Vector3 auRot = Vector3.zero;

        private float blinkScale;
        
        private float auStartTime = 0.0f;

        private float auRunTime = 0.0f;

        private float auCurveTime = 0.0f;

        ///////////////////////////
        [HideInInspector]
        public Action AnimationBeginEvent;

        [HideInInspector]
        public Action AnimationEndEvent;

        public void InitPos()
        {
            oPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
            oEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, transform.localEulerAngles.z);
            oScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }

        // Use this for initialization
        void Start()
        {
        }

        // Update is called once per frame
        void LateUpdate()
        {
            UpdateAnimNonCoroutine();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Int32.ToString")]
        private void UpdateAnimNonCoroutine()
        {
            if (updateAnim && animType != GUIAnimType.None)
            {
                switch (animType)
                {
                    case GUIAnimType.MoveHorizontal:
                    case GUIAnimType.MoveVertical:
                    case GUIAnimType.MoveToWorldPos:
                    case GUIAnimType.MoveToLocalPos:
                        if (auPlayCount > 0)
                        {
                            if (auRunTime < auCurveTime)
                            {
                                if (isReverseAnim)
                                    transform.localPosition = oPosition + auMove * animCurve.Evaluate(auCurveTime - auRunTime);
                                else
                                    transform.localPosition = oPosition +auMove * animCurve.Evaluate(auRunTime);
                                auRunTime = Time.time - auStartTime;
                            }
                            else
                            {
                                if (isReverseAnim)
                                    transform.localPosition = oPosition + auMove * animCurve.Evaluate(0);
                                else
                                    transform.localPosition = oPosition +auMove * animCurve.Evaluate(auCurveTime);

                                if (playType == GUIAnimPlayType.Once || playType == GUIAnimPlayType.Several)
                                    auPlayCount--;
                                auRunTime = 0.0f;
                                auStartTime = Time.time;

                                if (AnimationEndEvent != null)
                                    AnimationEndEvent();
                                if (auPlayCount > 0 && AnimationBeginEvent != null)
                                    AnimationBeginEvent();
                            }
                        }
                        else
                        {
                            if (isResetPos)
                                transform.localPosition = oPosition;
                            else
                                oPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
                            updateAnim = false;
                        }
                        break;
                    case GUIAnimType.RotateX:
                    case GUIAnimType.RotateY:
                    case GUIAnimType.RotateZ:
                        if (auPlayCount > 0)
                        {
                            if (auRunTime < auCurveTime)
                            {
                                if (isReverseAnim)
                                    transform.localEulerAngles = oEulerAngles + auRot * animCurve.Evaluate(auCurveTime - auRunTime);
                                else
                                    transform.localEulerAngles = oEulerAngles + auRot * animCurve.Evaluate(auRunTime);
                                auRunTime = Time.time - auStartTime;
                            }
                            else
                            {

                                if (isReverseAnim)
                                    transform.localEulerAngles = oEulerAngles + auRot * animCurve.Evaluate(0);
                                else
                                    transform.localEulerAngles = oEulerAngles + auRot * animCurve.Evaluate(auCurveTime);

                                if (playType == GUIAnimPlayType.Once || playType == GUIAnimPlayType.Several)
                                    auPlayCount--;
                                auRunTime = 0.0f;
                                auStartTime = Time.time;

                                if (AnimationEndEvent != null)
                                    AnimationEndEvent();
                                if (auPlayCount > 0 && AnimationBeginEvent != null)
                                    AnimationBeginEvent();
                            }
                        }
                        else
                        {
                            if (isResetPos)
                                transform.localEulerAngles = oEulerAngles;
                            else
                                oEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, transform.localEulerAngles.z);
                            updateAnim = false;
                        }
                        break;
                    case GUIAnimType.Zoom:
                        if (auPlayCount > 0)
                        {
                            if (auRunTime < auCurveTime)
                            {
                                if (isReverseAnim)
                                    transform.localScale = oScale * zoomScale * (1 + animCurve.Evaluate(auCurveTime - auRunTime));
                                else
                                    transform.localScale = oScale * zoomScale * (1 + animCurve.Evaluate(auRunTime));
                                auRunTime = Time.time - auStartTime;
                            }
                            else
                            {
                                if (isReverseAnim)
                                    transform.localScale = oScale * zoomScale * (1 + animCurve.Evaluate(0));
                                else
                                    transform.localScale = oScale * zoomScale * (1 + animCurve.Evaluate(auCurveTime));

                                if (playType == GUIAnimPlayType.Once || playType == GUIAnimPlayType.Several)
                                    auPlayCount--;
                                auRunTime = 0.0f;
                                auStartTime = Time.time;

                                if (AnimationEndEvent != null)
                                    AnimationEndEvent();
                                if (auPlayCount > 0 && AnimationBeginEvent != null)
                                    AnimationBeginEvent();
                            }
                        }
                        else
                        {
                            if (isResetPos)
                                transform.localScale = oScale;
                            else
                                oScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
                            updateAnim = false;
                        }
                        break;
                    case GUIAnimType.Blink:
                        if (auPlayCount > 0)
                        {
                            if (auRunTime < auCurveTime)
                            {
                                if (auRunTime < auCurveTime * showingPercent)
                                    blinkScale = 1.0f;
                                else
                                    blinkScale = 0.0f;

                                if (isReverseAnim)
                                    transform.localScale = oScale * (blinkScale * -1.0f + 1.0f);
                                else
                                    transform.localScale = oScale * blinkScale;
                                auRunTime = Time.time - auStartTime;
                            }
                            else
                            {
                                if (isReverseAnim)
                                    transform.localScale = Vector3.zero;
                                else
                                    transform.localScale = oScale;

                                if (playType == GUIAnimPlayType.Once || playType == GUIAnimPlayType.Several)
                                    auPlayCount--;
                                auRunTime = 0.0f;
                                auStartTime = Time.time;

                                if (AnimationEndEvent != null)
                                    AnimationEndEvent();
                                if (auPlayCount > 0 && AnimationBeginEvent != null)
                                    AnimationBeginEvent();
                            }
                        }
                        else
                        {
                            transform.localScale = oScale;
                            updateAnim = false;
                        }
                        break;
                    case GUIAnimType.Gradient:
                        break;
                    case GUIAnimType.ScrollInt:
                        if (auPlayCount > 0)
                        {
                            if (auRunTime < auCurveTime)
                            {
                                if (isReverseAnim)
                                    textBox.text = Mathf.RoundToInt(fromInt + animCurve.Evaluate(auCurveTime - auRunTime) * (toInt - fromInt)).ToString();
                                else
                                    textBox.text = Mathf.RoundToInt(fromInt + animCurve.Evaluate(auRunTime) * (toInt - fromInt)).ToString();
                                auRunTime = Time.time - auStartTime;
                            }
                            else
                            {
                                if (isReverseAnim)
                                    textBox.text = fromInt.ToString();
                                else
                                    textBox.text = toInt.ToString();

                                if (playType == GUIAnimPlayType.Once || playType == GUIAnimPlayType.Several)
                                    auPlayCount--;
                                auRunTime = 0.0f;
                                auStartTime = Time.time;

                                if (AnimationEndEvent != null)
                                    AnimationEndEvent();
                                if (auPlayCount > 0 && AnimationBeginEvent != null)
                                    AnimationBeginEvent();
                            }
                        }
                        else
                        {
                            if (isReverseAnim)
                                textBox.text = fromInt.ToString();
                            else
                                textBox.text = toInt.ToString();
                            updateAnim = false;
                        }
                        break;
                    case GUIAnimType.ScrollFloat:
                        if (auPlayCount > 0)
                        {
                            if (auRunTime < auCurveTime)
                            {
                                if (isReverseAnim)
                                    textBox.text = (fromFloat + animCurve.Evaluate(auCurveTime - auRunTime) * (toFloat - fromFloat)).ToString(CultureInfo.CurrentCulture.NumberFormat);
                                else
                                    textBox.text = (fromFloat + animCurve.Evaluate(auRunTime) * (toFloat - fromFloat)).ToString(CultureInfo.CurrentCulture.NumberFormat);
                                auRunTime = Time.time - auStartTime;
                            }
                            else
                            {
                                if (isReverseAnim)
                                    textBox.text = fromFloat.ToString(CultureInfo.CurrentCulture.NumberFormat);
                                else
                                    textBox.text = toFloat.ToString(CultureInfo.CurrentCulture.NumberFormat);

                                if (playType == GUIAnimPlayType.Once || playType == GUIAnimPlayType.Several)
                                    auPlayCount--;
                                auRunTime = 0.0f;
                                auStartTime = Time.time;

                                if (AnimationEndEvent != null)
                                    AnimationEndEvent();
                                if (auPlayCount > 0 && AnimationBeginEvent != null)
                                    AnimationBeginEvent();
                            }
                        }
                        else
                        {
                            if (isReverseAnim)
                                textBox.text = fromFloat.ToString(CultureInfo.CurrentCulture.NumberFormat);
                            else
                                textBox.text = toFloat.ToString(CultureInfo.CurrentCulture.NumberFormat);
                            updateAnim = false;
                        }
                        break;
                }
            }
        }

        public void PlayGUIAnimation(params object[] @parameter)
        {
            if (@parameter.Length > 0 && @parameter[0] is bool)
                isReverseAnim = (bool)@parameter[0];
            if (@parameter.Length > 1 && @parameter[1] is bool)
                isResetPos = (bool)@parameter[1];
            if (@parameter.Length > 2 && @parameter[2] is bool)
                isPlayCoroutine = (bool)@parameter[2];

            InitPos();
            if (isPlayCoroutine)
            {
                StartGUIAnimationCoroutine();
            }
            else
            {
                StartGUIAnimationNoneCoroutine();
            }
        }

        public void PlayGUIAnimation(GUIAnimType type, object dest, params object[] @parameter)
        {
            animType = type;
            switch (type)
            {
                case GUIAnimType.MoveHorizontal:
                case GUIAnimType.MoveVertical:
                    if (dest is float)
                    {
                        moveDistance = (float)dest;
                        PlayGUIAnimation(@parameter);
                    }
                    break;
                case GUIAnimType.MoveToWorldPos:
                case GUIAnimType.MoveToLocalPos:
                    if (dest is Vector3)
                    {
                        moveTo = (Vector3)dest;
                        PlayGUIAnimation(@parameter);
                    }
                    break;
                case GUIAnimType.RotateX:
                case GUIAnimType.RotateY:
                case GUIAnimType.RotateZ:
                    if (dest is float)
                    {
                        rotAngle = (float)dest;
                        PlayGUIAnimation(@parameter);
                    }
                    break;
                case GUIAnimType.Zoom:
                    if (dest is float)
                    {
                        zoomScale = (float)dest;
                        PlayGUIAnimation(@parameter);
                    }
                    break;
                case GUIAnimType.Blink:
                    if (dest is float)
                    {
                        showingPercent = (float)dest;
                        PlayGUIAnimation(@parameter);
                    }
                    break;
                case GUIAnimType.Gradient:
                    break;
                case GUIAnimType.ScrollInt:
                case GUIAnimType.ScrollFloat:
                    if (dest is Text)
                    {
                        textBox = (Text)dest;
                        PlayGUIAnimation(@parameter);
                    }
                    break;
            }
        }

        private void StartGUIAnimationNoneCoroutine()
        {
            updateAnim = false;

            if (animType == GUIAnimType.None)
                return;

            if (animCurve.length < 2)
                return;

            updateAnim = true;

            //Get total time of curve
            auCurveTime = animCurve[animCurve.length - 1].time;

            auStartTime = Time.time;
            auRunTime = 0.0f;

            if (AnimationBeginEvent != null)
                AnimationBeginEvent();

            switch (animType)
            {
                case GUIAnimType.MoveHorizontal:
                    auMove = new Vector3(moveDistance, 0, 0);
                    break;
                case GUIAnimType.MoveVertical:
                    auMove = new Vector3(0, moveDistance, 0);
                    break;
                case GUIAnimType.MoveToWorldPos:
                    auMove = transform.InverseTransformPoint(moveTo);
                    break;
                case GUIAnimType.MoveToLocalPos:
                    auMove = moveTo - transform.localPosition;
                    break;
                case GUIAnimType.RotateX:
                    auRot = new Vector3(rotAngle, 0, 0);
                    break;
                case GUIAnimType.RotateY:
                    auRot = new Vector3(0, rotAngle, 0);
                    break;
                case GUIAnimType.RotateZ:
                    auRot = new Vector3(0, 0, rotAngle);
                    break;
                case GUIAnimType.Zoom:
                case GUIAnimType.Blink:
                case GUIAnimType.Gradient:
                case GUIAnimType.ScrollInt:
                case GUIAnimType.ScrollFloat:
                    break;
            }

            auPlayCount = 1;
            if (playType == GUIAnimPlayType.Several)
            {
                auPlayCount = loopCount;
            }

            UpdateAnimNonCoroutine();
        }

        private void StartGUIAnimationCoroutine()
        {
            if (animType == GUIAnimType.None)
                return;

            if (animCurve.length < 2)
                return;

            if (AnimationBeginEvent != null)
                AnimationBeginEvent();

            switch (animType)
            {
                case GUIAnimType.MoveHorizontal:
                    move = new Vector3(moveDistance, 0, 0);
                    break;
                case GUIAnimType.MoveVertical:
                    move = new Vector3(0, moveDistance, 0);
                    break;
                case GUIAnimType.MoveToWorldPos:
                    move = transform.InverseTransformPoint(moveTo);
                    break;
                case GUIAnimType.MoveToLocalPos:
                    move = moveTo - transform.localPosition;
                    break;
                case GUIAnimType.RotateX:
                    rot = new Vector3(rotAngle, 0, 0);
                    break;
                case GUIAnimType.RotateY:
                    rot = new Vector3(0, rotAngle, 0);
                    break;
                case GUIAnimType.RotateZ:
                    rot = new Vector3(0, 0, rotAngle);
                    break;
                case GUIAnimType.Zoom:
                case GUIAnimType.Blink:
                case GUIAnimType.Gradient:
                case GUIAnimType.ScrollInt:
                case GUIAnimType.ScrollFloat:
                    break;
            }

            playCount = 1;
            if (playType == GUIAnimPlayType.Several)
            {
                playCount = loopCount;
            }

            animCoroutine = CoroutinePlayAnim();
            StartCoroutine(animCoroutine);
        }

        private IEnumerator CoroutinePlayAnim()
        {
            if (animType == GUIAnimType.None)
                yield break;

            if (animCurve.length < 2)
                yield break;

            //Get total time of curve
            float curveTime = 0.0f;
            curveTime = animCurve[animCurve.length - 1].time - animCurve[0].time;

            float startTime = Time.time;
            float runTime = 0.0f;

            if (AnimationBeginEvent != null)
                AnimationBeginEvent();

            switch (animType)
            {
                case GUIAnimType.MoveHorizontal:
                case GUIAnimType.MoveVertical:
                case GUIAnimType.MoveToWorldPos:
                case GUIAnimType.MoveToLocalPos:
                    while (playCount > 0)
                    {
                        while (runTime < curveTime)
                        {
                            if (isReverseAnim)
                                transform.localPosition = oPosition + move * animCurve.Evaluate(curveTime - runTime);
                            else
                                transform.localPosition = oPosition + move * animCurve.Evaluate(runTime);
                            runTime = Time.time - startTime;
                            yield return new WaitForSeconds(0.02f);
                        }

                        if (isReverseAnim)
                            transform.localPosition = oPosition + move * animCurve.Evaluate(0);
                        else
                            transform.localPosition = oPosition + move * animCurve.Evaluate(curveTime);

                        if (playType == GUIAnimPlayType.Once || playType == GUIAnimPlayType.Several)
                            playCount--;

                        runTime = 0.0f;
                        startTime = Time.time;

                        if (AnimationEndEvent != null)
                            AnimationEndEvent();
                        if (playCount > 0 && AnimationBeginEvent != null)
                            AnimationBeginEvent();
                    }

                    if (isResetPos)
                        transform.localPosition = oPosition;
                    else
                        oPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
                    break;
                case GUIAnimType.RotateX:
                case GUIAnimType.RotateY:
                case GUIAnimType.RotateZ:
                    while (playCount > 0)
                    {
                        while (runTime < curveTime)
                        {
                            if (isReverseAnim)
                                transform.localEulerAngles = oEulerAngles + rot * animCurve.Evaluate(curveTime - runTime);
                            else
                                transform.localEulerAngles = oEulerAngles + rot * animCurve.Evaluate(runTime);
                            runTime = Time.time - startTime;
                            yield return new WaitForSeconds(0.02f);
                        }

                        if (isReverseAnim)
                            transform.localEulerAngles = oEulerAngles + rot * animCurve.Evaluate(0);
                        else
                            transform.localEulerAngles = oEulerAngles + rot * animCurve.Evaluate(curveTime);

                        if (playType == GUIAnimPlayType.Once || playType == GUIAnimPlayType.Several)
                            playCount--;

                        runTime = 0.0f;
                        startTime = Time.time;

                        if (AnimationEndEvent != null)
                            AnimationEndEvent();
                        if (playCount > 0 && AnimationBeginEvent != null)
                            AnimationBeginEvent();
                    }

                    if (isResetPos)
                        transform.localEulerAngles = oEulerAngles;
                    else
                        oEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, transform.localEulerAngles.z);
                    break;
                case GUIAnimType.Zoom:
                    while (playCount > 0)
                    {
                        while (runTime < curveTime)
                        {
                            if (isReverseAnim)
                                transform.localScale = oScale * zoomScale * (1 + animCurve.Evaluate(curveTime - runTime));
                            else
                                transform.localScale = oScale * zoomScale * (1 + animCurve.Evaluate(runTime));
                            runTime = Time.time - startTime;
                            yield return new WaitForSeconds(0.02f);
                        }

                        if (isReverseAnim)
                            transform.localScale = oScale * zoomScale * (1 + animCurve.Evaluate(0));
                        else
                            transform.localScale = oScale * zoomScale * (1 + animCurve.Evaluate(curveTime));

                        if (playType == GUIAnimPlayType.Once || playType == GUIAnimPlayType.Several)
                            playCount--;

                        runTime = 0.0f;
                        startTime = Time.time;

                        if (AnimationEndEvent != null)
                            AnimationEndEvent();
                        if (playCount > 0 && AnimationBeginEvent != null)
                            AnimationBeginEvent();
                    }

                    if (isResetPos)
                        transform.localScale = oScale;
                    else
                        oScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
                    break;
                case GUIAnimType.Blink:
                    while (playCount > 0)
                    {
                        while (runTime < curveTime)
                        {
                            if (auRunTime < auCurveTime * showingPercent)
                                blinkScale = 1.0f;
                            else
                                blinkScale = 0.0f;

                            if (isReverseAnim)
                                transform.localScale = oScale * (blinkScale * -1.0f + 1.0f);
                            else
                                transform.localScale = oScale * blinkScale;

                            runTime = Time.time - startTime;
                            yield return new WaitForSeconds(0.02f);
                        }

                        if (isReverseAnim)
                            transform.localScale = Vector3.zero;
                        else
                            transform.localScale = oScale;

                        if (playType == GUIAnimPlayType.Once || playType == GUIAnimPlayType.Several)
                            playCount--;

                        runTime = 0.0f;
                        startTime = Time.time;

                        if (AnimationEndEvent != null)
                            AnimationEndEvent();
                        if (playCount > 0 && AnimationBeginEvent != null)
                            AnimationBeginEvent();
                    }

                    transform.localScale = oScale;
                    break;
                case GUIAnimType.Gradient:
                    break;
                case GUIAnimType.ScrollInt:
                    while (playCount > 0)
                    {
                        while (runTime < curveTime)
                        {
                            if (isReverseAnim)
                                textBox.text = Mathf.RoundToInt(fromInt + animCurve.Evaluate(curveTime - runTime) * (toInt - fromInt)).ToString();
                            else
                                textBox.text = Mathf.RoundToInt(fromInt + animCurve.Evaluate(runTime) * (toInt - fromInt)).ToString();
                            runTime = Time.time - startTime;
                            yield return new WaitForSeconds(0.02f);
                        }

                        if (isReverseAnim)
                            textBox.text = fromInt.ToString();
                        else
                            textBox.text = toInt.ToString();

                        if (playType == GUIAnimPlayType.Once || playType == GUIAnimPlayType.Several)
                            playCount--;

                        runTime = 0.0f;
                        startTime = Time.time;

                        if (AnimationEndEvent != null)
                            AnimationEndEvent();
                        if (playCount > 0 && AnimationBeginEvent != null)
                            AnimationBeginEvent();
                    }

                    if (isReverseAnim)
                        textBox.text = fromInt.ToString();
                    else
                        textBox.text = toInt.ToString();
                    break;
                case GUIAnimType.ScrollFloat:
                    while (playCount > 0)
                    {
                        while (runTime < curveTime)
                        {
                            if (isReverseAnim)
                                textBox.text = (fromFloat + animCurve.Evaluate(curveTime - runTime) * (toFloat - fromFloat)).ToString();
                            else
                                textBox.text = (fromFloat + animCurve.Evaluate(runTime) * (toFloat - fromFloat)).ToString();
                            runTime = Time.time - startTime;
                            yield return new WaitForSeconds(0.02f);
                        }

                        if (isReverseAnim)
                            textBox.text = fromFloat.ToString();
                        else
                            textBox.text = toFloat.ToString();

                        if (playType == GUIAnimPlayType.Once || playType == GUIAnimPlayType.Several)
                            playCount--;

                        runTime = 0.0f;
                        startTime = Time.time;

                        if (AnimationEndEvent != null)
                            AnimationEndEvent();
                        if (playCount > 0 && AnimationBeginEvent != null)
                            AnimationBeginEvent();
                    }

                    if (isReverseAnim)
                        textBox.text = fromFloat.ToString();
                    else
                        textBox.text = toFloat.ToString();
                    break;
            }

            animCoroutine = null;
        }

        public void ForceEndAnim()
        {
            if (isPlayCoroutine)
            {
                ForceEndAnimCoroutine();
            }
            else
            {
                ForceEndAnimNoneCoroutine();
            }
        }

        private void ForceEndAnimNoneCoroutine()
        {
            if (!updateAnim)
                return;

            if (animType == GUIAnimType.None)
                return;

            if (animCurve.length < 2)
                return;

            if (updateAnim)
            {
                updateAnim = false;
                if (AnimationEndEvent != null)
                    AnimationEndEvent();
            }

            auPlayCount = 0;

            switch (animType)
            {
                case GUIAnimType.MoveHorizontal:
                case GUIAnimType.MoveVertical:
                case GUIAnimType.MoveToWorldPos:
                case GUIAnimType.MoveToLocalPos:
                    if (isReverseAnim)
                        transform.localPosition = oPosition + auMove * animCurve.Evaluate(0);
                    else
                        transform.localPosition = oPosition + auMove * animCurve.Evaluate(auCurveTime);

                    if (isResetPos)
                        transform.localPosition = oPosition;
                    else
                        oPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
                    break;
                case GUIAnimType.RotateX:
                case GUIAnimType.RotateY:
                case GUIAnimType.RotateZ:
                    if (isReverseAnim)
                        transform.localEulerAngles = oEulerAngles + auRot * animCurve.Evaluate(0);
                    else
                        transform.localEulerAngles = oEulerAngles + auRot * animCurve.Evaluate(auCurveTime);

                    if (isResetPos)
                        transform.localEulerAngles = oEulerAngles;
                    else
                        oEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, transform.localEulerAngles.z);
                    break;
                case GUIAnimType.Zoom:
                    if (isReverseAnim)
                        transform.localScale = oScale * zoomScale * (1 + animCurve.Evaluate(0));
                    else
                        transform.localScale = oScale * zoomScale * (1 + animCurve.Evaluate(auCurveTime));

                    if (isResetPos)
                        transform.localScale = oScale;
                    else
                        oScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
                    break;
                case GUIAnimType.Blink:
                    transform.localScale = oScale;
                    break;
                case GUIAnimType.Gradient:
                    break;
                case GUIAnimType.ScrollInt:
                    if (isReverseAnim)
                        textBox.text = fromInt.ToString(CultureInfo.CurrentCulture.NumberFormat);
                    else
                        textBox.text = toInt.ToString(CultureInfo.CurrentCulture.NumberFormat);
                    break;
                case GUIAnimType.ScrollFloat:
                    if (isReverseAnim)
                        textBox.text = fromFloat.ToString(CultureInfo.CurrentCulture.NumberFormat);
                    else
                        textBox.text = toFloat.ToString(CultureInfo.CurrentCulture.NumberFormat);
                    break;
            }
        }

        private void ForceEndAnimCoroutine()
        {
            if (animCoroutine == null)
                return;

            if (animType == GUIAnimType.None)
                return;

            if (animCurve.length < 2)
                return;

            if (animCoroutine != null)
            {
                StopCoroutine(animCoroutine);
                animCoroutine = null;
                if (AnimationEndEvent != null)
                    AnimationEndEvent();
            }

            playCount = 0;

            //Get total time of curve
            float curveTime = 0.0f;
            curveTime = animCurve[animCurve.length - 1].time - animCurve[0].time;

            switch (animType)
            {
                case GUIAnimType.MoveHorizontal:
                case GUIAnimType.MoveVertical:
                case GUIAnimType.MoveToWorldPos:
                case GUIAnimType.MoveToLocalPos:
                    if (isReverseAnim)
                        transform.localPosition = oPosition + move * animCurve.Evaluate(0);
                    else
                        transform.localPosition = oPosition + move * animCurve.Evaluate(curveTime);

                    if (isResetPos)
                        transform.localPosition = oPosition;
                    else
                        oPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
                    break;
                case GUIAnimType.RotateX:
                case GUIAnimType.RotateY:
                case GUIAnimType.RotateZ:
                    if (isReverseAnim)
                        transform.localEulerAngles = oEulerAngles + rot * animCurve.Evaluate(0);
                    else
                        transform.localEulerAngles = oEulerAngles + rot * animCurve.Evaluate(curveTime);

                    if (isResetPos)
                        transform.localEulerAngles = oEulerAngles;
                    else
                        oEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, transform.localEulerAngles.z);
                    break;
                case GUIAnimType.Zoom:
                    if (isReverseAnim)
                        transform.localScale = oScale * zoomScale * (1 + animCurve.Evaluate(0));
                    else
                        transform.localScale = oScale * zoomScale * (1 + animCurve.Evaluate(curveTime));

                    if (isResetPos)
                        transform.localScale = oScale;
                    else
                        oScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
                    break;
                case GUIAnimType.Blink:
                    transform.localScale = oScale;
                    break;
                case GUIAnimType.Gradient:
                    break;
                case GUIAnimType.ScrollInt:
                    if (isReverseAnim)
                        textBox.text = fromInt.ToString(CultureInfo.CurrentCulture.NumberFormat);
                    else
                        textBox.text = toInt.ToString(CultureInfo.CurrentCulture.NumberFormat);
                    break;
                case GUIAnimType.ScrollFloat:
                    if (isReverseAnim)
                        textBox.text = fromFloat.ToString(CultureInfo.CurrentCulture.NumberFormat);
                    else
                        textBox.text = toFloat.ToString(CultureInfo.CurrentCulture.NumberFormat);
                    break;
            }
        }
    }
}
