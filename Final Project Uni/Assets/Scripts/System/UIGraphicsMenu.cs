using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PA
{
    public class UIGraphicsMenu : MonoBehaviour
    {
        [SerializeField] private GameObject graphicsPanel;
        [SerializeField] private TMP_Dropdown fpsDropdown;
        [SerializeField] private Toggle vSyncToggle;
        [SerializeField] private Toggle antiAliasingToggle;
        [SerializeField] private Toggle bloomToggle;
        [SerializeField] private Button backGraphicsSettingsButton;

        private UIMainMenu mainMenu;
        private MakeTransitionUI makeTransitionUI;

        private void Awake()
        {
            mainMenu = GetComponentInParent<UIMainMenu>();
            makeTransitionUI = mainMenu.makeTransitionUI;
        }

        private void Start()
        {
            LoadSettings();
        }

        private void LoadSettings()
        {
            GameSettings settings = GameSettings.Instance;
            vSyncToggle.isOn = settings.isVSyncEnabled;
            antiAliasingToggle.isOn = settings.isAntiAliasingEnabled;
            bloomToggle.isOn = settings.isBloomEnabled;
        }

        public void OnBackGraphicsSettingsClicked()
        {
            mainMenu.GeneralClick(mainMenu.SettingsMenu.SettingsPanel, graphicsPanel);
        }

        public void OnVSyncToggled(bool isOn)
        {
            GameSettings.Instance.isVSyncEnabled = isOn;
        }

        public void OnAntiAliasingToggled(bool isOn)
        {
            GameSettings.Instance.isAntiAliasingEnabled = isOn;
        }

        public void OnBloomToggled(bool isOn)
        {
            GameSettings.Instance.isBloomEnabled = isOn;
        }

        public void DropdownFPSChanged(int value)
        {
            QualitySettings.SetFPS(value);
        }

        public GameObject GraphicsPanel => graphicsPanel;
    }
} 