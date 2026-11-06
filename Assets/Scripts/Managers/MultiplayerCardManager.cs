using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MultiplayerCardManager : MonoBehaviour
{
    public static MultiplayerCardManager instance;

    public List<GameObject> AllActiveCards; 

    public List<AttackCardData> AttackCardOptions; 
    public List<DefenseCardData> DefenseCardOptions; 

    public ScenarioCardData currentScenario; 
    public AttackCardData placedAttack; 
    public DefenseCardData placedDefense; 

    [SerializeField]
    AllCards AllCards; 
    MultiplayerTableManager table; 
    bool correctDefensePlayed = false; 
    List<GameObject> AttackDeckCards, DefenseDeckCards; 

    
    List<int> attackCardOptionIds, defenseCardOptionsIds;

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
        gameObject.SetActive(false);
    }

    public void Initalize() 
    {
        table = MultiplayerTableManager.instance;
        AllActiveCards = new List<GameObject>();
        AttackDeckCards = new List<GameObject>();
        DefenseDeckCards = new List<GameObject>();
        attackCardOptionIds = new List<int>();
        defenseCardOptionsIds = new List<int>();
        
        MultiplayerTableManager.instance.profile1.setName(PhotonNetwork.MasterClient.NickName, MultiplayerManager.instance.player1DP);
        MultiplayerTableManager.instance.profile2.setName(PhotonNetwork.MasterClient.GetNext().NickName, MultiplayerManager.instance.player2DP);

        AudioManager.instance.PlayBGM("kuia");
    }

    void ResetScenario()
    {
        correctDefensePlayed = false;
        foreach (GameObject _card in AllActiveCards)
        {
            Destroy(_card);
        }
        AllActiveCards.Clear();
        AttackDeckCards.Clear();
        DefenseDeckCards.Clear();
        attackCardOptionIds.Clear();
        defenseCardOptionsIds.Clear();
        currentScenario = null;
        placedAttack = null;
        placedDefense = null;
        attackDeckIndex = 0;
        defenseDeckIndex = 0;

        // if (PhotonNetwork.IsMasterClient)
        // {
        //     MultiplayerManager.instance.RaiseTurnUpdate();
        // }
        MultiplayerManager.instance.TurnUpdate();
    }

    public void UpdateCurrentAssetData()
    {
        Debug.Log("UpdateCurrentAssetData");
        
        int currentRoundNumber = MultiplayerManager.instance.RoundNumber;
        if (currentRoundNumber%3 == 1) 
        {
            currentScenario = AllCards.Level2ScenarioCards[0];
            AttackCardOptions = AllCards.WifiAttackCards;
            DefenseCardOptions = AllCards.WifiDefenseCards;
        }
        else if (currentRoundNumber%3 == 2) 
        {
            currentScenario = AllCards.Level2ScenarioCards[1];
            AttackCardOptions = AllCards.PhoneAttackCards;
            DefenseCardOptions = AllCards.PhoneDefenseCards;
        }
        else if (currentRoundNumber%3 == 0) 
        {
            currentScenario = AllCards.Level2ScenarioCards[2];
            AttackCardOptions = AllCards.ComputerAttackCards;
            DefenseCardOptions = AllCards.ComputerDefenseCards;
        }

        

        attackDeckIndex = 0;
        defenseDeckIndex = 0;

        SpawnCards();
    }

    
    // void ShuffelDecks()
    // {
    //     int _rand;
    //     for (int x = 0; x < AttackCardOptions.Count; x++)
    //     {
    //         _rand = Random.Range(0, x);
    //         AttackCardData _tempAttackData = AttackCardOptions[x];
    //         AttackCardOptions[x] = AttackCardOptions[_rand];
    //         AttackCardOptions[_rand] = _tempAttackData;
    //     }
    //     for (int y = 0; y < DefenseCardOptions.Count; y++)
    //     {
    //         _rand = Random.Range(0, y);
    //         DefenseCardData _tempDefData = DefenseCardOptions[y];
    //         DefenseCardOptions[y] = DefenseCardOptions[_rand];
    //         DefenseCardOptions[_rand] = _tempDefData;
    //     }

    //     // update it across the network
    //     NetworkedShuffelDecks();
    //     SpawnCards();
    // }

    
    // public void NetworkedShuffelDecks()
    // {
    //     foreach (AttackCardData _data in AttackCardOptions)
    //     {
    //         attackCardOptionIds.Add(_data.ID);
    //     }
    //     foreach (DefenseCardData _data in DefenseCardOptions)
    //     {
    //         defenseCardOptionsIds.Add(_data.ID);
    //     }

    //     MultiplayerManager.instance.NetworkedShuffelCards(attackCardOptionIds, defenseCardOptionsIds);
    // }

    
    // public void ApplyNetworkedShuffelDecks(List<int> _attackCardOptionIds, List<int> _defenseCardOptionsIds)
    // {
    //     Debug.Log(_attackCardOptionIds.Count);
    //     Debug.Log(_defenseCardOptionsIds.Count);
    //     for (int x = 0; x < _attackCardOptionIds.Count; x++)
    //     {
    //         int tempIndex = AttackCardOptions.FindIndex((attackCard) => attackCard.ID == _attackCardOptionIds[x]);
    //         AttackCardData temp = AttackCardOptions[x];
    //         AttackCardOptions[x] = AttackCardOptions[tempIndex];
    //         AttackCardOptions[tempIndex] = temp;
    //     }
    //     for (int y = 0; y < _defenseCardOptionsIds.Count; y++)
    //     {
    //         int tempIndex = DefenseCardOptions.FindIndex((defenseCard) => defenseCard.ID == _defenseCardOptionsIds[y]);
    //         DefenseCardData temp = DefenseCardOptions[y];
    //         DefenseCardOptions[y] = DefenseCardOptions[tempIndex];
    //         DefenseCardOptions[tempIndex] = temp;
    //     }

    //     SpawnCards();
    // }

    void SpawnCards()
    {
        
        GameObject scenarioCard = Instantiate(currentScenario.CardPrefab);
        AllActiveCards.Add(scenarioCard);
        scenarioCard.GetComponent<ScenarioCard>().SetData(currentScenario);
        scenarioCard.transform.SetParent(table.scenarioCardHolder.transform, false);

        
        DefenseDeckTapped();
        AttackDeckTapped();
        AudioManager.instance.PlaySfx("shuffel");

        ReLayerCards(AllActiveCards[AllActiveCards.Count-1]);

        TouchHandler.instance.TouchIsEnabled = true; 
    }

    
    // public void CorrectCardFound(DeckType _type, GameObject _card)
    // {
    //     AudioManager.instance.PlaySfx("correct");

    //     if (DeckType.AttackDeck == _type)
    //     {
    //         attackCardFound = true;
    //         AttackDeckCards.Remove(_card);
    //     }
    //     else if (DeckType.DefenseDeck == _type)
    //     {
    //         defenseCardFound = true;
    //         DefenseDeckCards.Remove(_card);
    //     }

    //     if (attackCardFound && defenseCardFound)
    //     {
    //         Debug.Log("attack and defense card found! ");
    //         // LevelManager.instance.addScore(10);
    //         StartCoroutine(LoadNextScenario());
    //     }
    // }

    
    public void UpdateMyControls(DeckType _currentTurn, int _actorId)
    {
        Debug.Log("current actor "+_actorId);
        if (PhotonNetwork.LocalPlayer.ActorNumber == _actorId)
        {
            TouchHandler.instance.UpdateMultiplayerCardMask(_currentTurn);
        }
        else
        {
            TouchHandler.instance.UpdateMultiplayerCardMask(DeckType.None);
        }
    }

     
    public void UpdateCardPlaced(DeckType _type, GameObject _card) 
    {
        if (DeckType.AttackDeck == _type)
        {
            AttackDeckCards.Remove(_card);
            if (PhotonNetwork.IsMasterClient)
            {
                MultiplayerManager.instance.ForceStopTimer();
                MultiplayerManager.instance.RaiseControlsSwitch();
            }
        }
        else if (DeckType.DefenseDeck == _type)
        {
            DefenseDeckCards.Remove(_card);
            CheckCardsPlaced(_card);
        }
    }

    
    public void CheckCardsPlaced(GameObject _card)
    {
        if (placedAttack == null && placedDefense == null)
        {
            MultiplayerUi.instance.ToastMsg("No Card Played");
        }
        else if (placedAttack == null && placedDefense != null) // attack card not placed
        {
            MultiplayerUi.instance.ToastMsg("No Attack Played");
            MultiplayerManager.instance.addScore(DeckType.DefenseDeck, 5);
            MultiplayerManager.instance.subtractHealth(DeckType.AttackDeck, 0);
        }
        else if (placedAttack != null && placedDefense == null) // defense card not placed
        {
            MultiplayerUi.instance.ToastMsg("No Defense Played");
            MultiplayerManager.instance.addScore(DeckType.AttackDeck, 5);
            MultiplayerManager.instance.subtractHealth(DeckType.DefenseDeck, 0);
        }
        else if (placedAttack != null && placedDefense != null && _card != null)
        {
            if (placedAttack.correctDefense == placedDefense) // correct defense
            {
                MultiplayerUi.instance.ToastMsg("Correct Defense");
                MultiplayerManager.instance.addScore(DeckType.DefenseDeck, 5);
                MultiplayerManager.instance.subtractHealth(DeckType.AttackDeck, 0);
                _card.GetComponent<DefenseCard>().DisplayResultGfx(true);
            }
            else // wrong defense
            {
                MultiplayerUi.instance.ToastMsg("Wrong Defense");
                MultiplayerManager.instance.addScore(DeckType.AttackDeck, 5);
                MultiplayerManager.instance.subtractHealth(DeckType.DefenseDeck, 0);
                _card.GetComponent<DefenseCard>().DisplayResultGfx(false);
            }
        }

        ForceNextScenario();
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
        attackCard.GetComponent<AttackCard>().SetData(AttackCardOptions[attackDeckIndex]);
        AllActiveCards.Add(attackCard);
        AttackDeckCards.Add(attackCard);
        attackDeckIndex++;
        ReLayerCards(attackCard);
    }

    int defenseDeckIndex = 0;
    public void DefenseDeckTapped()
    {
       
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

        GameObject defenseCard = Instantiate(DefenseCardOptions[defenseDeckIndex].CardPrefab);
        defenseCard.transform.SetParent(table.defenseDeckHolder.transform, false);
        defenseCard.GetComponent<DefenseCard>().SetData(DefenseCardOptions[defenseDeckIndex]);
        AllActiveCards.Add(defenseCard);
        DefenseDeckCards.Add(defenseCard);
        defenseDeckIndex++;
        ReLayerCards(defenseCard);
    }

    public void DisableDragForDeck(DeckType _type)
    {
        if (DeckType.AttackDeck == _type)
        {
            foreach (GameObject _attackCard in AttackDeckCards)
            {
                _attackCard.GetComponent<AttackCard>().CanBeDragged = false;
            }
        }
        else
        {
            foreach (GameObject _defenseCard in DefenseDeckCards)
            {
                _defenseCard.GetComponent<DefenseCard>().CanBeDragged = false;
            }
        }
    }

   
    public void displayCorrectCards()
    {
        
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

    
    public void SyncMultiplayerCardPos(int _id, DeckType _type, Vector3 _pos)
    {
        if (_type == DeckType.AttackDeck)
        {
            AttackCard card = AttackDeckCards.Find(_attackCard => _attackCard.GetComponent<AttackCard>().data.ID == _id).GetComponent<AttackCard>();
            if (card != null)
            {
                card.transform.position = _pos;
            }
        }
        else if (_type == DeckType.DefenseDeck)
        {
            DefenseCard card = DefenseDeckCards.Find(_defenseCard => _defenseCard.GetComponent<DefenseCard>().data.ID == _id).GetComponent<DefenseCard>();
            if (card != null)
            {
                card.transform.position = _pos;
            }
        }
    }

    
    public void ApplyNetworkDeckClicked(DeckType _type)
    {
        if (_type == DeckType.AttackDeck)
        {
            AttackDeckTapped();
        }
        else if (_type == DeckType.DefenseDeck)
        {
            DefenseDeckTapped();
        }
    }

    
    public void ForceNextScenario()
    {
        MultiplayerManager.instance.ForceStopTimer();
        StartCoroutine(LoadNextScenario());
    }

    
    IEnumerator LoadNextScenario()
    {
        TouchHandler.instance.TouchIsEnabled = false; 
        yield return new WaitForSeconds(2);
        ResetScenario();
    }

}
