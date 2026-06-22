using UnityEngine;
using UnityEngine.UI;

namespace GFramework.Helper
{
    public class MovingTextHelper : MonoBehaviour
    {
        public string songName;

        public Text text1;
        public Text text2;
        private float textWidth = 0;

        private Vector2 v2Text1;
        private Vector2 v2Text2;

        private float offSetBetween2Text = 50;

        private bool bNeedScroll = false;

        private bool bInitDone = false;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (!bInitDone)
                return;

            if (bNeedScroll == false)
                return;

            UpdateMovingText();

            UpdateRollText();
        }

        private void UpdateMovingText()
        {
            v2Text1.x -= 1;
            text1.rectTransform.anchoredPosition = v2Text1;

            v2Text2.x -= 1;
            text2.rectTransform.anchoredPosition = v2Text2;
        }

        private void UpdateRollText()
        {
            if (v2Text1.x < -textWidth)
            {
                v2Text1.x = textWidth;
            }

            if (v2Text2.x < -textWidth)
            {
                v2Text2.x = textWidth;
            }
        }

        public void SetSongNameAndInit(string _songName)
        {
            songName = _songName;

            //Debug.LogError(text1.rectTransform.anchoredPosition.x);

            text1.text = songName;
            text2.text = songName;

            //offSetBetween2Text = songName.Length * 5;
            //textWidth = text1.preferredWidth + offSetBetween2Text;
            textWidth = text1.preferredWidth;

            v2Text1 = Vector2.zero;
            v2Text2 = new Vector2(textWidth, 0);

            text1.rectTransform.anchoredPosition = v2Text1;
            text2.rectTransform.anchoredPosition = v2Text2;

            CheckNeedScroll();

            bInitDone = true;
        }

        private void CheckNeedScroll()
        {
            text1.gameObject.SetActive(true);
            text2.gameObject.SetActive(true);

            //Debug.LogError(songName + " --- " + textWidth + " --- " + GetComponent<RectTransform>().sizeDelta.x);

            if (textWidth > GetComponent<RectTransform>().sizeDelta.x)
            {
                bNeedScroll = true;
                text2.gameObject.SetActive(true);
            }
            else
            {
                bNeedScroll = false;
                text2.gameObject.SetActive(false);
            }
        }
    }
}