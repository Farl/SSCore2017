using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class ArrayGUI<T> : IInspectorDrawer
{
	public					ArrayGUI					( String name, Func<IList<T>> array, Boolean readOnly = false, Boolean allowDuplicates = false, Action<Rect, T, Boolean> drawElementAction = null )
    {
        _title				= name;
        _itemsSource		= array;
        _readonly			= readOnly;
        _allowDuplicates	= allowDuplicates;

        if (!typeof(Object).IsAssignableFrom(typeof(T)) && drawElementAction == null)
        {
            throw new Exception(string.Format(
                "Trying to create ArrayGUI with type: {0}, " +
                "however type is not based on UnityEngine.Object and no drawElementAction is specified. " +
                "Either use a UnityEngine.Object derived type or provide a custom drawElementAction", 
                typeof(T).Name));
        }

        _drawElement = ((rect, item, dragging, index) =>
                           {
                               if (Event.current.type == EventType.Repaint)
                               {
                                   _styles.elementBackground.Draw(rect, false, false, false, false);
                                   var handleRect = new Rect(rect.x + 5f, rect.y + 7f, 10f, rect.height - 14f);
                                   _styles.draggingHandle.Draw(handleRect, false, false, false, false);
                               }

                               var middleRect = new Rect(rect.x + 20f, rect.y + 2f, rect.width - 48f, rect.height - 4);
                               if (drawElementAction == null)
                               {
                                   var previousO = item;
                                   var temp = EditorGUI.ObjectField(middleRect, item as Object, typeof (T), true);
                                   var o = temp == null ? default(T) : CastAs<T>(temp);
                                   var hasObject = o != null;
                                   if (hasObject)
                                   {
                                       var isChangeObject = !o.Equals(previousO);
									   //var isNewObject = previousO == null && o != null;

									   //if (hasObject && isNewObject)
									   //{
									   //	PreFrameActions.Enqueue(() => {});
									   //	ItemsSource.Add( o );
									   //}

                                       if (hasObject && isChangeObject)
									   {
										   PreFrameActions.Enqueue(() => {});
										   ItemsSource[index] = o;
									   }
                                           

                                       if (ShowDebug)
                                           Debug.Log(string.Format("HasObject: {0}, IsChangeObject: {1}", hasObject, isChangeObject ));
                                   }
                               }
                               else
                                   drawElementAction.Invoke(middleRect, item, dragging);

                               if (item != null)
                               {
                                   var removeRect = new Rect(rect.x + rect.width - 24f, rect.y + 2f, 24f, rect.height - 4);
                                   if (GUI.Button(removeRect, _styles.iconToolbarMinus, _styles.removeButton))
								   {
									   var si = index;
                                       PreFrameActions.Enqueue(() => Remove(si));
								   }
                               }
                           });
    }

    public const			String						Version				= "1.02";
    public static			Styles						_styles;

	private const			Boolean						ShowDebug			= false;
    private readonly		Func<IList<T>>				_itemsSource;
    private					IList<T>					ItemsSource			
	{
		get 
		{
			return _itemsSource( ); 
		}
	}
	public IEnumerable Source 
	{
		get
		{
			return ItemsSource;
		}
	}

    private					IList<T>					_viewItems;
    private					Boolean						_readonly;
    private					Boolean						_allowDuplicates	= true;
    private static			T							_dummyItem;
    
    private readonly		Int32[]						_roundingAmounts	= new[] { 0x3e8, 500, 100, 50, 10, 5, 1 };
    private					String						_title;
    //private					Vector2						_scroll				= Vector2.zero;

    private readonly		Action<Rect, T, Boolean, Int32>	_drawElement;
	private readonly		Queue<Action>			_preFrameActions	= new Queue<Action>();
	private readonly		Queue<Action>			_nextFrameActions	= new Queue<Action>();

    public					Action<Rect, T, bool, Int32>		DrawElement			
	{
		get 
		{
			return _drawElement; 
		}
	}
    public					Queue<Action>		PreFrameActions		
	{
		get 
		{
			return _preFrameActions; 
		}
	}
    public					Queue<Action>		NextFrameActions	
	{
		get 
		{
			return _nextFrameActions; 
		} 
	}

    public					void						Add					( T item )								
    {
        if (_readonly)
            return;

        if ((!_allowDuplicates && !ItemsSource.Contains(item)) && item != null && !item.Equals(_dummyItem))
        {
            ItemsSource.Add(item);
            NextFrameActions.Enqueue(() => {});
        }
    }
    public					void						Remove				( Int32 index )							
    {
        if ( _readonly )
            return;

        ItemsSource.RemoveAt( index );
    }
    public					void						Change				( T currentItem, T newItem )			
    {
        if (_readonly)
            return;

        var i = ItemsSource.IndexOf(currentItem);
        if (i >= 0)
        {
            ItemsSource[i] = newItem;
            NextFrameActions.Enqueue(() => { });
        }
    }
    public					void						MoveItemFromTo		( Int32 currentIndex, Int32 newIndex )	
    {
        if (currentIndex < 0 || newIndex < 0 || currentIndex == newIndex)
            return;

        if (ShowDebug)
            Debug.Log(string.Format("Move from: {0} to: {1}", currentIndex, newIndex));

        var item = _viewItems[currentIndex];
        if (item == null)
            return;

        ItemsSource.RemoveAt(currentIndex);
        ItemsSource.Insert(newIndex, item);

        PreFrameActions.Enqueue(() => { }); // Dummy to trigger item update
    }

    public					void						OnEnable			( )										
    {
        if ( _dummyItem == null )
            _dummyItem = default(T);
    }
	public					Boolean						OnInspectorGUI		( Boolean isExpaned )					
	{
		return OnInspectorGUI( isExpaned, GUILayoutUtility.GetRect( GUIContent.none, EditorStyles.foldout ) );
	}
	public					Boolean						OnInspectorGUI		( Boolean isExpaned, Rect position )	
    {
        if ( _styles == null )
            _styles = new Styles( );

		position.x		+= 4;
		position.width	-= 4;

        var foldoutState	= EditorGUI.Foldout( position, isExpaned, String.Format( "{0} ({1} items) ", _title, ItemsSource.Count ) );

		position.width	/= 2;
		position.x		+= position.width;
		position.width	-= 4;

		var newVal		= EditorGUI.ObjectField( position, (UnityEngine.Object)null, typeof(T), true);
		if( newVal != null )
		{
			ItemsSource.Add( (T)(System.Object)newVal );
			BufferItems();
		}

	    if ( !foldoutState || ItemsSource.Count == 0 )
			return foldoutState;

	    #region | Change Processing |
		{
			var hasPreFrameActions = PreFrameActions != null && PreFrameActions.Count > 0;
			if ( hasPreFrameActions )
			{
				foreach ( var action in PreFrameActions )
					action( );

				PreFrameActions.Clear( );
			}

			var hasNextFrameActions		= NextFrameActions != null && NextFrameActions.Count > 0;
			var changed					= hasPreFrameActions || hasNextFrameActions;

			if( ShowDebug )
				Debug.Log( String.Format( "PreFrame Count: {0}", PreFrameActions.Count ) );

			if( changed || _viewItems == null || _viewItems.Count <= 0 )
				BufferItems( );

			if ( changed )
			{
				foreach ( var action in NextFrameActions )
					action( );

				NextFrameActions.Clear( );
			}
		}
	    #endregion

	    EditorGUILayout.Space			( );
	    EditorGUILayout.BeginHorizontal	( new GUILayoutOption[0] );
		{
			GUILayout.Space					( 4f );
			EditorGUILayout.BeginVertical	( new GUILayoutOption[0] );
			{
				EditorGUILayout.BeginVertical	( _styles.boxBackground );
				{
					//_scroll = EditorGUILayout.BeginScrollView(_scroll);
					//{
						var options = new[] { GUILayout.ExpandWidth(true) };                            
						var dragResult = DragGUI<T>.Draggable(GUILayoutUtility.GetRect(10f, 21 * _viewItems.Count, options), 21, (List<T>)_viewItems, _drawElement );
						if( ShowDebug )
							Debug.Log( String.Format("PreFrame Count END: {0}", PreFrameActions.Count ) );

						if ( dragResult.WasMoved )
						{
							var newIndex = dragResult.NewIndex.Value;
							if ( ShowDebug )
								Debug.Log( String.Format("Trying to move from:{0} to:{1}", dragResult.PreviousIndex, newIndex ) );
                                
							MoveItemFromTo( dragResult.PreviousIndex, newIndex );
							//SetIndexAccordingToNeighbors(newIndex);
						}
					//}
					//EditorGUILayout.EndScrollView();
				}
				EditorGUILayout.EndVertical();

				//GUILayout.BeginHorizontal(_styles.toolbar, new GUILayoutOption[0]);
				//{
				//	GUILayout.FlexibleSpace();
				//	var iconToolbarPlus = _styles.iconToolbarPlus;
				//	var rect = GUILayoutUtility.GetRect(iconToolbarPlus, _styles.toolbarDropDown);

				//	if (EditorGUIExt.ButtonMouseDown(rect, _styles.iconToolbarClear, FocusType.Native, _styles.toolbarDropDown))
				//		PreFrameActions.Enqueue(() => ItemsSource.Clear());
				//}
				//GUILayout.EndHorizontal();
			}
			GUILayout.EndVertical( );
			GUILayout.Space( 4f );
		}
	    GUILayout.EndHorizontal( );
	    //GUILayout.FlexibleSpace( );

		return foldoutState;
    }

    private static			T							CastAs<T>							( Object o )													
    {
        return (T) Convert.ChangeType( o, o.GetType( ) );
    }
    private					void						BufferItems							( )																
    {
        if( ShowDebug )
            Debug.Log("Buffer Items");

        _viewItems = new List<T>( ItemsSource );// { _dummyItem };
    }
    private					Int32						GetAverageRoundedAwayFromZero		( Int32 a, Int32 b )											
    {
        if (((a + b) % 2) == 0)
            return ((a + b) / 2);

        return (((a + b) + Math.Sign(a + b)) / 2);
    }
    private					Int32						GetBestPushDirectionForOrderValue	( Int32 order )													
    {
        var num = (int) Mathf.Sign(order);
        if ((order >= -16000) && (order <= 16000))
            return num;

        return -num;
    }
    private					Int32						RoundBasedOnContext					( Int32 val, Int32 lowerBound, Int32 upperBound )				
    {
        var num = Mathf.Max(0, (upperBound - lowerBound) / 6);
        lowerBound += num;
        upperBound -= num;
        for (var i = 0; i < _roundingAmounts.Length; i++)
        {
            var num3 = RoundByAmount(val, _roundingAmounts[i]);
            if ((num3 > lowerBound) && (num3 < upperBound))
                return num3;
        }
        return val;
    }
    private					Int32						RoundByAmount						( Int32 val, Int32 rounding )									
    {
        return (Mathf.RoundToInt(((float) val) / ((float) rounding)) * rounding);
    }
    private					void						SetIndexAccordingToNeighbors		( Int32 indexOfChangedItem, Int32 pushDirection = 0 )			
    {
        if ((indexOfChangedItem >= 0) && (indexOfChangedItem < _viewItems.Count))
        {
            if (indexOfChangedItem == 0)
                MoveItemFromTo(indexOfChangedItem, indexOfChangedItem + 1);
            else if (indexOfChangedItem == (_viewItems.Count - 2)) // exclude end item which is always dummy
                MoveItemFromTo(indexOfChangedItem, indexOfChangedItem - 1);
            else
            {
                var executionOrderAtIndex = indexOfChangedItem - 1;
                var upperBound = indexOfChangedItem + 1;
                var order = RoundBasedOnContext(GetAverageRoundedAwayFromZero(executionOrderAtIndex, upperBound),
                                                executionOrderAtIndex, upperBound);
                if (order != 0)
                {
                    if (pushDirection == 0)
                        pushDirection = GetBestPushDirectionForOrderValue(order);

                    if (pushDirection > 0)
                        order = Mathf.Max(order, executionOrderAtIndex + 1);

                    else
                        order = Mathf.Min(order, upperBound - 1);
                }

                MoveItemFromTo(indexOfChangedItem, order);
            }
        }
    }

    public class Styles
    {
        public			Styles			( )				
        {
            this.boxBackground.margin = new RectOffset();
            this.boxBackground.padding = new RectOffset(1, 1, 1, 0);
            this.elementBackground.overflow = new RectOffset(1, 1, 1, 0);
            this.defaultTime.alignment = TextAnchor.MiddleCenter;
            this.defaultTime.overflow = new RectOffset(0, 0, 1, 0);
            this.dropField.overflow = new RectOffset(2, 2, 2, 2);
            this.dropField.normal.background = null;
            this.dropField.hover.background = null;
            this.dropField.active.background = null;
            this.dropField.focused.background = null;
        }

        public			GUIStyle		boxBackground			= "TE NodeBackground";
        public			GUIStyle		defaultTime				= EditorStyles.miniButton;
        public			GUIStyle		defaultTimeContent		= EditorStyles.miniButton;
        public			GUIStyle		draggingHandle			= "WindowBottomResize";
        public			GUIStyle		dropField				= new GUIStyle(EditorStyles.objectFieldThumb);
        public			GUIStyle		elementBackground		= new GUIStyle("OL Box");

        public			GUIContent		iconToolbarMinus		= IconContent("Toolbar Minus", "Remove item");
        public			GUIContent		iconToolbarClear		= IconContent("Toolbar Minus", "Clear items");
        public			GUIContent		iconToolbarPlus			= IconContent("Toolbar Plus", "Add item");
        public			GUIStyle		removeButton			= "InvisibleButton";
        public			GUIStyle		toolbar					= "TE Toolbar";
        public			GUIStyle		toolbarDropDown			= "TE ToolbarButton";

        public static	GUIContent		IconContent				( String name, String tooltip )			
        {
            var t				= typeof(EditorGUIUtility);
            var m				= t.GetMethod( "IconContent", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static, Type.DefaultBinder, new[] { typeof(String) }, null );
            var content			= (GUIContent) m.Invoke( t, new[] { name } );
            content.tooltip		= tooltip;

            return content;
        }
    }
}