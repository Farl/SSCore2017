//-------------------------------------------------
//	Copyright (C) 2005-.2012, Jes Obertyukh 
//	Email: JesObertyukh@gmail.com
//-------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using PropertyDrawers;
using UnityEditor;
using UnityEngine;
using Object = System.Object;

[CustomPropertyDrawer( typeof(ArrayAttribute) )]
public class ArrayAttributeDrawer : PropertyDrawer
{
	private IInspectorDrawer	_arrayGUI;

	public override void		OnGUI				( Rect position, SerializedProperty property, GUIContent label )	
	{	
		var displayName = EditorGUI.BeginProperty( position, label, property );
		{
			if( property.GetIntPtr( ) == IntPtr.Zero || _arrayGUI == null || ( _arrayGUI.Source is SerializedPropertyList && !SerializedProperty.EqualContents( ((SerializedPropertyList)_arrayGUI.Source).Property, property ) ) )
			{
				Type type;
				PropertyDrawerUtils.GetFieldInfoFromProperty( property, out type );
				type			= type.IsArray ? type.GetElementType( ) : type.GetGenericArguments()[0];

				GetType( ).GetMethod( "MakeArrayGUI", BindingFlags.NonPublic | BindingFlags.Instance ).MakeGenericMethod( type ).Invoke( this, new object[]{ property, displayName.text } );
			}

			property.isExpanded = _arrayGUI.OnInspectorGUI( property.isExpanded, position );
		}
		EditorGUI.EndProperty( );
	}
	private			void		MakeArrayGUI<T>		( SerializedProperty property, String displayName )					
	{
		var ggg			= new SerializedPropertyList<T>( property );
        _arrayGUI		= new ArrayGUI<T>( displayName, () => ggg );
	}

	private class  SerializedPropertyList
	{
		public SerializedPropertyList( SerializedProperty prop )
		{
			_property = prop;
		}

		protected SerializedProperty _property;

		public SerializedProperty Property
		{
			get 
			{
				return _property; 
			}
		}
	}

	private class  SerializedPropertyList<T> : SerializedPropertyList, IList<T>
	{
		public SerializedPropertyList( SerializedProperty prop ):base( prop )
		{
			
		}

		public IEnumerator<T> GetEnumerator		( )		
		{
			throw new NotImplementedException();
		}
		IEnumerator IEnumerable.GetEnumerator	( )		
		{
			return GetEnumerator();
		}

		public		Int32			Count		
		{
			get
			{
				return _property.arraySize;
			}
		}
		public		Boolean			IsReadOnly	
		{
			get
			{
				return false;
			}
		}
		public		T				this		[ Int32 index ]						
		{
			get 
			{
				return (T)(Object)_property.GetArrayElementAtIndex(index).objectReferenceValue;
			}
			set 
			{
				_property.GetArrayElementAtIndex(index).objectReferenceValue	= (UnityEngine.Object)(Object)value;
			}
		}

		public		void			Add			( T item )							
		{
			Insert( _property.arraySize, item );
		}
		public		void			Insert		( Int32 index, T item )				
		{
			_property.InsertArrayElementAtIndex	( index );
			_property.GetArrayElementAtIndex	( index ).objectReferenceValue = (UnityEngine.Object)(Object)item;
		}
		public		Boolean			Remove		( T item )							
		{
			throw new NotImplementedException();
		}
		public		void			RemoveAt	( Int32 index )						
		{
			_property.DeleteArrayElementAtIndex( index );

			for ( var i = index; i < _property.arraySize - 1; i++ )
				_property.MoveArrayElement( i + 1, i );	

			_property.arraySize--;
		}
		public		void			Clear		( )									
		{
			throw new NotImplementedException();
		}

		public		Boolean			Contains	( T item )							
		{
			throw new NotImplementedException();
		}
		public		void			CopyTo		( T[] array, Int32 arrayIndex )		
		{
			var index = arrayIndex;

			for ( var i = 0; i < _property.arraySize; i++, index++ )
			{
				array[index] = (T)(Object)_property.GetArrayElementAtIndex( i ).objectReferenceValue;
			}
		}
		public		Int32			IndexOf		( T item )							
		{
			throw new NotImplementedException();
		}
	}
}
