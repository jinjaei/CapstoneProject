using UnityEngine;
using UnityEngine.UI;
using System;

/*
 * Screen displayed at the start of the game.
 * If the game is launched for the first time, it will display a tutorial.
 * Otherwise it display how many games were generated while the demo was closed.
 * 
 */ 
namespace IV_Demo
{
    [DefaultExecutionOrder(100)]
    public class StartWindow : MonoBehaviour
    {
        // custom types
        public enum TextPosition
        {
            Mid,
            Left,
            Right,
        }

        public enum ShowPanel
        {
            NoChange,
            Play,
            Cheat,
            Upgrades,
            Incomes,
        }

        [Serializable] public class TutoStep
        {
            public GameObject cover;
            public ShowPanel showPanel;
            public bool enablePlayZone;
            [Space]
            public TextPosition textPosition;
            [TextArea(2, 10)] public string textContent;
        }

        // public fields
        [Header("Games while away screen")]
        public GameObject entryTextObj;
        [TextArea(2, 10)] public string gamesWhileAwayFormat = "You made {0:< 5} Games while you were away !";

        [Header("Tutorial")]
        public TutoStep[] steps;

        [Header("References")]
        public Text midText;
        public Text leftText;
        public Text rightText;
        [Space]
        public PlayZone playZone;
        public CenterPanelChooser leftChooser;
        public CenterPanelChooser rightChooser;
        public Button[] tutoDisabledButtons;
        
        // private fields and properties
        bool isInTuto;

        int stepIndex = -1;
        TutoStep currentStep => (stepIndex >= 0 && stepIndex < steps.Length ? steps[stepIndex] : null);

        // public methods
        public void OnCoverClick()
        {
            Audio.PlaySound(Audio.Sound.MenuChange);

            if (isInTuto)
                GoToNextStep();
            else
                gameObject.SetActive(false);
        }

        // unity
        void Start()
        {
            if (Inventory.games > 0)
            {
                entryTextObj.GetComponentInChildren<Text>().text = string.Format(gamesWhileAwayFormat, Inventory.gamesMadeWhileAway);
                entryTextObj.GetComponentInChildren<Button>().onClick.AddListener(BeginTutorial);
            }
            else
                BeginTutorial();
        }

        // private methods
        void BeginTutorial()
        {
            // disable stuffs
            entryTextObj.SetActive(false);

            playZone.enabled = false;

            foreach (Button b in tutoDisabledButtons)
                b.enabled = false;

            // start tuto
            isInTuto = true;

            midText.enabled = false;
            leftText.enabled = false;
            rightText.enabled = false;

            stepIndex = -1;
            GoToNextStep();
        }

        void EndTutorial()
        {
            // reenable stuffs
            playZone.enabled = true;

            foreach (Button b in tutoDisabledButtons)
                b.enabled = true;

            // set panels
            leftChooser.OnClickLeft(true);
            rightChooser.OnClickLeft(true);

            // stop the start window
            gameObject.SetActive(false);
        }

        void GoToNextStep()
        {
            // transition away from current
            if (currentStep != null)
            {
                if (currentStep.cover != null)
                    currentStep.cover.SetActive(true);

                if (currentStep.enablePlayZone)
                    playZone.enabled = false;

                GetText(currentStep.textPosition).enabled = false;
            }

            ++stepIndex;

            // transition to next
            if (currentStep != null)
            {
                if (currentStep.cover != null)
                    currentStep.cover.SetActive(false);

                switch (currentStep.showPanel)
                {
                    case ShowPanel.Play: leftChooser.OnClickLeft(true); break;
                    case ShowPanel.Cheat: leftChooser.OnClickRight(true); break;
                    case ShowPanel.Upgrades: rightChooser.OnClickLeft(true); break;
                    case ShowPanel.Incomes: rightChooser.OnClickRight(true); break;
                }

                if (currentStep.enablePlayZone)
                    playZone.enabled = true;

                GetText(currentStep.textPosition).enabled = true;
                GetText(currentStep.textPosition).text = currentStep.textContent;
            }
            else
                EndTutorial();
        }

        Text GetText(TextPosition pos)
        {
            if (pos == TextPosition.Left)
                return leftText;
            if (pos == TextPosition.Right)
                return rightText;
            return midText;
        }
    }
}