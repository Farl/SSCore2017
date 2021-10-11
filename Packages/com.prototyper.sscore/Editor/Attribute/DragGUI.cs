using System;
using System.Collections.Generic;
using UnityEngine;

public struct DragResult
{
    public				Int32				PreviousIndex;
    public				Int32?				NewIndex;
    public				Boolean				WasMoved			
	{
		get 
		{
			return NewIndex.HasValue; 
		}
	}

    public				DragResult			( Int32 previousIndex )						
    {
        PreviousIndex = previousIndex;
        NewIndex = null;
    }
    public				DragResult			( Int32 previousIndex, Int32 newIndex )		
    {
        PreviousIndex = previousIndex;
        NewIndex = newIndex;
    }
}

public class DragGUI<T>
{
    private static readonly		Int32				DragGUIHash			= "DragGUI".GetHashCode( );
    private static				Int32				_draggedElement;
    private static				Int32[]				_targets;
    private static				Single[]			_positions;

    public static				DragResult			Draggable		( Rect position, Int32 elementHeight, List<T> elements, Action<Rect, T, Boolean, Int32> drawElementAction, Func<T, Boolean> draggableFilter = null )	
    {
        if ( elements.Count <= 0 )
            return new DragResult( );

        draggableFilter = draggableFilter ?? ( o =>
            {
                if (o == null)
                    return false;

                if (elements.Count <= 1)
                    return false;

                if (IsDefaultItem(o))
                    return false;

                return true;
            });

        Rect rect;
        var controlID = GUIUtility.GetControlID(DragGUIHash, FocusType.Passive);
        var r = position;
        r.height = elementHeight;
        var index = 0;
        if ((GUIUtility.hotControl == controlID) && (Event.current.type == EventType.Repaint))
        {
            for (var i = 0; i < elements.Count; i++)
                if (i != _draggedElement)
                    if (IsDefaultItem(elements[i]))
                    {
                        index = i;
                        i++;
                    }
                    else
                    {
                        r.y = position.y + (_positions[i] * elementHeight);
                        drawElementAction(r, elements[i], false, i);
                    }
            rect = new Rect(r.x, position.y + (_positions[index] * elementHeight), r.width, ((_positions[index + 1] - _positions[index]) + 1f) * elementHeight);
        }
        else
        {
            for (var i = 0; i < elements.Count; i++)
            {
                r.y = position.y + (i * elementHeight);
                if (IsDefaultItem(elements[i]))
                {
                    index = i;
                    i++;
                }
                else
                    drawElementAction(r, elements[i], false, i);
            }
            rect = new Rect(r.x, position.y + (index * elementHeight), r.width, (float) (elementHeight * 2));
        }

        //GUI.Label(rect, ScriptExecutionOrderInspector._styles.defaultTimeContent, ScriptExecutionOrderInspector._styles.defaultTime);
        var flag = rect.height > (elementHeight * 2.5f);
        if (GUIUtility.hotControl == controlID)
        {
            if (flag)
                GUI.color = new Color(1f, 1f, 1f, 0.5f);
            r.y = position.y + (_positions[_draggedElement] * elementHeight);
            drawElementAction(r, elements[_draggedElement], true, _draggedElement );
            GUI.color = Color.white;
        }

        var result = new DragResult();
        result.PreviousIndex = _draggedElement;
        switch (Event.current.GetTypeForControl(controlID))
        {
            case EventType.MouseDown:
                if (position.Contains(Event.current.mousePosition))
                {
                    GUIUtility.keyboardControl = 0;
                    _draggedElement = Mathf.FloorToInt((Event.current.mousePosition.y - position.y) / ((float) elementHeight));
                    result.PreviousIndex = _draggedElement;

                    // Certain conditions where you can't drag the item
                    if (!draggableFilter(elements[_draggedElement]))
                        return result;

                    _positions = new float[elements.Count];
                    _targets = new int[elements.Count];
                    for (var i = 0; i < elements.Count; i++)
                    {
                        _targets[i] = i;
                        _positions[i] = i;
                    }
                    GUIUtility.hotControl = controlID;
                    Event.current.Use();
                }
                return result;

            case EventType.MouseUp:
                if (GUIUtility.hotControl == controlID)
                {
                    result.NewIndex = _targets[_draggedElement];
                    _targets = null;
                    _positions = null;
                    _draggedElement = -1;
                    GUIUtility.hotControl = 0;
                    Event.current.Use();
                    return result;
                }
                return result;

            case EventType.MouseMove:
            case EventType.KeyDown:
            case EventType.KeyUp:
            case EventType.ScrollWheel:
                return result;

            case EventType.MouseDrag:
                if (GUIUtility.hotControl == controlID)
                {
                    _positions[_draggedElement] = ((Event.current.mousePosition.y - position.y) / ((float) elementHeight)) - 0.5f;
                    _positions[_draggedElement] = Mathf.Clamp(_positions[_draggedElement], 0f, (float) (elements.Count - 1));
                    var elementToPush = Mathf.RoundToInt(_positions[_draggedElement]);
                    
                    if (elementToPush != _targets[_draggedElement] )
                    {
                        for (var i = 0; i < elements.Count; i++)
                            _targets[i] = i;

                        var num9 = (elementToPush <= _draggedElement) ? 1 : -1;
                        for (var i = _draggedElement; i != elementToPush; i -= num9)
                            _targets[i - num9] = i;

                        _targets[_draggedElement] = elementToPush;
                    }
                    Event.current.Use();
                    return result;
                }
                return result;

            case EventType.Repaint:
                if (GUIUtility.hotControl == controlID)
                {
                    for (var i = 0; i < elements.Count; i++)
                        if (i != _draggedElement)
                            _positions[i] = Mathf.MoveTowards(_positions[i], (float) _targets[i], 0.075f);

                    //GUIView.current.Repaint();
                }
                return result;
        }

        return result;
    }
    private static				Boolean				IsDefaultItem	( T element )																																	
    {
        if (element == null)
            return false;

        if (element.Equals(default(T)))
            return true;

        return false;
    }
}