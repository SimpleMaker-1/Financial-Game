using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.VolumeComponent;


public class GameController : MonoBehaviour
{
    public List<Button> btns = new List<Button>(); // list of all the buttons, instatiated @ GetButtons()

    private List<Tuple<int, int>> selectedPairs = new List<Tuple<int, int>>();

    public Sprite mainImg; //imports sprite
    public Sprite clickedImg;
    public Sprite wrongImg;
    public Sprite rightImg;

    private List<string> _answers = new List<string> 
    {"Stocks" , "Bonds" , "Mutual Funds" , "ETF's" , "CD's" , "Retirement Plans" , "Options" , "Annuities" , "Derivatives" , "Commodities" }; //holds all the instruments to access the dictionary  
    private Dictionary<string, List<string>> _definitions = new Dictionary<string, List<string>>
        {
            { "Stocks", new List<string>
                {
                    "Represents partial ownership in a company, granting you a share of its profits and assets.",
                    "Bought and sold on exchanges, prices fluctuate based on supply and demand.",
                    "May provide regular income through earnings distributions from the company.",
                    "Potential for profit through appreciation in the asset's value over time."

                }
            },
            { "Bonds", new List<string>
                {
                    "Loans made to corporations or governments that pay periodic interest and return principal at maturity.",
                    "Provides regular interest payments, making it a reliable income source.",
                    "Evaluated for creditworthiness by agencies, influencing interest rates and risk.",
                    "Has a set period before the principal amount is repaid, ranging from a few months to several years."

                }
            },
            { "Mutual Funds", new List<string>
                {
                    "Collects money from many investors to buy a diversified portfolio of stocks, bonds, or other assets.",
                    "Operated by fund managers who make investment decisions on behalf of investors.",
                    "Reduces risk by investing in a variety of assets within a single fund.",
                    "The fund's value per share, calculated daily based on the total market value of its assets."

                }
            },
            { "ETF's", new List<string>
                {
                    "Bought and sold on exchanges like individual stocks, with prices fluctuating throughout the trading day.",
                    "Often tracks an index, sector, commodity, or other assets, offering diversification.",
                    "Generally has lower fees compared to mutual funds due to passive management.",
                    "Can be traded at any time during market hours, providing liquidity and flexibility to investors."
                }
            },
            { "CD's", new List<string>
                {
                    "A savings certificate with a fixed maturity date, offering higher interest rates than regular savings accounts.",
                    "Pays a fixed interest rate for the term of the deposit, providing predictable returns.",
                    "Withdrawing funds before maturity typically incurs a penalty.",
                    "In the U.S., deposits are insured by a federal agency up to certain limits."
                }
            },
            { "Retirement Plans", new List<string>
                {
                    "Provides tax benefits to encourage long-term saving for retirement, such as specific accounts.",
                    "Often offered by employers, with potential matching contributions to boost savings.",
                    "Subject to annual contribution limits set by tax authorities.",
                    "Includes rules for withdrawal, often with penalties for early withdrawal before a certain age."
                }
            },
            { "Options", new List<string>
                {
                    "Financial contracts giving the right, but not the obligation, to buy or sell an asset at a set price before a certain date.",
                    "Some contracts allow buying at a set price, while others allow selling at a set price.",
                    "Can control a large position with a relatively small investment, amplifying potential gains or losses.",
                    "Each contract has an expiration date, after which it becomes worthless if not exercised."
                }
            },
            { "Annuities", new List<string>
                {
                    "Provides a steady income stream, often for retirees, in exchange for a lump sum or series of payments.",
                    "Some offer guaranteed payments, while others depend on the performance of invested funds.",
                    "Investment gains grow tax-deferred until withdrawal, similar to retirement accounts.",
                    "Can be structured to provide income for life, reducing the risk of outliving savings."
                }
            },
            { "Derivatives", new List<string>
                {
                    "Derives value from an underlying asset, such as stocks, bonds, commodities, or interest rates.",
                    "Used to hedge against risks or to speculate on price movements of underlying assets.",
                    "Common types include contracts obligating buying or selling an asset at a future date.",
                    "Involves high risk and complexity, often used by advanced traders."
                }
            },
            { "Commodities", new List<string>
                {
                    "Tangible goods such as precious metals, energy resources, agricultural products, and other raw materials traded on exchanges.",
                    "Prices can be influenced by supply and demand, weather, geopolitical events, and economic factors.",
                    "Often used as a hedge against inflation due to their intrinsic value.",
                    "Traded on specialized exchanges, offering futures contracts for delivery at a future date."
                }
            }
            
        }; //holds all the financial instruments and a list of definitions for each instrument

    private bool _firstMatch, _secondMatch;
    private bool isWaiting;

