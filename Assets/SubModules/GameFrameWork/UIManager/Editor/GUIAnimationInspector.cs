using UnityEngine;
using System.Collections;

using UnityEditor;
using UnityEngine.UI;

namespace GFramework.GUI
{
    [CustomEditor(typeof(GUIAnimation), true)]
    public class GUIAnimationInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            GUIAnimation myScript = (GUIAnimation)target;

            //Anim type
            myScript.animType = (GUIAnimType)EditorGUILayout.EnumPopup("Anim Type", (System.Enum)myScript.animType);

            //Moving distance or Rotation angle
            if (myScript.animType == GUIAnimType.MoveHorizontal || myScript.animType == GUIAnimType.MoveVertical)
            {
                myScript.moveDistance = EditorGUILayout.FloatField("Move Distance", myScript.moveDistance);
            }
            else if (myScript.animType == GUIAnimType.MoveToWorldPos || myScript.animType == GUIAnimType.MoveToLocalPos)
            {
                myScript.moveTo = EditorGUILayout.Vector3Field("Move To", myScript.moveTo);
            }
            else if (myScript.animType == GUIAnimType.RotateX || myScript.animType == GUIAnimType.RotateY || myScript.animType == GUIAnimType.RotateZ)
            {
                myScript.rotAngle = EditorGUILayout.FloatField("Rotate Angle", myScript.rotAngle);
            }
            else if (myScript.animType == GUIAnimType.Zoom)
            {
                myScript.zoomScale = EditorGUILayout.FloatField("Zoom Scale", myScript.zoomScale);
            }
            else if (myScript.animType == GUIAnimType.Blink)
            {
                myScript.showingPercent = EditorGUILayout.FloatField("Show Percent", myScript.showingPercent);
            }
            else if (myScript.animType == GUIAnimType.ScrollInt)
            {
                myScript.textBox = (Text)EditorGUILayout.ObjectField("Text", myScript.textBox, typeof(Text), true);
                myScript.fromInt = EditorGUILayout.IntField("From", myScript.fromInt);
                myScript.toInt = EditorGUILayout.IntField("To", myScript.toInt);
            }
            else if (myScript.animType == GUIAnimType.ScrollFloat)
            {
                myScript.textBox = (Text)EditorGUILayout.ObjectField("Text", myScript.textBox, typeof(Text), true);
                myScript.fromFloat = EditorGUILayout.FloatField("From", myScript.fromFloat);
                myScript.toFloat = EditorGUILayout.FloatField("To", myScript.toFloat);
            }

            //Anim curve
            myScript.animCurve = EditorGUILayout.CurveField("Anim Curve", myScript.animCurve);

            //Play type
            myScript.playType = (GUIAnimPlayType)EditorGUILayout.EnumPopup("Play Type", (System.Enum)myScript.playType);

            //Loop count
            if (myScript.playType == GUIAnimPlayType.Several)
            {
                myScript.loopCount = EditorGUILayout.IntField("Loop Count", myScript.loopCount);
            }

            //Reverse anim
            myScript.isReverseAnim = EditorGUILayout.Toggle("Reverse Anim", myScript.isReverseAnim);

            //Reset pos after play anim
            myScript.isResetPos = EditorGUILayout.Toggle("Reset Pos", myScript.isResetPos);

            //Use Coroutine
            myScript.isPlayCoroutine = EditorGUILayout.Toggle("Use Coroutine", myScript.isPlayCoroutine);
        }
    }
}
