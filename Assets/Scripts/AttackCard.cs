using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class AttackCard : MonoBehaviour, IDragDrop
{
    public TextMeshPro Title, Description;
    public bool CanBeDragged { get; set; }
    public float HoverScaleMultiplier = 1.5f; 
    public AttackCardData data; 
    public GameObject correctGfx, wrongGfx;

    BoxCollider cardCollider;
    Vector3 hoverScale, originalScale, originalPos, hoverPos; 

    void Start()
    {
        cardCollider = GetComponent<BoxCollider>();
        CanBeDragged = true;
        originalScale = transform.localScale;
        hoverScale = transform.localScale * HoverScaleMultiplier;
        correctGfx.SetActive(false);
        wrongGfx.SetActive(false);
    }

    public void SetData(AttackCardData _data)
    {
        data = _data;
        Title.SetText(_data.CardName);
        Description.SetText(_data.CardDescription);
    }

    public void SyncMultiplayerPos()
    {
        if (MultiplayerManager.instance != null)
        {
            MultiplayerManager.instance.UpdateCardPos(data.ID, DeckType.AttackDeck, transform.position);
        }
    }

    public void DisplayResultGfx(bool _result)
    {
        correctGfx.SetActive(_result);
        wrongGfx.SetActive(!_result);
    }

    // --------------------------------------- IDragDrop ---------------------------------------
    public void OnGrab(Vector3 _pos)
    {
        
        _pos.z = -10;
        transform.position = _pos;
        transform.localScale = hoverScale;
    }

    public void OnDrag(Vector3 _pos)
    {
       
        _pos.z = -10;
        transform.position = _pos;
        
        SyncMultiplayerPos();
    }

    public void OnDrop(Vector3 _pos)
    {
        
        transform.localScale = originalScale;

        if (CardManager.instance != null)
        {
            CardManager.instance.ReLayerCards(gameObject);
        }
        else if (MultiplayerCardManager.instance != null)
        {
            MultiplayerCardManager.instance.ReLayerCards(gameObject);
        }
        
        SyncMultiplayerPos();
    }

    public void OnHover(bool _value)
    {
        Debug.Log("OnHover");
         
        if (this == null || !CanBeDragged)
        {
            return;
        }

        originalPos = transform.position;
        hoverPos = originalPos;

        if (_value)
        {
            transform.localScale = hoverScale;
            hoverPos.z = -10;
            transform.position = hoverPos;
        }
        else
        {
            transform.localScale = originalScale;
            transform.position = originalPos;
        }
    }

}