    private int _firstMatchIndex, _secondMatchIndex;

    private Tuple<int, int> Guess;

    private int _guesses;
    private int _correctGuesses;
    private int _gameGuesses;

    private void Start() //instatiates everything
    {
        GetButtons();
        AddListener();
        linkButtons();
        SetBoard();
    }
    void GetButtons()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("PuzzleButton");
        for (int i = 0; i < objects.Length; i++) { 
            btns.Add ( objects[i].GetComponent<Button>() );
            btns[i].image.sprite = mainImg;
        }
    }
    void AddListener() //allows on click events for each button in the list
    {
        for (int i = 0; i < btns.Count; i++)
        {
            btns[i].onClick.AddListener( () => PickPuzzle() );
        }
    }
    public void PickPuzzle()
    {
        string buttonName = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name; // grabs the name of the button selected

        if (!_firstMatch){
            _firstMatch = true;
            _firstMatchIndex = int.Parse( buttonName );
            btns[_firstMatchIndex].image.sprite = clickedImg;
        }
        else if(!_secondMatch){
            _secondMatchIndex = int.Parse(buttonName);

            if(_secondMatchIndex != _firstMatchIndex)
            {
                _secondMatch = true;
                btns[_secondMatchIndex].image.sprite = clickedImg;

                Guess = Tuple.Create(_firstMatchIndex, _secondMatchIndex);

                StartCoroutine(CheckIfCorrect());
            }
        }
    }
    IEnumerator CheckIfCorrect() // checks if the tuple created when clicking on 2 boxes is a tuple in the list of answers.
    {
        yield return new WaitForSeconds(1f);

        if (selectedPairs.Contains(Guess) || selectedPairs.Contains(Tuple.Create(Guess.Item2 , Guess.Item1))) {

            btns[_firstMatchIndex].image.sprite = rightImg;
            btns[_secondMatchIndex].image.sprite = rightImg;

            yield return new WaitForSeconds(1f);

            btns[_firstMatchIndex].interactable = false;
            btns[_secondMatchIndex].interactable = false;

            btns[_firstMatchIndex].image.color = new Color(0, 0, 0, 0);
            btns[_secondMatchIndex].image.color = new Color(0, 0, 0, 0);

            btns[_firstMatchIndex].GetComponentInChildren<TextMeshProUGUI>().text = "";
            btns[_secondMatchIndex].GetComponentInChildren<TextMeshProUGUI>().text = "";

            _correctGuesses++;
            CheckIfGameOver();
        }
        else
        {
            btns[_firstMatchIndex].image.sprite = wrongImg;
            btns[_secondMatchIndex].image.sprite = wrongImg;
            yield return new WaitForSeconds(1f);
            btns[_firstMatchIndex].image.sprite = mainImg;
            btns[_secondMatchIndex].image.sprite = mainImg;
        }
        yield return new WaitForSeconds(0.1f);
        _firstMatch = _secondMatch = false;
    }
    void CheckIfGameOver()
    {
        if (_correctGuesses >= 10)
        {
            Debug.Log("gameOver");
            Debug.Log("it took " + _guesses + " guesses");

        }
    }
    void linkButtons()
    {
        // Generate a list of numbers from 0 to 19
        List<int> numbers = Enumerable.Range(0, 20).ToList();

        // Initialize the Random object
        System.Random rng = new System.Random();

        // Shuffle the list using the Fisher-Yates algorithm
        for (int i = numbers.Count - 1; i > 0; i--)
        {
            int swapIndex = rng.Next(i + 1);
            int temp = numbers[i];
            numbers[i] = numbers[swapIndex];
            numbers[swapIndex] = temp;
        }

        // Create pairs from the shuffled list
        for (int i = 0; i < numbers.Count; i += 2)
        {
            selectedPairs.Add(new Tuple<int, int>(numbers[i], numbers[i + 1]));
        }

    }
    void SetBoard()
    {
        System.Random rand = new System.Random();
        for (int i = 0; i < selectedPairs.Count; i++) {
            string temp = _answers.ElementAt(i); // holds the answer key at i
            btns[selectedPairs[i].Item1].GetComponentInChildren<TextMeshProUGUI>().text = temp;
            btns[selectedPairs[i].Item2].GetComponentInChildren<TextMeshProUGUI>().text = _definitions[temp].ElementAt(rand.Next(3));

            btns[selectedPairs[i].Item1].GetComponentInChildren<TextMeshProUGUI>().color = Color.black;
            btns[selectedPairs[i].Item2].GetComponentInChildren<TextMeshProUGUI>().color = Color.black;
        }
    }
    private bool doesMatch() // checks if the 2 selected buttons match
    {
        return false;
    }

}
