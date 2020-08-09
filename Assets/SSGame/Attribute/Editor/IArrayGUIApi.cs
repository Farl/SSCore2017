using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IArrayGUIApi<in T>
{
    Queue<Action> PreFrameActions { get; }

    void Add(T item);
    void Remove(T item);
    void Change(T currentItem, T newItem);
    void MoveItemFromTo(int currentIndex, int newIndex);
}

public interface IInspectorDrawer
{
	IEnumerable	Source
	{
		get;
	}

    Boolean OnInspectorGUI( Boolean isExpanded, Rect position );
	Boolean OnInspectorGUI( Boolean isExpanded );
}