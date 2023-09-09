using UnityEngine;
using UnityEngine.UI;

/*
 * Component used by the left and right panels to show two sub-panels.
 * 
 */ 
namespace IV_Demo
{
    public class CenterPanelChooser : MonoBehaviour
    {
        public Color onColor = Color.white;
        public Color offColor = Color.gray;

        public Button leftButton;
        public Button rightButton;

        public GameObject leftPanel;
        public GameObject rightPanel;

        void Awake()
        {
            leftButton.onClick.AddListener(() => OnClickLeft());
            rightButton.onClick.AddListener(() => OnClickRight());

            OnClickLeft();
        }

        public void OnClickLeft(bool calledFromTuto = false)
        {
            leftPanel.SetActive(true);
            rightPanel.SetActive(false);

            leftButton.targetGraphic.color = onColor;
            rightButton.targetGraphic.color = offColor;

            if (!calledFromTuto)
            {
                leftButton.enabled = false;
                rightButton.enabled = true;

                Audio.PlaySound(Audio.Sound.MenuChange);
            }
        }

        public void OnClickRight(bool calledFromTuto = false)
        {
            leftPanel.SetActive(false);
            rightPanel.SetActive(true);

            leftButton.targetGraphic.color = offColor;
            rightButton.targetGraphic.color = onColor;

            if (!calledFromTuto)
            {
                rightButton.enabled = false;
                leftButton.enabled = true;

                Audio.PlaySound(Audio.Sound.MenuChange);
            }
        }
    }
}