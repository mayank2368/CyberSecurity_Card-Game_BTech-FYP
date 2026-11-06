using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public static CardManager instance;

    public List<GameObject> AllActiveCards; 

    public List<AttackCardData> AttackCardOptions;
    public List<DefenseCardData> DefenseCardOptions;
    public List<ScenarioCardData> ScenarioCardOptions;

    
    public ScenarioCardData currentScenario;
    public AttackCardData correctAttack;
    public DefenseCardData correctDefense;

    [SerializeField]
    AllCards AllCards;
    TableManager table; 
    public bool attackCardFound = false, defenseCardFound = false; 
    List<GameObject> AttackDeckCards, DefenseDeckCards;

    int scenarioIndex = 0;

    void Awake()
    {
        if (instance != null) 
        {
            Destroy(gameObject);
        } 
        else 
        {
            instance = this;
        }
    }

    void Start() {
        table = TableManager.instance;
        AllActiveCards = new List<GameObject>();
        AttackDeckCards = new List<GameObject>();
        DefenseDeckCards = new List<GameObject>();
        AttackCardOptions = AllCards.AttackCards;
        DefenseCardOptions = AllCards.DefenseCards;
        ScenarioCardOptions = AllCards.ScenarioCards;
        ShuffleDecks();
        ShuffleScenarios();
        // CreateRandomScenario();
        CreateFixedScenario();
        AudioManager.instance.PlayBGM("kuia");
    }

    void ResetScenario()
    {
        attackCardFound = false;
        defenseCardFound = false;
        foreach (GameObject _card in AllActiveCards)
        {
            Destroy(_card);
        }
        AllActiveCards.Clear();
        AttackDeckCards.Clear();
        DefenseDeckCards.Clear();
        currentScenario = null;
        correctAttack = null;
        correctDefense = null;
        attackDeckIndex = 0;
        defenseDeckIndex = 0;
        ShuffleDecks();
    }

    
    void ShuffleDecks()
    {
        int _rand;
        for (int x = 0; x < AttackCardOptions.Count; x++)
        {
            _rand = Random.Range(0, x);
            AttackCardData _tempAttackData = AttackCardOptions[x];
            AttackCardOptions[x] = AttackCardOptions[_rand];
            AttackCardOptions[_rand] = _tempAttackData;
        }
        for (int y = 0; y < DefenseCardOptions.Count; y++)
        {
            _rand = Random.Range(0, y);
            DefenseCardData _tempDefData = DefenseCardOptions[y];
            DefenseCardOptions[y] = DefenseCardOptions[_rand];
            DefenseCardOptions[_rand] = _tempDefData;
        }
    }

    void ShuffleScenarios()
    {
        int _rand;
        for (int y = 0; y < ScenarioCardOptions.Count; y++)
        {
            _rand = Random.Range(0, y);
            ScenarioCardData _tempSceData = ScenarioCardOptions[y];
            ScenarioCardOptions[y] = ScenarioCardOptions[_rand];
            ScenarioCardOptions[_rand] = _tempSceData;
        }
    }

    public void CreateRandomScenario()
    {
        LevelManager.instance.resetAttempts();
        
        currentScenario = AllCards.ScenarioCards[Random.Range(0, AllCards.ScenarioCards.Count)];
        correctAttack = currentScenario.CorrectAttack;
        correctDefense = currentScenario.CorrectDefense;

        SpawnCards();
    }

    public void CreateFixedScenario()
    {
        LevelManager.instance.resetAttempts();
        if (scenarioIndex >= AllCards.ScenarioCards.Count)
        {
            scenarioIndex = 0;
        }

        currentScenario = AllCards.ScenarioCards[scenarioIndex];
        correctAttack = currentScenario.CorrectAttack;
        correctDefense = currentScenario.CorrectDefense;
        scenarioIndex++;
        SpawnCards();
    }

    void SpawnCards()
    {
        
        TouchHandler.instance.UpdateSingleplayerCardMask(DeckType.AttackDeck);

        
        GameObject scenarioCard = Instantiate(currentScenario.CardPrefab);
        AllActiveCards.Add(scenarioCard);
        scenarioCard.GetComponent<ScenarioCard>().SetData(currentScenario);
        scenarioCard.transform.SetParent(table.scenarioCardHolder.transform, false);

        
        DefenseDeckTapped();
        AttackDeckTapped();
        AudioManager.instance.PlaySfx("shuffel");
        // spawn the 4 attack and defense cards
        // for (int i = 0; i < 4; i++)
        // {
        //     GameObject attackCard = Instantiate(AttackCardOptions[i].CardPrefab);
        //     AllActiveCards.Add(attackCard);
        //     attackCard.transform.SetParent(table.attackCardsHolder[i].transform, false);
        //     attackCard.GetComponent<AttackCard>().SetData(AttackCardOptions[i]);

        //     GameObject defenseCard = Instantiate(DefenseCardOptions[i].CardPrefab);
        //     AllActiveCards.Add(defenseCard);
        //     defenseCard.transform.SetParent(table.defenseCardsHolder[i].transform, false);
        //     defenseCard.GetComponent<DefenseCard>().SetData(DefenseCardOptions[i]);
        // }

        ReLayerCards(AllActiveCards[AllActiveCards.Count-1]);
    }

   
    public void CorrectCardFound(DeckType _type, GameObject _card)
    {
        AudioManager.instance.PlaySfx("correct");

        if (DeckType.AttackDeck == _type)
        {
            attackCardFound = true;
            AttackDeckCards.Remove(_card);
            LevelManager.instance.addScore(5);
            LevelManager.instance.increaseScoreStreak();
           
            TouchHandler.instance.UpdateSingleplayerCardMask(DeckType.DefenseDeck);
        }
        else if (DeckType.DefenseDeck == _type)
        {
            defenseCardFound = true;
            DefenseDeckCards.Remove(_card);
            LevelManager.instance.addScore(5);
            LevelManager.instance.increaseScoreStreak();
        }

        if (attackCardFound && defenseCardFound)
        {
            Debug.Log("attack and defense card found! ");
            StartCoroutine(LoadNextScenario());
        }
    }

    public void RemoveFromDecks(GameObject _card)
    {
        AttackDeckCards.Remove(_card);
        DefenseDeckCards.Remove(_card);
    }
    public void DestoryCard(GameObject _card)
    {
        if (_card != null)
        {
            AllActiveCards.Remove(_card);
            Destroy(_card);
        }
    }

    int attackDeckIndex = 0; 
    public void AttackDeckTapped()
    {
        if (attackCardFound)
        {
            return;
        }

        
        if (attackDeckIndex >= AttackCardOptions.Count)
        {
            attackDeckIndex = 0;
        }
        foreach (GameObject _attackCard in AttackDeckCards)
        {
            AllActiveCards.Remove(_attackCard);
            Destroy(_attackCard);
        }
        AttackDeckCards.Clear();

        GameObject attackCard = Instantiate(AttackCardOptions[attackDeckIndex].CardPrefab);
        attackCard.transform.SetParent(table.attackDeckHolder.transform, false);
        attackCard.GetComponent<AttackCard>().SetData(AllCards.AttackCards[attackDeckIndex]);
        AllActiveCards.Add(attackCard);
        AttackDeckCards.Add(attackCard);
        attackDeckIndex++;
        ReLayerCards(attackCard);
    }

    int defenseDeckIndex = 0;
    public void DefenseDeckTapped()
    {
        if (defenseCardFound)
        {
            return;
        }

        
        if (defenseDeckIndex >= DefenseCardOptions.Count)
        {
            defenseDeckIndex = 0;
        }
        foreach (GameObject _defenseCard in DefenseDeckCards)
        {
            AllActiveCards.Remove(_defenseCard);
            Destroy(_defenseCard);
        }
        DefenseDeckCards.Clear();

        GameObject defenseCard = Instantiate(AllCards.DefenseCards[defenseDeckIndex].CardPrefab);
        defenseCard.transform.SetParent(table.defenseDeckHolder.transform, false);
        defenseCard.GetComponent<DefenseCard>().SetData(AllCards.DefenseCards[defenseDeckIndex]);
        AllActiveCards.Add(defenseCard);
        DefenseDeckCards.Add(defenseCard);
        defenseDeckIndex++;
        ReLayerCards(defenseCard);
    }

    public void displayCorrectCards()
    {
        StartCoroutine(showCorrectCards());
    }
    
    
    
    public void ReLayerCards(GameObject _card)
    {
        const float zMultiplier = -0.01f;
        Vector3 _cardPos;
        
        if (AllActiveCards.Remove(_card))
        {
            AllActiveCards.Add(_card);
        }

        
        for (int i = 0; i < AllActiveCards.Count; i++)
        {
            _cardPos = AllActiveCards[i].transform.position;
            _cardPos.z = zMultiplier * (i+1);
            AllActiveCards[i].transform.position = _cardPos;
        }
    }

    
    IEnumerator LoadNextScenario()
    {
        TouchHandler.instance.TouchIsEnabled = false; 
        yield return new WaitForSeconds(1);
        ResetScenario();
        yield return new WaitForSeconds(1);
        // CreateRandomScenario();
        CreateFixedScenario();
        TouchHandler.instance.TouchIsEnabled = true; 
    }

    IEnumerator showCorrectCards()
    {
        yield return new WaitForSeconds(2);
        if (!attackCardFound)
        {
            GameObject attackCard = Instantiate(correctAttack.CardPrefab);
            AllActiveCards.Add(attackCard);
            attackCard.GetComponent<AttackCard>().SetData(correctAttack);
            Vector3 _pos = table.attackPlacement.transform.position;
            _pos.z = -10;
            attackCard.transform.position = _pos;
        }
        if (!defenseCardFound)
        {
            GameObject defenseCard = Instantiate(correctDefense.CardPrefab);
            AllActiveCards.Add(defenseCard);
            defenseCard.GetComponent<DefenseCard>().SetData(correctDefense);
            Vector3 _pos = table.defensePlacement.transform.position;
            _pos.z = -10;
            defenseCard.transform.position = _pos;
        }
        yield return new WaitForSeconds(2);
        StartCoroutine(LoadNextScenario());
    }
}
