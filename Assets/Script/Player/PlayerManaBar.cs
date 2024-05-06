using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerManaBar : MonoBehaviour
{
   /* public ManaBar manaBar; 
    public PlayerQ3LikeController playerController;
    public TextMeshProUGUI manaText;

    private void Start()
    {
        playerController = GetComponent<PlayerQ3LikeController>();
        //UpdateManaBar();
    }

    private void Update()
    {
        manaBar.SetSlider(playerController.mana);
        int intMana = (int)playerController.mana;
        manaText.text = intMana.ToString();
    }
    

    /* public void UpdateManaBar()
    {
        manaBar.SetSlider(playerController.mana);
        int intMana = (int)playerController.mana;
        manaText.text = intMana.ToString();
    }
   public void SetMana(int amount)
    {
        manaBar.SetSlider(amount);
        playerController.mana = (int)amount;
        UpdateManaBar();
    }*/
    
    //private void UpdateManaBar() => manaText.text =  playerController.mana.ToString();
}