using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Viens no trim stieņiem (torņiem). Uzliec uz UI Image objekta (var būt caurspīdīgs,
/// bet Raycast Target jābūt IESLĒGTAM), kas sedz visu stieņa augstumu.
/// </summary>
public class HanoiPeg : MonoBehaviour, IDropHandler
{
    [HideInInspector] public HanoiGameManager manager;

    // Diski uz šī stieņa: [0] = apakšējais, pēdējais = augšējais
    public readonly List<HanoiDisc> stack = new List<HanoiDisc>();

    public HanoiDisc Top
    {
        get { return stack.Count > 0 ? stack[stack.Count - 1] : null; }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null || manager == null) return;

        HanoiDisc disc = eventData.pointerDrag.GetComponent<HanoiDisc>();
        if (disc != null)
            manager.TryMove(disc, this);
    }
}