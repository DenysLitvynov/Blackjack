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

    public int[] values = new int[52];
    int cardIndex = 0;

    private void Awake()
    {
        InitCardValues();
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
        for (int i = 0; i < 2; i++)
        {
            PushPlayer();
            PushDealer();
        }

        if (player.GetComponent<CardHand>().points == 21 && dealer.GetComponent<CardHand>().points != 21)
        {
            finalMessage.text = "¡Blackjack! Has ganado.";
        }
        else if (dealer.GetComponent<CardHand>().points == 21 && player.GetComponent<CardHand>().points != 21)
        {
            finalMessage.text = "El dealer tiene Blackjack. Has perdido.";
        }
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
        if (cardIndex <= 4)
        {
            dealer.GetComponent<CardHand>().InitialToggle();
        }

        PushPlayer();

        if (player.GetComponent<CardHand>().points > 21)
        {
            finalMessage.text = "Has perdido. Tus puntos superan los 21.";
        }
    }

    public void Stand()
    {
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
        }
        else if (dealer.GetComponent<CardHand>().points > player.GetComponent<CardHand>().points)
        {
            finalMessage.text = "Has perdido. El dealer tiene más puntos.";
        }
        else if (dealer.GetComponent<CardHand>().points < player.GetComponent<CardHand>().points)
        {
            finalMessage.text = "Has ganado. Tienes más puntos que el dealer.";
        }
        else
        {
            finalMessage.text = "Es un empate.";
        }
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


