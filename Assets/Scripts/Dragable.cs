using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(BoxCollider2D))]
public class Dragable : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    private bool m_dragable;

    private void Awake()
    {
        m_dragable = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Do nothing for now
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        m_dragable = true;

        StartCoroutine(MoveToMouse());
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        m_dragable = false;
    }

    private IEnumerator MoveToMouse()
    {
        while (m_dragable)
        {
            Vector3 _mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(_mouse.x, _mouse.y, transform.position.z);

            yield return new WaitForEndOfFrame();
        }
    }
}
