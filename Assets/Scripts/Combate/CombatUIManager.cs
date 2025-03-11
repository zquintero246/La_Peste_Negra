using UnityEngine;
using UnityEngine.UI;

public class CombatUIManager : MonoBehaviour
{
    public Button[] actionButtons;  
    private int selectedIndex = 0;

    public Slider playerHealthBar;
    public Slider enemyHealthBar;

    void Start()
    {
        UpdateSelection();

        playerHealthBar.maxValue = CombatManager.Instance.playerHealth;
        playerHealthBar.value = CombatManager.Instance.playerHealth;

        enemyHealthBar.maxValue = CombatManager.Instance.enemyHealth;
        enemyHealthBar.value = CombatManager.Instance.enemyHealth;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            selectedIndex = (selectedIndex + 1) % actionButtons.Length;
            UpdateSelection();
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            selectedIndex = (selectedIndex - 1 + actionButtons.Length) % actionButtons.Length;
            UpdateSelection();
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            SelectAction();
        }
    }

    void UpdateSelection()
    {
        for (int i = 0; i < actionButtons.Length; i++)
        {
            ColorBlock colors = actionButtons[i].colors;
            colors.normalColor = (i == selectedIndex) ? Color.gray : Color.white;
            actionButtons[i].colors = colors;
        }
    }

    public void UpdateHealthBars()
    {
        playerHealthBar.value = CombatManager.Instance.playerHealth;
        enemyHealthBar.value = CombatManager.Instance.enemyHealth;
    }

    void SelectAction()
    {
        string action = actionButtons[selectedIndex].name;

        if (CombatManager.Instance == null)
        {
            Debug.LogError("CombatManager no está inicializado en la escena.");
            return;
        }

        switch (action)
        {
            case "Atacar":
                CombatManager.Instance.PlayerAttack();
                break;
            case "Defender":
                CombatManager.Instance.Defend();
                break;
            case "Objeto":
                CombatManager.Instance.UseItem();
                break;
            case "Huir":
                CombatManager.Instance.Huir();
                break;
        }
    }

}
