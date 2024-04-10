using UnityEngine;
using UnityEngine.UI;

public class Deck : MonoBehaviour
{
    public Sprite[] faces;
    public GameObject dealer;
    public GameObject player;
    public Button hitButton;
    public Button stickButton;
    public Button playAgainButton;
    public Text finalMessage;
    public Text probMessage;
    public Text dealerPointsText;
    public Text playerPointsText;
    public Text creditText;
    public Dropdown betDropdown;

    public int[] values = new int[52];
    int cardIndex = 0;
    int bank = 1000;

    private void Awake()
    {
        InitCardValues();
        dealerPointsText.text = "";
        playerPointsText.text = "Puntos del jugador: 0";
        creditText.text = "Crédito: " + bank.ToString() + "€";
    }

    private void Start()
    {
        ShuffleCards();
        StartGame();
    }

    private void InitCardValues()
    {
        for (int i = 0; i < values.Length; i++)
        {
            if (i % 13 == 0)
                values[i] = 11; // As
            else if (i % 13 < 9)
                values[i] = (i % 13) + 1; // Cartas del 2 al 10
            else
                values[i] = 10; // J, Q, K
        }
    }

    private void ShuffleCards()
    {
        for (int i = 0; i < values.Length; i++)
        {
            int randomIndex = Random.Range(0, values.Length);
            int tempValue = values[i];
            Sprite tempFace = faces[i];

            values[i] = values[randomIndex];
            faces[i] = faces[randomIndex];

            values[randomIndex] = tempValue;
            faces[randomIndex] = tempFace;
        }
    }

    void StartGame()
    {
        int bet = int.Parse(betDropdown.options[betDropdown.value].text);
        if (bet > bank)
        {
            finalMessage.text = "Apuesta no válida.";
            return;
        }
        bank -= bet;
        creditText.text = "Crédito: " + bank.ToString() + "€";

        PushDealer();
        PushDealer();

        PushPlayer();
        PushPlayer();

        if (player.GetComponent<CardHand>().points == 21 && dealer.GetComponent<CardHand>().points != 21)
        {
            finalMessage.text = "¡Blackjack! Has ganado.";
            bank += bet * 2;
        }
        else if (dealer.GetComponent<CardHand>().points == 21 && player.GetComponent<CardHand>().points != 21)
        {
            finalMessage.text = "El dealer tiene Blackjack. Has perdido.";
        }
        creditText.text = "Crédito: " + bank.ToString() + "€";
        playerPointsText.text = "Puntos del jugador: " + player.GetComponent<CardHand>().points.ToString();
    }


    private void CalculateProbabilities()
    {
        int remainingCards = 52 - cardIndex;
        int favorableCards = 0;
        int bustCards = 0;
        int dealerWinCards = 0;

        for (int i = cardIndex; i < 52; i++)
        {
            if (values[i] + player.GetComponent<CardHand>().points >= 17 && values[i] + player.GetComponent<CardHand>().points <= 21)
            {
                favorableCards++;
            }
            if (values[i] + player.GetComponent<CardHand>().points > 21)
            {
                bustCards++;
            }
            if (values[i] + dealer.GetComponent<CardHand>().points > player.GetComponent<CardHand>().points)
            {
                dealerWinCards++;
            }
        }

        float probabilityOfGetting17To21 = (float)favorableCards / remainingCards;
        float probabilityOfBusting = (float)bustCards / remainingCards;
        float probabilityOfDealerWinning = (float)dealerWinCards / remainingCards;

        // Actualizamos los campos de la interfaz de usuario
        GameObject.Find("TitleProb").GetComponent<Text>().text = "Probabilidades:";
        GameObject.Find("TextProb").GetComponent<Text>().text = "Deal > Play: " + probabilityOfDealerWinning.ToString("P2") + "\n17<=X<=21: " + probabilityOfGetting17To21.ToString("P2") + "\nX > 21: " + probabilityOfBusting.ToString("P2");
    }

    void PushDealer()
    {
        dealer.GetComponent<CardHand>().Push(faces[cardIndex], values[cardIndex]);
        cardIndex++;
    }

    void PushPlayer()
    {
        player.GetComponent<CardHand>().Push(faces[cardIndex], values[cardIndex]);
        cardIndex++;
        CalculateProbabilities();
    }

    public void Hit()
    {
        int bet = int.Parse(betDropdown.options[betDropdown.value].text);

        if (cardIndex <= 4)
        {
            dealer.GetComponent<CardHand>().InitialToggle();
        }

        PushPlayer();

        if (player.GetComponent<CardHand>().points > 21)
        {
            finalMessage.text = "Has perdido. Tus puntos superan los 21.";
            dealerPointsText.text = "Puntos del dealer: " + dealer.GetComponent<CardHand>().points.ToString();
        }
        else if (player.GetComponent<CardHand>().points == 21)
        {
            finalMessage.text = "¡Blackjack! Has ganado.";
            bank += bet * 2;
            dealerPointsText.text = "Puntos del dealer: " + dealer.GetComponent<CardHand>().points.ToString();
        }
        creditText.text = "Crédito: " + bank.ToString() + "€";
        playerPointsText.text = "Puntos del jugador: " + player.GetComponent<CardHand>().points.ToString();
    }

    public void Stand()
    {
        int bet = int.Parse(betDropdown.options[betDropdown.value].text);

        if (cardIndex <= 4)
        {
            dealer.GetComponent<CardHand>().InitialToggle();
        }

        while (dealer.GetComponent<CardHand>().points <= 16)
        {
            PushDealer();
        }

        if (dealer.GetComponent<CardHand>().points > 21)
        {
            finalMessage.text = "Has ganado. El dealer ha superado los 21 puntos.";
            bank += bet * 2;
        }
        else if (dealer.GetComponent<CardHand>().points > player.GetComponent<CardHand>().points)
        {
            finalMessage.text = "Has perdido. El dealer tiene más puntos.";
        }
        else if (dealer.GetComponent<CardHand>().points < player.GetComponent<CardHand>().points)
        {
            finalMessage.text = "Has ganado. Tienes más puntos que el dealer.";
            bank += bet * 2;
        }
        else
        {
            finalMessage.text = "Es un empate.";
            bank += bet;
        }
        creditText.text = "Crédito: " + bank.ToString() + "€";
        dealerPointsText.text = "Puntos del dealer: " + dealer.GetComponent<CardHand>().points.ToString();
        playerPointsText.text = "Puntos del jugador: " + player.GetComponent<CardHand>().points.ToString();
    }

    public void PlayAgain()
    {
        hitButton.interactable = true;
        stickButton.interactable = true;
        finalMessage.text = "";
        player.GetComponent<CardHand>().Clear();
        dealer.GetComponent<CardHand>().Clear();
        cardIndex = 0;
        ShuffleCards();
        StartGame();
    }
}



